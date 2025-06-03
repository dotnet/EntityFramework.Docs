using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.DefaultValue;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region DefaultValue
        modelBuilder.Entity<Blog>()
            .Property(b => b.Rating)
            .HasDefaultValue(3);
        #endregion
    
        #region DefaultValueNamed
        modelBuilder.Entity<Blog>()
            .Property(b => b.Rating)
            .HasDefaultValue(3, "DF_Blog_IsActive");
        #endregion
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
}