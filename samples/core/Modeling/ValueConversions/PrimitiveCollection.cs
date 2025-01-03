// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class PrimitiveCollection : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a collections of primitive values...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            context.Add(new Post { Tags = new List<string> { "EF Core", "Unicorns", "Donkeys" } });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var post = await context.Set<Post>().SingleAsync();

            ConsoleWriteLines($"Post with tags {string.Join(", ", post.Tags)}.");

            ConsoleWriteLines("Changing the value object and saving again");

            post.Tags.Add("ASP.NET Core");
            await context.SaveChangesAsync();
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigurePrimitiveCollection
            modelBuilder.Entity<Post>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                    new ValueComparer<ICollection<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => (ICollection<string>)c.ToList()));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    #region PrimitiveCollectionModel
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        public ICollection<string> Tags { get; set; }
    }
    #endregion
}
