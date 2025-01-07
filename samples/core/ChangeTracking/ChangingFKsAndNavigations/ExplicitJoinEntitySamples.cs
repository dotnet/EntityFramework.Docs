using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace JoinEntity;

public class ExplicitJoinEntitySamples
{
    public static async Task Many_to_many_relationships_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Many_to_many_relationships_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Many_to_many_relationships_1
        using var context = new BlogsContext();

        var post = await context.Posts.SingleAsync(e => e.Id == 3);
        var tag = await context.Tags.SingleAsync(e => e.Id == 1);

        context.Add(new PostTag { PostId = post.Id, TagId = tag.Id });

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        #endregion

        await context.SaveChangesAsync();

        Console.WriteLine();
    }

    public static async Task Many_to_many_relationships_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Many_to_many_relationships_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var post = await context.Posts.SingleAsync(e => e.Id == 3);
        var tag = await context.Tags.SingleAsync(e => e.Id == 1);

        #region Many_to_many_relationships_2
        context.Add(new PostTag { Post = post, Tag = tag });
        #endregion

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();

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

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>();
}

#region Model
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int? BlogId { get; set; }
    public Blog Blog { get; set; }

    public IList<PostTag> PostTags { get; } = new List<PostTag>(); // Collection navigation
}

public class Tag
{
    public int Id { get; set; }
    public string Text { get; set; }

    public IList<PostTag> PostTags { get; } = new List<PostTag>(); // Collection navigation
}

public class PostTag
{
    public int PostId { get; set; } // First part of composite PK; FK to Post
    public int TagId { get; set; } // Second part of composite PK; FK to Tag

    public Post Post { get; set; } // Reference navigation
    public Tag Tag { get; set; } // Reference navigation
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostTag>().HasKey(e => new { e.PostId, e.TagId });
    }
}
