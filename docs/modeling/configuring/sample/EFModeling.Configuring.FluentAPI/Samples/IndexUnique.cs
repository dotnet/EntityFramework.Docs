using Microsoft.Data.Entity;

namespace EFModeling.Configuring.FluentAPI.Samples.IndexUnique
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Index(b => b.Url)
                .Unique();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
