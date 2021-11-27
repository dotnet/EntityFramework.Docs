﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static System.Console;

public static class CosmosQueriesSample
{
    public static void Cosmos_queries()
    {
        WriteLine($">>>> Sample: {nameof(Cosmos_queries)}\n");

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new ShapesContext();

        #region StringTranslations
        var stringResults
            = context.Triangles.Where(
                    e => e.Name.Length > 4
                         && e.Name.Trim().ToLower() != "obtuse"
                         && e.Name.TrimStart().Substring(2, 2).Equals("uT", StringComparison.OrdinalIgnoreCase))
                .ToList();
        #endregion

        WriteLine();
        foreach (var result in stringResults)
        {
            WriteLine($" {result.Name}");
        }
        WriteLine();

        #region MathTranslations
        var hypotenuse = 42.42;
        var mathResults
            = context.Triangles.Where(
                    e => (Math.Round(e.Angle1) == 90.0
                          || Math.Round(e.Angle2) == 90.0)
                         && (hypotenuse * Math.Sin(e.Angle1) > 30.0
                             || hypotenuse * Math.Cos(e.Angle2) > 30.0))
                .ToList();
        #endregion

        WriteLine();
        foreach (var result in mathResults)
        {
            WriteLine($" {result.Name}");
        }
        WriteLine();

        #region TimeTranslations
        var timeResults
            = context.Triangles.Where(
                    e => e.InsertedOn <= DateTime.UtcNow)
                .ToList();
        #endregion

        WriteLine();
        foreach (var result in timeResults)
        {
            WriteLine($" {result.Name}");
        }
        WriteLine();

        #region DistictTranslation
        var distinctResults
            = context.Triangles
                .Select(e => e.Angle1).OrderBy(e => e).Distinct()
                .ToList();
        #endregion

        WriteLine();
        foreach (var result in distinctResults)
        {
            WriteLine($" {result}");
        }
        WriteLine();

        #region FromSql
        var maxAngle = 60;
        var rawResults
            = context.Triangles.FromSqlRaw(
                    @"SELECT * FROM root c WHERE c[""Angle1""] <= {0} OR c[""Angle2""] <= {0}",
                    maxAngle)
                .ToList();
        #endregion

        WriteLine();
        foreach (var result in rawResults)
        {
            WriteLine($" {result.Name}");
        }
        WriteLine();
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

            context.AddRange(
                new Triangle { Name = "Acute", Angle1 = 75, Angle2 = 85, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(2) },
                new Triangle { Name = " Obtuse ", Angle1 = 110, Angle2 = 35, InsertedOn = DateTime.UtcNow - TimeSpan.FromDays(1) },
                new Triangle { Name = "Right", Angle1 = 90, Angle2 = 45, InsertedOn = DateTime.UtcNow },
                new Triangle { Name = "Isosceles", Angle1 = 75, Angle2 = 75, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(1) },
                new Triangle { Name = "Equilateral", Angle1 = 60, Angle2 = 60, InsertedOn = DateTime.UtcNow + TimeSpan.FromDays(2) }
            );

            context.SaveChanges();
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
                    new System.Text.RegularExpressions.Regex("\\\\").Replace(Environment.GetEnvironmentVariable("COSMOS_ENDPOINT"), "/"),
                    Environment.GetEnvironmentVariable("COSMOS_ACCOUNTKEY"),
                    "Queries");

            if (!_quiet)
            {
                optionsBuilder.LogTo(WriteLine, new[] { CosmosEventId.ExecutingSqlQuery });
            }
        }
    }
}
