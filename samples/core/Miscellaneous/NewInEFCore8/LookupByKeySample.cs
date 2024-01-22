namespace NewInEfCore8;

public static class LookupByKeySample
{
    public static async Task Lookup_tracked_entities_by_key()
    {
        PrintSampleName();

        await using var context = new JsonBlogsContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.ChangeTracker.Clear();

        await context.Blogs
            .Include(e => e.Posts).ThenInclude(e => e.Author)
            .Include(e => e.Posts).ThenInclude(e => e.Tags)
            .Include(e => e.Site)
            .LoadAsync();

        context.LoggingEnabled = true;

        #region LookupByPrimaryKey
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Blog> blogEntry = context.Blogs.Local.FindEntry(2)!;
        #endregion

        #region UseEntry
        Console.WriteLine($"Blog '{blogEntry.Entity.Name}' with key {blogEntry.Entity.Id} is tracked in the '{blogEntry.State}' state.");
        #endregion

        #region LookupByAlternateKey
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Website> siteEntry = context.Websites.Local.FindEntry(nameof(Website.Uri), new Uri("https://www.bricelam.net/"))!;
        #endregion

        Console.WriteLine($"Website '{siteEntry.Entity.Uri}' can be contacted at '{siteEntry.Entity.Email}'.");

        #region LookupByUniqueForeignKey
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Blog> blogAtSiteEntry = context.Blogs.Local.FindEntry(nameof(Blog.SiteUri), new Uri("https://www.bricelam.net/"))!;
        #endregion

        Console.WriteLine($"Website '{blogAtSiteEntry.Entity.SiteUri}' hosts blog '{blogAtSiteEntry.Entity.Name}'.");

        #region LookupByCompositePrimaryKey
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<PostTag>? postTagEntry = context.Set<PostTag>().Local.FindEntryUntyped(new object[] { 4, "TagEF" });
        #endregion

        Console.WriteLine($"Found PostTag linking post {postTagEntry!.Entity.PostId} with '{postTagEntry.Entity.TagId}'.");

        Console.WriteLine();

        #region LookupByForeignKey
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Post>> postEntries = context.Posts.Local.GetEntries(nameof(Post.BlogId), 2);
        #endregion

        Console.WriteLine("Blog with ID 2 has posts:");
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Post> postEntry in postEntries)
        {
            Console.WriteLine($"  {postEntry.Entity.Title}");
        }

        Console.WriteLine();

        #region LookupByAnyProperty
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Post>> archivedPostEntries = context.Posts.Local.GetEntries(nameof(Post.Archived), true);
        #endregion

        Console.WriteLine("Archived posts:");
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Post> postEntry in archivedPostEntries)
        {
            Console.WriteLine($"  {postEntry.Entity.Title}");
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
