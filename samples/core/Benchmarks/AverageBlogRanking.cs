using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

[MemoryDiagnoser]
public class AverageBlogRanking
{
    [Params(1000)]
    public int NumBlogs; // number of records to write [once], and read [each pass]

    [GlobalSetup]
    public async Task Setup()
    {
        using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(NumBlogs);
    }

    #region LoadEntities
    [Benchmark]
    public async Task<double> LoadEntities()
    {
        var sum = 0;
        var count = 0;
        using var ctx = new BloggingContext();
        await foreach (var blog in ctx.Blogs.AsAsyncEnumerable())
        {
            sum += blog.Rating;
            count++;
        }

        return (double)sum / count;
    }
    #endregion

    #region LoadEntitiesNoTracking
    [Benchmark]
    public async Task<double> LoadEntitiesNoTracking()
    {
        var sum = 0;
        var count = 0;
        using var ctx = new BloggingContext();
        await foreach (var blog in ctx.Blogs.AsNoTracking().AsAsyncEnumerable())
        {
            sum += blog.Rating;
            count++;
        }

        return (double)sum / count;
    }
    #endregion

    #region ProjectOnlyRanking
    [Benchmark]
    public async Task<double> ProjectOnlyRanking()
    {
        var sum = 0;
        var count = 0;
        using var ctx = new BloggingContext();
        await foreach (var rating in ctx.Blogs.Select(b => b.Rating).AsAsyncEnumerable())
        {
            sum += rating;
            count++;
        }

        return (double)sum / count;
    }
    #endregion

    #region CalculateInDatabase
    [Benchmark(Baseline = true)]
    public async Task<double> CalculateInDatabase()
    {
        using var ctx = new BloggingContext();
        return await ctx.Blogs.AverageAsync(b => b.Rating);
    }
    #endregion

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0");

        public async Task SeedData(int numblogs)
        {
            Blogs.AddRange(
                Enumerable.Range(0, numblogs).Select(
                    i => new Blog
                    {
                        Name = $"Blog{i}", Url = $"blog{i}.blogs.net", CreationTime = new DateTime(2020, 1, 1), Rating = i % 5
                    }));
            await SaveChangesAsync();
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
