using Microsoft.EntityFrameworkCore;

namespace EFSaving.Disconnected
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFSaving.Disconnected;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
}
