using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NewInEfCore8;

public static class PrimitiveCollectionsSample
{
    public static Task Queries_using_primitive_collections()
    {
        PrintSampleName();
        return ContainsTest<PubsAndWalksContext>();
    }

    public static Task Queries_using_primitive_collections_SQLite()
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

        #region WalksWithTerrain
        var terrains = new[] { Terrain.River, Terrain.Beach, Terrain.Park };
        var walksWithTerrain = await context.Walks
            .Where(e => terrains.Contains(e.Terrain))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nWalks with given terrain are {string.Join(", ", walksWithTerrain.Select(w => $"\"{w}\""))}");

        #region PubsWithHeineken
        var beer = "Heineken";
        var pubsWithHeineken = await context.Pubs
            .Where(e => e.Beers.Contains(beer))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with {beer} are {string.Join(", ", pubsWithHeineken)}");

        #region PubsWithLager
        var beers = new[] { "Carling", "Heineken", "Stella Artois", "Carlsberg" };
        var pubsWithLager = await context.Pubs
            .Where(e => beers.Any(b => e.Beers.Contains(b)))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with lager are {string.Join(", ", pubsWithLager)}");

        #region PubsVisitedThisYear
        var thisYear = DateTime.Now.Year;
        var pubsVisitedThisYear = await context.Pubs
            .Where(e => e.DaysVisited.Any(v => v.Year == thisYear))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs visited this year are {string.Join(", ", pubsVisitedThisYear)}");

        #region PubsVisitedInOrder
        var pubsVisitedInOrder = await context.Pubs
            .Select(e => new
            {
                e.Name,
                FirstVisited = e.DaysVisited.OrderBy(v => v).First(),
                LastVisited = e.DaysVisited.OrderByDescending(v => v).First(),
            })
            .OrderBy(p => p.FirstVisited)
            .ToListAsync();
        #endregion

        foreach (var pub in pubsVisitedInOrder)
        {
            Console.WriteLine($"{pub.Name} first visited on {pub.FirstVisited} and last visited on {pub.LastVisited}");
        }

