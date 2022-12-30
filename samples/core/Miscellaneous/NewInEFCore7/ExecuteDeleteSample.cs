namespace NewInEfCore7;

public static class ExecuteDeleteSample
{
    public static Task ExecuteDelete()
    {
        PrintSampleName();
        return ExecuteDeleteTest<TphBlogsContext>();
    }

    public static Task ExecuteDeleteTpt()
    {
        PrintSampleName();
        return ExecuteDeleteTest<TptBlogsContext>();
    }

    public static Task ExecuteDeleteTpc()
    {
        PrintSampleName();
        return ExecuteDeleteTest<TpcBlogsContext>();
    }

    public static Task ExecuteDeleteSqlite()
    {
        PrintSampleName();
        return ExecuteDeleteTest<TphSqliteBlogsContext>();
    }

    private static async Task ExecuteDeleteTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        await DeleteAllTags<TContext>();
        await DeleteTagsContainingDotNet<TContext>();
        await DeleteTagsFromOldPosts<TContext>();

        if (context.MappingStrategy == MappingStrategy.Tph)
        {
            await DeleteAllAuthors<TContext>();
            await DeleteAuthorsWithOnePost<TContext>();
        }

        if (context.MappingStrategy != MappingStrategy.Tpt)
        {
            await DeleteFeaturedPosts<TContext>();
        }

        if (context.MappingStrategy == MappingStrategy.Tph
            && !context.UseSqlite)
        {
            await DeletePostsForGivenAuthor<TContext>();
        }

        if (context.MappingStrategy != MappingStrategy.Tpt)
        {
            // https://github.com/dotnet/efcore/issues/28532
            await DeleteAllBlogsAndPosts<TContext>();
        }

        Console.WriteLine();
    }

    private static async Task DeleteAllTags<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete all tags...");
        Console.WriteLine(
            $"Tags before delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteAllTags
        await context.Tags.ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Tags after delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteTagsContainingDotNet<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete tags containing '.NET'...");
        Console.WriteLine(
            $"Tags before delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteTagsContainingDotNet
        await context.Tags.Where(t => t.Text.Contains(".NET")).ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Tags after delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteTagsFromOldPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete tags from old posts...");
        Console.WriteLine(
            $"Tags before delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteTagsFromOldPosts
        await context.Tags.Where(t => t.Posts.All(e => e.PublishedOn.Year < 2022)).ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Tags after delete: {string.Join(", ", await context.Tags.AsNoTracking().Select(e => "'" + e.Text + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteAllAuthors<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete all authors...");
        Console.WriteLine(
            $"Authors before delete: {string.Join(", ", await context.Authors.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteAllAuthors
        await context.Posts.TagWith("Deleting posts...").ExecuteDeleteAsync();
        await context.Authors.TagWith("Deleting authors...").ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Authors after delete: {string.Join(", ", await context.Authors.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteFeaturedPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete featured posts...");
        Console.WriteLine(
            $"Posts before delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteFeaturedPosts
        await context.Set<FeaturedPost>().ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Posts after delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeletePostsForGivenAuthor<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete posts for given author...");
        Console.WriteLine(
            $"Posts before delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeletePostsForGivenAuthor
        await context.Posts.Where(p => p.Author!.Name.StartsWith("Arthur")).ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Posts after delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteAllBlogsAndPosts<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete all blogs and posts...");
        Console.WriteLine(
            $"Blogs before delete: {string.Join(", ", await context.Blogs.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine(
            $"Posts before delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;

        #region DeleteAllBlogsAndPosts
        await context.Blogs.ExecuteDeleteAsync();
        #endregion

        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Blogs after delete: {string.Join(", ", await context.Blogs.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine(
            $"Posts after delete: {string.Join(", ", await context.Posts.AsNoTracking().Select(e => "'" + e.Id + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static async Task DeleteAuthorsWithOnePost<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.BeginTransactionAsync();

        Console.WriteLine("Delete authors with only one post...");
        Console.WriteLine(
            $"Authors before delete: {string.Join(", ", await context.Authors.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();

        context.LoggingEnabled = true;
        await context.Posts.Where(p => p.Author!.Posts.Count <= 1)
            .ExecuteUpdateAsync(s => s.SetProperty(p => EF.Property<int?>(p, "AuthorId"), p => null));
        await context.Authors.Where(a => a.Posts.Count <= 1).ExecuteDeleteAsync();
        context.LoggingEnabled = false;

        Console.WriteLine();
        Console.WriteLine(
            $"Authors after delete: {string.Join(", ", await context.Authors.AsNoTracking().Select(e => "'" + e.Name + "'").ToListAsync())}");
        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}

public class TphBlogsContext : BlogsContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().Ignore(e => e.Metadata);
        modelBuilder.Entity<Author>().OwnsOne(e => e.Contact).OwnsOne(e => e.Address);

        base.OnModelCreating(modelBuilder);
    }
}

public class TphSqliteBlogsContext : BlogsContext
{
    public TphSqliteBlogsContext()
        : base(useSqlite: true)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().Ignore(e => e.Metadata);
        modelBuilder.Entity<Author>().OwnsOne(e => e.Contact).OwnsOne(e => e.Address);

        base.OnModelCreating(modelBuilder);
    }
}

public class TptBlogsContext : BlogsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpt;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeaturedPost>().ToTable("FeaturedPosts");
        modelBuilder.Entity<Post>().Ignore(e => e.Metadata);
        modelBuilder.Entity<Author>().OwnsOne(e => e.Contact).OwnsOne(e => e.Address);

        base.OnModelCreating(modelBuilder);
    }
}

public class TpcBlogsContext : BlogsContext
{
    public override MappingStrategy MappingStrategy => MappingStrategy.Tpc;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().UseTpcMappingStrategy();
        modelBuilder.Entity<FeaturedPost>().ToTable("FeaturedPosts");
        modelBuilder.Entity<Post>().Ignore(e => e.Metadata);
        modelBuilder.Entity<Author>().OwnsOne(e => e.Contact).OwnsOne(e => e.Address);

        base.OnModelCreating(modelBuilder);
    }
}
