using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.DefaultDiscriminator
{
    public class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DiscriminatorConfiguration
            modelBuilder.Entity<Blog>()
                .Property("Discriminator")
                .HasMaxLength(200);
            #endregion
        }
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
}
