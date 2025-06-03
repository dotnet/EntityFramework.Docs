// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFModeling.ValueConversions;

public class KeyValueObjects : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for a value objects used as keys...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save a new entity...");

            var blog = new Blog
            {
                Id = new BlogKey(1),
                Posts = new List<Post>
                {
                    new Post
                    {
                        Id = new PostKey(1)
                    },
                    new Post
                    {
                        Id = new PostKey(2)
                    },
                }
            };
            context.Add(blog);
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entity back...");

            var blog = await context.Set<Blog>().Include(e => e.Posts).SingleAsync();
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        #region ConfigureKeyValueObjects
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var blogKeyConverter = new ValueConverter<BlogKey, int>(
                v => v.Id,
                v => new BlogKey(v));

            modelBuilder.Entity<Blog>().Property(e => e.Id).HasConversion(blogKeyConverter);

            modelBuilder.Entity<Post>(
                b =>
                {
                    b.Property(e => e.Id).HasConversion(v => v.Id, v => new PostKey(v));
                    b.Property(e => e.BlogId).HasConversion(blogKeyConverter);
                });
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlite("DataSource=test.db")
                .EnableSensitiveDataLogging();
    }

    #region KeyValueObjectsModel
    public class Blog
    {
        public BlogKey Id { get; set; }
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public PostKey Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public BlogKey? BlogId { get; set; }
        public Blog Blog { get; set; }
    }
    #endregion

    #region KeyValueObjects
    public readonly struct BlogKey
    {
        public BlogKey(int id) => Id = id;
        public int Id { get; }
    }

    public readonly struct PostKey
    {
        public PostKey(int id) => Id = id;
        public int Id { get; }
    }
    #endregion
}
