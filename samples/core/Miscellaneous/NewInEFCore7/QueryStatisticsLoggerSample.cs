using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NewInEfCore7;

public static class QueryStatisticsLoggerSample
{
    public static async Task Executing_commands_after_consuming_a_result_set()
    {
        PrintSampleName();

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        var serviceProvider = new ServiceCollection()
            .AddDbContext<CustomerContext>(
                b => b.UseLoggerFactory(loggerFactory)
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ConsumedDataReaderSample;ConnectRetryCount=0"))
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

            _ = await context.Customers.SingleAsync(e => e.Name == "Alice");
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class CustomerContext : DbContext
    {
        private static readonly StatisticsCommandInterceptor _statisticsCommandInterceptor = new();
        private static readonly InfoMessageInterceptor _infoMessageInterceptor = new();

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers
            => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(_statisticsCommandInterceptor, _infoMessageInterceptor);
    }

    public class InfoMessageInterceptor : DbConnectionInterceptor
    {
        #region InfoMessageInterceptor
        public override DbConnection ConnectionCreated(ConnectionCreatedEventData eventData, DbConnection result)
        {
            var logger = eventData.Context!.GetService<ILoggerFactory>().CreateLogger("InfoMessageLogger");
            ((SqlConnection)eventData.Connection).InfoMessage += (_, args) =>
            {
                logger.LogInformation(1, args.Message);
            };
            return result;
        }
        #endregion
    }

    public class StatisticsCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            command.CommandText = "SET STATISTICS IO ON;" + Environment.NewLine + command.CommandText;

            return result;
        }

        #region ReaderExecutingAsync
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            command.CommandText = "SET STATISTICS IO ON;" + Environment.NewLine + command.CommandText;

            return new(result);
        }
        #endregion

        public override InterceptionResult DataReaderClosing(
            DbCommand command,
            DataReaderClosingEventData eventData,
            InterceptionResult result)
        {
            eventData.DataReader.NextResult();

            return result;
        }

        #region DataReaderClosingAsync
        public override async ValueTask<InterceptionResult> DataReaderClosingAsync(
            DbCommand command,
            DataReaderClosingEventData eventData,
            InterceptionResult result)
        {
            await eventData.DataReader.NextResultAsync();

            return result;
        }
        #endregion
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
