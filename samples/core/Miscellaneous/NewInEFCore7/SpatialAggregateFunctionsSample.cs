using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Union;

namespace NewInEfCore7;

public static class SpatialAggregateFunctionsSample
{
    public static Task Translate_spatial_aggregate_functions_SqlServer()
    {
        PrintSampleName();
        return QueryTest<GeoCacheContextSqlServer>();
    }

    public static Task Translate_spatial_aggregate_functions_Sqlite()
    {
        PrintSampleName();
        return QueryTest<GeoCacheContextSqlite>();
    }

    public static Task Translate_spatial_aggregate_functions_InMemory()
    {
        PrintSampleName();
        return QueryTest<GeoCacheContextInMemory>();
    }

    private static async Task QueryTest<TContext>()
        where TContext : GeoCacheContext, new()
    {
        await using (var context = new TContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(
                new GeoCache
                {
                    Name = "Sandpiper Cache", Owner = "Nilwob Inc.", Location = new Point(-93.71855, 41.760783) { SRID = 4326 }
                },
                new GeoCache
                {
                    Name = "Paddy's Pot-O-Gold", Owner = "Nilwob Inc.", Location = new Point(-93.733633, 41.775633) { SRID = 4326 }
                },
                new GeoCache { Name = "Blazing Beacon", Owner = "The Spokes", Location = new Point(-1.606483, 55.392433) { SRID = 4326 } },
                new GeoCache
                {
                    Name = "133 Steps to Relaxation",
                    Owner = "Team isuforester",
                    Location = new Point(-93.854733, 41.9389) { SRID = 4326 }
                },
                new EuclideanPoint { Name = "A", Point = new Point(1.0, 1.0) },
                new EuclideanPoint { Name = "A", Point = new Point(1.0, 2.0) },
                new EuclideanPoint { Name = "B", Point = new Point(2.0, 1.0) },
                new EuclideanPoint { Name = "B", Point = new Point(2.0, 2.0) });

            await context.SaveChangesAsync();
        }

        await using (var context = new TContext())
        {
            #region GeometryCombinerCombine
            var query = context.Caches
                .Where(cache => cache.Location.X < -90)
                .GroupBy(cache => cache.Owner)
                .Select(
                    grouping => new { Id = grouping.Key, Combined = GeometryCombiner.Combine(grouping.Select(cache => cache.Location)) });
            #endregion

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine(group);
            }
        }

        await using (var context = new TContext())
        {
            var query = context.Caches
                .Where(cache => cache.Location.X < -90)
                .GroupBy(cache => cache.Owner)
                .Select(grouping => new { Id = grouping.Key, ConvexHull = ConvexHull.Create(grouping.Select(cache => cache.Location)) });

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine(group);
            }
        }

        await using (var context = new TContext())
        {
            var query = context.Caches
                .Where(cache => cache.Location.X < -90)
                .GroupBy(cache => cache.Owner)
                .Select(grouping => new { Id = grouping.Key, Union = UnaryUnionOp.Union(grouping.Select(cache => cache.Location)) });

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine(group);
            }
        }

        await using (var context = new TContext())
        {
            var query = context.Points
                .GroupBy(point => point.Name)
                .Select(
                    grouping => new
                    {
                        Id = grouping.Key, Combined = EnvelopeCombiner.CombineAsGeometry(grouping.Select(point => point.Point))
                    });

            await foreach (var group in query.AsAsyncEnumerable())
            {
                Console.WriteLine(group);
            }
        }

        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public abstract class GeoCacheContext : DbContext
    {
        public DbSet<GeoCache> Caches => Set<GeoCache>();
        public DbSet<EuclideanPoint> Points => Set<EuclideanPoint>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class GeoCacheContextSqlServer : GeoCacheContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=GeoCaches",
                    b => b.UseNetTopologySuite()));
    }

    public class GeoCacheContextSqlite : GeoCacheContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseSqlite(
                    "Data Source = geocaches.db",
                    b => b.UseNetTopologySuite()));
    }

    public class GeoCacheContextInMemory : GeoCacheContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => base.OnConfiguring(
                optionsBuilder.UseInMemoryDatabase(nameof(GeoCacheContextInMemory)));
    }

    public class GeoCache
    {
        public int Id { get; set; }
        public string? Owner { get; set; }
        public string Name { get; set; } = null!;
        public Point Location { get; set; } = null!;
    }

    public class EuclideanPoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName = "geometry")]
        public Point Point { get; set; } = null!;
    }
}
