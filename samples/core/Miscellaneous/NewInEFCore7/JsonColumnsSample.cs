using System.Net;

namespace NewInEfCore7;

public static class JsonColumnsSample
{
    public static Task Json_columns_with_TPH()
    {
        Console.WriteLine($">>>> Sample: {nameof(Json_columns_with_TPH)}");
        Console.WriteLine();

        return JsonColumnsTest<JsonBlogsContext>();
    }

    public static Task Json_columns_with_TPH_on_SQLite()
    {
        Console.WriteLine($">>>> Sample: {nameof(Json_columns_with_TPH_on_SQLite)}");
        Console.WriteLine();

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

        // Since query below cannot use Include
        // Issue: https://github.com/dotnet/efcore/issues/28808
        await context.Posts.LoadAsync();

        var authorsInChigleyWithPosts = await context.Authors
            .Where(
                author => author.Contact.Address.City == "Chigley"
                          && author.Posts.Count > 1)
            //.Include(author => author.Posts)
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
                    post.Author!.Name,
                    post.Metadata!.Views,
                    Searches = post.Metadata.TopSearches,
                    Commits = post.Metadata.Updates
                })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var postWithViews in postsWithViews)
        {
            Console.WriteLine($"Post by {postWithViews.Name} with {postWithViews.Views} views had {postWithViews.Commits.Count} commits with {postWithViews.Searches.Sum(term => term.Count)} searches");
        }

        Console.WriteLine();

        context.ChangeTracker.Clear();

        var arthur = await context.Authors.SingleAsync(author => author.Name.StartsWith("Arthur"));

        arthur.Contact.Phone = "01632 22345";
        arthur.Contact.Address.Country = "United Kingdom";

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var post = await context.Posts.SingleAsync(post => post.Title.StartsWith("Hacking"));

        post.Metadata!.Updates.Add(new PostUpdate(IPAddress.Broadcast, DateTime.UtcNow) { UpdatedBy = "User" });
        post.Metadata!.TopGeographies.Clear();

        await context.SaveChangesAsync();
    }
}
