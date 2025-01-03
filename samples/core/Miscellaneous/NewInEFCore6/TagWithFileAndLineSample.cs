using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class TagWithFileAndLineSample
{
    public static async Task Queries_can_be_tagged_with_filename_and_line_number()
    {
        Console.WriteLine($">>>> Sample: {nameof(Queries_can_be_tagged_with_filename_and_line_number)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        #region TagWithCallSite
        var results1 = await context
            .Customers
            .TagWithCallSite()
            .Where(c => c.Name.StartsWith("A"))
            .ToListAsync();
        #endregion

        Console.WriteLine();

        var results2 = await context
            .Customers
            .OrderBy(c => c.Name)
            .TagWith("Ordering query")
            .TagWithCallSite()
            .ToListAsync();

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
                },
                new Customer
                {
                    Name = "Alan",
                },
                new Customer
                {
                    Name = "Andrew",
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
