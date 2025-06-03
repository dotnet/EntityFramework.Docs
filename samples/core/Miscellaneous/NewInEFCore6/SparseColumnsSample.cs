using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SparseColumnsSample
{
    public static async Task Use_sparse_columns()
    {
        Console.WriteLine($">>>> Sample: {nameof(Use_sparse_columns)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        #region Query
        _ = await context.Users.ToListAsync();
        #endregion

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new BooksContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using var context = new BooksContext();

            context.AddRange(
                new ForumUser { Username = "arthur" },
                new ForumUser { Username = "wendy" },
                new ForumUser { Username = "smokey" },
                new ForumUser { Username = "alice" },
                new ForumUser { Username = "mac" },
                new ForumModerator { Username = "baxter", ForumName = "Viral Cats"},
                new ForumUser { Username = "olive" },
                new ForumUser { Username = "toast" });

            await context.SaveChangesAsync();
        }
    }

    #region UserEntityType
    public class ForumUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

    public class ForumModerator : ForumUser
    {
        public string ForumName { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<ForumUser> Users { get; set; }

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

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ForumModerator>()
                .Property(e => e.ForumName)
                .IsSparse();
        }
        #endregion
    }
}
