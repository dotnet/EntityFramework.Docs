using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class MathFTranslationSample
{
    public static void Translate_MathF_methods()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_MathF_methods)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        // Fails; see #25421
        // var result = context.Reviews
        //     .Select(e => new
        //     {
        //         e.Title,
        //         Rating = MathF.Truncate(e.Rating)
        //     })
        //     .OrderBy(e => e.Rating)
        //     .First();

        Console.WriteLine();
        // Console.WriteLine($"Found {result.Title} with rating {result.Rating}.");
        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.AddRange(
                new Review
                {
                    Title = "My yellow kettle",
                    Description = "Awesome!",
                    Rating = 5.0f
                },
                new Review
                {
                    Title = "My TV",
                    Description = "Would be nice if it didn't keep switching to German.",
                    Rating = 2.0f
                },
                new Review
                {
                    Title = "Big bed",
                    Description = "Good, but hard to put together",
                    Rating = 3.5f
                });

            context.SaveChanges();
        }
    }

    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float Rating { get; set; }
    }

    public class CustomersContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }

        private readonly bool _quiet;

        public CustomersContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
