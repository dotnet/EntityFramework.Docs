using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.TableAndSchema
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region table
            modelBuilder.Entity<Blog>()
                .ToTable("blogs", schema: "blogging");
            #endregion
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
