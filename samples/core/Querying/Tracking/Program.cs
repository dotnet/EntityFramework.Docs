using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.Tracking
{
    internal class Program
    {
        private static void Main(string[] args)
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
                #region Tracking
                var blog = context.Blogs.SingleOrDefault(b => b.BlogId == 1);
                blog.Rating = 5;
                context.SaveChanges();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region NoTracking
                var blogs = context.Blogs
                    .AsNoTracking()
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region NoTrackingWithIdentityResolution
                var blogs = context.Blogs
                    .AsNoTrackingWithIdentityResolution()
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region ContextDefaultTrackingBehavior
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var blogs = context.Blogs.ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region CustomProjection1
                var blog = context.Blogs
                    .Select(
                        b =>
                            new { Blog = b, PostCount = b.Posts.Count() });
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region CustomProjection2
                var blog = context.Blogs
                    .Select(
                        b =>
                            new { Blog = b, Post = b.Posts.OrderBy(p => p.Rating).LastOrDefault() });
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region CustomProjection3
                var blog = context.Blogs
                    .Select(
                        b =>
                            new { Id = b.BlogId, b.Url });
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region ClientProjection
                var blogs = context.Blogs
                    .OrderByDescending(blog => blog.Rating)
                    .Select(
                        blog => new { Id = blog.BlogId, Url = StandardizeUrl(blog) })
                    .ToList();
                #endregion
            }
        }

        #region ClientMethod
        public static string StandardizeUrl(Blog blog)
        {
            var url = blog.Url.ToLower();

            if (!url.StartsWith("http://"))
            {
                url = string.Concat("http://", url);
            }

            return url;
        }
        #endregion
    }
}
