using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore7;

public static class SimpleMaterializationSample
{
    public static async Task Simple_actions_on_entity_creation()
    {
        PrintSampleName();

        await using (var context = new CustomerContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Customer { Name = "Alice", PhoneNumber = "+1 515 555 0123" },
                new Customer { Name = "Mac", PhoneNumber = "+1 515 555 0124" });

            await context.SaveChangesAsync();
        }

        #region QueryCustomer
        await using (var context = new CustomerContext())
        {
            var customer = await context.Customers.SingleAsync(e => e.Name == "Alice");
            Console.WriteLine($"Customer '{customer.Name}' was retrieved at '{customer.Retrieved.ToLocalTime()}'");
        }
        #endregion
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region CustomerContext
    public class CustomerContext : DbContext
    {
        private static readonly SetRetrievedInterceptor _setRetrievedInterceptor = new();

        public DbSet<Customer> Customers
            => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .AddInterceptors(_setRetrievedInterceptor)
                .UseSqlite("Data Source = customers.db");
    }
    #endregion

    #region SetRetrievedInterceptor
    public class SetRetrievedInterceptor : IMaterializationInterceptor
    {
        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (instance is IHasRetrieved hasRetrieved)
            {
                hasRetrieved.Retrieved = DateTime.UtcNow;
            }

            return instance;
        }
    }
    #endregion

    #region IHasRetrieved
    public interface IHasRetrieved
    {
        DateTime Retrieved { get; set; }
    }
    #endregion

    #region Customer
    public class Customer : IHasRetrieved
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        [NotMapped]
        public DateTime Retrieved { get; set; }
    }
    #endregion
}
