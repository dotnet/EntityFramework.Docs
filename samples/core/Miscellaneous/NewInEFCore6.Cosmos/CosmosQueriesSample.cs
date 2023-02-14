using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class CosmosQueriesSample
{
    public static async Task Cosmos_queries()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_queries)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new ShapesContext();

        #region StringTranslations
        var stringResults = await context.Triangles.Where(
                e => e.Name.Length > 4
                     && e.Name.Trim().ToLower() != "obtuse"
                     && e.Name.TrimStart().Substring(2, 2).Equals("uT", StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var result in stringResults)
        {
            Console.WriteLine($" {result.Name}");
        }
        Console.WriteLine();

        #region MathTranslations
        var hypotenuse = 42.42;
        var mathResults = await context.Triangles.Where(
                e => (Math.Round(e.Angle1) == 90.0
                      || Math.Round(e.Angle2) == 90.0)
                     && (hypotenuse * Math.Sin(e.Angle1) > 30.0
                         || hypotenuse * Math.Cos(e.Angle2) > 30.0))
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var result in mathResults)
        {
            Console.WriteLine($" {result.Name}");
        }
        Console.WriteLine();

        #region TimeTranslations
        var timeResults = await context.Triangles.Where(
                e => e.InsertedOn <= DateTime.UtcNow)
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var result in timeResults)
        {
            Console.WriteLine($" {result.Name}");
        }
        Console.WriteLine();

        #region DistictTranslation
        var distinctResults = await context.Triangles
            .Select(e => e.Angle1).OrderBy(e => e).Distinct()
            .ToListAsync();
        #endregion

        Console.WriteLine();
        foreach (var result in distinctResults)
        {
            Console.WriteLine($" {result}");
        }
        Console.WriteLine();

        {
            #region FromSql
            var maxAngle = 60;
            var results = await context.Triangles.FromSqlRaw(
                    @"SELECT * FROM root c WHERE c[""Angle1""] <= {0} OR c[""Angle2""] <= {0}", maxAngle)
                .ToListAsync();
            #endregion

            Console.WriteLine();
            foreach (var result in results)
            {
                Console.WriteLine($" {result.Name}");
            }

            Console.WriteLine();
        }

        {
            #region FromSqlComposed
            var maxAngle = 60;
            var results = await context.Triangles.FromSqlRaw(
                    @"SELECT * FROM root c WHERE c[""Angle1""] <= {0} OR c[""Angle2""] <= {0}", maxAngle)
                .Where(e => e.InsertedOn <= DateTime.UtcNow)
                .Select(e => e.Angle1).Distinct()
                .ToListAsync();
            #endregion

            Console.WriteLine();
            foreach (var result in results)
            {
                Console.WriteLine($" {result}");
            }

            Console.WriteLine();
        }
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            await using var context = new ShapesContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            await using var context = new ShapesContext(quiet: true);

            context.AddRange(
                new Triangle { Name = "Acute", Angle1 = 75, Angle2 = 85, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(2) },
                new Triangle { Name = " Obtuse ", Angle1 = 110, Angle2 = 35, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(1) },
                new Triangle { Name = "Right", Angle1 = 90, Angle2 = 45, InsertedOn = DateTime.UtcNow },
                new Triangle { Name = "Isosceles", Angle1 = 75, Angle2 = 75, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(1) },
                new Triangle { Name = "Equilateral", Angle1 = 60, Angle2 = 60, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(2) }
            );

            await context.SaveChangesAsync();
        }
    }

    #region TriangleEntity
    public class Triangle
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Angle1 { get; set; }
        public double Angle2 { get; set; }
        public DateTime InsertedOn { get; set; }
    }
    #endregion

    public class ShapesContext : DbContext
    {
        public DbSet<Triangle> Triangles { get; set; }

        private readonly bool _quiet;

        public ShapesContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Triangle>().ToContainer("Shapes");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Queries");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { CosmosEventId.ExecutingSqlQuery });
            }
        }
    }
}
