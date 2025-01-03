// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class SimpleValueObject : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a simple value object...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            context.Add(new Order { Price = new Dollars(3.99m) });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var entity = await context.Set<Order>().SingleAsync();
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureImmutableStructProperty
            modelBuilder.Entity<Order>()
                .Property(e => e.Price)
                .HasConversion(
                    v => v.Amount,
                    v => new Dollars(v));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    #region SimpleValueObjectModel
    public class Order
    {
        public int Id { get; set; }

        public Dollars Price { get; set; }
    }
    #endregion

    #region SimpleValueObject
    public readonly struct Dollars
    {
        public Dollars(decimal amount)
            => Amount = amount;

        public decimal Amount { get; }

        public override string ToString()
            => $"${Amount}";
    }
    #endregion
}
