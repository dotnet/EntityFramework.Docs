using Microsoft.EntityFrameworkCore;
using System;

namespace EFModeling.FluentAPI.TableComment
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        #region TableComment
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasComment("Blogs managed on the website");
        }
        #endregion
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
