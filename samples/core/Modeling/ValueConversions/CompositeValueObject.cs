// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class CompositeValueObject : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a composite value object...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            context.Add(new Order { Price = new Money(3.99m, Currency.UsDollars) });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var order = await context.Set<Order>().SingleAsync();

            ConsoleWriteLines($"Order with price {order.Price.Amount} in {order.Price.Currency}.");
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureCompositeValueObject
            modelBuilder.Entity<Order>()
                .Property(e => e.Price)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Money>(v, (JsonSerializerOptions)null));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    #region CompositeValueObjectModel
    public class Order
    {
        public int Id { get; set; }

        public Money Price { get; set; }
    }
    #endregion

    #region CompositeValueObject
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
        PoundsSterling
    }
    #endregion
}