        #region WalksWithADrink
        var walksWithADrink = await context.Walks.Select(
            w => new
            {
                WalkName = w.Name,
                PubName = w.ClosestPub.Name,
                Count = w.DaysVisited.Count(v => w.ClosestPub.DaysVisited.Contains(v)),
                TotalCount = w.DaysVisited.Count
            }).ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var walk in walksWithADrink)
        {
            Console.WriteLine($"{walk.PubName} was visited {walk.Count} times in {walk.TotalCount} \"{walk.WalkName}\" walks.");
        }
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class MyCollection : IList<int>
    {
        private readonly List<int> _list = new();
        public IEnumerator<int> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(int item) => _list.Add(item);
        public void Clear() => _list.Clear();
        public bool Contains(int item) => _list.Contains(item);
        public void CopyTo(int[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public bool Remove(int item) => _list.Remove(item);
        public int Count => _list.Count;
        public bool IsReadOnly => ((ICollection<int>)_list).IsReadOnly;
        public int IndexOf(int item) => _list.IndexOf(item);
        public void Insert(int index, int item) => _list.Insert(index, item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public int this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }

    public class PrimitiveCollections
    {
        public int Id { get; set; }
        public IEnumerable<int> Ints { get; set; } = null!;
        public ICollection<string> Strings { get; set; } = null!;
        public Collection<DateTime> DateTimes { get; set; } = null!;
        public IList<DateOnly> Dates { get; set; } = null!;

        [MaxLength(2500)]
        [Unicode(false)]
        public uint[] UnsignedInts { get; set; } = null!;

        public ObservableCollection<Guid> Guids { get; set; } = null!;
        public List<bool> Booleans { get; set; } = null!;
        public List<Uri> Urls { get; set; } = null!;

        // https://github.com/dotnet/efcore/issues/32055
        // public MyCollection SomeInts { get; set; } = null!;

        public List<int> GetOnlyInts { get; } = new();

        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly List<int> _intsField = new();

        // public List<DddId> DddIds { get; set; } = null!;
    }

    public readonly struct DddId
    {
        public DddId(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public class DddIdConverter : ValueConverter<DddId, int>
    {
        public DddIdConverter()
            : base(v => v.Value, v => new DddId())
        {
        }
    }

    #region DogWalk
    public class DogWalk
    {
        public DogWalk(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Terrain Terrain { get; set; }
        public List<DateOnly> DaysVisited { get; private set; } = new();
        public Pub ClosestPub { get; set; } = null!;
    }

    public enum Terrain
    {
        Forest,
        River,
        Hills,
        Village,
        Park,
        Beach,
    }
    #endregion

    #region Pub
    public class Pub
    {
        public Pub(string name, string[] beers)
        {
            Name = name;
            Beers = beers;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Beers { get; set; }
        public List<DateOnly> DaysVisited { get; private set; } = new();
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

        public DbSet<DogWalk> Walks => Set<DogWalk>();
        public DbSet<Pub> Pubs => Set<Pub>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0"))
                        //sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseCompatibilityLevel(120)))
                .EnableSensitiveDataLogging()
                .LogTo(
                    s =>
                    {
                        if (LoggingEnabled)
                        {
                            Console.WriteLine(s);
                        }
                    }, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PrimitiveCollections>()
                .Property(e => e.Booleans)
                .HasMaxLength(1024)
                .IsUnicode(false);

            // modelBuilder
            //     .Entity<PrimitiveCollections>()
            //     .Property(e => e.Guids)
            //     .HasColumnType("nvarchar(2000)");

            modelBuilder
                .Entity<PrimitiveCollections>()
                .Property(e => e.GetOnlyInts);

            // modelBuilder
            //     .Entity<PrimitiveCollections>()
            //     .Property(e => e.DddIds);

            modelBuilder
                .Entity<PrimitiveCollections>()
                .Property<List<int>>("_intsField");

            //modelBuilder.Entity<DogWalk>().Property(e => e.Terrain).HasConversion<string>();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<List<DateOnly>>()
                .AreUnicode(false)
                .HaveMaxLength(4000);

            configurationBuilder
                .Properties<string[]>()
                .HaveMaxLength(2000);

            // configurationBuilder
            //     .DefaultTypeMapping<DddId>()
            //     .HasConversion<DddIdConverter>();
        }

        public async Task Seed()
        {
            Add(
                new PrimitiveCollections
                {
                    Ints = new[] { 1, 2, 3 },
                    Strings = new List<string> { "One", "Two", "Three" },
                    DateTimes = new Collection<DateTime> { new(2023, 1, 1, 1, 1, 1), new(2023, 2, 2, 2, 2, 2), new(2023, 3, 3, 3, 3, 3) },
                    Dates = new List<DateOnly> { new(2023, 1, 1), new(2023, 2, 2), new(2023, 3, 3) },
                    UnsignedInts = new uint[] { 1, 2, 3 },
                    Guids = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
                    Booleans = new() { true, false, true },
                    Urls = new() { new("https://127.0.0.1/"), new("http://192.168.0.1/"), new("https://devblogs.microsoft.com/dotnet/") },
                    //SomeInts = new() { 1, 2, 3 },
                    // DddIds = new() { new(1), new(2), new(3) }
                });

            var oak = new Pub("The Royal Oak", new[] { "Oakham", "Carling", "Guinness", "John Smiths", "Bathams", "Tennents" })
            {
                DaysVisited =
                {
                    new(2021, 1, 2),
                    new(2021, 8, 7),
                    new(2023, 3, 22),
                    new(2022, 4, 22),
                    new(2022, 7, 21),
                    new(2022, 6, 21),
                }
            };

            var feathers = new Pub("The Prince of Wales Feathers", new[] { "Heineken", "John Smiths", "Stella Artois", "Carlsberg" })
            {
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
            };

            var swan = new Pub(
                "The White Swan", new[] { "Oakham", "Carling", "Guinness", "Heineken", "Stella Artois", "Carlsberg", "Bathams" })
            {
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
            };

            var fbi = new Pub("Farr Bay Inn", new[] { "Guinness", "Heineken", "Carlsberg", "Bathams", "Tennents" })
            {
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
            };

            var eltisley = new Pub(
                "The Eltisley",
                new[] { "Oakham", "Carling", "Guinness", "Heineken", "John Smiths", "Stella Artois", "Carlsberg", "Bathams", "Tennents" })
            {
                DaysVisited =
                {
                    new(2022, 6, 19),
                    new(2021, 1, 15),
                    new(2022, 3, 24),
                    new(2023, 2, 26),
                    new(2021, 1, 11),
                    new(2022, 3, 19),
                }
            };

            await AddRangeAsync(
                new DogWalk("Ailsworth to Nene")
                {
                    ClosestPub = feathers,
                    Terrain = Terrain.River,
                    DaysVisited =
                    {
                        new(2022, 11, 17),
                        new(2021, 9, 25),
                        new(2022, 12, 21),
                        new(2022, 11, 7),
                        new(2022, 8, 9),
                        new(2021, 7, 21),
                        new(2022, 1, 15),
                        new(2022, 9, 29),
                    }
                },
                new DogWalk("Caster Hanglands")
                {
                    ClosestPub = feathers,
                    Terrain = Terrain.Forest,
                    DaysVisited =
                    {
                        new(2022, 3, 10),
                        new(2021, 6, 5),
                        new(2021, 12, 1),
                        new(2022, 3, 8),
                        new(2022, 6, 10),
                        new(2022, 11, 13),
                        new(2022, 2, 28),
                        new(2021, 5, 10),
                        new(2021, 6, 25),
                    }
                },
                new DogWalk("Ferry Meadows")
                {
                    ClosestPub = oak,
                    Terrain = Terrain.Park,
                    DaysVisited =
                    {
                        new(2021, 1, 2),
                        new(2021, 8, 7),
                        new(2023, 3, 22),
                        new(2021, 4, 25),
                        new(2022, 6, 9),
                        new(2022, 4, 22),
                        new(2022, 7, 21),
                        new(2022, 6, 21),
                    }
                },
                new DogWalk("Woodnewton")
                {
                    ClosestPub = swan,
                    Terrain = Terrain.Village,
                    DaysVisited =
                    {
                        new(2021, 2, 24),
                        new(2023, 4, 4),
                        new(2021, 2, 19),
                        new(2022, 8, 3),
                        new(2022, 9, 14),
                        new(2022, 5, 5),
                        new(2022, 7, 13),
                        new(2021, 7, 18),
                        new(2021, 7, 23),
                    }
                },
                new DogWalk("Eltisley")
                {
                    ClosestPub = eltisley,
                    Terrain = Terrain.Village,
                    DaysVisited =
                    {
                        new(2022, 6, 19),
                        new(2021, 1, 15),
                        new(2021, 9, 29),
                        new(2022, 3, 24),
                        new(2023, 3, 2),
                        new(2023, 2, 26),
                        new(2021, 1, 11),
                        new(2022, 3, 19),
                    }
                },
                new DogWalk("Farr Beach")
                {
                    ClosestPub = fbi,
                    Terrain = Terrain.Beach,
                    DaysVisited =
                    {
                        new(2022, 11, 30),
                        new(2021, 10, 28),
                        new(2022, 2, 15),
                        new(2021, 12, 9),
                        new(2022, 5, 22),
                        new(2022, 8, 5),
                        new(2021, 11, 19),
                        new(2022, 7, 29),
                        new(2022, 12, 20),
                        new(2021, 6, 5),
                        new(2021, 6, 25),
                    }
                },
                new DogWalk("Newlands")
                {
                    ClosestPub = fbi,
                    Terrain = Terrain.Hills,
                    DaysVisited =
                    {
                        new(2021, 2, 27),
                        new(2021, 6, 17),
                        new(2021, 8, 6),
                        new(2023, 3, 1),
                        new(2021, 2, 23),
                        new(2022, 3, 17),
                        new(2021, 9, 6),
                        new(2023, 2, 22),
                        new(2022, 1, 25),
                    }
                });

            await SaveChangesAsync();
        }
    }
}
