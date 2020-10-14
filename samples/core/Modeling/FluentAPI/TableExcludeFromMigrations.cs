using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.TableExcludeFromMigrations
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region TableExcludeFromMigrations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .ToTable("blogs", t => t.ExcludeFromMigrations());
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
