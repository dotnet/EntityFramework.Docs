using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFAsync;

public class Program
{
    public static async Task Main(string[] args)
    {
        await using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        #region SaveChangesAsync
        var blog = new Blog { Url = "http://sample.com" };
        context.Blogs.Add(blog);
        await context.SaveChangesAsync();
        #endregion

        #region ToListAsync
        var blogs = await context.Blogs.Where(b => b.Rating > 3).ToListAsync();
        #endregion
    }
}

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFAsync;Trusted_Connection=True;ConnectRetryCount=0");
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}
