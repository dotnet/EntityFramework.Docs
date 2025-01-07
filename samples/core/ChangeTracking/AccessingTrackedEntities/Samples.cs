using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

public static class Samples
{
    public static async Task Using_DbContext_Entry_and_EntityEntry_instances_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_DbContext_Entry_and_EntityEntry_instances_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Using_DbContext_Entry_and_EntityEntry_instances_1
        using var context = new BlogsContext();

        var blog = await context.Blogs.SingleAsync(e => e.Id == 1);
        var entityEntry = context.Entry(blog);
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_the_entity_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_the_entity_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

        #region Work_with_the_entity_1
        var currentState = context.Entry(blog).State;
        if (currentState == EntityState.Unchanged)
        {
            context.Entry(blog).State = EntityState.Modified;
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_the_entity_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_the_entity_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        #region Work_with_the_entity_2
        var newBlog = new Blog();
        Debug.Assert(context.Entry(newBlog).State == EntityState.Detached);

        context.Entry(newBlog).State = EntityState.Added;
        Debug.Assert(context.Entry(newBlog).State == EntityState.Added);
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_a_single_property_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_a_single_property_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        {
            var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

            #region Work_with_a_single_property_1a
            PropertyEntry<Blog, string> propertyEntry = context.Entry(blog).Property(e => e.Name);
            #endregion
        }

        {
            var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

            #region Work_with_a_single_property_1b
            PropertyEntry<Blog, string> propertyEntry = context.Entry(blog).Property<string>("Name");
            #endregion
        }

        {
            var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

            #region Work_with_a_single_property_1c
            PropertyEntry propertyEntry = context.Entry(blog).Property("Name");
            #endregion
        }

        {
            var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

            #region Work_with_a_single_property_1d
            string currentValue = context.Entry(blog).Property(e => e.Name).CurrentValue;
            context.Entry(blog).Property(e => e.Name).CurrentValue = "1unicorn2";
            #endregion
        }

        {
            #region Work_with_a_single_property_1e
            object blog = await context.Blogs.SingleAsync(e => e.Id == 1);

            object currentValue = context.Entry(blog).Property("Name").CurrentValue;
            context.Entry(blog).Property("Name").CurrentValue = "1unicorn2";
            #endregion
        }

        Console.WriteLine();
    }

    public static async Task Work_with_a_single_navigation_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_a_single_navigation_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var post = await context.Posts.Include(e => e.Blog).SingleAsync(e => e.Id == 1);

        #region Work_with_a_single_navigation_1
        ReferenceEntry<Post, Blog> referenceEntry1 = context.Entry(post).Reference(e => e.Blog);
        ReferenceEntry<Post, Blog> referenceEntry2 = context.Entry(post).Reference<Blog>("Blog");
        ReferenceEntry referenceEntry3 = context.Entry(post).Reference("Blog");
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_a_single_navigation_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_a_single_navigation_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.Include(e => e.Posts).SingleAsync(e => e.Id == 1);

        #region Work_with_a_single_navigation_2a
        CollectionEntry<Blog, Post> collectionEntry1 = context.Entry(blog).Collection(e => e.Posts);
        CollectionEntry<Blog, Post> collectionEntry2 = context.Entry(blog).Collection<Post>("Posts");
        CollectionEntry collectionEntry3 = context.Entry(blog).Collection("Posts");
        #endregion

        #region Work_with_a_single_navigation_2b
        NavigationEntry navigationEntry = context.Entry(blog).Navigation("Posts");
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_all_properties_of_an_entity_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_all_properties_of_an_entity_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.Include(e => e.Posts).SingleAsync(e => e.Id == 1);

