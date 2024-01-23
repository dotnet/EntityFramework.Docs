﻿using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.UserDefinedFunctionMapping;

class Program
{
    static void Main()
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
            @"CREATE FUNCTION [dbo].[ConcatStrings] (@prm1 nvarchar(max), @prm2 nvarchar(max))
                    RETURNS nvarchar(max)
                    AS
                    BEGIN
                        RETURN @prm1 + @prm2;
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
        IQueryable<Blog> query1 = from b in context.Blogs
                                  where context.ActivePostCountForBlog(b.BlogId) > 1
                                  select b;
        #endregion
        var result1 = query1.ToList();

        #region HasTranslationQuery
        IQueryable<double> query2 = from p in context.Posts
                                    select context.PercentageDifference(p.BlogId, 3);
        #endregion
        var result2 = query2.ToList();

        #region NullabilityPropagationExamples
        IQueryable<Blog> query3 = context.Blogs.Where(e => BloggingContext.ConcatStrings(e.Url, e.Rating.ToString()) != "https://mytravelblog.com/4");
        IQueryable<Blog> query4 = context.Blogs.Where(
            e => BloggingContext.ConcatStringsOptimized(e.Url, e.Rating.ToString()) != "https://mytravelblog.com/4");
        #endregion

        var result3 = query3.ToList();
        var result4 = query4.ToList();

        #region TableValuedFunctionQuery
        const int likeThreshold = 3;
        IOrderedQueryable<Post> query5 = from p in context.PostsWithPopularComments(likeThreshold)
                                         orderby p.Rating
                                         select p;
        #endregion
        var result5 = query5.ToList();
    }
}