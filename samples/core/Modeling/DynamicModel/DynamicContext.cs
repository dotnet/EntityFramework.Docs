using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFModeling.DynamicModel;

public class DynamicContext : DbContext
{
    public bool UseIntProperty { get; set; }

    public DbSet<ConfigurableEntity> Entities { get; set; }

    #region OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseInMemoryDatabase("DynamicContext")
            .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
    #endregion

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (UseIntProperty)
        {
            modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.StringProperty);
        }
        else
        {
            modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.IntProperty);
        }
    }
    #endregion
}