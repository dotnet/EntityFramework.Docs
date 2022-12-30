﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Optional;

public class OptionalRelationshipsSamples
{
    public static void Relationship_fixup_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Relationship_fixup_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Relationship_fixup_1
        using var context = new BlogsContext();

        var blogs = context.Blogs
            .Include(e => e.Posts)
            .Include(e => e.Assets)
            .ToList();

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }

    public static void Relationship_fixup_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Relationship_fixup_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Relationship_fixup_2
        using var context = new BlogsContext();

        var blogs = context.Blogs.ToList();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        var assets = context.Assets.ToList();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        var posts = context.Posts.ToList();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }

    public static void Changing_relationships_using_navigations_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Changing_relationships_using_navigations_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Changing_relationships_using_navigations_1
        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");
        var vsBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == "Visual Studio Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        var post = vsBlog.Posts.Single(e => e.Title.StartsWith("Disassembly improvements"));
        vsBlog.Posts.Remove(post);
        dotNetBlog.Posts.Add(post);

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        #endregion

        Console.WriteLine();
    }

    public static void Changing_relationships_using_navigations_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Changing_relationships_using_navigations_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");
        var vsBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == "Visual Studio Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        #region Changing_relationships_using_navigations_2
        var post = vsBlog.Posts.Single(e => e.Title.StartsWith("Disassembly improvements"));
        post.Blog = dotNetBlog;
        #endregion

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Changing_relationships_using_foreign_key_values_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Changing_relationships_using_foreign_key_values_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");
        var vsBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == "Visual Studio Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        #region Changing_relationships_using_foreign_key_values_1
        var post = vsBlog.Posts.Single(e => e.Title.StartsWith("Disassembly improvements"));
        post.BlogId = dotNetBlog.Id;
        #endregion

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Fixup_for_added_or_deleted_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Fixup_for_added_or_deleted_entities_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");
        var vsBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == "Visual Studio Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        #region Fixup_for_added_or_deleted_entities_1
        var post = vsBlog.Posts.Single(e => e.Title.StartsWith("Disassembly improvements"));
        vsBlog.Posts.Remove(post);
        dotNetBlog.Posts.Add(post);
        #endregion

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Fixup_for_added_or_deleted_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Fixup_for_added_or_deleted_entities_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");
        var vsBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == "Visual Studio Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        #region Fixup_for_added_or_deleted_entities_2
        var post = vsBlog.Posts.Single(e => e.Title.StartsWith("Disassembly improvements"));
        dotNetBlog.Posts.Add(post);
        #endregion

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Fixup_for_added_or_deleted_entities_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Fixup_for_added_or_deleted_entities_3)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Posts).Single(e => e.Name == ".NET Blog");

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        #region Fixup_for_added_or_deleted_entities_3
        var post = dotNetBlog.Posts.Single(e => e.Title == "Announcing F# 5");
        dotNetBlog.Posts.Remove(post);
        #endregion

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Fixup_for_added_or_deleted_entities_7()
    {
        Console.WriteLine($">>>> Sample: {nameof(Fixup_for_added_or_deleted_entities_7)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Fixup_for_added_or_deleted_entities_7
        using var context = new BlogsContext();

        var dotNetBlog = context.Blogs.Include(e => e.Assets).Single(e => e.Name == ".NET Blog");
        dotNetBlog.Assets = new BlogAssets();

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        #endregion

        Console.WriteLine();
    }

    public static void Deleting_an_entity_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_an_entity_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Deleting_an_entity_1
        using var context = new BlogsContext();

        var vsBlog = context.Blogs
            .Include(e => e.Posts)
            .Include(e => e.Assets)
            .Single(e => e.Name == "Visual Studio Blog");

        context.Remove(vsBlog);

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        #endregion

        Console.WriteLine();
    }

    public static void Many_to_many_relationships_6()
    {
        Console.WriteLine($">>>> Sample: {nameof(Many_to_many_relationships_6)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Many_to_many_relationships_6
        using var context = new BlogsContext();

        var post = context.Posts.Single(e => e.Id == 3);
        var tag = context.Tags.Single(e => e.Id == 1);

        post.Tags.Add(tag);

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }
}

public static class Helpers
{
    public static void RecreateCleanDatabase()
    {
        using var context = new BlogsContext(quiet: true);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public static void PopulateDatabase()
    {
        using var context = new BlogsContext(quiet: true);

        context.AddRange(
            new Blog
            {
                Name = ".NET Blog",
                Assets = new BlogAssets(),
                Posts =
                {
                    new Post
                    {
                        Title = "Announcing the Release of EF Core 5.0",
                        Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                    },
                    new Post
                    {
                        Title = "Announcing F# 5",
                        Content = "F# 5 is the latest version of F#, the functional programming language..."
                    },
                },
            },
            new Blog
            {
                Name = "Visual Studio Blog",
                Assets = new BlogAssets(),
                Posts =
                {
                    new Post
                    {
                        Title = "Disassembly improvements for optimized managed debugging",
                        Content =
                            "If you are focused on squeezing out the last bits of performance for your .NET service or..."
                    },
                    new Post
                    {
                        Title = "Database Profiling with Visual Studio",
                        Content = "Examine when database queries were executed and measure how long the take using..."
                    },
                }
            },
            new Tag { Text = ".NET" },
            new Tag { Text = "Visual Studio" },
            new Tag { Text = "EF Core" });

        context.SaveChanges();
    }
}

#region Model
public class Blog
{
    public int Id { get; set; } // Primary key
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>(); // Collection navigation
    public BlogAssets Assets { get; set; } // Reference navigation
}

public class BlogAssets
{
    public int Id { get; set; } // Primary key
    public byte[] Banner { get; set; }

    public int? BlogId { get; set; } // Foreign key
    public Blog Blog { get; set; } // Reference navigation
}

public class Post
{
    public int Id { get; set; } // Primary key
    public string Title { get; set; }
    public string Content { get; set; }

    public int? BlogId { get; set; } // Foreign key
    public Blog Blog { get; set; } // Reference navigation

    public IList<Tag> Tags { get; } = new List<Tag>(); // Skip collection navigation
}

public class Tag
{
    public int Id { get; set; } // Primary key
    public string Text { get; set; }

    public IList<Post> Posts { get; } = new List<Post>(); // Skip collection navigation
}
#endregion

public class BlogsContext : DbContext
{
    private readonly bool _quiet;

    public BlogsContext(bool quiet = false)
    {
        _quiet = quiet;
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<BlogAssets> Assets { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlite("DataSource=test.db");

        if (!_quiet)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}