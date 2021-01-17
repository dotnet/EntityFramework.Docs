using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            #region CustomSeeding
            using (var context = new DataSeedingContext())
            {
                context.Database.EnsureCreated();

                var testBlog = context.Blogs.FirstOrDefault(b => b.Url == "http://test.com");
                if (testBlog == null)
                {
                    context.Blogs.Add(new Blog { Url = "http://test.com" });
                }

                context.SaveChanges();
            }
            #endregion

            using (var context = new DataSeedingContext())
            {
                foreach (var blog in context.Blogs.Include(b => b.Posts))
                {
                    Console.WriteLine($"Blog {blog.Url}");

                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($"\t{post.Title}: {post.Content} by {post.AuthorName.First} {post.AuthorName.Last}");
                    }
                }
            }
        }
    }
}
