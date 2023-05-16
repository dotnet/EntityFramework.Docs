namespace NewInEfCore8;

public static class LazyLoadingSample
{
    public static Task Lazy_loading_for_no_tracking_queries()
    {
        PrintSampleName();
        return LazyLoadingTest<LazyLoadingBlogsContext>();
    }

    public static Task Lazy_loading_for_no_tracking_queries_SQLite()
    {
        PrintSampleName();
        return LazyLoadingTest<LazyLoadingBlogsContextSqlite>();
    }

    private static async Task LazyLoadingTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();
        var x = 1;

        #region NoTrackingForBlogs
        var blogs = await context.Blogs.AsNoTracking().ToListAsync();
        #endregion

        Console.WriteLine("Blogs:");
        for (var i = 0; i < blogs.Count; i++)
        {
            Console.WriteLine($" ({i + 1}) {blogs[i].Name} ");
        }

        #region ChooseABlog
        Console.WriteLine();
        Console.Write("Choose a blog: ");
        if (int.TryParse(ReadLine(), out var blogId))
        {
            Console.WriteLine("Posts:");
            foreach (var post in blogs[blogId - 1].Posts)
            {
                Console.WriteLine($"  {post.Title}");
            }
        }
        #endregion

        Console.WriteLine();
        Console.Write("Choose another blog: ");
        if (int.TryParse(ReadLine(), out blogId))
        {
            var blog = blogs[blogId - 1];
            #region ExplicitLoad
            await context.Entry(blog).Collection(e => e.Posts).LoadAsync();
            #endregion
            Console.WriteLine("Posts:");
            foreach (var post in blog.Posts)
            {
                Console.WriteLine($"  {post.Title}");
            }
        }

        #region IsLoaded
        foreach (var blog in blogs)
        {
            if (context.Entry(blog).Collection(e => e.Posts).IsLoaded)
            {
                Console.WriteLine($" Posts for blog '{blog.Name}' are loaded.");
            }
        }
        #endregion

        string ReadLine()
        {
            return (x++).ToString(); // Console.ReadLine();
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}

public class LazyLoadingBlogsContextBase : JsonBlogsContextBase
{
    protected LazyLoadingBlogsContextBase(bool useSqlite = false)
        : base(useSqlite)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        #region IgnoreNonVirtualNavigations
        optionsBuilder.UseLazyLoadingProxies(b => b.IgnoreNonVirtualNavigations());
        #endregion
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region NoLazyLoading
        modelBuilder
            .Entity<Post>()
            .Navigation(p => p.Author)
            .EnableLazyLoading(false);
        #endregion

        base.OnModelCreating(modelBuilder);
    }
}

public class LazyLoadingBlogsContext : LazyLoadingBlogsContextBase
{
}

public class LazyLoadingBlogsContextSqlite : LazyLoadingBlogsContextBase
{
    public LazyLoadingBlogsContextSqlite()
        : base(useSqlite: true)
    {
    }
}
