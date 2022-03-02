using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.DefaultValue;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region DefaultValue
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Rating)
            .HasDefaultValue(3);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}