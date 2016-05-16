using Microsoft.EntityFrameworkCore;
using System;

namespace EFModeling.Configuring.FluentAPI.Samples.IgnoreProperty
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Ignore(b => b.LoadedFromDatabase);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public DateTime LoadedFromDatabase { get; set; }
    }
}
