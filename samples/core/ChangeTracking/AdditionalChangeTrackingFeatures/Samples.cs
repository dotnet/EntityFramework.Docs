using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Optional;

public class Samples
{
    public static async Task DbContext_versus_DbSet_methods_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(DbContext_versus_DbSet_methods_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region DbContext_versus_DbSet_methods_1
        using var context = new BlogsContext();

        var post = await context.Posts.SingleAsync(e => e.Id == 3);
        var tag = await context.Tags.SingleAsync(e => e.Id == 1);

        var joinEntitySet = context.Set<Dictionary<string, int>>("PostTag");
        var joinEntity = new Dictionary<string, int> { ["PostId"] = post.Id, ["TagId"] = tag.Id };
        joinEntitySet.Add(joinEntity);

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public static async Task Temporary_values_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Temporary_values_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Temporary_values_1
        using var context = new BlogsContext();

        var blog = new Blog { Name = ".NET Blog" };

        context.Add(blog);

        Console.WriteLine($"Blog.Id set on entity is {blog.Id}");
        Console.WriteLine($"Blog.Id tracked by EF is {context.Entry(blog).Property(e => e.Id).CurrentValue}");
        #endregion

        Console.WriteLine();
    }

    public static async Task Temporary_values_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Temporary_values_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Temporary_values_2
        var blogs = new List<Blog> { new Blog { Id = -1, Name = ".NET Blog" }, new Blog { Id = -2, Name = "Visual Studio Blog" } };

        var posts = new List<Post>
        {
            new Post
            {
                Id = -1,
                BlogId = -1,
                Title = "Announcing the Release of EF Core 5.0",
                Content = "Announcing the release of EF Core 5.0, a full featured cross-platform..."
            },
            new Post
            {
                Id = -2,
                BlogId = -2,
                Title = "Disassembly improvements for optimized managed debugging",
                Content = "If you are focused on squeezing out the last bits of performance for your .NET service or..."
            }
        };

        using var context = new BlogsContext();

        foreach (var blog in blogs)
        {
            context.Add(blog).Property(e => e.Id).IsTemporary = true;
        }

        foreach (var post in posts)
        {
            context.Add(post).Property(e => e.Id).IsTemporary = true;
        }

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

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
            },
            new Tag { Text = ".NET" },
            new Tag { Text = "Visual Studio" },
            new Tag { Text = "EF Core" });

        await context.SaveChangesAsync();
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

    public IList<Tag> Tags { get; } = new List<Tag>();
}

public class Tag
{
    public int Id { get; set; } // Primary key
    public string Text { get; set; }

    public IList<Post> Posts { get; } = new List<Post>(); // Collection navigation
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

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .SharedTypeEntity<Dictionary<string, int>>(
                "PostTag",
                b =>
                {
                    b.IndexerProperty<int>("TagId");
                    b.IndexerProperty<int>("PostId");
                });

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Posts)
            .UsingEntity<Dictionary<string, int>>(
                "PostTag",
                j => j.HasOne<Tag>().WithMany(),
                j => j.HasOne<Post>().WithMany());
    }
    #endregion
}
