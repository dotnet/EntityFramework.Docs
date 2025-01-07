using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class RandomFunctionSample
{
    public static async Task Call_EF_Functions_Random()
    {
        Console.WriteLine($">>>> Sample: {nameof(Call_EF_Functions_Random)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = await context.Users.Where(u => u.Popularity == (int)(EF.Functions.Random() * 4.0) + 1).ToListAsync();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}' with popularity '{user.Popularity}'");
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
                new User { Popularity = 1, Username = "arthur" },
                new User { Popularity = 2, Username = "wendy" },
                new User { Popularity = 3, Username = "smokey" },
                new User { Popularity = 4, Username = "alice" },
                new User { Popularity = 5, Username = "mac" },
                new User { Popularity = 1, Username = "baxter" },
                new User { Popularity = 2, Username = "olive" },
                new User { Popularity = 3, Username = "toast" });

            await context.SaveChangesAsync();
        }
    }

    #region UserEntityType
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Popularity { get; set; }
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
