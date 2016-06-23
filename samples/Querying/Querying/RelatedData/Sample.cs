using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFQuerying.RelatedData
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .Select(blog => new
                    {
                        Id = blog.BlogId,
                        Url = blog.Url
                    })
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                    .Include(blog => blog.Owner)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                        .ThenInclude(post => post.Author)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                        .ThenInclude(post => post.Author)
                        .ThenInclude(author => author.Photo)
                    .ToList();
            }

            using (var context = new BloggingContext())
            {
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                        .ThenInclude(post => post.Author)
                        .ThenInclude(author => author.Photo)
                    .Include(blog => blog.Owner)
                        .ThenInclude(owner => owner.Photo)
                    .ToList();
            }


            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Single(b => b.BlogId == 1);

                context.Posts
                    .Where(p => p.BlogId == blog.BlogId)
                    .Load();
            }
        }
    }
}
