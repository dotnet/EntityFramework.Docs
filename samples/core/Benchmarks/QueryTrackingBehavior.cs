using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks;

[MemoryDiagnoser]
public class QueryTrackingBehavior
{
    [Params(10)]
    public int NumBlogs { get; set; }

    [Params(20)]
    public int NumPostsPerBlog { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        Console.WriteLine("Setting up database...");
        using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await BloggingContext.SeedData(NumBlogs, NumPostsPerBlog);
        Console.WriteLine("Setup complete.");
    }

    [Benchmark(Baseline = true)]
    public async Task<List<Post>> AsTracking()
    {
        using var context = new BloggingContext();

        return await context.Posts.AsTracking().Include(p => p.Blog).ToListAsync();
    }

    [Benchmark]
    public async Task<List<Post>> AsNoTracking()
    {
        using var context = new BloggingContext();

        return await context.Posts.AsNoTracking().Include(p => p.Blog).ToListAsync();
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0");

        public static async Task SeedData(int numBlogs, int numPostsPerBlog)
        {
            using var context = new BloggingContext();
            context.AddRange(
                Enumerable.Range(0, numBlogs)
                    .Select(_ => new Blog { Url = "Some URL", Posts = Enumerable.Range(0, numPostsPerBlog)
                    .Select(_ => new Post() { Title = "Some Title", Content = "Some Content"}).ToList() }));
            await context.SaveChangesAsync();
        }
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
