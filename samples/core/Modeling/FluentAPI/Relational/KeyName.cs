using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.KeyName
{
    #region KeyName
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasKey(b => b.BlogId)
                .HasName("PrimaryKey_BlogId");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
    #endregion
}
