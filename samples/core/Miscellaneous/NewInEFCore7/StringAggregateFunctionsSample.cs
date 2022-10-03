namespace NewInEfCore7;

public static class StringAggregateFunctionsSample
{
    public static async Task Translate_string_Concat_and_string_Join()
    {
        PrintSampleName();

        await using (var context = new BlogsContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await context.Seed();
            context.ChangeTracker.Clear();
            context.LoggingEnabled = true;

            #region Join
            var query = context.Posts
                .GroupBy(post => post.Author)
                .Select(grouping => new { Author = grouping.Key, Books = string.Join("|", grouping.Select(post => post.Title)) });
            #endregion

            await foreach (var author in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"{author}");
            }

            Console.WriteLine();
        }

        await using (var context = new BlogsContext { LoggingEnabled = true })
        {
            #region ConcatAndJoin
            var query = context.Posts
                .GroupBy(post => post.Author!.Name)
                .Select(
                    grouping =>
                        new
                        {
                            PostAuthor = grouping.Key,
                            Blogs = string.Concat(
                                grouping
                                    .Select(post => post.Blog.Name)
                                    .Distinct()
                                    .Select(postName => "'" + postName + "' ")),
                            ContentSummaries = string.Join(
                                " | ",
                                grouping
                                    .Where(post => post.Content.Length >= 10)
                                    .Select(post => "'" + post.Content.Substring(0, 10) + "' "))
                        });
            #endregion

            await foreach (var author in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"{author}");
            }

            Console.WriteLine();
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class BlogsContext : ModelBuildingBlogsContextBase
    {
    }
}
