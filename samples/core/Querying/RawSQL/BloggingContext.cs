using Microsoft.EntityFrameworkCore;

namespace EFQuerying.RawSQL
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasData(
                    new Blog { BlogId = 1, Url = @"https://devblogs.microsoft.com/dotnet", Rating = 5 },
                    new Blog { BlogId = 2, Url = @"https://mytravelblog.com/", Rating = 4 });

            modelBuilder.Entity<Post>()
                .HasData(
                    new Post
                    {
                        PostId = 1,
                        BlogId = 1,
                        Title = "What's new",
                        Content = "Lorem ipsum dolor sit amet",
                        Rating = 5
                    },
                    new Post
                    {
                        PostId = 2,
                        BlogId = 2,
                        Title = "Around the World in Eighty Days",
                        Content = "consectetur adipiscing elit",
                        Rating = 5
                    },
                    new Post
                    {
                        PostId = 3,
                        BlogId = 2,
                        Title = "Glamping *is* the way",
                        Content = "sed do eiusmod tempor incididunt",
                        Rating = 4
                    },
                    new Post
                    {
                        PostId = 4,
                        BlogId = 2,
                        Title = "Travel in the time of pandemic",
                        Content = "ut labore et dolore magna aliqua",
                        Rating = 3
                    });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EFQuerying.RawSQL;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
}
