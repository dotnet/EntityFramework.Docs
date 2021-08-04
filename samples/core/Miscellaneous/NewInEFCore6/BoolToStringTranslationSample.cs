using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class BoolToStringTranslationSample
{
    public static void Translate_bool_to_string_on_SQL_Server()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_bool_to_string_on_SQL_Server)}");
        Console.WriteLine();

        Helpers.RecreateCleanDatabase();
        Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        var results1 = context.Customers.Select(c => new { c.Name, IsActive = c.IsActive.ToString() }).ToList();

        Console.WriteLine();

        foreach (var customer in results1)
        {
            Console.WriteLine($"{customer.Name} is active: {customer.IsActive}");
        }

        Console.WriteLine();

        var results2 = context.Customers.Where(b => b.IsActive.ToString() == "true").ToList();

        Console.WriteLine();

        foreach (var customer in results2)
        {
            Console.WriteLine($"{customer.Name} is active: {customer.IsActive}");
        }

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
                    IsActive = true
                },
                new Customer
                {
                    Name = "Alan",
                    IsActive = false
                },
                new Customer
                {
                    Name = "Andrew",
                    IsActive = true
                });

            context.SaveChanges();
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
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
