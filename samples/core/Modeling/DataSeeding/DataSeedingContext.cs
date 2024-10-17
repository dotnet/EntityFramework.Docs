using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding;

public class DataSeedingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    #region ContextOptionSeeding
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFDataSeeding;Trusted_Connection=True;ConnectRetryCount=0")
            .UseSeeding((context, _) =>
            {
                var testBlog = context.Set<Blog>().FirstOrDefault(b => b.Url == "http://test.com");
                if (testBlog == null)
                {
                    context.Set<Blog>().Add(new Blog { Url = "http://test.com" });
                    context.SaveChanges();
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var testBlog = await context.Set<Blog>().FirstOrDefaultAsync(b => b.Url == "http://test.com", cancellationToken);
                if (testBlog == null)
                {
                    context.Set<Blog>().Add(new Blog { Url = "http://test.com" });
                    await context.SaveChangesAsync(cancellationToken);
                }
            });
    #endregion
}

