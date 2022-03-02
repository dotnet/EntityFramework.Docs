using Microsoft.EntityFrameworkCore;

namespace SqlServer.Indexes;

public class ClusteredIndexContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ClusteredIndex
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().HasIndex(b => b.PublishedOn).IsClustered();
    }
    #endregion
}