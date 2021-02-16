using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SqliteSamples
{
    public static void SavepointsApi()
    {
        Console.WriteLine($">>>> Sample: {nameof(SavepointsApi)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        Console.WriteLine("After update:");

        using (var context = new BooksContext())
        {
            foreach (var user in context.Users)
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }

        PerformUpdates();

        Console.WriteLine();
        Console.WriteLine("After update:");

        using (var context = new BooksContext())
        {
            foreach (var user in context.Users)
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }
    }

    private static void PerformUpdates()
    {
        #region PerformUpdates
        using var connection = new SqliteConnection("DataSource=test.db");
        connection.Open();

        using var transaction = connection.BeginTransaction();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'ajcvickers' WHERE Id = 1";
            command.ExecuteNonQuery();
        }

        transaction.Save("MySavepoint");

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'wfvickers' WHERE Id = 2";
            command.ExecuteNonQuery();
        }

        transaction.Rollback("MySavepoint");

        transaction.Commit();
        #endregion
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
                new User { Username = "arthur" },
                new User { Username = "wendy" },
                new User { Username = "microsoft" });

            context.SaveChanges();

            context.ChangeTracker.Entries<User>().First().Entity.Username = "ajcvickers";
            context.SaveChanges();
        }
    }

    #region UserEntityType
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
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
                .UseSqlite("Command Timeout=60;DataSource=test.db");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
