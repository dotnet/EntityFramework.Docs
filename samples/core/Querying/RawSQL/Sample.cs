using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;

namespace EFQuerying.RawSQL
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .FromSqlRaw("SELECT * FROM dbo.Blogs")
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogs")
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var user = "johndoe";

                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogsForUser {0}", user)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var user = "johndoe";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var user = new SqlParameter("user", "johndoe");

                var blogs = context.Blogs
                    .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogsForUser @user", user)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var searchTerm = ".NET";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .Where(b => b.Rating > 3)
                    .OrderByDescending(b => b.Rating)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var searchTerm = ".NET";

                var blogs = context.Blogs
                    .FromSqlInterpolated($"SELECT * FROM dbo.SearchBlogs({searchTerm})")
                    .Include(b => b.Posts)
                    .ToList();
            }
        }
    }
}
