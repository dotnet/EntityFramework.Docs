using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class IsNullOrWhitespaceSample
{
    public static void Translate_IsNullOrWhitespace()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_IsNullOrWhitespace)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = context.Users.Where(
            e => string.IsNullOrWhiteSpace(e.FirstName)
                 || string.IsNullOrWhiteSpace(e.LastName)).ToList();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"Found '{user.FirstName}' '{user.LastName}'");
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

            context.SaveChanges();
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
