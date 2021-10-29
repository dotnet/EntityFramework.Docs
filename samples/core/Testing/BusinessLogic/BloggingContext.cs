using System;
using Microsoft.EntityFrameworkCore;

namespace EF.Testing.BusinessLogic
{
    public class BloggingContext : DbContext
    {
        #region Constructors
        public BloggingContext()
        {
        }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        #endregion

        public DbSet<Blog> Blogs => Set<Blog>();

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True");
            }
        }
        #endregion
    }
}
