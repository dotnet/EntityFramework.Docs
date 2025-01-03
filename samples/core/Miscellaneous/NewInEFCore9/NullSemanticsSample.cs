namespace NewInEfCore9;

public static class NullSemanticsSample
{
    public static Task Null_semantics_improvements_in_EF9()
    {
        PrintSampleName();
        return NullSemanticsInComparisonTest<NullSemanticsContext>();
    }

    public static Task Null_semantics_improvements_in_EF9_on_SQLite()
    {
        PrintSampleName();
        return NullSemanticsInComparisonTest<NullSemanticsContextSqlite>();
    }

    private static async Task NullSemanticsInComparisonTest<TContext>()
        where TContext : NullSemanticsContextBase, new()
    {
        await using var context = new TContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Seed();

        context.LoggingEnabled = true;
        context.ChangeTracker.Clear();

        Console.WriteLine();
        Console.WriteLine("C# null semantics for comparison operators:");
        Console.WriteLine();

        #region NegatedNullableComparisonFilter
        var negatedNullableComparisonFilter = await context.Entities
            .Where(x => !(x.NullableIntOne > x.NullableIntTwo))
            .Select(x => new { x.NullableIntOne, x.NullableIntTwo }).ToListAsync();
        #endregion

        var negatedNullableComparisonFilterL2O = await context.Entities.AsAsyncEnumerable().Where(x => !(x.NullableIntOne > x.NullableIntTwo)).Select(x => new { x.NullableIntOne, x.NullableIntTwo }).ToListAsync();

        #region NullableComparisonProjection
        var nullableComparisonProjection = await context.Entities.Select(x => new
        {
            x.NullableIntOne,
            x.NullableIntTwo,
            Operation = x.NullableIntOne > x.NullableIntTwo
        }).ToListAsync();
        #endregion

        var nullableComparisonProjectionL2O = await context.Entities.AsAsyncEnumerable().Select(x => new { x.NullableIntOne, x.NullableIntTwo, Operation = x.NullableIntOne > x.NullableIntTwo }).ToListAsync();

        #region NegatedNullableComparisonProjection
        var negatedNullableComparisonProjection = await context.Entities.Select(x => new
        {
            x.NullableIntOne,
            x.NullableIntTwo,
            Operation = !(x.NullableIntOne > x.NullableIntTwo)
        }).ToListAsync();
        #endregion

        var negatedNullableComparisonProjectionL2O = await context.Entities.AsAsyncEnumerable().Select(x => new { x.NullableIntOne, x.NullableIntTwo, Operation = !(x.NullableIntOne > x.NullableIntTwo) }).ToListAsync();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class NullSemanticsEntity
    {
        public int Id { get; set; }

        public int? NullableIntOne { get; set; }
        public int? NullableIntTwo { get; set; }
        public bool? NullableBoolOne { get; set; }
        public bool? NullableBoolTwo { get; set; }

        public int Int { get; set; }
        public bool BoolOne { get; set;}
        public bool BoolTwo { get; set; }

    }

    public class NullSemanticsContext : NullSemanticsContextBase
    {
    }

    public class NullSemanticsContextSqlite : NullSemanticsContextBase
    {
        public NullSemanticsContextSqlite()
            : base(useSqlite: true)
        {
        }
    }

    public abstract class NullSemanticsContextBase : DbContext
    {
        protected NullSemanticsContextBase(bool useSqlite = false)
        {
            UseSqlite = useSqlite;
        }

        public bool UseSqlite { get; }
        public bool LoggingEnabled { get; set; }

        public DbSet<NullSemanticsEntity> Entities => Set<NullSemanticsEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => (UseSqlite
                    ? optionsBuilder.UseSqlite(@$"DataSource={GetType().Name}.db")
                    : optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0"))
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
            modelBuilder.Entity<NullSemanticsEntity>().Property(x => x.Id).ValueGeneratedNever();
        }

        public async Task Seed()
        {
            var entities = new List<NullSemanticsEntity>();
            (int? Int, bool? Bool)[] data = [ (1, true), (2, false), (null, null) ];

            var k = 0;
            for (var i = 0; i < data.Length; i++)
            {
                for (var j = 0; j < data.Length; j++)
                {
                    k++;
                    entities.Add(new NullSemanticsEntity
                    {
                        Id = k,
                        NullableBoolOne = data[i].Bool,
                        NullableBoolTwo = data[j].Bool,
                        NullableIntOne = data[i].Int,
                        NullableIntTwo = data[j].Int,
                        BoolOne = data[k % 2].Bool!.Value,
                        BoolTwo = data[(k + 1) % 2].Bool!.Value,
                        Int = data[k % 2].Int!.Value,
                    });
                }
            }

            await AddRangeAsync(entities);
            await SaveChangesAsync();
        }
    }
}
