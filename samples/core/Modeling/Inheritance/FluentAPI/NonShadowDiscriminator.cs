using Microsoft.EntityFrameworkCore;

namespace EFModeling.Inheritance.FluentAPI.NonShadowDiscriminator;

public class MyContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region NonShadowDiscriminator
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .HasDiscriminator(b => b.BlogType);

        modelBuilder.Entity<Blog>()
            .Property(e => e.BlogType)
            .HasMaxLength(200)
            .HasColumnName("blog_type");
            
        modelBuilder.Entity<RssBlog>();
    }
    #endregion
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public string BlogType { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}
