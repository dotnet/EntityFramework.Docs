using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFQuerying.Tracking;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new BloggingContext())
        {
            // seeding database
            context.Blogs.Add(new Blog { Url = "http://sample.com/blog" });
            context.Blogs.Add(new Blog { Url = "http://sample.com/another_blog" });
            await context.SaveChangesAsync();
        }

        using (var context = new BloggingContext())
        {
            #region Tracking
            var blog = await context.Blogs.SingleOrDefaultAsync(b => b.BlogId == 1);
            blog.Rating = 5;
            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region NoTracking
            var blogs = await context.Blogs
                .AsNoTracking()
                .ToListAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region NoTrackingWithIdentityResolution
            var blogs = await context.Blogs
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region ContextDefaultTrackingBehavior
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var blogs = await context.Blogs.ToListAsync();
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
            var blogs = await context.Blogs
                .OrderByDescending(blog => blog.Rating)
                .Select(
                    blog => new { Id = blog.BlogId, Url = StandardizeUrl(blog) })
                .ToListAsync();
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
