using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.NonShadowDiscriminator
{
    #region NonShadowDiscriminator
    public class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property("Discriminator")
                .HasMaxLength(200);

            modelBuilder.Entity<Blog>()
                .HasDiscriminator(b => b.BlogType);

            modelBuilder.Entity<Blog>()
                .Property(e => e.BlogType)
                .HasMaxLength(200)
                .HasColumnName("blog_type");
        }
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
    #endregion
}
