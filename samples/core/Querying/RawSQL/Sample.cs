using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EFQuerying.RawSQL
{
    public class Sample
    {
        public static void Run()
        {
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
                var searchTerm = ".NET";

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
                var searchTerm = ".NET";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .AsNoTracking()
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region FromSqlInterpolatedInclude
                var searchTerm = ".NET";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .Include(b => b.Posts)
                    .ToList();
                #endregion
            }
        }
    }
}
