using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding;

static class Program
{
    static void Main()
    {
        #region CustomSeeding
        using (var context = new DataSeedingContext())
        {
            context.Database.EnsureCreated();

            Blog testBlog = context.Blogs.FirstOrDefault(b => b.Url == "http://test.com");
            if (testBlog == null)
            {
                context.Blogs.Add(new Blog { Url = "http://test.com" });
            }

            context.SaveChanges();
        }
        #endregion

        using (var context = new DataSeedingContext())
        {
            foreach (Blog blog in context.Blogs.Include(b => b.Posts))
            {
                Console.WriteLine($"Blog {blog.Url}");

                foreach (Post post in blog.Posts)
                {
                    Console.WriteLine($"\t{post.Title}: {post.Content} by {post.AuthorName.First} {post.AuthorName.Last}");
                }
            }
        }
    }
}