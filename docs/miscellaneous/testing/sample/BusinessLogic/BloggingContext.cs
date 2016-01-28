using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System;

namespace BusinessLogic
{
    public class BloggingContext : DbContext
    {
        public BloggingContext()
        { }

        public BloggingContext(IServiceProvider serviceProvider, DbContextOptions<BloggingContext> options)
            : base(serviceProvider, options)
        { }

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
        }
    }
}
