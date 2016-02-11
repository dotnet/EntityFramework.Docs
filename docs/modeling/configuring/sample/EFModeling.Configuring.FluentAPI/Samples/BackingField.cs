using Microsoft.Data.Entity;

namespace EFModeling.Configuring.FluentAPI.Samples.BackingField
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(b => b.Url)
                .HasAnnotation("BackingField", "_blogUrl");
        }
    }

    public class Blog
    {
        private string _blogUrl;

        public int BlogId { get; set; }
        public string Url => _blogUrl;
    }
}
