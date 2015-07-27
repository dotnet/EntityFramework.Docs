using Microsoft.Data.Entity;

namespace EFModeling.Configuring.FluentAPI.Samples.SentinelValue
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Note there is no Fluent API for sentinel value in Beta6
            //      so we must drop down to metadata
            modelBuilder.Entity<Blog>()
                .Property(b => b.BlogId)
                .Metadata.SentinelValue = -1;
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
