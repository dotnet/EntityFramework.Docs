namespace NewInEfCore8;

public static class PrimitiveCollectionToTableSample
{
    public static Task Queries_against_a_table_wrapping_a_primitive_type()
    {
        PrintSampleName();
        return ContainsTest<PubsAndWalksContext>();
    }

    public static Task Queries_against_a_table_wrapping_a_primitive_type_SQLite()
    {
        PrintSampleName();
        return ContainsTest<PubsAndWalksContextSqlite>();
    }

    private static async Task ContainsTest<TContext>()
        where TContext : PubsAndWalksContextBase, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        #region PubsWithHeineken
        var beer = "Heineken";
        var pubsWithHeineken = await context.Pubs
            .Where(e => e.Beers.Any(e => e.Name == beer))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with {beer} are {string.Join(", ", pubsWithHeineken)}");

        #region PubsWithLager
        var beers = new[] { "Carling", "Heineken", "Stella Artois", "Carlsberg" };
        var pubsWithLager = await context.Pubs
            .Where(e => beers.Any(b => e.Beers.Any(t => t.Name == b)))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with lager are {string.Join(", ", pubsWithLager)}");
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    #region Pub
    public class Pub
    {
        public Pub(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Beer> Beers { get; set; } = new();
        public List<DateOnly> DaysVisited { get; private set; } = new();
    }
    #endregion

    #region Beer
    [Owned]
    public class Beer
    {
        public Beer(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
    #endregion

    public class PubsAndWalksContext : PubsAndWalksContextBase
    {
    }

    public class PubsAndWalksContextSqlite : PubsAndWalksContextBase
    {
        public PubsAndWalksContextSqlite()
            : base(useSqlite: true)
        {
        }
    }

    public abstract class PubsAndWalksContextBase : DbContext
    {
        protected PubsAndWalksContextBase(bool useSqlite = false)
        {
            UseSqlite = useSqlite;
        }

        public bool UseSqlite { get; }
        public bool LoggingEnabled { get; set; }

        public DbSet<Pub> Pubs => Set<Pub>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    s =>
                    {
                        if (LoggingEnabled)
                        {
                            Console.WriteLine(s);
                        }
                    }, LogLevel.Information);

        public async Task Seed()
        {
            await AddRangeAsync(
                new Pub("The Royal Oak")
                {
                    Beers =
                    {
                        new("Oakham"),
                        new("Carling"),
                        new("Guinness"),
                        new("John Smiths"),
                        new("Bathams"),
                        new("Tennents")
                    },
                    DaysVisited =
                    {
                        new(2021, 1, 2),
                        new(2021, 8, 7),
                        new(2023, 3, 22),
                        new(2022, 4, 22),
                        new(2022, 7, 21),
                        new(2022, 6, 21),
                    }
                },
                new Pub("The Prince of Wales Feathers")
                {
                    Beers = { new("Heineken"), new("John Smiths"), new("Stella Artois"), new("Carlsberg") },
                    DaysVisited =
                    {
                        new(2022, 11, 17),
                        new(2021, 9, 25),
                        new(2022, 12, 21),
                        new(2022, 11, 7),
                        new(2022, 8, 9),
                        new(2022, 3, 10),
                        new(2021, 12, 1),
                        new(2022, 3, 8),
                        new(2022, 2, 28),
                        new(2021, 5, 10),
                        new(2021, 6, 25),
                    }
                },
                new Pub("The White Swan")
                {
                    Beers =
                    {
                        new("Oakham"),
                        new("Carling"),
                        new("Guinness"),
                        new("Heineken"),
                        new("Stella Artois"),
                        new("Carlsberg"),
                        new("Bathams")
                    },
                    DaysVisited =
                    {
                        new(2021, 2, 24),
                        new(2022, 8, 3),
                        new(2022, 9, 14),
                        new(2022, 5, 5),
                        new(2022, 7, 13),
                        new(2021, 7, 18),
                        new(2021, 7, 23),
                    }
                },
                new Pub("Farr Bay Inn")
                {
                    Beers =
                    {
                        new("Guinness"),
                        new("Heineken"),
                        new("Carlsberg"),
                        new("Bathams"),
                        new("Tennents")
                    },
                    DaysVisited =
                    {
                        new(2022, 11, 30),
                        new(2022, 2, 15),
                        new(2021, 12, 9),
                        new(2022, 8, 5),
                        new(2021, 11, 19),
                        new(2022, 12, 20),
                        new(2021, 6, 5),
                        new(2021, 2, 27),
                        new(2021, 6, 17),
                        new(2023, 3, 1),
                        new(2021, 2, 23),
                        new(2021, 9, 6),
                        new(2023, 2, 22),
                        new(2022, 1, 25),
                    }
                },
                new Pub("The Eltisley")
                {
                    Beers =
                    {
                        new("Oakham"),
                        new("Carling"),
                        new("Guinness"),
                        new("Heineken"),
                        new("John Smiths"),
                        new("Stella Artois"),
                        new("Carlsberg"),
                        new("Bathams"),
                        new("Tennents")
                    },
                    DaysVisited =
                    {
                        new(2022, 6, 19),
                        new(2021, 1, 15),
                        new(2022, 3, 24),
                        new(2023, 2, 26),
                        new(2021, 1, 11),
                        new(2022, 3, 19),
                    }
                });

            await SaveChangesAsync();
        }
    }
}
