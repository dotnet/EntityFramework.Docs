using Microsoft.EntityFrameworkCore;
using System;

namespace EFModeling.Configuring.FluentAPI.Samples.ValueGeneratedOnAdd
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Sample
            modelBuilder.Entity<Blog>()
                .Property(b => b.Inserted)
                .ValueGeneratedOnAdd();
            #endregion
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public DateTime Inserted { get; set; }
    }
}
