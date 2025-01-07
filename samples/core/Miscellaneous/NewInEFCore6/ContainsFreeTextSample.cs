using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ContainsFreeTextSample
{
    public static async Task Contains_with_non_string()
    {
        Console.WriteLine($">>>> Sample: {nameof(Contains_with_non_string)}");
        Console.WriteLine($">>>> Note: does not work with SQL Server LocalDb");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomerContext();

        #region Query
        var result = await context.Customers.Where(e => EF.Functions.Contains(e.Name, "Martin")).ToListAsync();
        #endregion

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new CustomerContext(quiet: true);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.Database.ExecuteSqlRawAsync(
                @"CREATE FULLTEXT CATALOG CustomersAndNames AS DEFAULT;
    CREATE FULLTEXT INDEX ON Customers (Name) KEY INDEX IX_Names;");
        }

        public static async Task PopulateDatabase()
        {
            using var context = new CustomerContext(quiet: true);

            context.AddRange(
                new Customer
                {
                    Name = new Name
                    {
                        First = "Arthur",
                        MiddleInitial = "J",
                        Last = "Vickers"
                    }
                },
                new Customer
                {
                    Name = new Name
                    {
                        First = "Wendy",
                        MiddleInitial = "F",
                        Last = "Vickers"
                    }
                },
                new Customer
                {
                    Name = new Name
                    {
                        First = "George",
                        Last = "Martin"
                    }
                },
                new Customer
                {
                    Name = new Name
                    {
                        First = "Martin",
                        Last = "Clunes"
                    }
                });

            await context.SaveChangesAsync();
        }
    }

    #region EntityType
    public class Customer
    {
        public int Id { get; set; }

        public Name Name{ get; set; }
    }

    public class Name
    {
        public string First { get; set; }
        public string MiddleInitial { get; set; }
        public string Last { get; set; }
    }
    #endregion

    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        private readonly bool _quiet;

        public CustomerContext(bool quiet = false)
        {
            _quiet = quiet;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureCompositeValueObject
            modelBuilder.Entity<Customer>()
                .Property(e => e.Name)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Name>(v, (JsonSerializerOptions)null));
            #endregion
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
