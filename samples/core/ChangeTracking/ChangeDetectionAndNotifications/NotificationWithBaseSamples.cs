using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NotificationWithBase;

public class NotificationWithBaseSamples
{
    public static async Task Notification_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Notification_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Notification_entities_2
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

public class Post : NotifyingEntity
{
    private int _id;
    private string _title;
    private string _content;
    private int? _blogId;
    private Blog _blog;

    public int Id
    {
        get => _id;
        set => SetWithNotify(value, out _id);
    }

    public string Title
    {
        get => _title;
        set => SetWithNotify(value, out _title);
    }

    public string Content
    {
        get => _content;
        set => SetWithNotify(value, out _content);
    }

    public int? BlogId
    {
        get => _blogId;
        set => SetWithNotify(value, out _blogId);
    }

    public Blog Blog
    {
        get => _blog;
        set => SetWithNotify(value, out _blog);
    }
}

#region Model
public class Blog : NotifyingEntity
{
    private int _id;

    public int Id
    {
        get => _id;
        set => SetWithNotify(value, out _id);
    }

    private string _name;

    public string Name
    {
        get => _name;
        set => SetWithNotify(value, out _name);
    }

    public IList<Post> Posts { get; } = new ObservableCollection<Post>();
}

public abstract class NotifyingEntity : INotifyPropertyChanging, INotifyPropertyChanged
{
    protected void SetWithNotify<T>(T value, out T field, [CallerMemberName] string propertyName = "")
    {
        NotifyChanging(propertyName);
        field = value;
        NotifyChanged(propertyName);
    }

    public event PropertyChangingEventHandler PropertyChanging;
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void NotifyChanging(string propertyName)
        => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
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

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }
    #endregion
}
