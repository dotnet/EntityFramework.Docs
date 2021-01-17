using Microsoft.EntityFrameworkCore;

namespace EFModeling.DataSeeding
{
    public class DataSeedingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFDataSeeding;Trusted_Connection=True;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity => { entity.Property(e => e.Url).IsRequired(); });

            #region BlogSeed
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 1, Url = "http://sample.com" });
            #endregion

            modelBuilder.Entity<Post>(
                entity =>
                {
                    entity.HasOne(d => d.Blog)
                        .WithMany(p => p.Posts)
                        .HasForeignKey("BlogId");
                });

            #region PostSeed
            modelBuilder.Entity<Post>().HasData(
                new Post { BlogId = 1, PostId = 1, Title = "First post", Content = "Test 1" });
            #endregion

            #region AnonymousPostSeed
            modelBuilder.Entity<Post>().HasData(
                new { BlogId = 1, PostId = 2, Title = "Second post", Content = "Test 2" });
            #endregion

            #region OwnedTypeSeed
            modelBuilder.Entity<Post>().OwnsOne(p => p.AuthorName).HasData(
                new { PostId = 1, First = "Andriy", Last = "Svyryd" },
                new { PostId = 2, First = "Diego", Last = "Vega" });
            #endregion
        }
    }
}
