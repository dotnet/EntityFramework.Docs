using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public static class ToInMemoryQuerySample
{
    public static async Task Can_query_keyless_types_from_in_memory_database()
    {
        Console.WriteLine($">>>> Sample: {nameof(Can_query_keyless_types_from_in_memory_database)}");
        Console.WriteLine();

        await Helpers.PopulateDatabase();

        using var context = new CustomerContext();

        #region Query
        var results = await context.CustomerDensities.ToListAsync();
        #endregion

        foreach (var customerDensity in results)
        {
            Console.WriteLine($"Postcodes in {customerDensity.Postcode} have {customerDensity.CustomerCount} customers.");
        }

        Console.WriteLine();
    }

    public static class Helpers
    {
        public static async Task PopulateDatabase()
        {
            using var context = new CustomerContext();

            context.AddRange(
                new Customer
                {
                    Name = "Gimick",
                    Address = new()
                    {
                        House = "4c",
                        Street = "Cable Street",
                        City = "Ankh",
                        Postcode = "AN1 5CS",
                    }
                },
                new Customer
                {
                    Name = "Extremelia Mume",
                    Address = new()
                    {
                        House = "The Temple of Anoia",
                        Street = "Cable Street",
                        City = "Ankh",
                        Postcode = "AN1 3CS",
                    }
                },
                new Customer
                {
                    Name = "Gimlet Gimlet",
                    Address = new()
                    {
                        House = "Gimlet's Hole Food Delicatessen",
                        Street = "Cable Street",
                        City = "Ankh",
                        Postcode = "AN1 9CD",
                    }
                },
                new Customer
                {
                    Name = "Agnes Nitt",
                    Address = new()
                    {
                        House = "4",
                        Street = "Treacle Mine Road",
                        City = "Morpork",
                        Postcode = "MK3 1TM",
                    }
                },
                new Customer
                {
                    Name = "Kroll Thighbiter",
                    Address = new()
                    {
                        House = "Kroll Thighbiter’s Rat Pie & Chips",
                        Street = "Treacle Mine Road",
                        City = "Morpork",
                        Postcode = "MK3 8RP",
                    }
                },
                new Customer
                {
                    Name = "John Keel",
                    Address = new()
                    {
                        House = "Watch House",
                        Street = "Treacle Mine Road",
                        City = "Morpork",
                        Postcode = "MK3 8WH",
                    }
                },
                new Customer
                {
                    Name = "Albert Spangler",
                    Address = new()
                    {
                        House = "Post Office",
                        Street = "Treacle Mine Road",
                        City = "Morpork",
                        Postcode = "MK3 2ND",
                    }
                });
            await context.SaveChangesAsync();
        }
    }

    #region ViewType
    public class CustomerDensity
    {
        public string Postcode { get; set; }
        public int CustomerCount { get; set; }
    }
    #endregion

    #region EntityTypes
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string House { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }
    #endregion

    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(nameof(CustomerContext));
        }

        #region ToInMemoryQuery
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CustomerDensity>()
                .HasNoKey()
                .ToInMemoryQuery(
                    () => Customers
                        .GroupBy(c => c.Address.Postcode.Substring(0, 3))
                        .Select(
                            g =>
                                new CustomerDensity
                                {
                                    Postcode = g.Key,
                                    CustomerCount = g.Count()
                                }));
        }
        #endregion

        #region DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerDensity> CustomerDensities { get; set; }
        #endregion
    }
}
