using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFQuerying.UserDefinedFunctionMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new BloggingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(
                @"CREATE FUNCTION dbo.CommentedPostCountForBlog(@id int)
                    RETURNS int
                    AS
                    BEGIN
                        RETURN (SELECT COUNT(*)
                            FROM [Posts] AS [p]
                            WHERE ([p].[BlogId] = @id) AND ((
                                SELECT COUNT(*)
                                FROM [Comments] AS [c]
                                WHERE [p].[PostId] = [c].[PostId]) > 0));
                    END");

            context.Database.ExecuteSqlRaw(
                @"CREATE FUNCTION dbo.PostsWithPopularComments(@likeThreshold int)
                    RETURNS TABLE
                    AS
                    RETURN
                    (
                        SELECT [p].[PostId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
                        FROM [Posts] AS [p]
                        WHERE (
                            SELECT COUNT(*)
                            FROM [Comments] AS [c]
                            WHERE ([p].[PostId] = [c].[PostId]) AND ([c].[Likes] >= @likeThreshold)) > 0
                    )");

            #region BasicQuery
            var query1 = from b in context.Blogs
                         where context.ActivePostCountForBlog(b.BlogId) > 1
                         select b;
            #endregion
            var result1 = query1.ToList();

            #region HasTranslationQuery
            var query2 = from p in context.Posts
                         select context.PercentageDifference(p.BlogId, 3);
                         #endregion
            var result2 = query2.ToList();

            #region TableValuedFunctionQuery
            var likeThreshold = 3;
            var query3 = from p in context.PostsWithPopularComments(likeThreshold)
                         orderby p.Rating
                         select p;
            #endregion
            var result3 = query3.ToList();
        }
    }
}
