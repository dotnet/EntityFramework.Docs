using Microsoft.EntityFrameworkCore;
using System;

namespace EFModeling.Configuring.FluentAPI.Samples.ValueGeneratedOnAddOrUpdate
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Sample
            modelBuilder.Entity<Blog>()
                .Property(b => b.LastUpdated)
                .ValueGeneratedOnAddOrUpdate();
            #endregion
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
