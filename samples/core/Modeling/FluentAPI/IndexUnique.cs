using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.IndexUnique
{
    internal class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region IndexUnique
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.Url)
                .IsUnique();
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