        #region Work_with_all_properties_of_an_entity_1
        foreach (var propertyEntry in context.Entry(blog).Properties)
        {
            if (propertyEntry.Metadata.ClrType == typeof(DateTime))
            {
                propertyEntry.CurrentValue = DateTime.Now;
            }
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_all_properties_of_an_entity_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_all_properties_of_an_entity_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.Include(e => e.Posts).SingleAsync(e => e.Id == 1);

        {
            #region Work_with_all_properties_of_an_entity_2a
            var currentValues = context.Entry(blog).CurrentValues;
            var originalValues = context.Entry(blog).OriginalValues;
            var databaseValues = await context.Entry(blog).GetDatabaseValuesAsync();
            #endregion
        }

        {
            #region Work_with_all_properties_of_an_entity_2b
            var blogDto = new BlogDto { Id = 1, Name = "1unicorn2" };

            context.Entry(blog).CurrentValues.SetValues(blogDto);
            #endregion
        }

        {
            #region Work_with_all_properties_of_an_entity_2c
            var databaseValues = await context.Entry(blog).GetDatabaseValuesAsync();
            context.Entry(blog).CurrentValues.SetValues(databaseValues);
            context.Entry(blog).OriginalValues.SetValues(databaseValues);
            #endregion
        }

        {
            #region Work_with_all_properties_of_an_entity_2d
            var blogDictionary = new Dictionary<string, object> { ["Id"] = 1, ["Name"] = "1unicorn2" };

            context.Entry(blog).CurrentValues.SetValues(blogDictionary);
            #endregion
        }

        {
            #region Work_with_all_properties_of_an_entity_2e
            var clonedBlog = (await context.Entry(blog).GetDatabaseValuesAsync()).ToObject();
            #endregion
        }

        Console.WriteLine();
    }

    public static async Task Work_with_all_navigations_of_an_entity_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_all_navigations_of_an_entity_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

        #region Work_with_all_navigations_of_an_entity_1
        foreach (var navigationEntry in context.Entry(blog).Navigations)
        {
            navigationEntry.Load();
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Work_with_all_members_of_an_entity_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Work_with_all_members_of_an_entity_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        var blog = await context.Blogs.SingleAsync(e => e.Id == 1);

        #region Work_with_all_members_of_an_entity_1
        foreach (var memberEntry in context.Entry(blog).Members)
        {
            Console.WriteLine(
                $"Member {memberEntry.Metadata.Name} is of type {memberEntry.Metadata.ClrType.ShortDisplayName()} and has value {memberEntry.CurrentValue}");
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Find_and_FindAsync_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Find_and_FindAsync_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Find_and_FindAsync_1
        using var context = new BlogsContext();

        Console.WriteLine("First call to Find...");
        var blog1 = await context.Blogs.FindAsync(1);

        Console.WriteLine($"...found blog {blog1.Name}");

        Console.WriteLine();
        Console.WriteLine("Second call to Find...");
        var blog2 = await context.Blogs.FindAsync(1);
        Debug.Assert(blog1 == blog2);

        Console.WriteLine("...returned the same instance without executing a query.");
        #endregion

        Console.WriteLine();
    }

    public static async Task Find_and_FindAsync_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Find_and_FindAsync_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();
        var orderId = 1;
        var productId = 2;

        #region Find_and_FindAsync_2
        var orderline = await context.OrderLines.FindAsync(orderId, productId);
        #endregion

        Console.WriteLine();
    }

    public static async Task Using_ChangeTracker_Entries_to_access_all_tracked_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_ChangeTracker_Entries_to_access_all_tracked_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Using_ChangeTracker_Entries_to_access_all_tracked_entities_1a
        using var context = new BlogsContext();
        var blogs = await context.Blogs.Include(e => e.Posts).ToListAsync();

        foreach (var entityEntry in context.ChangeTracker.Entries())
        {
            Console.WriteLine($"Found {entityEntry.Metadata.Name} entity with ID {entityEntry.Property("Id").CurrentValue}");
        }
        #endregion

        Console.WriteLine();

