// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFModeling.ValueConversions;

public class PreserveDateTimeKind : Program
{
    public async Task Run()
    {
        ConsoleWriteLines("Sample showing value conversions for preserving/setting DateTime.Kind...");

        using (var context = new SampleDbContext())
        {
            await CleanDatabase(context);

            ConsoleWriteLines("Save new entities...");

            context.AddRange(
                new Post
                {
                    Title = "Post 1",
                    PostedOn = new DateTime(1973, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdated = new DateTime(1974, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc),
                    DeletedOn = new DateTime(2007, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc)
                },
                new Post
                {
                    Title = "Post 2",
                    PostedOn = new DateTime(1975, 9, 3, 0, 0, 0, 0, DateTimeKind.Local),
                    LastUpdated = new DateTime(1976, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc),
                    DeletedOn = new DateTime(2017, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc)
                });
            await context.SaveChangesAsync();
        }

        using (var context = new SampleDbContext())
        {
            ConsoleWriteLines("Read the entities back...");

            var blog1 = await context.Set<Post>().SingleAsync(e => e.Title == "Post 1");

            ConsoleWriteLines($"Blog 1: PostedOn.Kind = {blog1.PostedOn.Kind} LastUpdated.Kind = {blog1.LastUpdated.Kind} DeletedOn.Kind = {blog1.DeletedOn.Kind}");

            var blog2 = await context.Set<Post>().SingleAsync(e => e.Title == "Post 2");

            ConsoleWriteLines($"Blog 2: PostedOn.Kind = {blog2.PostedOn.Kind} LastUpdated.Kind = {blog2.LastUpdated.Kind} DeletedOn.Kind = {blog2.DeletedOn.Kind}");
        }

        ConsoleWriteLines("Sample finished.");
    }

    public class SampleDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigurePreserveDateTimeKind1
            modelBuilder.Entity<Post>()
                .Property(e => e.PostedOn)
                .HasConversion<long>();
            #endregion

            #region ConfigurePreserveDateTimeKind2
            modelBuilder.Entity<Post>()
                .Property(e => e.LastUpdated)
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc));
            #endregion

            #region ConfigurePreserveDateTimeKind3
            modelBuilder.Entity<Post>()
                .Property(e => e.LastUpdated)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => new DateTime(v.Ticks, DateTimeKind.Utc));
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PreserveDateTimeKind;Trusted_Connection=True;ConnectRetryCount=0")
                .EnableSensitiveDataLogging();
    }

    #region PreserveDateTimeKindModel
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime PostedOn { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DeletedOn { get; set; }
    }
    #endregion
}
