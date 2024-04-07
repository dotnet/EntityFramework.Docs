using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore9;

public class BlogsContext : DbContext
{
    public DbSet<Blog> Blogs => Set<Blog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EF9CompiledModels;ConnectRetryCount=0")
            .LogTo(_ => Console.WriteLine(">> EF is building the model..."), [CoreEventId.ShadowPropertyCreated])
            .EnableSensitiveDataLogging();
}
