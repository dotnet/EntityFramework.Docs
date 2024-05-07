namespace NewInEfCore9;

public static class ComplexTypesSample
{
    public static Task GropupBy_complex_type_instances()
        => ExecuteQueries(sqlite: false);

    public static Task GropupBy_complex_type_instances_on_SQLite()
        => ExecuteQueries(sqlite: true);

    public static async Task ExecuteQueries(bool sqlite)
    {
        PrintSampleName();

        await using var context = new CustomerContext(useSqlite: sqlite);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(
            new Store
            {
                Customers = { new CustomerWithStores { Name = "Smokey", Region = "Germany" } },
                Region = "Germany",
                StoreAddress = new("L1", null, "Ci1", "Co1", "P1")
            },
            new Customer
            {
                Name = "Smokey",
                CustomerInfo = new("EF")
                {
                    HomeAddress = new("L2", null, "Ci2", "Co2", "P2"), WorkAddress = new("L3", null, "Ci3", "Co3", "P3")
                },
            },
            new CustomerTpt
            {
                Name = "Willow",
                CustomerInfo = new("EF")
                {
                    HomeAddress = new("L5", null, "Ci5", "Co5", "P5"), WorkAddress = new("L6", null, "Ci6", "Co6", "P6")
                },
            },
            new SpecialCustomerTpt
            {
                Name = "Olive",
                CustomerInfo = new("EF")
                {
                    HomeAddress = new("L7", null, "Ci7", "Co7", "P7"), WorkAddress = new("L8", null, "Ci8", "Co8", "P8")
                }
            });

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("GroupBy complex type:");
        Console.WriteLine();

        #region GroupByComplexType
        var groupedAddresses = await context.Stores
            .GroupBy(b => b.StoreAddress)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();
        #endregion

        foreach (var groupedAddress in groupedAddresses)
        {
            Console.WriteLine($"{groupedAddress.Key.PostCode}: {groupedAddress.Count}");
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    [Table("TptSpecialCustomers")]
    public class SpecialCustomerTpt : CustomerTpt
    {
        public string? Note { get; set; }
    }

    [Table("TptCustomers")]
    public class CustomerTpt
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CustomerInfo CustomerInfo { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CustomerInfo CustomerInfo { get; set; }
    }

    public record struct CustomerInfo(string? Tag)
    {
        public required Address HomeAddress { get; init; }
        public required Address WorkAddress { get; init; }
    }

    [ComplexType]
    public record class Address(string Line1, string? Line2, string City, string Country, string PostCode);

    public class CustomerWithStores
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Region { get; set; }
        public string? Tag { get; set; }
    }

    public class Store
    {
        public int Id { get; set; }
        public List<CustomerWithStores> Customers { get; } = new();
        public string? Region { get; set; }
        public required Address StoreAddress { get; set; }
    }

    public class CustomerContext(bool useSqlite = false) : DbContext
    {
        public bool UseSqlite { get; } = useSqlite;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db",
                        sqliteOptionsBuilder => sqliteOptionsBuilder.UseNetTopologySuite())
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                        sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseNetTopologySuite()))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.ComplexProperties<CustomerInfo>();
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerWithStores> CustomersWithStores => Set<CustomerWithStores>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<CustomerTpt> TptCustomers => Set<CustomerTpt>();
        public DbSet<SpecialCustomerTpt> TptSpecialCustomers => Set<SpecialCustomerTpt>();
    }
}
