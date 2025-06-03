using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Proxies;

public class ChangeTrackingProxiesSamples
{
    public static async Task Change_tracking_proxies_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Change_tracking_proxies_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Change_tracking_proxies_1
        using var context = new BlogsContext();
        var blog = await context.Blogs.Include(e => e.Posts).FirstAsync(e => e.Name == ".NET Blog");

        // Change a property value
        blog.Name = ".NET Blog (Updated!)";

        // Add a new entity to a navigation
        blog.Posts.Add(
            context.CreateProxy<Post>(
                p =>
                {
                    p.Title = "What’s next for System.Text.Json?";
                    p.Content = ".NET 5.0 was released recently and has come with many...";
                }));

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
            context.CreateProxy<Blog>(
                b =>
                {
                    b.Name = ".NET Blog";
                    b.Posts.Add(
                        context.CreateProxy<Post>(
                            p =>
                            {
                                p.Title = "Announcing the Release of EF Core 5.0";
                                p.Content = "Announcing the release of EF Core 5.0, a full featured cross-platform...";
                            }));
                    b.Posts.Add(
                        context.CreateProxy<Post>(
                            p =>
                            {
                                p.Title = "Announcing F# 5";
                                p.Content = "F# 5 is the latest version of F#, the functional programming language...";
                            }));
                }),
            context.CreateProxy<Blog>(
                b =>
                {
                    b.Name = "Visual Studio Blog";
                    b.Posts.Add(
                        context.CreateProxy<Post>(
                            p =>
                            {
                                p.Title = "Disassembly improvements for optimized managed debugging";
                                p.Content =
                                    "If you are focused on squeezing out the last bits of performance for your .NET service or...";
                            }));
                    b.Posts.Add(
                        context.CreateProxy<Post>(
                            p =>
                            {
                                p.Title = "Database Profiling with Visual Studio";
                                p.Content = "Examine when database queries were executed and measure how long the take using...";
                            }));
                }));

        await context.SaveChangesAsync();
    }
}

#region Model
public class Blog
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }

    public virtual IList<Post> Posts { get; } = new ObservableCollection<Post>();
}

public class Post
{
    public virtual int Id { get; set; }
    public virtual string Title { get; set; }
    public virtual string Content { get; set; }

    public virtual int BlogId { get; set; }
    public virtual Blog Blog { get; set; }
}
#endregion

public class BlogsContextBase : DbContext
{
    #region OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseChangeTrackingProxies();
    #endregion
}

public class BlogsContext : BlogsContextBase
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

        base.OnConfiguring(optionsBuilder);
    }
}
