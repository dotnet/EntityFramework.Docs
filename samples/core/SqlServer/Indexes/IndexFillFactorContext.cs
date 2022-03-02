using Microsoft.EntityFrameworkCore;

namespace SqlServer.Indexes;

public class IndexFillFactorContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region IndexFillFactor
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().HasIndex(b => b.PublishedOn).HasFillFactor(10);
    }
    #endregion
}