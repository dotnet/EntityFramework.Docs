using Microsoft.EntityFrameworkCore;

namespace EFModeling.Inheritance.FluentAPI.DiscriminatorPropertyConfiguration;

public class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region DiscriminatorPropertyConfiguration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property("blog_type")
            .HasMaxLength(200);
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}
