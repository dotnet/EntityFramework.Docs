using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.Configuring.FluentAPI.Samples.Relational.ForSqlServerHasIndex
{
    #region Model
    class MyContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .ForSqlServerHasIndex(p => p.Url)
                .ForSqlServerInclude(p => new
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
