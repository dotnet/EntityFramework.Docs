using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class IsNullOrWhitespaceSample
{
    public static async Task Translate_IsNullOrWhitespace()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_IsNullOrWhitespace)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = await context.Users.Where(
            e => string.IsNullOrWhiteSpace(e.FirstName)
                 || string.IsNullOrWhiteSpace(e.LastName)).ToListAsync();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"Found '{user.FirstName}' '{user.LastName}'");
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
                new User
                {
                    FirstName = "Arthur"
                },
                new User
                {
                    FirstName = "Arthur",
                    LastName = "Vickers"
                },
                new User
                {
                    FirstName = "",
                    LastName = "Vickers"
                },
                new User
                {
                    FirstName = " ",
                    LastName = "Vickers"
                },
                new User
                {
                    FirstName = "Arthur",
                    LastName = "\t"
                });

            await context.SaveChangesAsync();
        }
    }

    #region ProductEntityType
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<User> Users { get; set; }

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
