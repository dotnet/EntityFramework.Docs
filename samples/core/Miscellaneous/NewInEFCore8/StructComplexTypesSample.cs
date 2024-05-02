namespace NewInEfCore8;

public static class StructComplexTypesSample
{
    public static Task Use_mutable_struct_as_complex_type()
    {
        PrintSampleName();
        return ComplexTypeTest<CustomerContext>();
    }

    public static Task Use_mutable_struct_as_complex_type_SQLite()
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

        var customer = new Customer()
        {
            Name = "Willow",
            Address = new() { Line1 = "Barking Gate", City = "Walpole St Peter", Country = "UK", PostCode = "PE14 7AV" }
        };

        context.Add(customer);
        await context.SaveChangesAsync();

        customer.Orders.Add(
            new Order { Contents = "Tesco Tasty Treats", BillingAddress = customer.Address, ShippingAddress = customer.Address, });

        await context.SaveChangesAsync();

        var currentAddress = customer.Address;
        currentAddress.Line1 = "Peacock Lodge";
        customer.Address = currentAddress;
        await context.SaveChangesAsync();

        customer.Address = customer.Address with { Line1 = "Peacock Lodge" };

        await context.SaveChangesAsync();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Address Address { get; set; }
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

    #region AddressStruct
    public struct Address
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
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0"))
                //sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseCompatibilityLevel(120)))
                .EnableSensitiveDataLogging()
                .LogTo(
                    s =>
                    {
                        if (LoggingEnabled)
                        {
                            Console.WriteLine(s);
                        }
                    }, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ComplexProperty(e => e.Address,
                b =>
                {
                    b.Property(e => e.Line1);
                    b.Property(e => e.Line2);
                    b.Property(e => e.City);
                    b.Property(e => e.Country);
                    b.Property(e => e.PostCode);
                });

            modelBuilder.Entity<Order>(b =>
            {
                b.ComplexProperty(e => e.BillingAddress,
                    b =>
                    {
                        b.Property(e => e.Line1);
                        b.Property(e => e.Line2);
                        b.Property(e => e.City);
                        b.Property(e => e.Country);
                        b.Property(e => e.PostCode);
                    });

                b.ComplexProperty(e => e.ShippingAddress,
                    b =>
                    {
                        b.Property(e => e.Line1);
                        b.Property(e => e.Line2);
                        b.Property(e => e.City);
                        b.Property(e => e.Country);
                        b.Property(e => e.PostCode);
                    });
            });
        }

        public async Task Seed()
        {
            await SaveChangesAsync();
        }
    }
}
