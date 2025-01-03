using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class StringConcatSample
{
    public static async Task Concat_with_multiple_args()
    {
        Console.WriteLine($">>>> Sample: {nameof(Concat_with_multiple_args)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region StringConcat
        var shards = await context.Shards
            .Where(e => string.Concat(e.Token1, e.Token2, e.Token3) != e.TokensProcessed).ToListAsync();
        #endregion

        foreach (var shard in shards)
        {
            Console.WriteLine($"Found shard {shard.Id} with unprocessed tokens.");
        }

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

        public static async Task PopulateDatabase()
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

            await context.SaveChangesAsync();
        }
    }

    #region ProductEntityType
    public class Shard
    {
        public int Id { get; set; }

        [Required]
        public string Token1 { get; set; }

        [Required]
        public string Token2 { get; set; }

        [Required]
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
