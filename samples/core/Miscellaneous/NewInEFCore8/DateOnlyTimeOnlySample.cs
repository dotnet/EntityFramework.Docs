namespace NewInEfCore8;

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

        var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "UTC", "GMT");

        var today = DateOnly.FromDateTime(now);
        var currentTerms = await context.Schools
            .Include(s => s.Terms.Where(t => t.FirstDay <= today && t.LastDay >= today))
            .ToListAsync();

        Console.WriteLine();
        Console.WriteLine("Current terms:");
        foreach (var school in currentTerms)
        {
            var term = school.Terms.SingleOrDefault();
            if (term == null)
            {
                Console.WriteLine($"  {school.Name} is not current in term.");
            }
            else
            {
                Console.WriteLine($"  The current term for {school.Name} is '{term.Name}'.");
            }
        }

        Console.WriteLine();

        var time = TimeOnly.FromDateTime(now);
        var dayOfWeek = today.DayOfWeek;
        List<School> openSchools;

        if (context.UsesJson)
        {
            #region OpenSchoolsJson
            openSchools = await context.Schools
                .Where(
                    s => s.Terms.Any(
                             t => t.FirstDay <= today
                                  && t.LastDay >= today)
                         && s.OpeningHours[(int)dayOfWeek].OpensAt < time
                         && s.OpeningHours[(int)dayOfWeek].ClosesAt >= time)
                .ToListAsync();
            #endregion
        }
        else
        {
            #region OpenSchools
            openSchools = await context.Schools
                .Where(
                    s => s.Terms.Any(
                             t => t.FirstDay <= today
                                  && t.LastDay >= today)
                         && s.OpeningHours.Any(
                             o => o.DayOfWeek == dayOfWeek
                                  && o.OpensAt < time && o.ClosesAt >= time))
                .ToListAsync();
            #endregion
        }

        Console.WriteLine();
        Console.WriteLine("Open schools:");
        foreach (var school in openSchools)
        {
            Console.WriteLine($"  {school.Name} is open and closes at {school.OpeningHours.Single(e => e.DayOfWeek == dayOfWeek).ClosesAt}.");
        }

        Console.WriteLine();

        context.ChangeTracker.Clear();

        foreach (var school in await context.Schools.Include(e => e.Terms).ToListAsync())
        {
            var winter = school.Terms.Single(e => e.LastDay.Year == 2022);
            winter.LastDay = winter.LastDay.AddDays(1);
            var friday = school.OpeningHours.Single(e => e.DayOfWeek == DayOfWeek.Friday);
            friday.OpensAt = friday.OpensAt?.AddHours(-1);
        }

        await context.SaveChangesAsync();

        Console.WriteLine();

        context.ChangeTracker.Clear();

        #region UpdateForSnowDay
        await context.Schools
            .Where(e => e.Terms.Any(t => t.LastDay.Year == 2022))
            .SelectMany(e => e.Terms)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.LastDay, t => t.LastDay.AddDays(1)));
        #endregion

        // Issue https://github.com/dotnet/efcore/issues/30223
        if (!context.UsesJson
            && !context.UseSqlite)
        {
            await context.Schools
                .SelectMany(e => e.OpeningHours)
                .Where(e => e.DayOfWeek == DayOfWeek.Friday)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.OpensAt, t => t.OpensAt!.Value.AddHours(-1)));
        }

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }
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

#region BritishSchools
public class School
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly Founded { get; set; }
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
#endregion
