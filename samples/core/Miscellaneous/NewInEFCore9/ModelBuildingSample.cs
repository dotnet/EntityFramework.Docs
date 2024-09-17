namespace NewInEfCore9;

public static class ModelBuildingSample
{
    public static async Task Model_building_improvements_in_EF9()
    {
        PrintSampleName();

        await using var context = new ModelBuildingContext();
        await context.Database.EnsureDeletedAsync();

        context.LoggingEnabled = true;

        await context.Database.EnsureCreatedAsync();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class ModelBuildingContext : DbContext
    {
        public bool LoggingEnabled { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={GetType().Name};ConnectRetryCount=0")
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
            #region FillFactor
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id)
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasAlternateKey(e => new { e.Region, e.Ssn })
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Name })
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Region, e.Tag })
                .HasFillFactor(80);
            #endregion

            #region DefaultCache
            modelBuilder.HasSequence<int>("MySequence")
                .HasMin(10).HasMax(255000)
                .IsCyclic()
                .StartsAt(11).IncrementsBy(2);
            #endregion
        }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Region { get; set; }
        public required string Name { get; set; }
        public string? Ssn { get; set; }
        public string? Tag { get; set; }
    }
}
