using System;

namespace EFLogging
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureCreated();
                db.Blogs.Add(new Blog { Url = "http://sample.com" });
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(blog.Url);
                }
            }

            using (var db = new BloggingContextWithFiltering())
            {
                db.Database.EnsureCreated();
                db.Blogs.Add(new Blog { Url = "http://sample.com" });
                db.SaveChanges();
            }

            using (var db = new BloggingContextWithFiltering())
            {
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(blog.Url);
                }
            }
        }
    }
}
