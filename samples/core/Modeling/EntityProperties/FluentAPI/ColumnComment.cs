using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.FluentAPI.ColumnComment;

internal class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region ColumnComment
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Url)
            .HasComment("The URL of the blog");
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}