namespace NewInEfCore8;

public static class NestedComplexTypesSample
{
    public static Task Use_mutable_classes_as_complex_types()
    {
        PrintSampleName();
        return ComplexTypeTest<CustomerContext>();
    }

    public static Task Use_mutable_classes_as_complex_types_SQLite()
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

        var customerId = 1;

        #region QueryCustomer
        var customer = await context.Customers.FirstAsync(e => e.Id == customerId);
        #endregion

        var address = customer.Contact.Address;
        var phone = customer.Contact.MobilePhone;

        var order = new Order { Contents = "Tesco Tasty Treats", BillingAddress = address, ShippingAddress = address, ContactPhone = phone };
        customer.Orders.Add(order);

        await context.SaveChangesAsync();

        order.ContactPhone = customer.Contact.WorkPhone;

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var customers = await context.Customers.Where(e => e.Id == customerId).Include(e => e.Orders).ToListAsync();

        customer = customers.First();
        order = customer.Orders.First();

        #region BillingAddressCurrentValue
        var billingAddress = context.Entry(order)
            .ComplexProperty(e => e.BillingAddress)
            .CurrentValue;
        #endregion

        #region PostCodeCurrentValue
        var postCode = context.Entry(order)
            .ComplexProperty(e => e.BillingAddress)
            .Property(e => e.PostCode)
            .CurrentValue;
        #endregion

        #region CityCurrentValue
        var currentCity = context.Entry(customer)
            .ComplexProperty(e => e.Contact)
            .ComplexProperty(e => e.Address)
            .Property(e => e.City)
            .CurrentValue;
        #endregion

        #region SetCityCurrentValue
        context.Entry(customer)
            .ComplexProperty(e => e.Contact)
            .ComplexProperty(e => e.Address)
            .Property(e => e.City)
            .CurrentValue = "Ames";
        #endregion

        #region CityOriginalValue
        var originalCity = context.Entry(customer)
            .ComplexProperty(e => e.Contact)
            .ComplexProperty(e => e.Address)
            .Property(e => e.City)
            .OriginalValue;
        #endregion

        #region SetPostCodeIsModified
        context.Entry(customer)
            .ComplexProperty(e => e.Contact)
            .ComplexProperty(e => e.Address)
            .Property(e => e.PostCode)
            .IsModified = true;
        #endregion

        var orderId = 1;

        #region QueryShippingAddress
        var shippingAddress = await context.Orders
            .Where(e => e.Id == orderId)
            .Select(e => e.ShippingAddress)
            .SingleAsync();
        #endregion

        var shippingCity = await context.Orders.Where(e => e.Id == orderId).Select(e => e.ShippingAddress.City).SingleAsync();

        var addresses = await context.Orders.Select(e => new { e.ShippingAddress, e.BillingAddress }).ToListAsync();

        #region QueryOrdersInCity
        var city = "Walpole St Peter";
        var walpoleOrders = await context.Orders.Where(e => e.ShippingAddress.City == city).ToListAsync();
        #endregion

        var countryCode = 44;
        var customersWithUkOrders = await context.Customers
            .Where(e => e.Orders.Select(e => e.ContactPhone.CountryCode == countryCode).Any())
            .Select(e => new { e.Name, Phone = e.Orders.FirstOrDefault(e => e.ContactPhone.CountryCode == countryCode) }).ToListAsync();

        #region QueryWithPhoneNumber
        var phoneNumber = new PhoneNumber(44, 7777555777);
        var customersWithNumber = await context.Customers
            .Where(
                e => e.Contact.MobilePhone == phoneNumber
                     || e.Contact.WorkPhone == phoneNumber
                     || e.Contact.HomePhone == phoneNumber)
            .ToListAsync();
        #endregion

        await context.Customers
            .Where(
                e => e.Contact.MobilePhone == phoneNumber
                     || e.Contact.WorkPhone == phoneNumber
                     || e.Contact.HomePhone == phoneNumber)
            .ExecuteDeleteAsync();

        var allNumbers = await context.Customers
            .Select(e => e.Contact.MobilePhone)
            .Distinct()
            .ToListAsync();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region CustomerWithContact
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Contact Contact { get; set; }
        public List<Order> Orders { get; } = new();
    }
    #endregion

    #region OrderWithPhone
    public class Order
    {
        public int Id { get; set; }
        public required string Contents { get; set; }
        public required PhoneNumber ContactPhone { get; set; }
        public required Address ShippingAddress { get; set; }
        public required Address BillingAddress { get; set; }
        public Customer Customer { get; set; } = null!;
    }
    #endregion

    #region NestedComplexTypes
    public record Address(string Line1, string? Line2, string City, string Country, string PostCode);

    public record PhoneNumber(int CountryCode, long Number);

    public record Contact
    {
        public required Address Address { get; init; }
        public required PhoneNumber HomePhone { get; init; }
        public required PhoneNumber WorkPhone { get; init; }
        public required PhoneNumber MobilePhone { get; init; }
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

        #region ConfigureNestedTypes
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(
                b =>
                {
                    b.ComplexProperty(
                        e => e.Contact,
                        b =>
                        {
                            b.ComplexProperty(e => e.Address);
                            b.ComplexProperty(e => e.HomePhone);
                            b.ComplexProperty(e => e.WorkPhone);
                            b.ComplexProperty(e => e.MobilePhone);
                        });
                });

            modelBuilder.Entity<Order>(
                b =>
                {
                    b.ComplexProperty(e => e.ContactPhone);
                    b.ComplexProperty(e => e.BillingAddress);
                    b.ComplexProperty(e => e.ShippingAddress);
                });
        }
        #endregion

        public async Task Seed()
        {
            var customer = new Customer
            {
                Name = "Willow",
                Contact = new()
                {
                    Address = new("Barking Gate", null, "Walpole St Peter", "UK", "PE14 7AV"),
                    HomePhone = new(44, 7777555777),
                    WorkPhone = new(1, 12345678901),
                    MobilePhone = new(49, 987654321)
                }
            };

            Add(customer);
            await SaveChangesAsync();
        }
    }
}
