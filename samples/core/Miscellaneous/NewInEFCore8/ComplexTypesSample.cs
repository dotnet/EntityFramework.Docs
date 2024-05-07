namespace NewInEfCore8;

public static class ComplexTypesSample
{
    public static Task Use_mutable_class_as_complex_type()
    {
        PrintSampleName();
        return ComplexTypeTest<CustomerContext>();
    }

    public static Task Use_mutable_class_as_complex_type_SQLite()
    {
        PrintSampleName();
        return ComplexTypeTest<CustomerContextSqlite>();
    }

    private static async Task ComplexTypeTest<TContext>()
        where TContext : CustomerContextBase, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        #region SaveCustomer
        var customer = new Customer
        {
            Name = "Willow",
            Address = new() { Line1 = "Barking Gate", City = "Walpole St Peter", Country = "UK", PostCode = "PE14 7AV" }
        };

        context.Add(customer);
        await context.SaveChangesAsync();
        #endregion

        #region CreateOrder
        customer.Orders.Add(
            new Order { Contents = "Tesco Tasty Treats", BillingAddress = customer.Address, ShippingAddress = customer.Address, });

        await context.SaveChangesAsync();
        #endregion

        #region ChangeSharedAddress
        customer.Address.Line1 = "Peacock Lodge";
        await context.SaveChangesAsync();
        #endregion

        context.ChangeTracker.Clear();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region CustomerOrders
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Address Address { get; set; }
        public List<Order> Orders { get; } = new();
    }

    public class Order
    {
        public int Id { get; set; }
        public required string Contents { get; set; }
        public required Address ShippingAddress { get; set; }
        public required Address BillingAddress { get; set; }
        public Customer Customer { get; set; } = null!;
    }
    #endregion

    #region Address
    public class Address
    {
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string PostCode { get; set; }
    }
    #endregion

    public class CustomerContext : CustomerContextBase
    {
    }

    public class CustomerContextSqlite : CustomerContextBase
    {
        public CustomerContextSqlite()
            : base(useSqlite: true)
        {
        }
    }

    public abstract class CustomerContextBase(bool useSqlite = false) : DbContext
    {
        public bool UseSqlite { get; } = useSqlite;
        public bool LoggingEnabled { get; set; }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    s =>
                    {
                        if (LoggingEnabled)
                        {
                            Console.WriteLine(s);
                        }
                    }, LogLevel.Information);

        #region ComplexTypeConfig
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .ComplexProperty(e => e.Address);

            modelBuilder.Entity<Order>(b =>
            {
                b.ComplexProperty(e => e.BillingAddress);
                b.ComplexProperty(e => e.ShippingAddress);
            });
        }
        #endregion

        public async Task Seed()
        {
            await SaveChangesAsync();
        }
    }
}
