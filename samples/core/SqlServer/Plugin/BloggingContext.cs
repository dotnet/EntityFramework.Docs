using Microsoft.EntityFrameworkCore;

namespace SqlServer.Plugin;
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFSaving.Basics;Trusted_Connection=True"
            , o => o.UseAugmentation()
            );
}
