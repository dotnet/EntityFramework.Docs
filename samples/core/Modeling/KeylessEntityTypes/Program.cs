using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#pragma warning disable 169

namespace EFModeling.KeylessEntityTypes;

public class Program
{
    private static async Task Main()
    {
        await SetupDatabase();

        using var db = new BloggingContext();

        #region Query
        var postCounts = await db.BlogPostCounts.ToListAsync();

        foreach (var postCount in postCounts)
        {
            Console.WriteLine($"{postCount.BlogName} has {postCount.PostCount} posts.");
            Console.WriteLine();
        }
        #endregion
    }

    private static async Task SetupDatabase()
    {
        using var db = new BloggingContext();

        if (await db.Database.EnsureCreatedAsync())
        {
            db.Blogs.Add(
                new Blog
                {
                    Name = "Fish Blog",
                    Url = "http://sample.com/blogs/fish",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Fish care 101" },
                        new Post { Title = "Caring for tropical fish" },
                        new Post { Title = "Types of ornamental fish" }
                    }
                });

            db.Blogs.Add(
                new Blog
                {
                    Name = "Cats Blog",
                    Url = "http://sample.com/blogs/cats",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Cat care 101" },
                        new Post { Title = "Caring for tropical cats" },
                        new Post { Title = "Types of ornamental cats" }
                    }
                });

            db.Blogs.Add(
                new Blog
                {
                    Name = "Catfish Blog",
                    Url = "http://sample.com/blogs/catfish",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Catfish care 101" }, new Post { Title = "History of the catfish name" }
                    }
                });

            await db.SaveChangesAsync();

            #region View
            await db.Database.ExecuteSqlRawAsync(
                @"CREATE VIEW View_BlogPostCounts AS
                            SELECT b.Name, Count(p.PostId) as PostCount
                            FROM Blogs b
                            JOIN Posts p on p.BlogId = b.BlogId
                            GROUP BY b.Name");
            #endregion
        }
    }
}

public class BloggingContext : DbContext
{
    private static readonly ILoggerFactory _loggerFactory
        = LoggerFactory.Create(
            builder => builder.AddConsole().AddFilter((c, l) => l == LogLevel.Information && !c.EndsWith("Connection")));

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    #region DbSet
    public DbSet<BlogPostsCount> BlogPostCounts { get; set; }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Sample.KeylessEntityTypes;Trusted_Connection=True;ConnectRetryCount=0")
            .UseLoggerFactory(_loggerFactory);
    }

    #region Configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<BlogPostsCount>(
                eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("View_BlogPostCounts");
                    eb.Property(v => v.BlogName).HasColumnName("Name");
                });
    }
    #endregion
}

#region Entities
public class Blog
{
    public int BlogId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public ICollection<Post> Posts { get; set; }
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int BlogId { get; set; }
}
#endregion

#region KeylessEntityType
public class BlogPostsCount
{
    public string BlogName { get; set; }
    public int PostCount { get; set; }
}
#endregion
