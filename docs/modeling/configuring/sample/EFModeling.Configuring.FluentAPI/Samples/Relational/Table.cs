using Microsoft.Data.Entity;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.Table
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .ForRelational()
                .Table("blogs");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
