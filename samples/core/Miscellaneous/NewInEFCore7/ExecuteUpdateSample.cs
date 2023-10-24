namespace NewInEfCore7;

public static class ExecuteUpdateSample
{
    public static Task ExecuteUpdate()
    {
        PrintSampleName();
        return ExecuteUpdateTest<TphBlogsContext>();
    }

    public static Task ExecuteUpdateTpt()
    {
        PrintSampleName();
        return ExecuteUpdateTest<TptBlogsContext>();
    }

    public static Task ExecuteUpdateTpc()
    {
        PrintSampleName();
        return ExecuteUpdateTest<TpcBlogsContext>();
    }

    public static Task ExecuteUpdateSqlite()
    {
        PrintSampleName();
        return ExecuteUpdateTest<TphSqliteBlogsContext>();
    }

    private static async Task ExecuteUpdateTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        await UpdateAllBlogs<TContext>();

        if (context.MappingStrategy == MappingStrategy.Tph)
        {
            await UpdateOldPosts<TContext>();
        }

        if (context.MappingStrategy != MappingStrategy.Tpt)
        {
            await UpdateFeaturedPosts<TContext>();
        }

        await UpdateTagsOnOldPosts<TContext>();

        Console.WriteLine();
    }

    private static async Task UpdateAllBlogs<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Update names for all blogs...");
        Console.WriteLine(
            $"Blogs before update: {string.Join(", ", await context.Blogs.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region UpdateAllBlogs
        await context.Blogs.ExecuteUpdateAsync(
            s => s.SetProperty(b => b.Name, b => b.Name + " *Featured!*"));
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Blogs after update: {string.Join(", ", await context.Blogs.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task UpdateOldPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Update title and content for old posts...");
        Console.WriteLine(
            $"Posts before update: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'..." + e.Title.Substring(e.Title.Length - 12) + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region UpdateOldPosts
        await context.Posts
            .Where(p => p.PublishedOn.Year < 2022)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Title, b => b.Title + " (" + b.PublishedOn.Year + ")")
                .SetProperty(b => b.Content, b => b.Content + " ( This content was published in " + b.PublishedOn.Year + ")"));
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Posts after update: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'..." + e.Title.Substring(e.Title.Length - 12) + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task UpdateFeaturedPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Update title and content for featured posts...");
        Console.WriteLine(
            $"Posts before update: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'..." + e.Title.Substring(e.Title.Length - 12) + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;
        await context.Set<FeaturedPost>()
            .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.Title, b => b.Title + " *Featured!*")
                    .SetProperty(b => b.Content, b => "Featured: " + b.Content));
        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Posts after update: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'..." + e.Title.Substring(e.Title.Length - 12) + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task UpdateTagsOnOldPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Update tags on old posts");
        Console.WriteLine(
            $"Tags before update: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region UpdateTagsOnOldPosts
        await context.Tags
            .Where(t => t.Posts.All(e => e.PublishedOn.Year < 2022))
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.Text, t => t.Text + " (old)"));
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Tags after update: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
