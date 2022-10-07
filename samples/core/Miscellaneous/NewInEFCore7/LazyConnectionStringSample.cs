using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore7;

public static class LazyConnectionStringSample
{
    public static async Task Lazy_initialization_of_a_connection_string()
    {
        PrintSampleName();

        var services = new ServiceCollection();

        services.AddScoped<IClientConnectionStringFactory, TestClientConnectionStringFactory>();

        services.AddDbContext<CustomerContext>(
            b => b.UseSqlServer()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging());

        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CustomerContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Customer { Name = "Alice" },
                new Customer { Name = "Mac" });

            await context.SaveChangesAsync();

            var customer = await context.Customers.SingleAsync(e => e.Name == "Alice");
            Console.WriteLine();
            Console.WriteLine($"Loaded {customer.Name}");
            Console.WriteLine();
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class CustomerContext : DbContext
    {
        private readonly IClientConnectionStringFactory _connectionStringFactory;

        public CustomerContext(
            DbContextOptions<CustomerContext> options,
            IClientConnectionStringFactory connectionStringFactory)
            : base(options)
        {
            _connectionStringFactory = connectionStringFactory;
        }

        public DbSet<Customer> Customers
            => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(
                new ConnectionStringInitializationInterceptor(_connectionStringFactory));
    }

    public interface IClientConnectionStringFactory
    {
        Task<string> GetConnectionStringAsync(CancellationToken cancellationToken);
    }

    public class TestClientConnectionStringFactory : IClientConnectionStringFactory
    {
        public Task<string> GetConnectionStringAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine();
            Console.WriteLine(">>> Getting connection string...");
            Console.WriteLine();
            return Task.FromResult(@"Server=(localdb)\mssqllocaldb;Database=LazyConnectionStringSample");
        }
    }

    #region ConnectionStringInitializationInterceptor
    public class ConnectionStringInitializationInterceptor : DbConnectionInterceptor
    {
        private readonly IClientConnectionStringFactory _connectionStringFactory;

        public ConnectionStringInitializationInterceptor(IClientConnectionStringFactory connectionStringFactory)
        {
            _connectionStringFactory = connectionStringFactory;
        }

        public override InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
            => throw new NotSupportedException("Synchronous connections not supported.");

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = new())
        {
            if (string.IsNullOrEmpty(connection.ConnectionString))
            {
                connection.ConnectionString = (await _connectionStringFactory.GetConnectionStringAsync(cancellationToken));
            }

            return result;
        }
    }
    #endregion

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
