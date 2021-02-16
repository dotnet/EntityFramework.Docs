using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class StringConcatSample
{
    public static void Concat_with_multiple_args()
    {
        Console.WriteLine($">>>> Sample: {nameof(Concat_with_multiple_args)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BooksContext();

        var shards = context.Shards.Where(e => string.Concat(e.Token1, e.Token2, e.Token3) != e.TokensProcessed).ToList();

        foreach (var shard in shards)
        {
            Console.WriteLine($"Found shard {shard.Id} with unprocessed tokens.");
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new BooksContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new BooksContext(quiet: true);

            context.AddRange(
                new Shard
                {
                    Token1 = "A",
                    Token2 = "B",
                    Token3 = "C",
                    TokensProcessed = "ABC"
                },
                new Shard
                {
                    Token1 = "D",
                    Token2 = "H",
                    Token3 = "C",
                    TokensProcessed = "DH"
                },
                new Shard
                {
                    Token1 = "A",
                    Token2 = "B",
                    Token3 = "C"
                },
                new Shard
                {
                    Token1 = "D",
                    Token2 = "E",
                    Token3 = "F",
                    TokensProcessed = "DEF"
                },
                new Shard
                {
                    Token1 = "J",
                    Token2 = "A",
                    Token3 = "M",
                    TokensProcessed = "JAM"
                });

            context.SaveChanges();
        }
    }

    #region ProductEntityType
    public class Shard
    {
        public int Id { get; set; }

        public string Token1 { get; set; }
        public string Token2 { get; set; }
        public string Token3 { get; set; }

        public string TokensProcessed { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<Shard> Shards { get; set; }

        private readonly bool _quiet;

        public BooksContext(bool quiet = false)
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
