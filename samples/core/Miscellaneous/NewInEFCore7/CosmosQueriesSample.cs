using System.Text.RegularExpressions;

namespace NewInEfCore7;

public static class CosmosQueriesSample
{
    public static async Task Cosmos_translations_for_RegEx_Match()
    {
        PrintSampleName();

        await using var context = new ShapesContext();

        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"Cosmos emulator not found: '{exception.Message}'");
            return;
        }

        await context.AddRangeAsync(
            new Triangle { Name = "Acute", Angle1 = 75, Angle2 = 85, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(2) },
            new Triangle { Name = "Obtuse", Angle1 = 110, Angle2 = 35, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(1) },
            new Triangle { Name = "Right", Angle1 = 90, Angle2 = 45, InsertedOn = DateTime.UtcNow },
            new Triangle { Name = "Isosceles", Angle1 = 75, Angle2 = 75, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(1) },
            new Triangle { Name = "Equilateral", Angle1 = 60, Angle2 = 60, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(2) }
        );

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        #region RegexIsMatch
        var containsInnerT = await context.Triangles
            .Where(o => Regex.IsMatch(o.Name, "[a-z]t[a-z]", RegexOptions.IgnoreCase))
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var result in containsInnerT)
        {
            Console.WriteLine($" {result.Name}");
        }
        Console.WriteLine();
    }

    private static void PrintSampleName([CallerMemberName] string? methodName = null)
    {
        Console.WriteLine($">>>> Sample: {methodName}");
        Console.WriteLine();
    }

    public class Triangle
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public double Angle1 { get; set; }
        public double Angle2 { get; set; }
        public DateTime InsertedOn { get; set; }
    }

    public class ShapesContext : DbContext
    {
        public DbSet<Triangle> Triangles => Set<Triangle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Triangle>().ToContainer("Shapes");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Queries")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
    }
}
