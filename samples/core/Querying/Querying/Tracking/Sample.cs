using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFQuerying.Tracking
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            using (var context = new BloggingContext())
            {
                // seeding database
                context.Blogs.Add(new Blog { Url = "http://sample.com/blog" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/another_blog" });
                context.SaveChanges();
            }

            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.SingleOrDefault(b => b.BlogId == 1);
                blog.Rating = 5;
                context.SaveChanges();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .AsNoTracking()
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var blogs = context.Blogs.ToList();
            }

            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Select(b =>
                        new
                        {
                            Blog = b,
                            Posts = b.Posts.Count()
                        });
            }

            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Select(b =>
                        new
                        {
                            Id = b.BlogId,
                            Url = b.Url
                        });
            }
        }
    }
}
