using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.TableAndSchema
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .ToTable("blogs", schema: "blogging");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
