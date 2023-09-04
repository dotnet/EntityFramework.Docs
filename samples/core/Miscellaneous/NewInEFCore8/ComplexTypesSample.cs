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

        context.ChangeTracker.Clear();
        #endregion
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
        public string Name { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public List<Order> Orders { get; } = new();
    }

    public class Order
    {
        public int Id { get; set; }
        public string Contents { get; set; } = null!;
        public Address ShippingAddress { get; set; } = null!;
        public Address BillingAddress { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
    #endregion

    #region Address
    public class Address
    {
        public string Line1 { get; set; } = null!;
        public string? Line2 { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostCode { get; set; } = null!;
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
                    : optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name}"))
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
