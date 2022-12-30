using Microsoft.EntityFrameworkCore;

namespace GetStartedWinForms;

public class ProductsContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=products.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Cheese" },
            new Category { CategoryId = 2, Name = "Meat" },
            new Category { CategoryId = 3, Name = "Fish" },
            new Category { CategoryId = 4, Name = "Bread" });

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, CategoryId = 1, Name = "Cheddar" },
            new Product { ProductId = 2, CategoryId = 1, Name = "Brie" },
            new Product { ProductId = 3, CategoryId = 1, Name = "Stilton" },
            new Product { ProductId = 4, CategoryId = 1, Name = "Cheshire" },
            new Product { ProductId = 5, CategoryId = 1, Name = "Swiss" },
            new Product { ProductId = 6, CategoryId = 1, Name = "Gruyere" },
            new Product { ProductId = 7, CategoryId = 1, Name = "Colby" },
            new Product { ProductId = 8, CategoryId = 1, Name = "Mozzela" },
            new Product { ProductId = 9, CategoryId = 1, Name = "Ricotta" },
            new Product { ProductId = 10, CategoryId = 1, Name = "Parmesan" },
            new Product { ProductId = 11, CategoryId = 2, Name = "Ham" },
            new Product { ProductId = 12, CategoryId = 2, Name = "Beef" },
            new Product { ProductId = 13, CategoryId = 2, Name = "Chicken" },
            new Product { ProductId = 14, CategoryId = 2, Name = "Turkey" },
            new Product { ProductId = 15, CategoryId = 2, Name = "Prosciutto" },
            new Product { ProductId = 16, CategoryId = 2, Name = "Bacon" },
            new Product { ProductId = 17, CategoryId = 2, Name = "Mutton" },
            new Product { ProductId = 18, CategoryId = 2, Name = "Pastrami" },
            new Product { ProductId = 19, CategoryId = 2, Name = "Hazlet" },
            new Product { ProductId = 20, CategoryId = 2, Name = "Salami" },
            new Product { ProductId = 21, CategoryId = 3, Name = "Salmon" },
            new Product { ProductId = 22, CategoryId = 3, Name = "Tuna" },
            new Product { ProductId = 23, CategoryId = 3, Name = "Mackerel" },
            new Product { ProductId = 24, CategoryId = 4, Name = "Rye" },
            new Product { ProductId = 25, CategoryId = 4, Name = "Wheat" },
            new Product { ProductId = 26, CategoryId = 4, Name = "Brioche" },
            new Product { ProductId = 27, CategoryId = 4, Name = "Naan" },
            new Product { ProductId = 28, CategoryId = 4, Name = "Focaccia" },
            new Product { ProductId = 29, CategoryId = 4, Name = "Malted" },
            new Product { ProductId = 30, CategoryId = 4, Name = "Sourdough" },
            new Product { ProductId = 31, CategoryId = 4, Name = "Corn" },
            new Product { ProductId = 32, CategoryId = 4, Name = "White" },
            new Product { ProductId = 33, CategoryId = 4, Name = "Soda" });
    }
}
