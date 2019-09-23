using Microsoft.EntityFrameworkCore;

namespace EFSaving.Async
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFSaving.Async;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
}
