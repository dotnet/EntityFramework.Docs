using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntroOptional;

public static class IntroOptionalSamples
{
    public static async Task Deleting_principal_parent_1b()
    {
        Console.WriteLine($">>>> Sample: {nameof(Deleting_principal_parent_1b)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Deleting_principal_parent_1b
        using var context = new BlogsContext();

        var blog = await context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).FirstAsync();

        context.Remove(blog);

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public static async Task Severing_a_relationship_1b()
    {
        Console.WriteLine($">>>> Sample: {nameof(Severing_a_relationship_1b)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Severing_a_relationship_1b
        using var context = new BlogsContext();

        var blog = await context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).FirstAsync();

        foreach (var post in blog.Posts)
        {
            post.Blog = null;
        }

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public static async Task Severing_a_relationship_2b()
    {
        Console.WriteLine($">>>> Sample: {nameof(Severing_a_relationship_2b)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Severing_a_relationship_2b
        using var context = new BlogsContext();

        var blog = await context.Blogs.OrderBy(e => e.Name).Include(e => e.Posts).FirstAsync();

        blog.Posts.Clear();

        await context.SaveChangesAsync();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .HasMany(e => e.Posts)
            .WithOne(e => e.Blog)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Scratch;Trusted_Connection=True;ConnectRetryCount=0");
        //.UseSqlite("DataSource=test.db");

        if (!_quiet)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
        }
    }
}
