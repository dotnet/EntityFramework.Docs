using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BenchCommon
{
    public class BlogContext : DbContext
    {
        public const int NumBlogs = 1000;

        public BlogContext()
        { }
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        { }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.BlogId)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Email, "IX_People_Email")
                    .IsUnique();
        }

        public void SeedData()
        {
            Blogs.AddRange(
                Enumerable.Range(0, NumBlogs).Select(i => new Blog
                {
                    BlogId = i + 1,
                    //Title = "",
                    Name = $"Blog{i}",
                    Url = $"blog{i}.blogs.net",
                    CreationTime = new DateTime(2020,1,2),
                    Rating = i % 5
                }));
            SaveChanges();
        }
    }
}
