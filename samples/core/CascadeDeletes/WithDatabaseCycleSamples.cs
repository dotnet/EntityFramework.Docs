using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DatabaseCycles;

public static class WithDatabaseCycleSamples
{
    public static void Database_cascade_limitations_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Database_cascade_limitations_1)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        #region Database_cascade_limitations_1
        using var context = new BlogsContext();

        var owner = context.People.Single(e => e.Name == "ajcvickers");
        var blog = context.Blogs.Single(e => e.Owner == owner);

        context.Remove(owner);

        context.SaveChanges();
        #endregion

        Console.WriteLine();
    }

    public static void Database_cascade_limitations_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Database_cascade_limitations_2)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        try
        {
            #region Database_cascade_limitations_2
            using var context = new BlogsContext();

            var owner = context.People.Single(e => e.Name == "ajcvickers");

            context.Remove(owner);

            context.SaveChanges();
            #endregion
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            if (e.InnerException != null)
            {
                Console.WriteLine($"{e.InnerException.GetType().FullName}: {e.InnerException.Message}");
            }
        }

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

        var person = new Person { Name = "ajcvickers" };

        context.Add(
            new Blog
            {
                Owner = person,
                Name = ".NET Blog",
                Posts =
                {
                    new Post
                    {
                        Title = "Announcing the Release of EF Core 5.0",
                        Content = "Announcing the release of EF Core 5.0, a full featured cross-platform...",
                        Author = person
                    },
                    new Post
                    {
                        Title = "Announcing F# 5",
                        Content = "F# 5 is the latest version of F#, the functional programming language...",
                        Author = person
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

    public int OwnerId { get; set; }
    public Person Owner { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public int AuthorId { get; set; }
    public Person Author { get; set; }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>();

    public Blog OwnedBlog { get; set; }
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
    public DbSet<Person> People { get; set; }

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .HasOne(e => e.Owner)
            .WithOne(e => e.OwnedBlog)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Scratch;Trusted_Connection=True");
        //.UseSqlite("DataSource=test.db");

        if (!_quiet)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
        }
    }
}