using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public class Samples
{
    public static async Task Change_tracker_debug_view_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Change_tracker_debug_view_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Change_tracker_debug_view_1a
        using var context = new BlogsContext();

        var blogs = await context.Blogs
            .Include(e => e.Posts).ThenInclude(e => e.Tags)
            .Include(e => e.Assets)
            .ToListAsync();

        // Mark something Added
        blogs[0].Posts.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many new features and..."
            });

        // Mark something Deleted
        blogs[1].Posts.Remove(blogs[1].Posts[1]);

        // Make something Modified
        blogs[0].Name = ".NET Blog (All new!)";

        context.ChangeTracker.DetectChanges();
        #endregion

        Console.WriteLine();

        #region Change_tracker_debug_view_1b
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        #endregion

        Console.WriteLine();

        #region Change_tracker_debug_view_1c
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        Console.WriteLine();
    }

    public static async Task Change_tracker_logging_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Change_tracker_logging_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext(LogLevel.Debug);

        var blogs = await context.Blogs
            .Include(e => e.Posts).ThenInclude(e => e.Tags)
            .Include(e => e.Assets)
            .ToListAsync();

        // Mark something Added
        blogs[0].Posts.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many new features and..."
            });

        // Mark something Deleted
        blogs[1].Posts.Remove(blogs[1].Posts[1]);

        // Make something Modified
        blogs[0].Name = ".NET Blog (All new!)";

        context.ChangeTracker.DetectChanges();

        Console.WriteLine();
    }
}

public static class Helpers
{
    public static async Task RecreateCleanDatabase()
    {
        using var context = new BlogsContext(LogLevel.Error);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task PopulateDatabase()
    {
        using var context = new BlogsContext(LogLevel.Error);

        var blogs = new[]
        {
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
            }
        };

        var tags = new[]
        {
            new Tag { Text = ".NET", Posts = { blogs[0].Posts[0], blogs[0].Posts[1] } },
            new Tag { Text = "Visual Studio", Posts = { blogs[1].Posts[0], blogs[1].Posts[1] } },
            new Tag { Text = "EF Core", Posts = { blogs[0].Posts[0] } }
        };

        context.AddRange(blogs);
        context.AddRange(tags);

        await context.SaveChangesAsync();
    }
}

#region Model
public class Blog
{
    public int Id { get; set; } // Primary key
    public Guid AssetsId { get; set; } // Alternate key
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>(); // Collection navigation
    public BlogAssets Assets { get; set; } // Reference navigation
}

public class BlogAssets
{
    public Guid Id { get; set; } // Primary key and foreign key
    public byte[] Banner { get; set; }

    public Blog Blog { get; set; } // Reference navigation
}

public class Post
{
    public int Id { get; set; } // Primary key
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; } // Foreign key
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
    private readonly LogLevel _logLevel;

    public BlogsContext(LogLevel logLevel = LogLevel.Information)
    {
        _logLevel = logLevel;
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<BlogAssets> Assets { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, _logLevel)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning))
            .EnableSensitiveDataLogging()
            .UseSqlite("DataSource=test.db");
    }

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .Property(e => e.AssetsId)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<BlogAssets>()
            .HasOne(e => e.Blog)
            .WithOne(e => e.Assets)
            .HasForeignKey<BlogAssets>(e => e.Id)
            .HasPrincipalKey<Blog>(e => e.AssetsId);
    }
    #endregion
}
