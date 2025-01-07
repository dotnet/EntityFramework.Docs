// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class ValueObjectCollection : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a collection of value objects...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            context.Add(
                new Blog
                {
                    Finances = new List<AnnualFinance>
                    {
                        new AnnualFinance(2018, new Money(326.65m, Currency.UsDollars), new Money(125m, Currency.UsDollars)),
                        new AnnualFinance(2019, new Money(112.20m, Currency.UsDollars), new Money(125m, Currency.UsDollars)),
                        new AnnualFinance(2020, new Money(25.77m, Currency.UsDollars), new Money(125m, Currency.UsDollars))
                    }
                });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var blog = await context.Set<Blog>().SingleAsync();

            ConsoleWriteLines($"Blog with finances {string.Join(", ", blog.Finances.Select(f => $"{f.Year}: I={f.Income} E={f.Expenses} R={f.Revenue}"))}.");

            ConsoleWriteLines("Changing the value object and saving again");

            blog.Finances.Add(new AnnualFinance(2021, new Money(12.0m, Currency.UsDollars), new Money(125m, Currency.UsDollars)));
            await context.SaveChangesAsync();
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureValueObjectCollection
            modelBuilder.Entity<Blog>()
                .Property(e => e.Finances)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<AnnualFinance>>(v, (JsonSerializerOptions)null),
                    new ValueComparer<IList<AnnualFinance>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => (IList<AnnualFinance>)c.ToList()));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    #region ValueObjectCollection
    public readonly struct AnnualFinance
    {
        [JsonConstructor]
        public AnnualFinance(int year, Money income, Money expenses)
        {
            Year = year;
            Income = income;
            Expenses = expenses;
        }

        public int Year { get; }
        public Money Income { get; }
        public Money Expenses { get; }
        public Money Revenue => new Money(Income.Amount - Expenses.Amount, Income.Currency);
    }
    #endregion

    #region ValueObjectCollectionMoney
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

    #region ValueObjectCollectionModel
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<AnnualFinance> Finances { get; set; }
    }
    #endregion
}
