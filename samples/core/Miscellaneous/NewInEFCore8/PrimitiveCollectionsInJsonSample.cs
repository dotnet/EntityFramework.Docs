namespace NewInEfCore8;

public static class PrimitiveCollectionsInJsonSample
{
    public static Task Queries_using_primitive_collections_in_JSON_documents()
    {
        PrintSampleName();
        return ContainsTest<PubsAndWalksContext>();
    }

    public static Task Queries_using_primitive_collections_in_JSON_documents_SQLite()
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
            .Where(e => e.Beers.Beers.Contains(beer))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with {beer} are {string.Join(", ", pubsWithHeineken)}");

        #region PubsWithLager
        var beers = new[] { "Carling", "Heineken", "Stella Artois", "Carlsberg" };
        var pubsWithLager = await context.Pubs
            .Where(e => beers.Any(b => e.Beers.Beers.Contains(b)))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs with lager are {string.Join(", ", pubsWithLager)}");

        #region PubsVisitedThisYear
        var thisYear = DateTime.Now.Year;
        var pubsVisitedThisYear = await context.Pubs
            .Where(e => e.Visits.DaysVisited.Any(v => v.Year == thisYear))
            .Select(e => e.Name)
            .ToListAsync();
        #endregion

        Console.WriteLine($"\nPubs visited this year are {string.Join(", ", pubsVisitedThisYear)}");

        #region PubsVisitedInOrder
        var pubsVisitedInOrder = await context.Pubs
            .Select(e => new
            {
                e.Name,
                FirstVisited = e.Visits.DaysVisited.OrderBy(v => v).First(),
                LastVisited = e.Visits.DaysVisited.OrderByDescending(v => v).First(),
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
                WalkLocationTag = w.Visits.LocationTag,
                PubLocationTag = w.ClosestPub.Visits.LocationTag,
                Count = w.Visits.DaysVisited.Count(v => w.ClosestPub.Visits.DaysVisited.Contains(v)),
                TotalCount = w.Visits.DaysVisited.Count
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
        public Visits Visits { get; set; } = null!;
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
        public Pub(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public BeerData Beers { get; set; } = null!;
        public Visits Visits { get; set; } = null!;
    }

    public class Visits
    {
        public string? LocationTag { get; set; }
        public List<DateOnly> DaysVisited { get; set; } = null!;
    }
    #endregion

    public class BeerData
    {
        public string[] Beers { get; set; } = null!;
    }


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
            modelBuilder.Entity<Pub>(
                b =>
                {
                    b.OwnsOne(e => e.Beers).ToJson();
                    b.OwnsOne(e => e.Visits).ToJson();
                });

            modelBuilder.Entity<DogWalk>().OwnsOne(e => e.Visits).ToJson();
        }

        public async Task Seed()
        {
            var oak = new Pub("The Royal Oak")
            {
                Beers = new BeerData { Beers = new[] { "Oakham", "Carling", "Guinness", "John Smiths", "Bathams", "Tennents" } },
                Visits = new Visits
                {
                    LocationTag = "Pub visit",
                    DaysVisited = new()
                    {
                        new(2021, 1, 2),
                        new(2021, 8, 7),
                        new(2023, 3, 22),
                        new(2022, 4, 22),
                        new(2022, 7, 21),
                        new(2022, 6, 21),
                    }
                }
            };

            var feathers = new Pub("The Prince of Wales Feathers")
            {
                Beers = new BeerData { Beers = new[] { "Heineken", "John Smiths", "Stella Artois", "Carlsberg" } },
                Visits = new Visits
                {
                    LocationTag = "Pub visit",
                    DaysVisited = new()
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
                }
            };

            var swan = new Pub("The White Swan")
            {
                Beers = new BeerData { Beers = new[] { "Oakham", "Carling", "Guinness", "Heineken", "Stella Artois", "Carlsberg", "Bathams" } },
                Visits = new Visits
                {
                    LocationTag = "Pub visit",
                    DaysVisited = new()
                    {
                        new(2021, 2, 24),
                        new(2022, 8, 3),
                        new(2022, 9, 14),
                        new(2022, 5, 5),
                        new(2022, 7, 13),
                        new(2021, 7, 18),
                        new(2021, 7, 23),
                    }
                }
            };

            var fbi = new Pub("Farr Bay Inn")
            {
                Beers = new BeerData { Beers = new[] { "Guinness", "Heineken", "Carlsberg", "Bathams", "Tennents" } },
                Visits = new Visits
                {
                    LocationTag = "Pub visit",
                    DaysVisited = new()
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
                }
            };

            var eltisley = new Pub("The Eltisley")
            {
                Beers = new BeerData
                {
                    Beers = new[]
                    {
                        "Oakham", "Carling", "Guinness", "Heineken", "John Smiths", "Stella Artois", "Carlsberg", "Bathams",
                        "Tennents"
                    }
                },
                Visits = new Visits
                {
                    LocationTag = "Pub visit",
                    DaysVisited = new()
                    {
                        new(2022, 6, 19),
                        new(2021, 1, 15),
                        new(2022, 3, 24),
                        new(2023, 2, 26),
                        new(2021, 1, 11),
                        new(2022, 3, 19),
                    }
                }
            };

            await AddRangeAsync(
                new DogWalk("Ailsworth to Nene")
                {
                    ClosestPub = feathers,
                    Terrain = Terrain.River,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },

                new DogWalk("Caster Hanglands")
                {
                    ClosestPub = feathers,
                    Terrain = Terrain.Forest,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },
                new DogWalk("Ferry Meadows")
                {
                    ClosestPub = oak,
                    Terrain = Terrain.Park,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },
                new DogWalk("Woodnewton")
                {
                    ClosestPub = swan,
                    Terrain = Terrain.Village,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },
                new DogWalk("Eltisley")
                {
                    ClosestPub = eltisley,
                    Terrain = Terrain.Village,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },
                new DogWalk("Farr Beach")
                {
                    ClosestPub = fbi,
                    Terrain = Terrain.Beach,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                },
                new DogWalk("Newlands")
                {
                    ClosestPub = fbi,
                    Terrain = Terrain.Hills,
                    Visits = new Visits
                    {
                        LocationTag = "Walk",
                        DaysVisited = new()
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
                    }
                });

            await SaveChangesAsync();
        }
    }
}
