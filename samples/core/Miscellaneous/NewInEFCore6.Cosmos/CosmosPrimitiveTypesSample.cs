using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class CosmosPrimitiveTypesSample
{
    public static async Task Collections_and_dictionaries_of_primitive_types()
    {
        Console.WriteLine($">>>> Sample: {nameof(Collections_and_dictionaries_of_primitive_types)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();

        #region Insert
        using var context = new BooksContext();

        var book = new Book
        {
            Title = "How It Works: Incredible History",
            Quotes = new List<string>
            {
                "Thomas (Tommy) Flowers was the British engineer behind the design of the Colossus computer.",
                "Invented originally for Guinness, plastic widgets are nitrogen-filled spheres.",
                "For 20 years after its introduction in 1979, the Walkman dominated the personal stereo market."
            },
            Notes = new Dictionary<string, string>
            {
                { "121", "Fridges" },
                { "144", "Peter Higgs" },
                { "48", "Saint Mark's Basilica" },
                { "36", "The Terracotta Army" }
            }
        };

        context.Add(book);
        await context.SaveChangesAsync();
        #endregion

        #region Updates
        book.Quotes.Add("Pressing the emergency button lowered the rods again.");
        book.Notes["48"] = "Chiesa d'Oro";

        await context.SaveChangesAsync();
        #endregion

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new BooksContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    #region BookEntity
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<string> Quotes { get; set; }
        public IDictionary<string, string> Notes { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        private readonly bool _quiet;

        public BooksContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToContainer("Shapes");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #region HttpClientFactory
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseCosmos(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "PrimitiveCollections",
                    cosmosOptionsBuilder =>
                    {
                        cosmosOptionsBuilder.HttpClientFactory(
                            () => new HttpClient(
                                new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback =
                                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                }));
                    });
            #endregion

            if (!_quiet)
            {
                optionsBuilder.LogTo(
                    Console.WriteLine,
                    new[] { CosmosEventId.ExecutedCreateItem, CosmosEventId.ExecutingSqlQuery, CoreEventId.SaveChangesCompleted });
            }
        }
    }
}
