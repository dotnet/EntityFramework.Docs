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

        Console.WriteLine("Before update:");

        using (var context = new UsersContext())
        {
            foreach (var user in context.Users)
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }

        PerformUpdates();

        Console.WriteLine();
        Console.WriteLine("After update:");

        using (var context = new UsersContext())
        {
            foreach (var user in context.Users)
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }
    }

    public static void DateOnly_and_TimeOnly()
    {
        Console.WriteLine($">>>> Sample: {nameof(DateOnly_and_TimeOnly)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new UsersContext();

        #region DateOnlyQuery
        var users = context.Users.Where(u => u.Birthday < new DateOnly(1900, 1, 1)).ToList();
        #endregion
            
        Console.WriteLine();

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}'");
            user.Birthday = new(user.Birthday.Year + 100, user.Birthday.Month, user.Birthday.Day);
        }
            
        Console.WriteLine();

        context.SaveChanges();
    }

    private static void PerformUpdates()
    {
        #region PerformUpdates
        using var connection = new SqliteConnection("Command Timeout=60;DataSource=test.db");
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

    public static void ConnectionPooling()
    {
        Console.WriteLine($">>>> Sample: {nameof(ConnectionPooling)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using (var context = new UsersContext(quiet: false, connectionEvents: true))
        {
            #region ConnectionExample
            Console.WriteLine("Starting query...");
            Console.WriteLine();

            var users = context.Users.ToList();

            Console.WriteLine();
            Console.WriteLine("Query finished.");
            Console.WriteLine();

            foreach (var user in users)
            {
                if (user.Username.Contains("microsoft"))
                {
                    user.Username = "msft:" + user.Username;

                    Console.WriteLine("Starting SaveChanges...");
                    Console.WriteLine();

                    context.SaveChanges();

                    Console.WriteLine();
                    Console.WriteLine("SaveChanges finished.");
                }
            }
            #endregion
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new UsersContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new UsersContext(quiet: true);

            context.AddRange(
                new User
                {
                    Username = "arthur", 
                    Birthday = new(1869, 9, 3),
                    TokensRenewed = new(16, 30) 
                },
                new User
                {
                    Username = "wendy",
                    Birthday = new(1873, 8, 3),
                    TokensRenewed = new(9, 25) 
                },
                new User
                {
                    Username = "microsoft",
                    Birthday = new(1975, 4, 4),
                    TokensRenewed = new(0, 0) 
                });

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
        
        public DateOnly Birthday { get; set; }
        public TimeOnly TokensRenewed { get; set; }
    }
    #endregion

    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private readonly bool _quiet;
        private readonly bool _connectionEvents;

        public UsersContext(
            bool quiet = false,
            bool connectionEvents = false)
        {
            _quiet = quiet;
            _connectionEvents = connectionEvents;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlite("Command Timeout=60;DataSource=test.db");

            if (!_quiet)
            {
                if (_connectionEvents)
                {
                    optionsBuilder.LogTo(
                        Console.WriteLine,
                        new[] { RelationalEventId.ConnectionOpened, RelationalEventId.ConnectionClosed });
                }
                else
                {
                    optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
                }
            }
        }
    }
}
