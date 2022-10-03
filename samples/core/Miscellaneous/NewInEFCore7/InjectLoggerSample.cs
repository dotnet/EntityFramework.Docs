using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NewInEfCore7;

public static class InjectLoggerSample
{
    public static async Task Injecting_services_into_entities()
    {
        PrintSampleName();

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerContext>(
                b => b.UseLoggerFactory(loggerFactory)
                    .UseSqlite("Data Source = customers.db"))
            .BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CustomerContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Customer { Name = "Alice", PhoneNumber = "+1 515 555 0123" },
                new Customer { Name = "Mac", PhoneNumber = "+1 515 555 0124" });

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CustomerContext>();

            var customer = await context.Customers.SingleAsync(e => e.Name == "Alice");
            customer.PhoneNumber = "+1 515 555 0125";
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers
            => Set<Customer>();

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new LoggerInjectionInterceptor());
        #endregion
    }

    #region LoggerInjectionInterceptor
    public class LoggerInjectionInterceptor : IMaterializationInterceptor
    {
        private ILogger? _logger;

        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (instance is IHasLogger hasLogger)
            {
                _logger ??= materializationData.Context.GetService<ILoggerFactory>().CreateLogger("CustomersLogger");
                hasLogger.Logger = _logger;
            }

            return instance;
        }
    }
    #endregion

    #region IHasLogger
    public interface IHasLogger
    {
        ILogger? Logger { get; set; }
    }
    #endregion

    #region CustomerIHasLogger
    public class Customer : IHasLogger
    {
        private string? _phoneNumber;

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                Logger?.LogInformation(1, $"Updating phone number for '{Name}' from '{_phoneNumber}' to '{value}'.");

                _phoneNumber = value;
            }
        }

        [NotMapped]
        public ILogger? Logger { get; set; }
    }
    #endregion
}
