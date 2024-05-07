using System.Collections.ObjectModel;

namespace NewInEfCore7;

public static class ReadOnlySetQuerySample
{
    public static Task Use_Contains_with_IReadOnlySet_SqlServer()
    {
        PrintSampleName();
        return QueryTest<ReadOnlySetContextSqlServer>();
    }

    public static Task Use_Contains_with_IReadOnlySet_Sqlite()
    {
        PrintSampleName();
        return QueryTest<ReadOnlySetContextSqlite>();
    }

    public static Task Use_Contains_with_IReadOnlySet_InMemory()
    {
        PrintSampleName();
        return QueryTest<ReadOnlySetContextInMemory>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : ReadOnlySetContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var toast = new Customer1 { Name = "Toast" };
            var alice = new Customer2 { Name = "Alice" };
            var mac = new Customer3 { Name = "Mac" };

            await context.AddRangeAsync(
                new Order { Customer1 = toast, Customer2 = alice, Customer3 = mac, Amount = 10 },
                new Order { Customer1 = toast, Customer2 = alice, Customer3 = mac, Amount = 10 },
                new Order { Customer1 = toast, Customer2 = alice, Customer3 = mac, Amount = 12 },
                new Order { Customer1 = toast, Customer2 = alice, Customer3 = mac, Amount = 12 },
                new Order { Customer1 = toast, Customer2 = alice, Customer3 = mac, Amount = 14 });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region ReadOnlySetQuery
            IReadOnlySet<int> searchIds = new HashSet<int> { 1, 3, 5 };
            var query = context.Customers.Where(p => p.Orders.Any(l => searchIds.Contains(l.Id)));
            #endregion

            await foreach (var customer in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {customer.Name}");
            }
        }

        await using (var context = new TContext())
        {
            IReadOnlyCollection<int> searchIds = new ReadOnlyCollection<int>(new Collection<int> { 1, 3, 5 });
            var query = context.Customers.Where(p => p.Orders.Any(l => searchIds.Contains(l.Id)));

            await foreach (var customer in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {customer.Name}");
            }
        }

        await using (var context = new TContext())
        {
            var searchIds = new ReadOnlyCollection<int>(new Collection<int> { 1, 3, 5 });
            var query = context.Customer2s.Where(p => p.Orders.Any(l => searchIds.Contains(l.Id)));

            await foreach (var customer in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {customer.Name}");
            }
        }

        await using (var context = new TContext())
        {
            IReadOnlyList<int> searchIds = new List<int> { 1, 3, 5 };
            var query = context.Customer3s.Where(p => p.Orders.Any(l => searchIds.Contains(l.Id)));

            await foreach (var customer in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {customer.Name}");
            }
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class ReadOnlySetContext : DbContext
    {
        public DbSet<Customer1> Customers => Set<Customer1>();
        public DbSet<Customer2> Customer2s => Set<Customer2>();
        public DbSet<Customer3> Customer3s => Set<Customer3>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class ReadOnlySetContextSqlServer : ReadOnlySetContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Customers;ConnectRetryCount=0"));
    }

    public class ReadOnlySetContextSqlite : ReadOnlySetContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite("Data Source = customers.db"));
    }

    public class ReadOnlySetContextInMemory : ReadOnlySetContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseInMemoryDatabase(nameof(ReadOnlySetContextInMemory)));
    }

    public class Customer1
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public IReadOnlySet<Order> Orders { get; } = new HashSet<Order>();
    }

    public class Customer2
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public IReadOnlyCollection<Order> Orders { get; } = new HashSet<Order>();
    }

    public class Customer3
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public IReadOnlyList<Order> Orders { get; } = new List<Order>();
    }

    public class Order
    {
        public int Id { get; set; }

        public int? Customer1Id { get; set; }
        public Customer1? Customer1 { get; set; }

        public int Customer2Id { get; set; }
        public Customer2 Customer2 { get; set; } = null!;

        public int Customer3Id { get; set; }
        public Customer3 Customer3 { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Amount { get; set; }
    }
}
