// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class ULongConcurrency : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing how to map rowversion to ulong...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            context.Add(
                new Blog
                {
                    Name = "OneUnicorn"
                });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back in one context...");

            var blog = await context.Set<Blog>().SingleAsync();
            blog.Name = "TwoUnicorns";

            using (var context2 = new SampleDbContext())
            {
                ConsoleWriteLines("Change the blog name and save in a different context...");

                (await context2.Set<Blog>().SingleAsync()).Name = "1unicorn2";
                await context2.SaveChangesAsync();
            }

            try
            {
                ConsoleWriteLines("Change the blog name and save in the first context...");

                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                ConsoleWriteLines($"{e.GetType().FullName}: {e.Message}");

                var databaseValues = await context.Entry(blog).GetDatabaseValuesAsync();
                context.Entry(blog).OriginalValues.SetValues(databaseValues);

                ConsoleWriteLines("Refresh original values and save again...");

                await context.SaveChangesAsync();
            }
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureULongConcurrency
            modelBuilder.Entity<Blog>()
                .Property(e => e.Version)
                .IsRowVersion()
                .HasConversion<byte[]>();
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ULongConcurrency;Trusted_Connection=True;ConnectRetryCount=0")
                .EnableSensitiveDataLogging();
    }

    #region ULongConcurrencyModel
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong Version { get; set; }
    }
    #endregion
}
