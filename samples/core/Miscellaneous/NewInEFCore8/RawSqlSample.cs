namespace NewInEfCore8;

public static class RawSqlSample
{
    public static async Task SqlQuery_for_unmapped_types()
    {
        PrintSampleName();

        await using var context = new TphBlogsContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.Database.ExecuteSql(
            @$"CREATE PROCEDURE GetRecentPostSummariesProc as
                      BEGIN
                          SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                          FROM Posts AS p
                          INNER JOIN Blogs AS b ON p.BlogId = b.Id
                          ORDER BY PublishedOn
                      END");

        context.Database.ExecuteSql(
            @$"CREATE FUNCTION GetPostsPublishedAfter(@publicationDate datetime2)
                                                    RETURNS @posts table
                                                    (
                                                        BlogName nvarchar(max) not null,
                                                        PostTitle nvarchar(max) not null,
                                                        PublishedOn date not null
                                                    )
                                                    AS
                                                    BEGIN
                                                        INSERT INTO @posts
                                                        SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                                                        FROM Posts AS p
                                                        INNER JOIN Blogs AS b ON p.BlogId = b.Id
                                                        WHERE p.PublishedOn >= @publicationDate
                                                        RETURN
                                                    END");

        context.Database.ExecuteSql(
            @$"CREATE VIEW PostAndBlogSummariesView as
                          SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                    FROM Posts AS p
                    INNER JOIN Blogs AS b ON p.BlogId = b.Id");

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        #region SqlQueryAllColumns
        var start = new DateOnly(2022, 1, 1);
        var end = new DateOnly(2023, 1, 1);
        var postsIn2022 =
            await context.Database
                .SqlQuery<BlogPost>($"SELECT * FROM Posts as p WHERE p.PublishedOn >= {start} AND p.PublishedOn < {end}")
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in postsIn2022)
        {
            Console.WriteLine($"Post '{post.BlogTitle.Substring(0, 10)}...' published on {post.PublishedOn} with FK to Blog '{post.BlogId}'.");
        }

        Console.WriteLine();

        #region SqlQueryJoin

        var cutoffDate = new DateOnly(2022, 1, 1);
        var summaries =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                       FROM Posts AS p
                       INNER JOIN Blogs AS b ON p.BlogId = b.Id
                       WHERE p.PublishedOn >= {cutoffDate}")
                .ToListAsync();

        #endregion

        Console.WriteLine();
        foreach (var post in summaries)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' in blog '{post.BlogName}' published on {post.PublishedOn}.");
        }

        Console.WriteLine();

        #region SqlQueryJoinComposed
        var summariesIn2022 =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
                       FROM Posts AS p
                       INNER JOIN Blogs AS b ON p.BlogId = b.Id")
                .Where(p => p.PublishedOn >= cutoffDate && p.PublishedOn < end)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in summariesIn2022)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' in blog '{post.BlogName}' published on {post.PublishedOn}.");
        }

        Console.WriteLine();

        #region SqlQueryJoinComposedLinq
        var summariesByLinq =
            await context.Posts.Select(
                    p => new PostSummary
                    {
                        BlogName = p.Blog.Name,
                        PostTitle = p.Title,
                        PublishedOn = p.PublishedOn,
                    })
                .Where(p => p.PublishedOn >= start && p.PublishedOn < end)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in summariesByLinq)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' in blog '{post.BlogName}' published on {post.PublishedOn}.");
        }

        Console.WriteLine();

        #region SqlQueryView
        var summariesFromView =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT * FROM PostAndBlogSummariesView")
                .Where(p => p.PublishedOn >= cutoffDate && p.PublishedOn < end)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in summariesFromView)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' in blog '{post.BlogName}' published on {post.PublishedOn}.");
        }

        Console.WriteLine();

        #region SqlQueryFunction
        var summariesFromFunc =
            await context.Database.SqlQuery<PostSummary>(
                    @$"SELECT * FROM GetPostsPublishedAfter({cutoffDate})")
                .Where(p => p.PublishedOn < end)
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in summariesFromFunc)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' in blog '{post.BlogName}' published on {post.PublishedOn}.");
        }

        Console.WriteLine();

        #region SqlQueryStoredProc
        var summariesFromStoredProc =
            await context.Database.SqlQuery<PostSummary>(
                    @$"exec GetRecentPostSummariesProc")
                .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var post in summariesFromStoredProc)
        {
            Console.WriteLine($"Post '{post.PostTitle.Substring(0, 10)}...' published on {post.PublishedOn}.");
        }

        Console.WriteLine();
    }

    #region BlogPost
    public class BlogPost
    {
        public BlogPost(string blogTitle, string content, DateOnly publishedOn)
        {
            BlogTitle = blogTitle;
            Content = content;
            PublishedOn = publishedOn;
        }

        public int Id { get; private set; }

        [Column("Title")]
        public string BlogTitle { get; set; }

        public string Content { get; set; }
        public DateOnly PublishedOn { get; set; }
        public int BlogId { get; set; }
    }
    #endregion

    #region PostSummary
    public class PostSummary
    {
        public string BlogName { get; set; } = null!;
        public string PostTitle { get; set; } = null!;
        public DateOnly? PublishedOn { get; set; }
    }
    #endregion

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
}
