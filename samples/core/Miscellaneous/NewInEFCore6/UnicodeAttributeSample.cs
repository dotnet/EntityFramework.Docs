using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class UnicodeAttributeSample
{
    public static async Task Using_UnicodeAttribute()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_UnicodeAttribute)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new BooksContext();

        var book = await context.Books.SingleAsync(e => e.Id == 1);

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

            context.Add(
                new Book() { Isbn = "978-0-39-481823-8", Title = "What Do People Do All Day?" });

            await context.SaveChangesAsync();
        }
    }

    #region BookEntityType
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [Unicode(false)]
        [MaxLength(22)]
        public string Isbn { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

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
