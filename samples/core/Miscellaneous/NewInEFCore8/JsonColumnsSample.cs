namespace NewInEfCore8;

public static class JsonColumnsSample
{
    public static Task Json_columns_with_TPH()
    {
        PrintSampleName();
        return JsonColumnsTest<JsonBlogsContext>();
    }

    public static Task Json_columns_with_TPH_on_SQLite()
    {
        PrintSampleName();
        return JsonColumnsTest<JsonBlogsContextSqlite>();
    }

    private static async Task JsonColumnsTest<TContext>()
        where TContext : BlogsContext, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        #region CollectionIndexPredicate
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(365));
        var updatedPosts = await context.Posts
            .Where(
                p => p.Metadata!.Updates[0].UpdatedOn < cutoff
                     && p.Metadata!.Updates[1].UpdatedOn < cutoff)
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in updatedPosts)
        {
            Console.WriteLine($"Post '{post.Title.Substring(0, 10)}...' with updates on {post.Metadata!.Updates[0].UpdatedOn} and {post.Metadata.Updates[1].UpdatedOn}.");
        }

        Console.WriteLine();

        #region CollectionIndexNestedPredicate
        var twentyTen = DateOnly.FromDateTime(new DateTime(2010, 1, 1));
        var postsWithFirstCommit = await context.Posts
            .Where(
                p => p.Metadata!.Updates[0].UpdatedOn > twentyTen
                     && p.Metadata!.Updates[0].Commits[0].Comment == "Commit #1")
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in postsWithFirstCommit)
        {
            Console.WriteLine($"Post '{post.Title.Substring(0, 10)}...' with first commit on {post.Metadata!.Updates[0].Commits[0].CommittedOn}.");
        }

        Console.WriteLine();

        #region CollectionIndexProjectionNullable
        var postsAndRecentUpdatesNullable = await context.Posts
            .Select(p => new
            {
                p.Title,
                LatestUpdate = (DateOnly?)p.Metadata!.Updates[0].UpdatedOn,
                SecondLatestUpdate = (DateOnly?)p.Metadata.Updates[1].UpdatedOn
            })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in postsAndRecentUpdatesNullable)
        {
            Console.WriteLine($"Post '{post.Title.Substring(0, 10)}...' with updates on {post.LatestUpdate?.ToString() ?? "<none>"} and {post.SecondLatestUpdate?.ToString() ?? "<none>"}.");
        }

        Console.WriteLine();

#pragma warning disable CS8073
        #region CollectionIndexProjection
        var postsAndRecentUpdates = await context.Posts
            .Where(p => p.Metadata!.Updates[0].UpdatedOn != null
                        && p.Metadata!.Updates[1].UpdatedOn != null)
            .Select(p => new
            {
                p.Title,
                LatestUpdate = p.Metadata!.Updates[0].UpdatedOn,
                SecondLatestUpdate = p.Metadata.Updates[1].UpdatedOn
            })
            .ToListAsync();
        #endregion
#pragma warning restore CS8073

        Console.WriteLine();
        foreach (var post in postsAndRecentUpdates)
        {
            Console.WriteLine($"Post '{post.Title.Substring(0, 10)}...' with updates on {post.LatestUpdate} and {post.SecondLatestUpdate}.");
        }

        Console.WriteLine();

        #region CollectionIndexNestedProjection
        var postsAndFirstCommit = await context.Posts
            .Select(p => new
            {
                p.Title,
                CommitComment = (string?)p.Metadata!.Updates[0].Commits[0].Comment
            })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in postsAndFirstCommit)
        {
            Console.WriteLine($"Post '{post.Title.Substring(0, 10)}...' with commit '{post.CommitComment?.ToString() ?? "<none>"}'.");
        }

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}

public abstract class JsonBlogsContextBase : BlogsContext
{
    protected JsonBlogsContextBase(bool useSqlite = false)
        : base(useSqlite)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
                ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
            });

        #region PostMetadataConfig
        modelBuilder.Entity<Post>().OwnsOne(
            post => post.Metadata, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
                ownedNavigationBuilder.OwnsMany(metadata => metadata.TopSearches);
                ownedNavigationBuilder.OwnsMany(metadata => metadata.TopGeographies);
                ownedNavigationBuilder.OwnsMany(
                    metadata => metadata.Updates,
                    ownedOwnedNavigationBuilder => ownedOwnedNavigationBuilder.OwnsMany(update => update.Commits));
            });
        #endregion

        base.OnModelCreating(modelBuilder);
    }
}

public class JsonBlogsContext : JsonBlogsContextBase
{
}

public class JsonBlogsContextSqlite : JsonBlogsContextBase
{
    public JsonBlogsContextSqlite()
        : base(useSqlite: true)
    {
    }
}
