using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFSaving.CascadeDelete
{
    public class Sample
    {
        public static void Run()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Saving Data with EF" },
                        new Post { Title = "Cascade Delete with EF" }
                    }
                });

                context.SaveChanges();
            }

            #region CascadingOnTrackedEntities
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.Include(b => b.Posts).First();
                context.Remove(blog);
                context.SaveChanges();
            }
            #endregion

            using (var context = new BloggingContext())
            {
                context.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Saving Data with EF" },
                        new Post { Title = "Cascade Delete with EF" }
                    }
                });

                context.SaveChanges();
            }
            #region CascadingOnDatabaseEntities
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.First();
                context.Remove(blog);
                context.SaveChanges();
            }
            #endregion
        }
    }
}
