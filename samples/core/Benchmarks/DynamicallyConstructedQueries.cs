using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class DynamicallyConstructedQueries
    {
        private int _blogNumber;

        [GlobalSetup]
        public static void GlobalSetup()
        {
            using var context = new BloggingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        #region WithConstant
        [Benchmark]
        public int WithConstant()
        {
            return GetBlogCount("blog" + Interlocked.Increment(ref _blogNumber));

            static int GetBlogCount(string url)
            {
                using var context = new BloggingContext();

                IQueryable<Blog> blogs = context.Blogs;

                if (url is not null)
                {
                    var blogParam = Expression.Parameter(typeof(Blog), "b");
                    var whereLambda = Expression.Lambda<Func<Blog, bool>>(
                        Expression.Equal(
                            Expression.MakeMemberAccess(
                                blogParam,
                                typeof(Blog).GetMember(nameof(Blog.Url)).Single()
                            ),
                            Expression.Constant(url)),
                        blogParam);

                    blogs = blogs.Where(whereLambda);
                }

                return blogs.Count();
            }
        }
        #endregion

        #region WithParameter
        [Benchmark]
        public int WithParameter()
        {
            return GetBlogCount("blog" + Interlocked.Increment(ref _blogNumber));

            int GetBlogCount(string url)
            {
                using var context = new BloggingContext();

                IQueryable<Blog> blogs = context.Blogs;

                if (url is not null)
                {
                    blogs = blogs.Where(b => b.Url == url);
                }

                return blogs.Count();
            }
        }
        #endregion

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
            public int Rating { get; set; }
            public List<Post> Posts { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }
    }
}
