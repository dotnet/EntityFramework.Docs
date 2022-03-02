using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.PrecisionAndScale;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region PrecisionAndScale
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Score)
            .HasPrecision(14, 2);

        modelBuilder.Entity<Blog>()
            .Property(b => b.LastUpdated)
            .HasPrecision(3);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public decimal Score { get; set; }
    public DateTime LastUpdated { get; set; }
}