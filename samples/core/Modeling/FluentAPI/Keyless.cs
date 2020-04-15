using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Keyless
{
    class BlogsContext : DbContext
    {
        public DbSet<BlogPostsCount> BlogPostCounts { get; set; }

        #region Keyless
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPostCounts>()
                .HasNoKey();
        }
        #endregion
    }

    public class BlogPostsCount
    {
        public string BlogName { get; set; }
        public int PostCount { get; set; }
    }
}
