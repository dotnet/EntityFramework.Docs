using Microsoft.EntityFrameworkCore;
using System;

namespace EFModeling.FluentAPI.IgnoreType
{
    class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<BlogMetadata>();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public BlogMetadata Metadata { get; set; }
    }

    public class BlogMetadata
    {
        public DateTime LoadedFromDatabase { get; set; }
    }
}
