using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SqliteSamples
{
    public static async Task SavepointsApi()
    {
        Console.WriteLine($">>>> Sample: {nameof(SavepointsApi)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        Console.WriteLine("Before update:");

        using (var context = new UsersContext())
        {
            await foreach (var user in context.Users.AsAsyncEnumerable())
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }

        await PerformUpdates();

        Console.WriteLine();
        Console.WriteLine("After update:");

        using (var context = new UsersContext())
        {
            await foreach (var user in context.Users.AsAsyncEnumerable())
            {
                Console.WriteLine($"  Found '{user.Username}'");
            }
        }
    }

    public static async Task DateOnly_and_TimeOnly()
    {
        Console.WriteLine($">>>> Sample: {nameof(DateOnly_and_TimeOnly)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new UsersContext();

        #region DateOnlyQuery
        var users = await context.Users.Where(u => u.Birthday < new DateOnly(1900, 1, 1)).ToListAsync();
        #endregion

        Console.WriteLine();

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}'");
            user.Birthday = new(user.Birthday.Year + 100, user.Birthday.Month, user.Birthday.Day);
        }

        Console.WriteLine();

        await context.SaveChangesAsync();
    }

    private static async Task PerformUpdates()
    {
        #region PerformUpdates
        using var connection = new SqliteConnection("Command Timeout=60;DataSource=test.db");
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'ajcvickers' WHERE Id = 1";
            await command.ExecuteNonQueryAsync();
        }

        await transaction.SaveAsync("MySavepoint");

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"UPDATE Users SET Username = 'wfvickers' WHERE Id = 2";
            await command.ExecuteNonQueryAsync();
        }

        await transaction.RollbackAsync("MySavepoint");

        await transaction.CommitAsync();
        #endregion
    }

    public static async Task ConnectionPooling()
    {
        Console.WriteLine($">>>> Sample: {nameof(ConnectionPooling)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using (var context = new UsersContext(quiet: false, connectionEvents: true))
        {
            #region ConnectionExample
            Console.WriteLine("Starting query...");
            Console.WriteLine();

            var users = await context.Users.ToListAsync();

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

                    await context.SaveChangesAsync();

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
        public static async Task RecreateCleanDatabase()
        {
            using var context = new UsersContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
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

            await context.SaveChangesAsync();

            context.ChangeTracker.Entries<User>().First().Entity.Username = "ajcvickers";
            await context.SaveChangesAsync();
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
