using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class RandomFunctionSample
{
    public static void Call_EF_Functions_Random()
    {
        Console.WriteLine($">>>> Sample: {nameof(Call_EF_Functions_Random)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        var users = context.Users.Where(u => u.Popularity == (int)(EF.Functions.Random() * 5.0) + 1).ToList();
        #endregion

        foreach (var user in users)
        {
            Console.WriteLine($"  Found '{user.Username}' with popularity '{user.Popularity}'");
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
                new User { Popularity = 1, Username = "arthur" },
                new User { Popularity = 2, Username = "wendy" },
                new User { Popularity = 3, Username = "smokey" },
                new User { Popularity = 4, Username = "alice" },
                new User { Popularity = 5, Username = "mac" },
                new User { Popularity = 1, Username = "baxter" },
                new User { Popularity = 2, Username = "olive" },
                new User { Popularity = 3, Username = "toast" });

            context.SaveChanges();
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
