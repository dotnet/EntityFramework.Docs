using Microsoft.Data.Entity;

namespace EFLogging
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFLogging;Trusted_Connection=True;");
        }
    }
}
