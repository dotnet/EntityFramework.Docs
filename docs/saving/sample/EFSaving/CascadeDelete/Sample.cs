using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFSaving.CascadeDelete
{
    public class Sample
    {
        public static void Run()
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Saving Data with EF" },
                        new Post { Title = "Cascade Delete with EF" }
                    }
                });

                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blog = db.Blogs.Include(b => b.Posts).First();
                db.Remove(blog);
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                db.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Saving Data with EF" },
                        new Post { Title = "Cascade Delete with EF" }
                    }
                });

                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blog = db.Blogs.First();
                db.Remove(blog);
                db.SaveChanges();
            }
        }
    }
}
