using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace EFGetStarted.UWP
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = "Blogging2.db";
            try
            {
                databaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch (InvalidOperationException)
            { }

            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make Blog.Url required
            modelBuilder.Entity<Blog>()
                .Property(b => b.Url)
                .Required();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}