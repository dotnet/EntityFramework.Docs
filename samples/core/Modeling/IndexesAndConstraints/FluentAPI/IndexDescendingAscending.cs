using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.FluentAPI.IndexDescendingAscending;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region IndexDescendingAscending
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasIndex(b => new { b.Url, b.Rating })
            .IsDescending(false, true);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}
