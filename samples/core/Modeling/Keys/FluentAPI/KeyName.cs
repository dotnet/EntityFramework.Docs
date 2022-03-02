using Microsoft.EntityFrameworkCore;

namespace EFModeling.Keys.FluentAPI.KeyName;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region KeyName
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasKey(b => b.BlogId)
            .HasName("PrimaryKey_BlogId");
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}