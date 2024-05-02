namespace NewInEfCore7;

public static class UngroupedColumnsQuerySample
{
    public static Task Subqueries_dont_reference_ungrouped_columns_from_outer_query_SqlServer()
    {
        PrintSampleName();
        return QueryTest<InvoiceContextSqlServer>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : InvoiceContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new Invoice { Amount = 10.20m, History = new DateTime(1973, 9, 3) },
                new Invoice { Amount = 11.20m, History = new DateTime(1973, 9, 13) },
                new Invoice { Amount = 12.20m, History = new DateTime(1973, 10, 3) },
                new Invoice { Amount = 13.20m, History = new DateTime(1973, 11, 3) });

            await context.AddRangeAsync(
                new Payment { Amount = 0.20m, History = new DateTime(1973, 9, 5) },
                new Payment { Amount = 1.20m, History = new DateTime(1973, 10, 13) },
                new Payment { Amount = 2.20m, History = new DateTime(1973, 10, 5) },
                new Payment { Amount = 3.20m, History = new DateTime(1973, 11, 7) });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region UngroupedColumns
            var query = from s in (from i in context.Invoices
                                   group i by i.History.Month
                                   into g
                                   select new { Month = g.Key, Total = g.Sum(p => p.Amount), })
                        select new
                        {
                            s.Month, s.Total, Payment = context.Payments.Where(p => p.History.Month == s.Month).Sum(p => p.Amount)
                        };
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Month: {group.Month}; Total = {group.Total}; Payment = {group.Payment}");
            }
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class InvoiceContext : DbContext
    {
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class InvoiceContextSqlServer : InvoiceContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Invoices;ConnectRetryCount=0"));
    }

    public class InvoiceContextSqlite : InvoiceContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite("Data Source = invoices.db"));
    }

    public class Invoice
    {
        public int Id { get; set; }
        public DateTime History { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }
        public DateTime History { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }
    }
}
