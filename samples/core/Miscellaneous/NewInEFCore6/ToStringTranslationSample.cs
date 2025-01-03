using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ToStringTranslationSample
{
    public static async Task Using_ToString_in_queries()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_ToString_in_queries)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = await context.Users.Where(u => EF.Functions.Like(u.PhoneNumber.ToString(), "%555%")).ToListAsync();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}' with phone number '{user.PhoneNumber}'");
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
                new User { PhoneNumber = 4255551234, Username = "arthur" },
                new User { PhoneNumber = 5155552234, Username = "wendy" },
                new User { PhoneNumber = 18005525123, Username = "microsoft" });

            await context.SaveChangesAsync();
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
                .UseSqlite("Command Timeout=60;DataSource=test.db");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
