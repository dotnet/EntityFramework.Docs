using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

[MemoryDiagnoser]
public class ContextPooling
{
    private DbContextOptions<BloggingContext> _options;
    private PooledDbContextFactory<BloggingContext> _poolingFactory;

    [Params(1)]
    public int NumBlogs { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        _options = new DbContextOptionsBuilder<BloggingContext>()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;ConnectRetryCount=0")
            .Options;

        using var context = new BloggingContext(_options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedData(NumBlogs);

        _poolingFactory = new PooledDbContextFactory<BloggingContext>(_options);
    }

    [Benchmark]
    public async Task<List<Blog>> WithoutContextPooling()
    {
        using var context = new BloggingContext(_options);

        return await context.Blogs.ToListAsync();
    }

    [Benchmark]
    public async Task<List<Blog>> WithContextPooling()
    {
        using var context = _poolingFactory.CreateDbContext();

        return await context.Blogs.ToListAsync();
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public BloggingContext(DbContextOptions options) : base(options) {}

        public async Task SeedData(int numBlogs)
        {
            Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(i => new Blog { Url = $"http://www.someblog{i}.com"}));
            await SaveChangesAsync();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
    }
}
