using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class CosmosDiagnosticsSample
{
    public static async Task Cosmos_diagnostics()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_diagnostics)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new ShapesContext();

        Console.WriteLine();
        Console.WriteLine("Insert diagnostic events:");
        Console.WriteLine();

        #region InsertEvents
        var triangle = new Triangle
        {
            Name = "Impossible",
            PartitionKey = "TrianglesPartition",
            Angle1 = 90,
            Angle2 = 90,
            InsertedOn = DateTime.UtcNow
        };
        context.Add(triangle);
        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Query diagnostic events:");
        Console.WriteLine();

        #region QueryEvents
        var equilateral = await context.Triangles.SingleAsync(e => e.Name == "Equilateral");
        #endregion

        Console.WriteLine();
        Console.WriteLine("Find diagnostic events:");
        Console.WriteLine();

        #region FindEvents
        var isosceles = await context.Triangles.FindAsync("Isosceles", "TrianglesPartition");
        #endregion

        Console.WriteLine();
        Console.WriteLine("Update diagnostic events:");
        Console.WriteLine();

        #region UpdateEvents
        triangle.Angle2 = 89;
        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Delete diagnostic events:");
        Console.WriteLine();

        #region DeleteEvents
        context.Remove(triangle);
        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new ShapesContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using var context = new ShapesContext(quiet: true);

            var triangles = new List<Triangle>
            {
                new() { Name = "Acute", PartitionKey = "TrianglesPartition", Angle1 = 75, Angle2 = 85, InsertedOn = DateTime.UtcNow },
                new() { Name = " Obtuse ", PartitionKey = "TrianglesPartition", Angle1 = 110, Angle2 = 35, InsertedOn = DateTime.UtcNow },
                new() { Name = "Right", PartitionKey = "TrianglesPartition", Angle1 = 90, Angle2 = 45, InsertedOn = DateTime.UtcNow },
                new() { Name = "Isosceles", PartitionKey = "TrianglesPartition", Angle1 = 75, Angle2 = 75, InsertedOn = DateTime.UtcNow },
                new() { Name = "Equilateral", PartitionKey = "TrianglesPartition", Angle1 = 60, Angle2 = 60, InsertedOn = DateTime.UtcNow }
            };

            context.AddRange(triangles);
            await context.SaveChangesAsync();
        }
    }

    #region TriangleEntity
    public class Triangle
    {
        public string PartitionKey { get; set; }
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
            modelBuilder.Entity<Triangle>(
                b =>
                {
                    b.ToContainer("Shapes");
                    b.HasPartitionKey(e => e.PartitionKey);
                    b.HasKey(e => new { e.Name, e.PartitionKey });
                    b.Property(c => c.Name).ToJsonProperty("id");
                    b.Property(c => c.PartitionKey).ToJsonProperty("pk");
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Triangles");

            if (!_quiet)
            {
                optionsBuilder.LogTo(
                    Console.WriteLine,
                    new[]
                    {
                        CosmosEventId.ExecutedCreateItem,
                        CosmosEventId.ExecutedDeleteItem,
                        CosmosEventId.ExecutedReadItem,
                        CosmosEventId.ExecutedReadNext,
                        CosmosEventId.ExecutedReplaceItem,
                        CosmosEventId.ExecutingReadItem,
                        CosmosEventId.ExecutingSqlQuery
                    });
            }
        }
    }
}
