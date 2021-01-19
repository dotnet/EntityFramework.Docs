using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Performance.LazyLoading;

namespace Performance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(
                    new Blog
                    {
                        Url = "http://someblog.microsoft.com",
                        Rating = 0,
                        Posts = new List<Post>
                        {
                            new Post { Title = "Post 1", Content = "Sometimes..." },
                            new Post { Title = "Post 2", Content = "Other times..." }
                        }
                    });

                context.SaveChanges();
            }

            using (var context = new BloggingContext())
            {
                #region Indexes
                // Matches on start, so uses an index (on SQL Server)
                var posts1 = context.Posts.Where(p => p.Title.StartsWith("A")).ToList();
                // Matches on end, so does not use the index
                var posts2 = context.Posts.Where(p => p.Title.EndsWith("A")).ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region ProjectEntities
                foreach (var blog in context.Blogs)
                {
                    Console.WriteLine("Blog: " + blog.Url);
                }
                #endregion

                #region ProjectSingleProperty
                foreach (var blogName in context.Blogs.Select(b => b.Url))
                {
                    Console.WriteLine("Blog: " + blogName);
                }
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region NoLimit
                var blogsAll = context.Posts
                    .Where(p => p.Title.StartsWith("A"))
                    .ToList();
                #endregion

                #region Limit25
                var blogs25 = context.Posts
                    .Where(p => p.Title.StartsWith("A"))
                    .Take(25)
                    .ToList();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region EagerlyLoadRelatedAndProject
                foreach (var blog in context.Blogs.Select(b => new { b.Url, b.Posts }).ToList())
                {
                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($"Blog {blog.Url}, Post: {post.Title}");
                    }
                }
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region BufferingAndStreaming
                // ToList and ToArray cause the entire resultset to be buffered:
                var blogsList = context.Posts.Where(p => p.Title.StartsWith("A")).ToList();
                var blogsArray = context.Posts.Where(p => p.Title.StartsWith("A")).ToArray();

                // Foreach streams, processing one row at a time:
                foreach (var blog in context.Posts.Where(p => p.Title.StartsWith("A")))
                {
                    // ...
                }

                // AsEnumerable also streams, allowing you to execute LINQ operators on the client-side:
                var doubleFilteredBlogs = context.Posts
                    .Where(p => p.Title.StartsWith("A")) // Translated to SQL and executed in the database
                    .AsEnumerable()
                    .Where(p => SomeDotNetMethod(p)); // Executed at the client on all database results
                #endregion

                // This method represents a filter that cannot be translated to SQL for execution in the
                // database, and must be run on the client as a .NET method
                static bool SomeDotNetMethod(Post post) => true;
            }

            using (var context = new BloggingContext())
            {
                #region SaveChangesBatching
                var blog = context.Blogs.Single(b => b.Url == "http://someblog.microsoft.com");
                blog.Url = "http://someotherblog.microsoft.com";
                context.Add(new Blog { Url = "http://newblog1.microsoft.com" });
                context.Add(new Blog { Url = "http://newblog2.microsoft.com" });
                context.SaveChanges();
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region QueriesWithConstants
                var post1 = context.Posts.FirstOrDefault(p => p.Title == "post1");
                var post2 = context.Posts.FirstOrDefault(p => p.Title == "post2");
                #endregion
            }

            using (var context = new BloggingContext())
            {
                #region QueriesWithParameterization
                var postTitle = "post1";
                var post1 = context.Posts.FirstOrDefault(p => p.Title == postTitle);
                postTitle = "post2";
                var post2 = context.Posts.FirstOrDefault(p => p.Title == postTitle);
                #endregion
            }

            using (var context = new LazyBloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                for (var i = 0; i < 10; i++)
                {
                    context.Blogs.Add(
                        new LazyLoading.Blog
                        {
                            Url = $"http://blog{i}.microsoft.com",
                            Posts = new List<LazyLoading.Post>
                            {
                                new() { Title = $"1st post of blog{i}" }, new() { Title = $"2nd post of blog{i}" }
                            }
                        });
                }

                context.SaveChanges();
            }

            using (var context = new LazyBloggingContext())
            {
                #region NPlusOne
                foreach (var blog in context.Blogs.ToList())
                {
                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($"Blog {blog.Url}, Post: {post.Title}");
                    }
                }
                #endregion
            }

            using (var context = new EmployeeContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                for (var i = 0; i < 10; i++)
                {
                    context.Employees.Add(new Employee());
                }
            }

            using (var context = new EmployeeContext())
            {
                #region UpdateWithoutBulk
                foreach (var employee in context.Employees)
                {
                    employee.Salary += 1000;
                }

                context.SaveChanges();
                #endregion
            }

            using (var context = new EmployeeContext())
            {
                #region UpdateWithBulk
                context.Database.ExecuteSqlRaw("UPDATE [Employees] SET [Salary] = [Salary] + 1000");
                #endregion
            }
        }
    }
}
