namespace NewInEfCore8;

public static class ExecuteUpdateDeleteSample
{
    public static Task ExecuteUpdate_ExecuteDelete_with_multiple_entities_targeting_one_table()
        => ExecuteUpdateDeleteTest(sqlite: false);

    public static Task ExecuteUpdate_ExecuteDelete_with_multiple_entities_targeting_one_table_SQLite()
        => ExecuteUpdateDeleteTest(sqlite: true);

    public static async Task ExecuteUpdateDeleteTest(bool sqlite)
    {
        PrintSampleName();

        await using var context = new CustomerContext(useSqlite: sqlite);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(
            new Store { Customers = { new CustomerWithStores { Name = "Smokey", Region = "France" } }, Region = "France" },
            new Customer { Name = "Smokey", CustomerInfo = new() { Tag = "EF" } },
            new CustomerTpt { Name = "Willow" },
            new SpecialCustomerTpt { Name = "Olive" });

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("Update properties of owner and owned type mapped to the same table:");

        var name = "Smokey";

        #region UpdateWithOwned
        await context.Customers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.CustomerInfo.Tag, "Tagged")
                    .SetProperty(b => b.Name, b => b.Name + "_Tagged"));
        #endregion

        Console.WriteLine();
        Console.WriteLine("Update properties of entities returned by union but targeting a single table:");

        #region UpdateWithUnion
        await context.CustomersWithStores
            .Where(e => e.Region == "France")
            .Union(context.Stores.Where(e => e.Region == "France").SelectMany(e => e.Customers))
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Tag, "The French Connection"));
        #endregion


        Console.WriteLine();
        Console.WriteLine("Update properties of a single table in a TPT hierarchy:");

        #region TptUpdateName
        await context.TptSpecialCustomers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Name, b => b.Name + " (Noted)"));
        #endregion

        #region TptUpdateNote
        await context.TptSpecialCustomers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Note, "Noted"));
        #endregion

        try
        {
            Console.WriteLine();
            Console.WriteLine("Attempt to update properties of two tables in a TPT hierarchy:");

            #region TptUpdateBoth
            await context.TptSpecialCustomers
                .Where(e => e.Name == name)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.Note, "Noted")
                    .SetProperty(b => b.Name, b => b.Name + " (Noted)"));
            #endregion
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Delete owner with  owned type mapped to the same table:");

        await context.Customers.Where(e => e.Name == name).ExecuteDeleteAsync();

        Console.WriteLine();
        Console.WriteLine("Delete entities returned by union but targeting a single table:");

        await context.CustomersWithStores
            .Where(e => e.Region == "France")
            .Union(context.Stores.Where(e => e.Region == "France").SelectMany(e => e.Customers))
            .ExecuteDeleteAsync();

        Console.WriteLine();
    }


    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region CustomerTpt
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
    }
    #endregion

    #region CustomerAndInfo
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CustomerInfo CustomerInfo { get; set; }
    }

    [Owned]
    public class CustomerInfo
    {
        public string? Tag { get; set; }
    }
    #endregion

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
    }

    public class CustomerContext(bool useSqlite = false) : DbContext
    {
        public bool UseSqlite { get; } = useSqlite;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                        sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseNetTopologySuite()))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerWithStores> CustomersWithStores => Set<CustomerWithStores>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<CustomerTpt> TptCustomers => Set<CustomerTpt>();
        public DbSet<SpecialCustomerTpt> TptSpecialCustomers => Set<SpecialCustomerTpt>();
    }
}
