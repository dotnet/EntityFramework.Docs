using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class AverageBlogRanking
    {
        [Params(1000)]
        public int NumBlogs; // number of records to write [once], and read [each pass]

        [GlobalSetup]
        public void Setup()
        {
            using var context = new BloggingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SeedData(NumBlogs);
        }

        #region LoadEntities
        [Benchmark]
        public double LoadEntities()
        {
            var sum = 0;
            var count = 0;
            using var ctx = new BloggingContext();
            foreach (var blog in ctx.Blogs)
            {
                sum += blog.Rating;
                count++;
            }

            return (double)sum / count;
        }
        #endregion

        #region LoadEntitiesNoTracking
        [Benchmark]
        public double LoadEntitiesNoTracking()
        {
            var sum = 0;
            var count = 0;
            using var ctx = new BloggingContext();
            foreach (var blog in ctx.Blogs.AsNoTracking())
            {
                sum += blog.Rating;
                count++;
            }

            return (double)sum / count;
        }
        #endregion

        #region ProjectOnlyRanking
        [Benchmark]
        public double ProjectOnlyRanking()
        {
            var sum = 0;
            var count = 0;
            using var ctx = new BloggingContext();
            foreach (var rating in ctx.Blogs.Select(b => b.Rating))
            {
                sum += rating;
                count++;
            }

            return (double)sum / count;
        }
        #endregion

        #region CalculateInDatabase
        [Benchmark(Baseline = true)]
        public double CalculateInDatabase()
        {
            using var ctx = new BloggingContext();
            return ctx.Blogs.Average(b => b.Rating);
        }
        #endregion

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");

            public void SeedData(int numblogs)
            {
                Blogs.AddRange(
                    Enumerable.Range(0, numblogs).Select(
                        i => new Blog
                        {
                            Name = $"Blog{i}", Url = $"blog{i}.blogs.net", CreationTime = new DateTime(2020, 1, 1), Rating = i % 5
                        }));
                SaveChanges();
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public DateTime CreationTime { get; set; }
            public int Rating { get; set; }
        }
    }
}
