// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions
{
    public class EncryptPropertyValues : Program
    {
        public void Run()
        {
            ConsoleWriteLines("Sample showing value conversions for encrypting property values...");

            using (var context = new SampleDbContext())
            {
                CleanDatabase(context);

                ConsoleWriteLines("Save a new entity...");

                context.Add(new User { Name = "arthur", Password = "password" });
                context.SaveChanges();
            }

            using (var context = new SampleDbContext())
            {
                ConsoleWriteLines("Read the entity back...");

                var user = context.Set<User>().Single();

                ConsoleWriteLines($"User {user.Name} has password '{user.Password}'");
            }

            ConsoleWriteLines("Sample finished.");
        }

        public class SampleDbContext : DbContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                #region ConfigureEncryptPropertyValues
                modelBuilder.Entity<User>().Property(e => e.Password).HasConversion(
                    v => new string(v.Reverse().ToArray()),
                    v => new string(v.Reverse().ToArray()));
                #endregion
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EncryptPropertyValues;Integrated Security=True")
                    .EnableSensitiveDataLogging();
        }

        #region EncryptPropertyValuesModel
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
        }
        #endregion
    }
}
