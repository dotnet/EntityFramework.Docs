using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public static class CosmosSyncApisSample
{
    public static void Cosmos_provider_blocks_sync_APIs()
    {
        Console.WriteLine($">>>> Sample: {nameof(Cosmos_provider_blocks_sync_APIs)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new ShapesContext();

        var triangle = new Triangle
        {
            Name = "Impossible",
            PartitionKey = "TrianglesPartition",
            Angle1 = 90,
            Angle2 = 90,
            InsertedOn = DateTime.UtcNow
        };
        context.Add(triangle);
        context.SaveChanges();

        var equilateral = context.Triangles.Single(e => e.Name == "Equilateral");
        var isosceles = context.Triangles.Find("Isosceles", "TrianglesPartition");

        triangle.Angle2 = 89;
        context.SaveChanges();

        context.Remove(triangle);
        context.SaveChanges();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new ShapesContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
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
            context.SaveChanges();
        }
    }

    public class Triangle
    {
        public string PartitionKey { get; set; }
        public string Name { get; set; }
        public double Angle1 { get; set; }
        public double Angle2 { get; set; }
        public DateTime InsertedOn { get; set; }
    }

    public class ShapesContext(bool quiet = false) : DbContext
    {
        public DbSet<Triangle> Triangles { get; set; }

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
            optionsBuilder = optionsBuilder
                .ConfigureWarnings(b => b.Log(CosmosEventId.SyncNotSupported))
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Triangles");

            if (!quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }
    }
}
