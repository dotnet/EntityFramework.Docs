using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Samples.DataSeeding
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new DataSeedingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                foreach (var blog in context.Blogs.Include(b => b.Posts))
                {
                    Console.WriteLine($"Blog {blog.Url}");

                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($"\t{post.Title}: {post.Content}");
                    }
                }
            }
        }
    }
}
