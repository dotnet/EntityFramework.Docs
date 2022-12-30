using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.MaxLength;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region MaxLength
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Url)
            .HasMaxLength(500);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}