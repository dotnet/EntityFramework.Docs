namespace NewInEfCore7;

public static class StatisticalAggregateFunctionsSample
{
    public static async Task Translate_statistical_aggregate_functions()
    {
        PrintSampleName();

        await using (var context = new StatisticsContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var toast = new Uploader { Name = "Toast" };
            var alice = new Uploader { Name = "Alice" };

            await context.AddRangeAsync(
                new Download { Uploader = alice, DownloadCount = 1024 },
                new Download { Uploader = alice, DownloadCount = 2048 },
                new Download { Uploader = toast, DownloadCount = 4096 },
                new Download { Uploader = toast, DownloadCount = 8192 },
                new Download { Uploader = toast, DownloadCount = 16384 });

            await context.SaveChangesAsync();

            #region StatsForAll
            var query = context.Downloads
                .GroupBy(download => download.Uploader.Id)
                .Select(
                    grouping => new
                    {
                        Author = grouping.Key,
                        TotalCost = grouping.Sum(d => d.DownloadCount),
                        AverageViews = grouping.Average(d => d.DownloadCount),
                        VariancePopulation = EF.Functions.VariancePopulation(grouping.Select(d => d.DownloadCount)),
                        VarianceSample = EF.Functions.VarianceSample(grouping.Select(d => d.DownloadCount)),
                        StandardDeviationPopulation = EF.Functions.StandardDeviationPopulation(grouping.Select(d => d.DownloadCount)),
                        StandardDeviationSample = EF.Functions.StandardDeviationSample(grouping.Select(d => d.DownloadCount))
                    });
            #endregion

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

    public class StatisticsContext : DbContext
    {
        public DbSet<Download> Downloads => Set<Download>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Downloads;ConnectRetryCount=0")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }

    public class Uploader
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<Download> Downloads { get; } = new List<Download>();
    }

    public class Download
    {
        public int Id { get; set; }
        public Uploader Uploader { get; set; } = default!;
        public int DownloadCount { get; set; }
    }
}
