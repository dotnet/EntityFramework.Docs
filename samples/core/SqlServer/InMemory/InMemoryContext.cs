using Microsoft.EntityFrameworkCore;

namespace SqlServer.InMemory
{
    public class InMemoryContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region IsMemoryOptimized
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().IsMemoryOptimized();
        }
        #endregion
    }
}
