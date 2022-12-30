﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GeneratedKeys;

public static class GeneratedKeysSamples
{
    public static void Simple_query_and_update_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Simple_query_and_update_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Simple_query_and_update_1
        using var context = new BlogsContext();

        var blog = context.Blogs.Include(e => e.Posts).First(e => e.Name == ".NET Blog");

        blog.Name = ".NET Blog (Updated!)";

        foreach (var post in blog.Posts.Where(e => !e.Title.Contains("5.0")))
        {
            post.Title = post.Title.Replace("5", "5.0");
        }

        context.SaveChanges();
        #endregion

        Console.WriteLine();
    }

    public static void Simple_query_and_update_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Simple_query_and_update_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = context.Blogs.Include(e => e.Posts).First(e => e.Name == ".NET Blog");

        blog.Name = ".NET Blog (Updated!)";

        foreach (var post in blog.Posts.Where(e => !e.Title.Contains("5.0")))
        {
            post.Title = post.Title.Replace("5", "5.0");
        }

        #region Simple_query_and_update_2
        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        context.SaveChanges();

        Console.WriteLine();
    }

    public static void Query_then_insert_update_and_delete_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Query_then_insert_update_and_delete_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Query_then_insert_update_and_delete_1
        using var context = new BlogsContext();

        var blog = context.Blogs.Include(e => e.Posts).First(e => e.Name == ".NET Blog");

        // Modify property values
        blog.Name = ".NET Blog (Updated!)";

        // Insert a new Post
        blog.Posts.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?", Content = ".NET 5.0 was released recently and has come with many..."
            });

        // Mark an existing Post as Deleted
        var postToDelete = blog.Posts.Single(e => e.Title == "Announcing F# 5");
        context.Remove(postToDelete);

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        #endregion
    }

    public static void Inserting_new_entities_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inserting_new_entities_3)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();

        using var context = new BlogsContext();

        #region Inserting_new_entities_3
        context.Add(
            new Blog
            {
                Name = ".NET Blog",
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
                    }
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static void Attaching_existing_entities_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_existing_entities_3)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Attaching_existing_entities_3
        context.Attach(
            new Blog
            {
                Id = 1,
                Name = ".NET Blog",
                Posts =
                {
                    new Post
                    {
                        Id = 1,
                        Title = "Announcing the Release of EF Core 5.0",
                        Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                    },
                    new Post
                    {
                        Id = 2,
                        Title = "Announcing F# 5",
                        Content = "F# 5 is the latest version of F#, the functional programming language..."
                    },
                    new Post
                    {
                        Title = "Announcing .NET 5.0",
                        Content = ".NET 5.0 includes many enhancements, including single file applications, more..."
                    },
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static void Updating_existing_entities_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Updating_existing_entities_3)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Updating_existing_entities_3
        context.Update(
            new Blog
            {
                Id = 1,
                Name = ".NET Blog",
                Posts =
                {
                    new Post
                    {
                        Id = 1,
                        Title = "Announcing the Release of EF Core 5.0",
                        Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
                    },
                    new Post
                    {
                        Id = 2,
                        Title = "Announcing F# 5",
                        Content = "F# 5 is the latest version of F#, the functional programming language..."
                    },
                    new Post
                    {
                        Title = "Announcing .NET 5.0",
                        Content = ".NET 5.0 includes many enhancements, including single file applications, more..."
                    },
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static void Custom_tracking_with_TrackGraph_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Custom_tracking_with_TrackGraph_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = context.Blogs.AsNoTracking().Include(e => e.Posts).Single(e => e.Name == ".NET Blog");

        #region Custom_tracking_with_TrackGraph_1a
        blog.Posts.Add(
            new Post
            {
                Title = "Announcing .NET 5.0",
                Content = ".NET 5.0 includes many enhancements, including single file applications, more..."
            }
        );

        var toDelete = blog.Posts.Single(e => e.Title == "Announcing F# 5");
        toDelete.Id = -toDelete.Id;
        #endregion

        UpdateBlog(blog);

        Console.WriteLine();
    }

    #region Custom_tracking_with_TrackGraph_1b
    public static void UpdateBlog(Blog blog)
    {
        using var context = new BlogsContext();

        context.ChangeTracker.TrackGraph(
            blog, node =>
            {
                var propertyEntry = node.Entry.Property("Id");
                var keyValue = (int)propertyEntry.CurrentValue;

                if (keyValue == 0)
                {
                    node.Entry.State = EntityState.Added;
                }
                else if (keyValue < 0)
                {
                    propertyEntry.CurrentValue = -keyValue;
                    node.Entry.State = EntityState.Deleted;
                }
                else
                {
                    node.Entry.State = EntityState.Modified;
                }

                Console.WriteLine($"Tracking {node.Entry.Metadata.DisplayName()} with key value {keyValue} as {node.Entry.State}");
            });

        context.SaveChanges();
    }
    #endregion
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

        context.Add(
            new Blog
            {
                Name = ".NET Blog",
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
                }
            });

        context.SaveChanges();
    }
}

#region Model
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int? BlogId { get; set; }
    public Blog Blog { get; set; }
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
}