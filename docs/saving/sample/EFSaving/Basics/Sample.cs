using System;
using System.Collections.Generic;
using System.Linq;

namespace EFSaving.Basics
{
    public class Sample
    {
        public static void Run()
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = "http://sample.com" };
                db.Blogs.Add(blog);
                db.SaveChanges();

                Console.WriteLine($"{blog.BlogId}: {blog.Url}");
            }

            using (var db = new BloggingContext())
            {
                var blog = db.Blogs.First();
                blog.Url = "http://sample.com/blog";
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blog = db.Blogs.First();
                db.Blogs.Remove(blog);
                db.SaveChanges();
            }

            // Insert some seed data for the final example
            using (var db = new BloggingContext())
            {
                db.Blogs.Add(new Blog { Url = "http://sample.com/blog" });
                db.Blogs.Add(new Blog { Url = "http://sample.com/another_blog" });
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                db.Blogs.Add(new Blog { Url = "http://sample.com/blog_one" });
                db.Blogs.Add(new Blog { Url = "http://sample.com/blog_two" });

                var firstBlog = db.Blogs.First();
                firstBlog.Url = "";

                var lastBlog = db.Blogs.First();
                db.Blogs.Remove(lastBlog);

                db.SaveChanges();
            }
        }
    }
}