        #region Using_ChangeTracker_Entries_to_access_all_tracked_entities_1b
        foreach (var entityEntry in context.ChangeTracker.Entries<Post>())
        {
            Console.WriteLine(
                $"Found {entityEntry.Metadata.Name} entity with ID {entityEntry.Property(e => e.Id).CurrentValue}");
        }
        #endregion

        Console.WriteLine();

        #region Using_ChangeTracker_Entries_to_access_all_tracked_entities_1c
        foreach (var entityEntry in context.ChangeTracker.Entries<IEntityWithKey>())
        {
            Console.WriteLine(
                $"Found {entityEntry.Metadata.Name} entity with ID {entityEntry.Property(e => e.Id).CurrentValue}");
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Using_DbSet_Local_to_query_tracked_entities_1()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_DbSet_Local_to_query_tracked_entities_1)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Using_DbSet_Local_to_query_tracked_entities_1
        using var context = new BlogsContext();

        await context.Blogs.Include(e => e.Posts).LoadAsync();

        foreach (var blog in context.Blogs.Local)
        {
            Console.WriteLine($"Blog: {blog.Name}");
        }

        foreach (var post in context.Posts.Local)
        {
            Console.WriteLine($"Post: {post.Title}");
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Using_DbSet_Local_to_query_tracked_entities_2()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_DbSet_Local_to_query_tracked_entities_2)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Using_DbSet_Local_to_query_tracked_entities_2
        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        Console.WriteLine("Local view after loading posts:");

        foreach (var post in context.Posts.Local)
        {
            Console.WriteLine($"  Post: {post.Title}");
        }

        context.Remove(posts[1]);

        context.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many...",
                Blog = posts[0].Blog
            });

        Console.WriteLine("Local view after adding and deleting posts:");

        foreach (var post in context.Posts.Local)
        {
            Console.WriteLine($"  Post: {post.Title}");
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Using_DbSet_Local_to_query_tracked_entities_3()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_DbSet_Local_to_query_tracked_entities_3)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        #region Using_DbSet_Local_to_query_tracked_entities_3
        using var context = new BlogsContext();

        var posts = await context.Posts.Include(e => e.Blog).ToListAsync();

        Console.WriteLine("Local view after loading posts:");

        foreach (var post in context.Posts.Local)
        {
            Console.WriteLine($"  Post: {post.Title}");
        }

        context.Posts.Local.Remove(posts[1]);

        context.Posts.Local.Add(
            new Post
            {
                Title = "What’s next for System.Text.Json?",
                Content = ".NET 5.0 was released recently and has come with many...",
                Blog = posts[0].Blog
            });

        Console.WriteLine("Local view after adding and deleting posts:");

        foreach (var post in context.Posts.Local)
        {
            Console.WriteLine($"  Post: {post.Title}");
        }
        #endregion

        Console.WriteLine();
    }

    public static async Task Using_DbSet_Local_to_query_tracked_entities_4()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_DbSet_Local_to_query_tracked_entities_4)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BlogsContext();

        await context.Posts.Include(e => e.Blog).LoadAsync();

        #region Using_DbSet_Local_to_query_tracked_entities_4
        ObservableCollection<Post> observableCollection = context.Posts.Local.ToObservableCollection();
        BindingList<Post> bindingList = context.Posts.Local.ToBindingList();
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

#region OrderLine
public class OrderLine
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    //...
}
#endregion

#region BlogDto
public class BlogDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
#endregion

public class Blog : IEntityWithKey
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IList<Post> Posts { get; } = new List<Post>();
}

public class Post : IEntityWithKey
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int? BlogId { get; set; }
    public Blog Blog { get; set; }
}

#region IEntityWithKey
public interface IEntityWithKey
{
    int Id { get; set; }
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
    public DbSet<OrderLine> OrderLines { get; set; }

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
            .Entity<OrderLine>()
            .HasKey(e => new { e.OrderId, e.ProductId });
    }
    #endregion
}
