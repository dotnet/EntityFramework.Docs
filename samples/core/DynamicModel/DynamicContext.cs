using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.Samples.DynamicModel
{

    #region Class
    public class DynamicContext : DbContext
    {
        public bool? IgnoreIntProperty { get; set; }

        public DbSet<ConfigurableEntity> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseInMemoryDatabase("DynamicContext")
                .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (IgnoreIntProperty.HasValue)
            {
                if (IgnoreIntProperty.Value)
                {
                    modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.IntProperty);
                }
                else
                {
                    modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.StringProperty);
                }
            }
        }
    }
    #endregion
}
