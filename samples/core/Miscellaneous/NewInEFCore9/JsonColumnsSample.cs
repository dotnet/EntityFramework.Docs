using System.Net;

namespace NewInEfCore9;

public static class JsonColumnsSample
{
    public static Task Columns_from_JSON_are_pruned_when_needed()
    {
        PrintSampleName();
        return JsonColumnsTest<JsonBlogsContext>();
    }

    public static Task Columns_from_JSON_are_pruned_when_needed_on_SQLite()
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

        Console.WriteLine();
        Console.WriteLine("JSON column pruning:");
        Console.WriteLine();

        var date = new DateOnly(2022, 10, 11);

        #region PruneJSON
        var postsUpdatedOn = await context.Posts
            .Where(p => p.Metadata!.Updates.Count(e => e.UpdatedOn >= date) == 1)
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("JSON column pruning in primitive collection count:");
        Console.WriteLine();

        #region PruneJSONPrimitive
        var tagsWithCount = await context.Tags.Where(p => p.Text.Length == 1).ToListAsync();
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
