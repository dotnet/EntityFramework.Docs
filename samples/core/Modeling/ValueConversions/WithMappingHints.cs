// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFModeling.ValueConversions
{
    public class WithMappingHints : Program
    {
        public void Run()
        {
            ConsoleWriteLines("Sample showing value conversions with mapping hints for facets...");

            using (var context = new SampleDbContext())
            {
                CleanDatabase(context);

                ConsoleWriteLines("Save a entities...");

                context.Add(new Order1 { Price = new Dollars(3.99m) });
                context.Add(new Order2 { Price = new Dollars(3.99m) });
                context.SaveChanges();
            }

            using (var context = new SampleDbContext())
            {
                ConsoleWriteLines("Read the entities back...");

                var entity1 = context.Set<Order1>().Single();
                var entity2 = context.Set<Order2>().Single();
            }

            ConsoleWriteLines("Sample finished.");
        }

        public class SampleDbContext : DbContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                #region ConverterWithMappingHints
                var converter = new ValueConverter<Dollars, decimal>(
                    v => v.Amount,
                    v => new Dollars(v),
                    new ConverterMappingHints(precision: 16, scale: 2));
                #endregion

                modelBuilder.Entity<Order1>()
                    .Property(e => e.Price)
                    .HasConversion(converter);

                #region ConfigureWithFacets
                modelBuilder.Entity<Order2>()
                    .Property(e => e.Price)
                    .HasConversion(converter)
                    .HasPrecision(20, 2);
                #endregion
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=WithMappingHints;Integrated Security=True")
                    .EnableSensitiveDataLogging();
        }

        public class Order1
        {
            public int Id { get; set; }

            public Dollars Price { get; set; }
        }

        public class Order2
        {
            public int Id { get; set; }

            public Dollars Price { get; set; }
        }

        public readonly struct Dollars
        {
            public Dollars(decimal amount) => Amount = amount;
            public decimal Amount { get; }
        }
    }
}
