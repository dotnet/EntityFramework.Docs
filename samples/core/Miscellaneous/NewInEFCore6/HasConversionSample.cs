using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public static class HasConversionSample
{
    public static async Task Can_set_value_converter_type_using_generic_method()
    {
        Console.WriteLine($">>>> Sample: {nameof(Can_set_value_converter_type_using_generic_method)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new CurrencyContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using var context = new CurrencyContext();

            context.AddRange(
                new TestEntity1
                {
                    Currency = Currency.UsDollars
                },
                new TestEntity1
                {
                    Currency = Currency.Euros
                },
                new TestEntity1
                {
                    Currency = Currency.PoundsSterling
                },
                new TestEntity2
                {
                    Currency = Currency.UsDollars
                },
                new TestEntity2
                {
                    Currency = Currency.Euros
                },
                new TestEntity2
                {
                    Currency = Currency.PoundsSterling
                },
                new TestEntity3
                {
                    Currency = Currency.UsDollars
                },
                new TestEntity3
                {
                    Currency = Currency.Euros
                },
                new TestEntity3
                {
                    Currency = Currency.PoundsSterling
                });

            await context.SaveChangesAsync();
        }
    }

    public class TestEntity1
    {
        public int Id { get; set; }
        public Currency Currency{ get; set; }
    }

    public class TestEntity2
    {
        public int Id { get; set; }
        public Currency Currency{ get; set; }
    }

    public class TestEntity3
    {
        public int Id { get; set; }
        public Currency Currency{ get; set; }
    }

    #region CurrencyEnum
    public enum Currency
    {
        UsDollars,
        PoundsSterling,
        Euros
    }
    #endregion

    #region CurrencyConverter
    public class CurrencyToSymbolConverter : ValueConverter<Currency, string>
    {
        public CurrencyToSymbolConverter()
            : base(
                v => v == Currency.PoundsSterling ? "£" : v == Currency.Euros ? "€" : "$",
                v => v == "£" ? Currency.PoundsSterling : v == "€" ? Currency.Euros : Currency.UsDollars)
        {
        }
    }
    #endregion

    public class CurrencyContext : DbContext
    {
        private readonly bool _quiet;

        public CurrencyContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region AsString
            modelBuilder.Entity<TestEntity1>()
                .Property(e => e.Currency)
                .HasConversion<string>();
            #endregion

            #region AsShort
            modelBuilder.Entity<TestEntity2>()
                .Property(e => e.Currency)
                .HasConversion<EnumToNumberConverter<Currency, short>>();
            #endregion

            #region AsSymbol
            modelBuilder.Entity<TestEntity3>()
                .Property(e => e.Currency)
                .HasConversion<CurrencyToSymbolConverter>();
            #endregion
        }
    }
}
