namespace NewInEfCore9;

public static class ExecuteUpdateSample
{
    public static Task ExecuteUpdate_for_complex_type_instances()
        => ExecuteUpdateDeleteTest(sqlite: false);

    public static Task ExecuteUpdate_for_complex_type_instances_on_SQLite()
        => ExecuteUpdateDeleteTest(sqlite: true);

    public static async Task ExecuteUpdateDeleteTest(bool sqlite)
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
        Console.WriteLine("Non-complex type update:");
        Console.WriteLine();

        #region NormalExecuteUpdate
        await context.Stores
            .Where(e => e.Region == "Germany")
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Region, "Deutschland"));
        #endregion

        Console.WriteLine();
        Console.WriteLine("Update complex type members:");
        Console.WriteLine();

        {
            #region UpdateComplexTypeByMember
            var newAddress = new Address("Gressenhall Farm Shop", null, "Beetley", "Norfolk", "NR20 4DR");

            await context.Stores
                .Where(e => e.Region == "Deutschland")
                .ExecuteUpdateAsync(
                    s => s.SetProperty(b => b.StoreAddress.Line1, newAddress.Line1)
                        .SetProperty(b => b.StoreAddress.Line2, newAddress.Line2)
                        .SetProperty(b => b.StoreAddress.City, newAddress.City)
                        .SetProperty(b => b.StoreAddress.Country, newAddress.Country)
                        .SetProperty(b => b.StoreAddress.PostCode, newAddress.PostCode));
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Update a top-level complex type instance:");
        Console.WriteLine();

        {
            #region UpdateComplexType
            var newAddress = new Address("Gressenhall Farm Shop", null, "Beetley", "Norfolk", "NR20 4DR");

            await context.Stores
                .Where(e => e.Region == "Germany")
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.StoreAddress, newAddress));
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Update a nested complex type instance:");
        Console.WriteLine();

        var name = "Smokey";

        #region UpdateNestedComplexType
        await context.Customers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.CustomerInfo.WorkAddress, new Address("Gressenhall Workhouse", null, "Beetley", "Norfolk", "NR20 4DR")));
        #endregion

        Console.WriteLine();
        Console.WriteLine("Update multiple complex type instances and a normal property:");
        Console.WriteLine();

        #region UpdateMultipleComplexType
        await context.Customers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                        b => b.CustomerInfo.WorkAddress, new Address("Gressenhall Workhouse", null, "Beetley", "Norfolk", "NR20 4DR"))
                    .SetProperty(b => b.CustomerInfo.HomeAddress, new Address("Gressenhall Farm", null, "Beetley", "Norfolk", "NR20 4DR"))
                    .SetProperty(b => b.CustomerInfo.Tag, "Tog"));
        #endregion

        // Console.WriteLine();
        // Console.WriteLine("Update a complex type instance using existing values from the table:");
        // Console.WriteLine();

        #region UpdateWithExistingValues
        // See https://github.com/dotnet/efcore/issues/32987
        // await context.Stores
        //     .Where(e => e.Region == "Germany")
        //     .ExecuteUpdateAsync(
        //         s => s.SetProperty(b => b.StoreAddress, b => new Address(b.StoreAddress.Line1, null, "Cib", "Cob", "Pb")));
        #endregion

        // Console.WriteLine();
        // Console.WriteLine("Update multiple nested complex type instances using existing values from the table:");
        // Console.WriteLine();

        #region UpdateMultiplWithExisting
        // See https://github.com/dotnet/efcore/issues/32987
        // await context.Customers
        //     .Where(e => e.Name == name)
        //     .ExecuteUpdateAsync(
        //         s => s.SetProperty(
        //                 b => b.CustomerInfo.HomeAddress,
        //                 b => new Address(
        //                     b.CustomerInfo.WorkAddress.Line1, b.CustomerInfo.WorkAddress.Line2, b.CustomerInfo.WorkAddress.City,
        //                     b.CustomerInfo.WorkAddress.Country, b.CustomerInfo.WorkAddress.PostCode))
        //             .SetProperty(
        //                 b => b.CustomerInfo.WorkAddress,
        //                 b => new Address(
        //                     b.CustomerInfo.HomeAddress.Line1, b.CustomerInfo.HomeAddress.Line2, b.CustomerInfo.HomeAddress.City,
        //                     b.CustomerInfo.HomeAddress.Country, b.CustomerInfo.HomeAddress.PostCode)));
        #endregion
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
                        sqliteOptionsBuilder =>
                        {
                            sqliteOptionsBuilder.UseNetTopologySuite();
                            sqliteOptionsBuilder.CommandTimeout(0);
                        })
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                        sqlServerOptionsBuilder =>
                        {
                            sqlServerOptionsBuilder.UseNetTopologySuite();
                            sqlServerOptionsBuilder.CommandTimeout(0);
                        }))
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
