using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFModeling.Configuring.FluentAPI.Samples.Relationships.NoNavigation
{
    #region Model
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne<Blog>()
                .WithMany()
                .HasForeignKey(p => p.PostForeignKey);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int PostForeignKey { get; set; }
    }
    #endregion
}
