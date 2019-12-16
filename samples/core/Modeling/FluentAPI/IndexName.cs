using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.IndexName
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region IndexName
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.Url)
                .HasName("Index_Url");
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
