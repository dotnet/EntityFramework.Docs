using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFSaving.Basics
{
    public class Sample
    {
        public static async Task RunAsync()
        {
            await using (var context = new BloggingContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            #region Add
            await using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = "http://example.com" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();
            }
            #endregion

            #region Update
            await using (var context = new BloggingContext())
            {
                var blog = await context.Blogs.FirstAsync();
                blog.Url = "http://example.com/blog";
                await context.SaveChangesAsync();
            }
            #endregion

            #region Remove
            await using (var context = new BloggingContext())
            {
                var blog = await context.Blogs.FirstAsync();
                context.Blogs.Remove(blog);
                await context.SaveChangesAsync();
            }
            #endregion

            #region MultipleOperations
            await using (var context = new BloggingContext())
            {
                // seeding database
                context.Blogs.Add(new Blog { Url = "http://example.com/blog" });
                context.Blogs.Add(new Blog { Url = "http://example.com/another_blog" });
                await context.SaveChangesAsync();
            }

            await using (var context = new BloggingContext())
            {
                // add
                context.Blogs.Add(new Blog { Url = "http://example.com/blog_one" });
                context.Blogs.Add(new Blog { Url = "http://example.com/blog_two" });

                // update
                var firstBlog = await context.Blogs.FirstAsync();
                firstBlog.Url = "";

                // remove
                var lastBlog = await context.Blogs.LastAsync();
                context.Blogs.Remove(lastBlog);

                await context.SaveChangesAsync();
            }
            #endregion
        }
    }
}
