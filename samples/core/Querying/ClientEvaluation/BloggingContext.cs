using Microsoft.EntityFrameworkCore;

namespace EFQuerying.ClientEvaluation;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasData(
                new Blog { BlogId = 1, Url = @"https://devblogs.microsoft.com/dotnet", Rating = 5 },
                new Blog { BlogId = 2, Url = @"https://mytravelblog.com/", Rating = 4 });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFQuerying.ClientEvaluation;Trusted_Connection=True;ConnectRetryCount=0");
    }
}
