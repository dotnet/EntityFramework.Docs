using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ToStringTranslationSample
{
    public static void Using_ToString_in_queries()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_ToString_in_queries)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = context.Users.Where(u => EF.Functions.Like(u.PhoneNumber.ToString(), "%555%")).ToList();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}' with phone number '{user.PhoneNumber}'");
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
                new User { PhoneNumber = 4255551234, Username = "arthur" },
                new User { PhoneNumber = 5155552234, Username = "wendy" },
                new User { PhoneNumber = 18005525123, Username = "microsoft" });

            context.SaveChanges();
        }
    }

    #region UserEntityType
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public long PhoneNumber { get; set; }
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
                .UseSqlite("DataSource=test.db");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
