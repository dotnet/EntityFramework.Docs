using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Snapshot;

public class SnapshotSamples
{
    public static async Task Snapshot_change_tracking_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Snapshot_change_tracking_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Snapshot_change_tracking_1
        using var context = new BlogsContext();
        var blog = await context.Blogs.Include(e => e.Posts).FirstAsync(e => e.Name == ".NET Blog");

        // Change a property value
        blog.Name = ".NET Blog (Updated!)";

        // Add a new entity to a navigation
        blog.Posts.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?", Content = ".NET 5.0 was released recently and has come with many..."
            });

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }

    public static async Task Snapshot_change_tracking_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Snapshot_change_tracking_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Snapshot_change_tracking_2
        using var context = new BlogsContext();
        var blog = await context.Blogs.Include(e => e.Posts).FirstAsync(e => e.Name == ".NET Blog");

        // Change a property value
        context.Entry(blog).Property(e => e.Name).CurrentValue = ".NET Blog (Updated!)";

        // Add a new entity to the DbContext
        context.Add(
            new Post
            {
                Blog = blog,
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many..."
            });

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
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

        context.AddRange(
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
                },
            },
            new Blog
            {
                Name = "Visual Studio Blog",
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
            });

        await context.SaveChangesAsync();
    }
}

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

public class PostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }

    public DateTime TaggedOn { get; set; }
    public string TaggedBy { get; set; }
}

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostTag>().HasKey(e => new { e.PostId, e.TagId });
    }

    #region SaveChanges
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entityEntry in ChangeTracker.Entries<PostTag>()) // Detects changes automatically
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.TaggedBy = "ajcvickers";
                entityEntry.Entity.TaggedOn = DateTime.Now;
            }
        }

        try
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            return await base.SaveChangesAsync(cancellationToken); // Avoid automatically detecting changes again here
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
    #endregion
}
