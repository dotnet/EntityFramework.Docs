using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.InheritanceTphAbstractDiscriminator
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasDiscriminator(b => b.BlogId)
                .HasValue<MySpecialBlog>(42);
        }
    }

    public abstract class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }

    public class MyCustomBlog : Blog
    {
        public virtual string MyCustomMethod()
        {
            return "I'm a custom blog";
        }
    }
}
