using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.NoIdentity
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Sample
            modelBuilder.Entity<Blog>()
                .Property(b => b.BlogId)
                .ValueGeneratedNever();
            #endregion
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
