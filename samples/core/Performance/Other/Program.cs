using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Performance.LazyLoading;

namespace Performance;

internal class Program
{
    #region CompiledQueryCompile
    private static readonly Func<BloggingContext, int, IAsyncEnumerable<Blog>> _compiledQuery
        = EF.CompileAsyncQuery(
            (BloggingContext context, int length) => context.Blogs.Where(b => b.Url.StartsWith("http://") && b.Url.Length == length));
    #endregion

    private static async Task Main(string[] args)
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

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

            await context.SaveChangesAsync();
        }

        using (var context = new BloggingContext())
        {
            #region Indexes
            // Matches on start, so uses an index (on SQL Server)
            var posts1 = await context.Posts.Where(p => p.Title.StartsWith("A")).ToListAsync();
            // Matches on end, so does not use the index
            var posts2 = await context.Posts.Where(p => p.Title.EndsWith("A")).ToListAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region ProjectEntities
            await foreach (var blog in context.Blogs.AsAsyncEnumerable())
            {
                Console.WriteLine("Blog: " + blog.Url);
            }
            #endregion

            #region ProjectSingleProperty
            await foreach (var blogName in context.Blogs.Select(b => b.Url).AsAsyncEnumerable())
            {
                Console.WriteLine("Blog: " + blogName);
            }
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region NoLimit
            var blogsAll = await context.Posts
                .Where(p => p.Title.StartsWith("A"))
                .ToListAsync();
            #endregion

            #region Limit25
            var blogs25 = await context.Posts
                .Where(p => p.Title.StartsWith("A"))
                .Take(25)
                .ToListAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region EagerlyLoadRelatedAndProject
            await foreach (var blog in context.Blogs.Select(b => new { b.Url, b.Posts }).AsAsyncEnumerable())
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
            var blogsList = await context.Posts.Where(p => p.Title.StartsWith("A")).ToListAsync();
            var blogsArray = await context.Posts.Where(p => p.Title.StartsWith("A")).ToArrayAsync();

            // Foreach streams, processing one row at a time:
            await foreach (var blog in context.Posts.Where(p => p.Title.StartsWith("A")).AsAsyncEnumerable())
            {
                // ...
            }

            // AsAsyncEnumerable also streams, allowing you to execute LINQ operators on the client-side:
            var doubleFilteredBlogs = context.Posts
                .Where(p => p.Title.StartsWith("A")) // Translated to SQL and executed in the database
                .AsAsyncEnumerable()
                .Where(p => SomeDotNetMethod(p)); // Executed at the client on all database results
            #endregion

            // This method represents a filter that cannot be translated to SQL for execution in the
            // database, and must be run on the client as a .NET method
            static bool SomeDotNetMethod(Post post) => true;
        }

        using (var context = new BloggingContext())
        {
            #region SaveChangesBatching
            var blog = await context.Blogs.SingleAsync(b => b.Url == "http://someblog.microsoft.com");
            blog.Url = "http://someotherblog.microsoft.com";
            context.Add(new Blog { Url = "http://newblog1.microsoft.com" });
            context.Add(new Blog { Url = "http://newblog2.microsoft.com" });
            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region QueriesWithConstants
            var post1 = await context.Posts.FirstOrDefaultAsync(p => p.Title == "post1");
            var post2 = await context.Posts.FirstOrDefaultAsync(p => p.Title == "post2");
            #endregion
        }

        using (var context = new BloggingContext())
        {
            #region QueriesWithParameterization
            var postTitle = "post1";
            var post1 = await context.Posts.FirstOrDefaultAsync(p => p.Title == postTitle);
            postTitle = "post2";
            var post2 = await context.Posts.FirstOrDefaultAsync(p => p.Title == postTitle);
            #endregion
        }

        using (var context = new LazyBloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

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

            await context.SaveChangesAsync();
        }

        using (var context = new LazyBloggingContext())
        {
            #region NPlusOne
            foreach (var blog in await context.Blogs.ToListAsync())
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
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            for (var i = 0; i < 10; i++)
            {
                context.Employees.Add(new Employee());
            }
        }

        using (var context = new EmployeeContext())
        {
            #region UpdateWithoutBulk
            await foreach (var employee in context.Employees.AsAsyncEnumerable())
            {
                employee.Salary += 1000;
            }

            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new EmployeeContext())
        {
            #region UpdateWithBulk
            await context.Database.ExecuteSqlRawAsync("UPDATE [Employees] SET [Salary] = [Salary] + 1000");
            #endregion
        }

        using (var context = new PooledBloggingContext(
                   new DbContextOptionsBuilder()
                       .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0")
                       .Options))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        #region DbContextPoolingWithoutDI
        var options = new DbContextOptionsBuilder<PooledBloggingContext>()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0")
            .Options;

        var factory = new PooledDbContextFactory<PooledBloggingContext>(options);

        using (var context = factory.CreateDbContext())
        {
            var allPosts = await context.Posts.ToListAsync();
        }
        #endregion

        using (var context = new BloggingContext())
        {
            #region CompiledQueryExecute
            await foreach (var blog in _compiledQuery(context, 8))
            {
                // Do something with the results
            }
            #endregion
        }
    }
}
