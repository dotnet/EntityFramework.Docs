using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SubstringTranslationSample
{
    public static async Task Translate_Substring_with_single_parameter()
    {
        Console.WriteLine($">>>> Sample: {nameof(Translate_Substring_with_single_parameter)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        #region Substring
        var result = await context.Customers
            .Select(a => new { Name = a.Name.Substring(3) })
            .FirstOrDefaultAsync(a => a.Name == "hur");
        #endregion

        Console.WriteLine();
        Console.WriteLine($"Found {result.Name}.");
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

            await context.SaveChangesAsync();
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
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample;ConnectRetryCount=0");

            if (!_quiet)
            {
                optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            }
        }
    }
}
