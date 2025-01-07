using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExplicitKeys;

public static class ExplicitKeysSamples
{
    public static async Task Inserting_new_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inserting_new_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using var context = new BlogsContext();

        #region Inserting_new_entities_1
        context.Add(
            new Blog { Id = 1, Name = ".NET Blog", });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Inserting_new_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Inserting_new_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        using var context = new BlogsContext();

        #region Inserting_new_entities_2
        context.Add(
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
                    }
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Attaching_existing_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_existing_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Attaching_existing_entities_1
        context.Attach(
            new Blog { Id = 1, Name = ".NET Blog", });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Attaching_existing_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_existing_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Attaching_existing_entities_2
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
                    }
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Updating_existing_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Updating_existing_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Updating_existing_entities_1
        context.Update(
            new Blog { Id = 1, Name = ".NET Blog", });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Updating_existing_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Updating_existing_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Updating_existing_entities_2
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
                    }
                }
            });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Deleting_existing_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_existing_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Deleting_existing_entities_1
        context.Remove(
            new Post { Id = 2 });
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();
    }

    public static async Task Deleting_dependent_child_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_dependent_child_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var post = await GetDisconnectedPost();

        #region Deleting_dependent_child_entities_1
        context.Attach(post);
        context.Remove(post);
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();

        async Task<Post> GetDisconnectedPost()
        {
            using var tempContext = new BlogsContext();
            return await tempContext.Posts.FindAsync(2);
        }
    }

    public static async Task Deleting_dependent_child_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_dependent_child_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await GetDisconnectedBlogAndPosts();

        #region Deleting_dependent_child_entities_2
        // Attach a blog and associated posts
        context.Attach(blog);

        // Mark one post as Deleted
        context.Remove(blog.Posts[1]);
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();

        async Task<Blog> GetDisconnectedBlogAndPosts()
        {
            using var tempContext = new BlogsContext();
            return await tempContext.Blogs.Include(e => e.Posts).SingleAsync();
        }
    }

    public static async Task Deleting_principal_parent_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_principal_parent_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await GetDisconnectedBlogAndPosts();

        #region Deleting_principal_parent_entities_1
        // Attach a blog and associated posts
        context.Attach(blog);

        // Mark the blog as deleted
        context.Remove(blog);
        #endregion

        Console.WriteLine("Before SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

        Console.WriteLine("After SaveChanges:");
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        Console.WriteLine();

        async Task<Blog> GetDisconnectedBlogAndPosts()
        {
            using var tempContext = new BlogsContext();
            return await tempContext.Blogs.Include(e => e.Posts).SingleAsync();
        }
    }
}

public static class Helpers
{
    public static async Task RecreateCleanDatabase()
    {
        using var context = new BlogsContext(quiet: true);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task PopulateDatabase()
    {
        using var context = new BlogsContext(quiet: true);

        context.Add(
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
                }
            });

        await context.SaveChangesAsync();
    }
}

#region Model
public class Blog
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>();
}

public class Post
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
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
