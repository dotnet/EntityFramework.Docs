using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class TagWithFileAndLineSample
{
    public static void Queries_can_be_tagged_with_filename_and_line_number()
    {
        Console.WriteLine($">>>> Sample: {nameof(Queries_can_be_tagged_with_filename_and_line_number)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        #region TagWithCallSite
        var results1 = context
            .Customers
            .TagWithCallSite()
            .Where(c => c.Name.StartsWith("A"))
            .ToList();
        #endregion

        Console.WriteLine();

        var results2 = context
            .Customers
            .OrderBy(c => c.Name)
            .TagWith("Ordering query")
            .TagWithCallSite()
            .ToList();

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
                    Name = "Arthur",
                },
                new Customer
                {
                    Name = "Alan",
                },
                new Customer
                {
                    Name = "Andrew",
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
