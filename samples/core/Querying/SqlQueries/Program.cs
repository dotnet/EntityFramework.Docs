using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RawSQL;

#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
internal class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.Database.ExecuteSqlRawAsync(
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

            await context.Database.ExecuteSqlRawAsync(
                @"create procedure [dbo].[GetMostPopularBlogs] as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");

            await context.Database.ExecuteSqlRawAsync(
                @"create procedure [dbo].[GetMostPopularBlogsForUser] @filterByUser nvarchar(max) as
                          begin
                              select * from dbo.Blogs order by Rating
                          end");
        }

        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .FromSql($"SELECT * FROM dbo.Blogs")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var blogs = await context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var user = "johndoe";

            var blogs = await context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var user = new SqliteParameter("user", "johndoe");

            var blogs = await context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var user = new SqliteParameter("user", "johndoe");

            var blogs = await context.Blogs
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var columnName = "Url";
            var columnValue = new SqliteParameter("columnValue", "http://SomeURL");

            var blogs = await context.Blogs
                .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = await context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Where(b => b.Rating > 3)
                .OrderByDescending(b => b.Rating)
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = await context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .Include(b => b.Posts)
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var searchTerm = "Lorem ipsum";

            var blogs = await context.Blogs
                .FromSql($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                .AsNoTracking()
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var ids = await context.Database
                .SqlQuery<int>($"SELECT [BlogId] FROM [Blogs]")
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var overAverageIds = await context.Database
                .SqlQuery<int>($"SELECT [BlogId] AS [Value] FROM [Blogs]")
                .Where(id => id > context.Blogs.Average(b => b.BlogId))
                .ToListAsync();
        }

        using (var context = new BloggingContext())
        {
            var rowsModified = await context.Database.ExecuteSqlAsync($"UPDATE [Blogs] SET [Url] = NULL");
        }
    }
}
