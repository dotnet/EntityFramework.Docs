namespace NewInEfCore9;

public static class DateOnlyTimeOnlySample
{
    public static Task Can_use_DateOnly_TimeOnly_on_SQL_Server()
    {
        PrintSampleName();
        return DateOnlyTimeOnlyTest<BritishSchoolsContext>();
    }

    public static Task Can_use_DateOnly_TimeOnly_on_SQL_Server_with_JSON()
    {
        PrintSampleName();
        return DateOnlyTimeOnlyTest<BritishSchoolsContextJson>();
    }

    public static Task Can_use_DateOnly_TimeOnly_on_SQLite()
    {
        PrintSampleName();
        return DateOnlyTimeOnlyTest<BritishSchoolsContextSqlite>();
    }

    private static async Task DateOnlyTimeOnlyTest<TContext>()
        where TContext : BritishSchoolsContextBase, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        Console.WriteLine();

        // Issue https://github.com/dotnet/efcore/issues/25103
        if (!context.UseSqlite)
        {
            #region FromDateTime
            var visitedTime = new TimeOnly(12, 0);
            var visited = await context.Schools
                .Where(p => TimeOnly.FromDateTime(p.LastVisited) >= visitedTime)
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();

        // Issue https://github.com/dotnet/efcore/issues/25103
        if (!context.UseSqlite)
        {
            #region FromTimeSpan
            var visitedTime = new TimeOnly(12, 0);
            var visited = await context.Schools
                .Where(p => TimeOnly.FromTimeSpan(p.LegacyTime) >= visitedTime)
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();

        // Issue https://github.com/dotnet/efcore/issues/25103
        if (!context.UseSqlite)
        {
            var visitedAt = DateTime.UtcNow;
            var visitedSchools = await context.Schools
                .AsNoTracking()
                .SelectMany(e => e.OpeningHours)
                .Where(e => e.OpensAt <= TimeOnly.FromDateTime(visitedAt) && e.OpensAt > TimeOnly.FromDateTime(visitedAt))
                .ToListAsync();
        }

        // Issue https://github.com/dotnet/efcore/issues/33937
        // // Issue https://github.com/dotnet/efcore/issues/30223
        // if (!context.UsesJson
        //     && !context.UseSqlite)
        // {
        //     await context.Schools
        //         .SelectMany(e => e.OpeningHours)
        //         .Where(e => e.DayOfWeek == DayOfWeek.Friday)
        //         .ExecuteUpdateAsync(s => s.SetProperty(t => t.OpensAt, t => t.OpensAt!.Value.AddHours(-1)));
        // }

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class BritishSchoolsContextBase : DbContext
    {
        protected BritishSchoolsContextBase(bool useSqlite = false)
        {
            UseSqlite = useSqlite;
        }

        public bool UseSqlite { get; }
        public virtual bool UsesJson => false;
        public bool LoggingEnabled { get; set; }

        public DbSet<School> Schools => Set<School>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(
                        @$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0",
                        sqlServerOptionsBuilder => sqlServerOptionsBuilder.UseNetTopologySuite()))
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
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }

        public async Task Seed()
        {
            AddRange(
                new School
                {
                    Name = "Stowe School",
                    Founded = new(1923, 5, 11),
                    Terms =
                    {
                        new() { Name = "Michaelmas", FirstDay = new(2022, 9, 7), LastDay = new(2022, 12, 16) },
                        new() { Name = "Lent", FirstDay = new(2023, 1, 8), LastDay = new(2023, 3, 24) },
                        new() { Name = "Summer", FirstDay = new(2023, 4, 18), LastDay = new(2023, 7, 8) }
                    },
                    OpeningHours =
                    {
                        new(DayOfWeek.Sunday, null, null),
                        new(DayOfWeek.Monday, new(8, 00), new(18, 00)),
                        new(DayOfWeek.Tuesday, new(8, 00), new(18, 00)),
                        new(DayOfWeek.Wednesday, new(8, 00), new(18, 00)),
                        new(DayOfWeek.Thursday, new(8, 00), new(18, 00)),
                        new(DayOfWeek.Friday, new(8, 00), new(18, 00)),
                        new(DayOfWeek.Saturday, new(8, 00), new(17, 00))
                    }
                },
                new School
                {
                    Name = "Farr High School",
                    Founded = new(1964, 5, 1),
                    Terms =
                    {
                        new() { Name = "Autumn", FirstDay = new(2022, 8, 16), LastDay = new(2022, 12, 23) },
                        new() { Name = "Winter", FirstDay = new(2023, 1, 9), LastDay = new(2023, 3, 31) },
                        new() { Name = "Summer", FirstDay = new(2023, 4, 17), LastDay = new(2023, 6, 29) }
                    },
                    OpeningHours =
                    {
                        new(DayOfWeek.Sunday, null, null),
                        new(DayOfWeek.Monday, new(8, 45), new(15, 35)),
                        new(DayOfWeek.Tuesday, new(8, 45), new(15, 35)),
                        new(DayOfWeek.Wednesday, new(8, 45), new(15, 35)),
                        new(DayOfWeek.Thursday, new(8, 45), new(15, 35)),
                        new(DayOfWeek.Friday, new(8, 45), new(12, 50)),
                        new(DayOfWeek.Saturday, null, null)
                    }
                });

            await SaveChangesAsync();
        }
    }

    public class BritishSchoolsContext : BritishSchoolsContextBase
    {
    }

    public class BritishSchoolsContextJson : BritishSchoolsContextBase
    {

        public override bool UsesJson => true;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<School>().OwnsMany(e => e.OpeningHours).ToJson();
        }
    }

    public class BritishSchoolsContextSqlite : BritishSchoolsContextBase
    {
        public BritishSchoolsContextSqlite()
            : base(useSqlite: true)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<School>().OwnsMany(
                e => e.OpeningHours, b =>
                {
                    b.Property<int>("Id");
                    b.HasKey("Id");
                });
        }
    }

    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly Founded { get; set; }
        public DateTime LastVisited { get; set; }
        public TimeSpan LegacyTime { get; set; }
        public List<Term> Terms { get; } = new();
        public List<OpeningHours> OpeningHours { get; } = new();
    }

    public class Term
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly FirstDay { get; set; }
        public DateOnly LastDay { get; set; }
        public School School { get; set; } = null!;
    }

    [Owned]
    public class OpeningHours
    {
        public OpeningHours(DayOfWeek dayOfWeek, TimeOnly? opensAt, TimeOnly? closesAt)
        {
            DayOfWeek = dayOfWeek;
            OpensAt = opensAt;
            ClosesAt = closesAt;
        }

        public DayOfWeek DayOfWeek { get; private set; }
        public TimeOnly? OpensAt { get; set; }
        public TimeOnly? ClosesAt { get; set; }
    }
}
