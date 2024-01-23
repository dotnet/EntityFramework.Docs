using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFAsyncWithSystemInteractive;

public class Program
{
    public static async Task Main()
    {
        await using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        #region SystemInteractiveAsync
        System.Collections.Generic.List<IAsyncGrouping<int, Blog>> groupedHighlyRatedBlogs = await context.Blogs
            .AsQueryable()
            .Where(b => b.Rating > 3) // server-evaluated
            .AsAsyncEnumerable()
            .GroupBy(b => b.Rating) // client-evaluated
            .ToListAsync();
        #endregion
    }
}

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFAsync;Trusted_Connection=True");
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}
