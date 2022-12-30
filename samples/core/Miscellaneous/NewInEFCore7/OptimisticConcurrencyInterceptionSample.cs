using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NewInEfCore7;

public static class OptimisticConcurrencyInterceptionSample
{
    public static async Task Optimistic_concurrency_interception()
    {
        PrintSampleName();

        await using (var context = new CustomerContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Customer { Name = "Bill" },
                new Customer { Name = "Bob" });

            await context.SaveChangesAsync();
        }

        await using (var context1 = new CustomerContext())
        {
            var customer1 = await context1.Customers.SingleAsync(e => e.Name == "Bill");

            await using (var context2 = new CustomerContext())
            {
                var customer2 = await context1.Customers.SingleAsync(e => e.Name == "Bill");
                context2.Entry(customer2).State = EntityState.Deleted;
                await context2.SaveChangesAsync();
            }

            context1.Entry(customer1).State = EntityState.Deleted;
            await context1.SaveChangesAsync();
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class CustomerContext : DbContext
    {
        private static readonly SuppressDeleteConcurrencyInterceptor _concurrencyInterceptor = new();

        public DbSet<Customer> Customers
            => Set<Customer>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .AddInterceptors(_concurrencyInterceptor)
                .UseSqlite("Data Source = customers.db")
                .LogTo(Console.WriteLine, LogLevel.Information);
    }

    #region SuppressDeleteConcurrencyInterceptor
    public class SuppressDeleteConcurrencyInterceptor : ISaveChangesInterceptor
    {
        public InterceptionResult ThrowingConcurrencyException(
            ConcurrencyExceptionEventData eventData,
            InterceptionResult result)
        {
            if (eventData.Entries.All(e => e.State == EntityState.Deleted))
            {
                Console.WriteLine("Suppressing Concurrency violation for command:");
                Console.WriteLine(((RelationalConcurrencyExceptionEventData)eventData).Command.CommandText);

                return InterceptionResult.Suppress();
            }

            return result;
        }

        public ValueTask<InterceptionResult> ThrowingConcurrencyExceptionAsync(
            ConcurrencyExceptionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
            => new(ThrowingConcurrencyException(eventData, result));
    }
    #endregion

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
