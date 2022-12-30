using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.FluentAPI.IndexNoFilter;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region IndexNoFilter
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasIndex(b => b.Url)
            .IsUnique()
            .HasFilter(null);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}