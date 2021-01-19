using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RawSQL
{
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
                #region FromSqlRaw
                var blogs = context.Blogs
                    .FromSqlRaw("SELECT * FROM dbo.Blogs")
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlRawStoredProcedure
                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogs")
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlRawStoredProcedureParameter
                var user = "johndoe";

                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogsForUser {0}", user)
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlInterpolatedStoredProcedureParameter
                var user = "johndoe";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlRawStoredProcedureSqlParameter
                var user = new SqlParameter("user", "johndoe");

                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogsForUser @user", user)
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlRawStoredProcedureNamedSqlParameter
                var user = new SqlParameter("user", "johndoe");

                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser=@user", user)
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlInterpolatedComposed
                var searchTerm = "Lorem ipsum";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .Where(b => b.Rating > 3)
                    .OrderByDescending(b => b.Rating)
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlInterpolatedAsNoTracking
                var searchTerm = "Lorem ipsum";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .AsNoTracking()
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlInterpolatedInclude
                var searchTerm = "Lorem ipsum";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .Include(b => b.Posts)
                    .ToList();
                #endregion
            }
        }
    }
}
