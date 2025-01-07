// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class CaseInsensitiveStrings : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for case-insensitive string keys...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save new entities...");

            context.AddRange(
                new Blog
                {
                    Id = "dotnet",
                    Name = ".NET Blog",
                },
                new Post
                {
                    Id = "post1",
                    BlogId = "dotnet",
                    Title = "Some good .NET stuff"
                },
                new Post
                {
                    Id = "Post2",
                    BlogId = "DotNet",
                    Title = "Some more good .NET stuff"
                });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entities back...");

            var blog = await context.Set<Blog>().Include(e => e.Posts).SingleAsync();

            ConsoleWriteLines($"The blog has {blog.Posts.Count} posts with foreign keys '{blog.Posts.First().BlogId}' and '{blog.Posts.Skip(1).First().BlogId}'");
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        #region ConfigureCaseInsensitiveStrings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var comparer = new ValueComparer<string>(
                (l, r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase),
                v => v.ToUpper().GetHashCode(),
                v => v);

            modelBuilder.Entity<Blog>()
                .Property(e => e.Id)
                .Metadata.SetValueComparer(comparer);

            modelBuilder.Entity<Post>(
                b =>
                {
                    b.Property(e => e.Id).Metadata.SetValueComparer(comparer);
                    b.Property(e => e.BlogId).Metadata.SetValueComparer(comparer);
                });
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CaseInsensitiveStrings;Trusted_Connection=True;ConnectRetryCount=0")
                .EnableSensitiveDataLogging();
    }

    #region CaseInsensitiveStringsModel
    public class Blog
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string BlogId { get; set; }
        public Blog Blog { get; set; }
    }
    #endregion
}
