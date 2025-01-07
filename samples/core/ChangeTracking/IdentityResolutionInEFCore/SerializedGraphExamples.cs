using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Graphs;

public static class SerializedGraphExamples
{
    public static async Task Attaching_a_serialized_graph_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_a_serialized_graph_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Attaching_a_serialized_graph_1a
        using var context = new BlogsContext();

        var blogs = await context.Blogs.Include(e => e.Posts).ToListAsync();

        var serialized = JsonConvert.SerializeObject(
            blogs,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

        Console.WriteLine(serialized);
        #endregion

        await UpdateBlogsFromJson(serialized);
    }

    #region Attaching_a_serialized_graph_1b
    public static async Task UpdateBlogsFromJson(string json)
    {
        using var context = new BlogsContext();

        var blogs = JsonConvert.DeserializeObject<List<Blog>>(json);

        foreach (var blog in blogs)
        {
            context.Update(blog);
        }

        await context.SaveChangesAsync();
    }
    #endregion

    public static async Task Attaching_a_serialized_graph_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_a_serialized_graph_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Attaching_a_serialized_graph_2
        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        var serialized = JsonConvert.SerializeObject(
            posts,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

        Console.WriteLine(serialized);
        #endregion

        await UpdatePostsFromJsonBad(serialized);
    }

    public static async Task UpdatePostsFromJsonBad(string json)
    {
        using var context = new BlogsContext();

        var posts = JsonConvert.DeserializeObject<List<Post>>(json);

        try
        {
            foreach (var post in posts)
            {
                context.Update(post); // Will throw
            }

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
        }
    }

    public static async Task Attaching_a_serialized_graph_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_a_serialized_graph_3)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        #region Attaching_a_serialized_graph_3
        var serialized = JsonConvert.SerializeObject(
            posts,
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All, Formatting = Formatting.Indented
            });
        #endregion

        Console.WriteLine(serialized);

        await UpdatePostsFromJson(serialized);
    }

    public static async Task UpdatePostsFromJson(string json)
    {
        using var context = new BlogsContext();

        var posts = JsonConvert.DeserializeObject<List<Post>>(json);

        foreach (var post in posts)
        {
            context.Update(post);
        }

        await context.SaveChangesAsync();
    }

    public static async Task Attaching_a_serialized_graph_4()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_a_serialized_graph_4)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        #region Attaching_a_serialized_graph_4
        var serialized = JsonSerializer.Serialize(
            posts, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true });
        #endregion

        Console.WriteLine(serialized);

        await UpdatePostsFromJson(serialized);
    }

    public static async Task Attaching_a_serialized_graph_5()
    {
        Console.WriteLine($">>>> Sample: {nameof(Attaching_a_serialized_graph_4)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        var serialized = JsonConvert.SerializeObject(
            posts,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented });

        Console.WriteLine(serialized);

        Console.WriteLine();

        await UpdatePostsFromJsonWithIdentityResolution(serialized);
    }

    #region Attaching_a_serialized_graph_5
    public static async Task UpdatePostsFromJsonWithIdentityResolution(string json)
    {
        using var context = new BlogsContext();

        var posts = JsonConvert.DeserializeObject<List<Post>>(json);

        foreach (var post in posts)
        {
            context.ChangeTracker.TrackGraph(
                post, node =>
                {
                    var keyValue = node.Entry.Property("Id").CurrentValue;
                    var entityType = node.Entry.Metadata;

                    var existingEntity = node.Entry.Context.ChangeTracker.Entries()
                        .FirstOrDefault(
                            e => Equals(e.Metadata, entityType)
                                 && Equals(e.Property("Id").CurrentValue, keyValue));

                    if (existingEntity == null)
                    {
                        Console.WriteLine($"Tracking {entityType.DisplayName()} entity with key value {keyValue}");

                        node.Entry.State = EntityState.Modified;
                    }
                    else
                    {
                        Console.WriteLine($"Discarding duplicate {entityType.DisplayName()} entity with key value {keyValue}");
                    }
                });
        }

        await context.SaveChangesAsync();
    }
    #endregion
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
                Summary = "Posts about .NET",
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
                Summary = "Posts about Visual Studio",
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
    public string Summary { get; set; }

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
