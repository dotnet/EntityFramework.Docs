namespace NewInEfCore7;

public static class GroupByFinalOperatorSample
{
    public static Task GroupBy_final_operator_SqlServer()
    {
        PrintSampleName();
        return QueryTest<BookContextSqlServer>();
    }

    public static Task GroupBy_final_operator_Sqlite()
    {
        PrintSampleName();
        return QueryTest<BookContextSqlite>();
    }

    public static Task GroupBy_final_operator_InMemory()
    {
        PrintSampleName();
        return QueryTest<BookContextInMemory>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : BookContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var toast = new Author { Name = "Toast" };
            var alice = new Author { Name = "Alice" };

            await context.AddRangeAsync(
                new Book { Author = alice, Price = 10 },
                new Book { Author = alice, Price = 10 },
                new Book { Author = toast, Price = 12 },
                new Book { Author = toast, Price = 12 },
                new Book { Author = toast, Price = 14 });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region GroupByFinalOperator
            var query = context.Books.GroupBy(s => s.Price);
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Price: {group.Key}; Count = {group.Count()}");
            }
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class BookContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class BookContextSqlServer : BookContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Books"));
    }

    public class BookContextSqlite : BookContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite("Data Source = books.db"));
    }

    public class BookContextInMemory : BookContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseInMemoryDatabase(nameof(BookContextInMemory)));
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class Book
    {
        public int Id { get; set; }
        public Author Author { get; set; } = default!;
        public int? Price { get; set; }
    }
}
