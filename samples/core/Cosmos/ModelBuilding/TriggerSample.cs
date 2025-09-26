using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.ModelBuilding;

public static class TriggerSample
{
    public static void ConfigureTriggers()
    {
        var contextOptions = new DbContextOptionsBuilder<TriggerContext>()
            .UseCosmos("https://localhost:8081", "account-key", "sample");
        
        using var context = new TriggerContext(contextOptions.Options);
        
        // Triggers would be configured in OnModelCreating when the API is available
    }
}

public class TriggerContext : DbContext
{
    public TriggerContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region TriggerConfiguration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasPartitionKey(p => p.Category);
            
            // Configure pre-trigger for create operations (EF Core 9+)
            // entity.HasTrigger("PreInsertTrigger", TriggerType.Pre, TriggerOperation.Create);
            
            // Configure post-trigger for delete operations (EF Core 9+)
            // entity.HasTrigger("PostDeleteTrigger", TriggerType.Post, TriggerOperation.Delete);
            
            // Configure trigger for replace operations (EF Core 9+)
            // entity.HasTrigger("UpdateTrigger", TriggerType.Pre, TriggerOperation.Replace);
        });
        #endregion
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Category { get; set; } = null!;
}