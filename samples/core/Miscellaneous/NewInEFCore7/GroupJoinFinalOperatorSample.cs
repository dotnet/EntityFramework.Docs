namespace NewInEfCore7;

public static class GroupJoinFinalOperatorSample
{
    public static Task GroupJoin_final_operator_SqlServer()
    {
        PrintSampleName();
        return QueryTest<GroupJoinContextSqlServer>();
    }

    public static Task GroupJoin_final_operator_Sqlite()
    {
        PrintSampleName();
        return QueryTest<GroupJoinContextSqlite>();
    }

    public static Task GroupJoin_final_operator_InMemory()
    {
        PrintSampleName();
        return QueryTest<GroupJoinContextInMemory>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : GroupJoinContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var toast = new Customer { Name = "Toast" };
            var alice = new Customer { Name = "Alice" };

            await context.AddRangeAsync(
                new Order { Customer = alice, Amount = 10 },
                new Order { Customer = alice, Amount = 10 },
                new Order { Customer = toast, Amount = 12 },
                new Order { Customer = toast, Amount = 12 },
                new Order { Customer = toast, Amount = 14 });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region GroupJoinFinalOperator
            var query = context.Customers.GroupJoin(
                context.Orders, c => c.Id, o => o.CustomerId, (c, os) => new { Customer = c, Orders = os });
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {group.Customer.Name}; Count = {group.Orders.Count()}");
            }
        }

        await using (var context = new TContext())
        {
            var query = context.Customers
                .GroupJoin(
                    context.Orders,
                    o => o.Id,
                    bt => bt.CustomerId,
                    (o, bt) => new { Customer = o, BotTasks = bt, });

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {group.Customer.Name}; Count = {group.BotTasks.Count()}");
            }
        }

        await using (var context = new TContext())
        {
            var query =
                from customer in context.Customers
                join order in context.Orders on customer.Id equals order.CustomerId into orderDetails
                select new CustomerWithNavigationProperties { Customer = customer, Orders = orderDetails.ToList() };

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Customer: {group.Customer.Name}; Count = {group.Orders.Count()}");
            }
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class GroupJoinContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class GroupJoinContextSqlServer : GroupJoinContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Customers;ConnectRetryCount=0"));
    }

    public class GroupJoinContextSqlite : GroupJoinContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite("Data Source = customers.db"));
    }

    public class GroupJoinContextInMemory : GroupJoinContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseInMemoryDatabase(nameof(GroupJoinContextInMemory)));
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Order> Orders { get; } = new();
    }

    public class Order
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }
    }

    public class CustomerWithNavigationProperties
    {
        public Customer Customer { get; set; } = null!;
        public List<Order> Orders { get; set; } = null!;
    }
}
