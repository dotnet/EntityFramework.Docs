using System;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic
{
    public class BloggingContext : DbContext
    {
        private readonly Action<BloggingContext, ModelBuilder> _customizeModel;

        #region Constructors
        public BloggingContext()
        {
        }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        public BloggingContext(DbContextOptions<BloggingContext> options, Action<BloggingContext, ModelBuilder> customizeModel)
            : base(options)
        {
            // customizeModel must be the same for every instance in a given application.
            // Otherwise a custom IModelCacheKeyFactory implementation must be provided.
            _customizeModel = customizeModel;
        }
        #endregion

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<UrlResource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlResource>().HasNoKey()
                .ToView("AllResources");

            if (_customizeModel != null)
            {
                _customizeModel(this, modelBuilder);
            }
        }

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }
        #endregion
    }
}
