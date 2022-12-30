using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.FluentAPI.IndexDescending;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region IndexDescending
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasIndex(b => new { b.Url, b.Rating })
            .IsDescending();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}
