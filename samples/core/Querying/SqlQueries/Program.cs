using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RawSQL;

internal class Program
{
    private static void Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(
                @"create function [dbo].[SearchBlogs] (@searchTerm nvarchar(max))
                          returns @found table
                          (
                              BlogId int not null,
                              Url nvarchar(max),
                              Rating int
                          )
                          as
                          begin
                              insert into @found
                              select * from dbo.Blogs as b
                              where exists (
                                  select 1
                                  from [Post] as [p]
                                  where ([b].[BlogId] = [p].[BlogId]) and (charindex(@searchTerm, [p].[Title]) > 0))

                                  return
                          end");

            context.Database.ExecuteSqlRaw(
                @"create procedure [dbo].[GetMostPopularBlogs] as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");

            context.Database.ExecuteSqlRaw(
                @"create procedure [dbo].[GetMostPopularBlogsForUser] @filterByUser nvarchar(max) as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");
        }

        using (var context = new BloggingContext())
        {
            #region FromSql
            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.Blogs")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlStoredProcedure
            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlStoredProcedureParameter
            var user = "johndoe";

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlStoredProcedureNamedSqlParameter
            var user = new SqlParameter("user", "johndoe");

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlStoredProcedureSqlParameter
            var user = new SqlParameter("user", "johndoe");

            var blogs = context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlRawStoredProcedureParameter
            var columnName = "Url";
            var columnValue = new SqlParameter("columnValue", "http://SomeURL");

            var blogs = context.Blogs
                .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlComposed
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Where(b => b.Rating > 3)
                .OrderByDescending(b => b.Rating)
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlInclude
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Include(b => b.Posts)
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region FromSqlAsNoTracking
            var searchTerm = "Lorem ipsum";

            var blogs = context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .AsNoTracking()
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region SqlQuery
            var ids = context.Database
                .SqlQuery<int>($"SELECT [BlogId] FROM [Blogs]")
                .ToList();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region SqlQueryComposed
            var overAverageIds = context.Database
                .SqlQuery<int>($"SELECT [BlogId] AS [Value] FROM [Blogs]")
                .Where(id => id > context.Blogs.Average(b => b.BlogId))
                .ToList();
            #endregion
        }

        #region ExecuteSql
        using (var context = new BloggingContext())
        {
            var rowsModified = context.Database.ExecuteSql($"UPDATE [Blogs] SET [Url] = NULL");
        }
        #endregion
    }
}
