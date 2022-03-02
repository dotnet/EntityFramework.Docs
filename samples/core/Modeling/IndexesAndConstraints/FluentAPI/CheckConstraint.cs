using Microsoft.EntityFrameworkCore;

namespace EFModeling.IndexesAndConstraints.FluentAPI.CheckConstraint;

internal class MyContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    #region CheckConstraint
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasCheckConstraint("CK_Prices", "[Price] > [DiscountedPrice]", c => c.HasName("CK_Product_Prices"));
    }
    #endregion
}

public class Product
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
}