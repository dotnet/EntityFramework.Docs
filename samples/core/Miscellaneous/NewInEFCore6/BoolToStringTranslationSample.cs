using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class BoolToStringTranslationSample
{
    public static async Task Translate_bool_to_string_on_SQL_Server()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_bool_to_string_on_SQL_Server)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        var results1 = await context.Customers.Select(c => new { c.Name, IsActive = c.IsActive.ToString() }).ToListAsync();

        Console.WriteLine();

        foreach (var customer in results1)
        {
            Console.WriteLine($"{customer.Name} is active: {customer.IsActive}");
        }

        Console.WriteLine();

        var results2 = await context.Customers.Where(b => b.IsActive.ToString() == "true").ToListAsync();

        Console.WriteLine();

        foreach (var customer in results2)
        {
            Console.WriteLine($"{customer.Name} is active: {customer.IsActive}");
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new CustomersContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
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

            await context.SaveChangesAsync();
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
