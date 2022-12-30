using Microsoft.EntityFrameworkCore;

namespace EFModeling.GeneratedProperties.FluentAPI.ValueGeneratedNever;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ValueGeneratedNever
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.BlogId)
            .ValueGeneratedNever();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}