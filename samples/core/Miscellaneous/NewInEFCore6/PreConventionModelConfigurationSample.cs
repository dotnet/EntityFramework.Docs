using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public static class PreConventionModelConfigurationSample
{
    public static void Configure_property_types_and_value_converter_in_one_place()
    {
        Console.WriteLine($">>>> Sample: {nameof(Configure_property_types_and_value_converter_in_one_place)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.AddRange(
                new Customer
                {
                    Name = "Arthur",
                    IsActive = true,
                    AccountValue = new Money(1090.0m, Currency.PoundsStirling),
                    Orders =
                    {
                        new()
                        {
                            OrderDate = new DateTime(2021, 7, 31),
                            Price = new Money(29.0m, Currency.PoundsStirling),
                            Discount = new Money(5.0m, Currency.PoundsStirling)
                        }
                    }
                },
                new Customer
                {
                    Name = "Alan",
                    AccountValue = new Money(0.0m, Currency.UsDollars),
                    Orders =
                    {
                        new()
                        {
                            OrderDate = new DateTime(2021, 7, 21),
                            Price = new Money(2.99m, Currency.UsDollars)
                        }
                    }
                },
                new Customer
                {
                    Name = "Andrew",
                });

            context.SaveChanges();
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Money AccountValue { get; set; }

        public Session CurrentSession { get; set; }

        public ICollection<Order> Orders { get; } = new List<Order>();
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsComplete { get; set; }
        public Money Price { get; set; }
        public Money? Discount { get; set; }

        public Customer Customer { get; set; }
    }

    public readonly struct Money
    {
        [JsonConstructor]
        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public override string ToString()
            => (Currency == Currency.UsDollars ? "$" : "£") + Amount;

        public decimal Amount { get; }
        public Currency Currency { get; }
    }

    public enum Currency
    {
        UsDollars,
        PoundsStirling
    }

    public class Session
    {
        public string SessionData { get; set; }
        public string SessionCookies { get; set; }
    }

    public class CustomersContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        private readonly bool _quiet;

        public CustomersContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            //if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<bool>()
                .HaveConversion(typeof(BoolToZeroOneIntConverter), null);

            configurationBuilder
                .Properties<Money>()
                .HaveConversion(typeof(MoneyConverter), null)
                .AreUnicode(false)
                .HaveMaxLength(1024);


            // See #25416
            configurationBuilder
                .Properties<Money?>()
                .HaveConversion(typeof(MoneyConverter), null)
                .AreUnicode(false)
                .HaveMaxLength(1024);

            configurationBuilder
                .Properties<DateTime>()
                .HaveConversion<long>();

            configurationBuilder
                .Properties<string>()
                .AreUnicode(false)
                .HaveMaxLength(1024);

            configurationBuilder.IgnoreAny<Session>();
        }
    }

    // Remove once #25415 is fixed.
    public class BoolToZeroOneIntConverter : BoolToZeroOneConverter<int>
    {
    }

    public class MoneyConverter : ValueConverter<Money, string>
    {
        public MoneyConverter()
            : base(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Money>(v, (JsonSerializerOptions)null))
        {
        }
    }
}
