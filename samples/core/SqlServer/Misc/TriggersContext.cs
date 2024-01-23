
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace SqlServer.Misc;

public class TriggersContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    #region TriggerConfiguration
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Blog>()
            .ToTable(tb => tb.HasTrigger("SomeTrigger"));
    #endregion

    #region ConfigureConventions
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) => configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    #endregion
}

#region BlankTriggerAddingConvention
public class BlankTriggerAddingConvention : IModelFinalizingConvention
{
    public virtual void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            StoreObjectIdentifier? table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            if (table != null
                && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null)
                && (entityType.BaseType == null
                    || entityType.GetMappingStrategy() != RelationalAnnotationNames.TphMappingStrategy))
            {
                entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
            }

            foreach (IConventionEntityTypeMappingFragment fragment in entityType.GetMappingFragments(StoreObjectType.Table))
            {
                if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null))
                {
                    entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                }
            }
        }
    }
}
#endregion
