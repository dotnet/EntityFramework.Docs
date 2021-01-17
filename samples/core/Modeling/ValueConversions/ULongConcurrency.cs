// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions
{
    public class ULongConcurrency : Program
    {
        public void Run()
        {
            ConsoleWriteLines("Sample showing how to map rowversion to ulong...");

            using (var context = new SampleDbContext())
            {
                CleanDatabase(context);

                ConsoleWriteLines("Save a new entity...");

                context.Add(
                    new Blog
                    {
                        Name = "OneUnicorn"
                    });
                context.SaveChanges();
            }

            using (var context = new SampleDbContext())
            {
                ConsoleWriteLines("Read the entity back in one context...");

                var blog = context.Set<Blog>().Single();
                blog.Name = "TwoUnicorns";

                using (var context2 = new SampleDbContext())
                {
                    ConsoleWriteLines("Change the blog name and save in a different context...");

                    context2.Set<Blog>().Single().Name = "1unicorn2";
                    context2.SaveChanges();
                }

                try
                {
                    ConsoleWriteLines("Change the blog name and save in the first context...");

                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    ConsoleWriteLines($"{e.GetType().FullName}: {e.Message}");

                    var databaseValues = context.Entry(blog).GetDatabaseValues();
                    context.Entry(blog).OriginalValues.SetValues(databaseValues);

                    ConsoleWriteLines("Refresh original values and save again...");

                    context.SaveChanges();
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
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ULongConcurrency;Integrated Security=True")
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
}
