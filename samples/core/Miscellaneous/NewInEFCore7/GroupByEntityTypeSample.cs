namespace NewInEfCore7;

public static class GroupByEntityTypeSample
{
    public static Task GroupBy_entity_type_SqlServer()
    {
        PrintSampleName();
        return QueryTest<BookContextSqlServer>();
    }

    public static Task GroupBy_entity_type_Sqlite()
    {
        PrintSampleName();
        return QueryTest<BookContextSqlite>();
    }

    public static Task GroupBy_entity_type_InMemory()
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
                new Book { Author = alice, Price = 11 },
                new Book { Author = toast, Price = 12 },
                new Book { Author = toast, Price = 13 },
                new Book { Author = toast, Price = 14 });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region GroupByEntityType
            var query = context.Books
                .GroupBy(s => s.Author)
                .Select(s => new { Author = s.Key, MaxPrice = s.Max(p => p.Price) });
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Author: {group.Author.Name}; MaxPrice = {group.MaxPrice}");
            }
        }

        await using (var context = new TContext())
        {
            #region GroupByEntityTypeReversed
            var query = context.Authors
                .Select(a => new { Author = a, MaxPrice = a.Books.Max(b => b.Price) });
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine($"Author: {group.Author.Name}; MaxPrice = {group.MaxPrice}");
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
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Books;ConnectRetryCount=0"));
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
        public ICollection<Book> Books { get; } = new List<Book>();
    }

    public class Book
    {
        public int Id { get; set; }
        public Author Author { get; set; } = default!;
        public int Price { get; set; }
    }
}
