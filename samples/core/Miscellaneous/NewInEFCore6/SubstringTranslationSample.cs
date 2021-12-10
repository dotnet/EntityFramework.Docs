using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SubstringTranslationSample
{
    public static void Translate_Substring_with_single_parameter()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_Substring_with_single_parameter)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        #region Substring
        var result = context.Customers
            .Select(a => new { Name = a.Name.Substring(3) })
            .FirstOrDefault(a => a.Name == "hur");
        #endregion

        Console.WriteLine();
        Console.WriteLine($"Found {result.Name}.");
        Console.WriteLine();
    }

    public static class Helpers
    {
        public static void RecreateCleanDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void PopulateDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            context.AddRange(
                new Customer
                {
                    Name = "Arthur"
                },
                new Customer
                {
                    Name = "Alan"
                },
                new Customer
                {
                    Name = "Andrew"
                });

            context.SaveChanges();
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomersContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        private readonly bool _quiet;

        public CustomersContext(bool quiet = false)
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
