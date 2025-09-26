using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cosmos.ModelBuilding;

public static class TriggerSample
{
    public static async Task ConfigureTriggers()
    {
        var contextOptions = new DbContextOptionsBuilder<TriggerContext>()
            .UseCosmos("https://localhost:8081", "account-key", "sample");
        
        using var context = new TriggerContext(contextOptions.Options);
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Create a new product - this will trigger the PreInsertTrigger
        var product = new Product 
        { 
            Id = 1, 
            Name = "Sample Product", 
            Price = 19.99m, 
            Category = "Electronics" 
        };
        
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        // Update the product - this will trigger the UpdateTrigger
        product.Price = 24.99m;
        await context.SaveChangesAsync();
        
        // Delete the product - this will trigger the PostDeleteTrigger
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}

public class TriggerContext : DbContext
{
    public TriggerContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasPartitionKey(p => p.Category);
            
            #region TriggerConfiguration
            // Configure pre-trigger for create operations
            entity.HasTrigger("PreInsertTrigger", TriggerType.Pre, TriggerOperation.Create);
            
            // Configure post-trigger for delete operations
            entity.HasTrigger("PostDeleteTrigger", TriggerType.Post, TriggerOperation.Delete);
            
            // Configure trigger for replace operations
            entity.HasTrigger("UpdateTrigger", TriggerType.Pre, TriggerOperation.Replace);
            #endregion
        });
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Category { get; set; } = null!;
}