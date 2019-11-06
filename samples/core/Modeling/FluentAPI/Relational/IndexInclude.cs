using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.FluentAPI.Relational.IndexInclude
{
    #region Model
    class MyContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Url)
                .IncludeProperties(p => new
                {
                    p.Title,
                    p.PublishedOn
                })
                .HasName("Index_Url_Include_Title_PublishedOn");
        }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; }
    }
    #endregion
}
