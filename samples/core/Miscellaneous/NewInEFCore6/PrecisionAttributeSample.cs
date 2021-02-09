using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class PrecisionAttributeSample
{
    public static void Using_PrecisionAttribute()
    {
        Console.WriteLine($">>>> Sample: {nameof(Using_PrecisionAttribute)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new BooksContext();

        _ = context.Products.Single(e => e.Id == 1);

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new BooksContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new BooksContext();

            context.Add(new Product { Price = 3.99m });

            context.SaveChanges();
        }
    }

    #region ProductEntityType
    public class Product
    {
        public int Id { get; set; }

        [Precision(precision: 10, scale: 2)]
        public decimal Price { get; set; }
    }
    #endregion

    public class BooksContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

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
