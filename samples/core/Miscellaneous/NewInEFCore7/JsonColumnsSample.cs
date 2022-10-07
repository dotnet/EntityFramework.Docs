using System.Net;

namespace NewInEfCore7;

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

        #region AuthorsInChigley
        var authorsInChigley = await context.Authors
            .Where(author => author.Contact.Address.City == "Chigley")
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var author in authorsInChigley)
        {
            Console.WriteLine($"{author.Name} lives at '{author.Contact.Address.Street}' in Chigley.");
        }

        Console.WriteLine();

        #region PostcodesInChigley
        var postcodesInChigley = await context.Authors
            .Where(author => author.Contact.Address.City == "Chigley")
            .Select(author => author.Contact.Address.Postcode)
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine($"Postcodes in Chigley are '{string.Join("', '", postcodesInChigley)}'");
        Console.WriteLine();

        #region OrderedAddresses
        var orderedAddresses = await context.Authors
            .Where(
                author => (author.Contact.Address.City == "Chigley"
                           && author.Contact.Phone != null)
                          || author.Name.StartsWith("D"))
            .OrderBy(author => author.Contact.Phone)
            .Select(
                author => author.Name + " (" + author.Contact.Address.Street
                          + ", " + author.Contact.Address.City
                          + " " + author.Contact.Address.Postcode + ")")
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var address in orderedAddresses)
        {
            Console.WriteLine(address);
        }

        Console.WriteLine();

        var authorsInChigleyWithPosts = await context.Authors
            .Where(
                author => author.Contact.Address.City == "Chigley"
                          && author.Posts.Count > 1)
            .Include(author => author.Posts)
            .ToListAsync();

        Console.WriteLine();
        foreach (var author in authorsInChigleyWithPosts)
        {
            Console.WriteLine($"{author.Name} has {author.Posts.Count} posts");
        }

        Console.WriteLine();

        #region PostsWithViews
        var postsWithViews = await context.Posts.Where(post => post.Metadata!.Views > 3000)
            .AsNoTracking()
            .Select(
                post => new
                {
                    post.Author!.Name, post.Metadata!.Views, Searches = post.Metadata.TopSearches, Commits = post.Metadata.Updates
                })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var postWithViews in postsWithViews)
        {
            Console.WriteLine(
                $"Post by {postWithViews.Name} with {postWithViews.Views} views had {postWithViews.Commits.Count} commits with {postWithViews.Searches.Sum(term => term.Count)} searches");
        }

        Console.WriteLine();

        context.ChangeTracker.Clear();

        Console.WriteLine("Updating a 'Contact' JSON document...");
        Console.WriteLine();

        #region UpdateDocument
        var jeremy = await context.Authors.SingleAsync(author => author.Name.StartsWith("Jeremy"));

        jeremy.Contact = new() { Address = new("2 Riverside", "Trimbridge", "TB1 5ZS", "UK"), Phone = "01632 88346" };

        await context.SaveChangesAsync();
        #endregion

        context.ChangeTracker.Clear();

        Console.WriteLine("Updating an 'Address' inside the 'Contact' JSON document...");
        Console.WriteLine();

        #region UpdateSubDocument
        var brice = await context.Authors.SingleAsync(author => author.Name.StartsWith("Brice"));

        brice.Contact.Address = new("4 Riverside", "Trimbridge", "TB1 5ZS", "UK");

        await context.SaveChangesAsync();
        #endregion

        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("Updating only 'Country' in a 'Contact' JSON document...");
        Console.WriteLine();

        #region UpdateProperty
        var arthur = await context.Authors.SingleAsync(author => author.Name.StartsWith("Arthur"));

        arthur.Contact.Address.Country = "United Kingdom";

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();

        context.ChangeTracker.Clear();

        var post = await context.Posts.SingleAsync(post => post.Title.StartsWith("Hacking"));

        post.Metadata!.Updates.Add(new PostUpdate(IPAddress.Broadcast, DateTime.UtcNow) { UpdatedBy = "User" });
        post.Metadata!.TopGeographies.Clear();

        await context.SaveChangesAsync();
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

// Used only for code snippets:

public abstract class TableSharingAggregateContext : TphBlogsContext
{
    #region TableSharingAggregate
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
            });
    }
    #endregion
}

public abstract class TableMappedAggregateContext : TphBlogsContext
{
    #region TableMappedAggregate
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToTable("Contacts");
                ownedNavigationBuilder.OwnsOne(
                    contactDetails => contactDetails.Address, ownedOwnedNavigationBuilder =>
                    {
                        ownedOwnedNavigationBuilder.ToTable("Addresses");
                    });
            });
    }
    #endregion
}

public abstract class JsonColumnAggregateContext : TphBlogsContext
{
    #region JsonColumnAggregate
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(
            author => author.Contact, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
                ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
            });
    }
    #endregion
}
