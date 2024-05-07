using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NewInEfCore7;

public static class ValueGenerationSample
{
    public static async Task Can_use_value_generation_with_converted_types()
    {
        PrintSampleName();

        await using var context = new ProductsContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.AddRangeAsync(
            new Category("Foods") { Products = { new("Marmite"), new("Toast"), new("Butter") } },
            new Category("Beverages") { Products = { new("Tea"), new("Milk") } });

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var categoriesAndProducts = await context.Categories.Include(category => category.Products).ToListAsync();

        Console.WriteLine();
        foreach (var category in categoriesAndProducts)
        {
            Console.WriteLine($"Category {category.Id.Value} is '{category.Name}'");
            foreach (var product in category.Products)
            {
                Console.WriteLine($"   Product {product.Id.Value} is '{product.Name}'");
            }
        }
        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database=Products;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ValueGeneratedOnAdd
            modelBuilder.Entity<Product>().Property(product => product.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(category => category.Id).ValueGeneratedOnAdd();
            #endregion

            #region ConfigureSequence
            modelBuilder
                .HasSequence<int>("ProductsSequence", "northwind")
                .StartsAt(1000)
                .IncrementsBy(2);
            #endregion

            #region Sequence
            modelBuilder
                .Entity<Product>()
                .Property(product => product.Id)
                .UseSequence("ProductsSequence", "northwind");
            #endregion
        }

        #region KeyConverters
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
            configurationBuilder.Properties<CategoryId>().HaveConversion<CategoryIdConverter>();
        }

        private class ProductIdConverter : ValueConverter<ProductId, int>
        {
            public ProductIdConverter()
                : base(v => v.Value, v => new(v))
            {
            }
        }

        private class CategoryIdConverter : ValueConverter<CategoryId, int>
        {
            public CategoryIdConverter()
                : base(v => v.Value, v => new(v))
            {
            }
        }
        #endregion
    }

    #region GuardedKeys
    public readonly struct ProductId
    {
        public ProductId(int value) => Value = value;
        public int Value { get; }
    }

    public readonly struct CategoryId
    {
        public CategoryId(int value) => Value = value;
        public int Value { get; }
    }
    #endregion

    #region ProductAndCategory
    public class Product
    {
        public Product(string name) => Name = name;
        public ProductId Id { get; set; }
        public string Name { get; set; }
        public CategoryId CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }

    public class Category
    {
        public Category(string name) => Name = name;
        public CategoryId Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; } = new();
    }
    #endregion
}
