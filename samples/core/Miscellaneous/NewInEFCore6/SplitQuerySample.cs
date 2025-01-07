using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class SplitQuerySample
{
    public static async Task Split_query_for_non_navigation_collections()
    {
        Console.WriteLine($">>>> Sample: {nameof(Split_query_for_non_navigation_collections)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        Console.WriteLine("LINQ query: 'context.Customers.Select(c => new { c, Orders = c.Orders.Where(o => o.Id > 1) })'");
        Console.WriteLine();
        Console.WriteLine("Executed as a single query:");

        #region SplitQuery1
        await context.Customers
            .Select(
                c => new
                {
                    c,
                    Orders = c.Orders
                        .Where(o => o.Id > 1)
                })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Executed as split queries:");
        await context.Customers.AsSplitQuery().Select(c => new { c, Orders = c.Orders.Where(o => o.Id > 1) }).ToListAsync();

        Console.WriteLine();
        Console.WriteLine("LINQ query: 'context.Customers.Select(c => new { c, OrderDates = c.Orders.Where(o => o.Id > 1).Select(o => o.OrderDate) })'");
        Console.WriteLine();
        Console.WriteLine("Executed as a single query:");

        #region SplitQuery2
        await context.Customers
            .Select(
                c => new
                {
                    c,
                    OrderDates = c.Orders
                        .Where(o => o.Id > 1)
                        .Select(o => o.OrderDate)
                })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Executed as split queries:");
        await context.Customers.AsSplitQuery().Select(c => new { c, OrderDates = c.Orders.Where(o => o.Id > 1).Select(o => o.OrderDate) }).ToListAsync();

        Console.WriteLine();
        Console.WriteLine("LINQ query: 'context.Customers.Select(c => new { c, OrderDates = c.Orders.Where(o => o.Id > 1).Select(o => o.OrderDate).Distinct() })'");
        Console.WriteLine();
        Console.WriteLine("Executed as a single query:");

        #region SplitQuery3
        await context.Customers
            .Select(
                c => new
                {
                    c,
                    OrderDates = c.Orders
                        .Where(o => o.Id > 1)
                        .Select(o => o.OrderDate)
                        .Distinct()
                })
            .ToListAsync();
        #endregion

        Console.WriteLine();
        Console.WriteLine("Executed as split queries:");
        await context.Customers.AsSplitQuery().Select(c => new { c, OrderDates = c.Orders.Where(o => o.Id > 1).Select(o => o.OrderDate).Distinct() }).ToListAsync();

        Console.WriteLine();
    }

    public static async Task Last_column_in_ORDER_BY_removed_when_joining_for_collection()
    {
        Console.WriteLine($">>>> Sample: {nameof(Last_column_in_ORDER_BY_removed_when_joining_for_collection)}");
        Console.WriteLine();

        await Helpers.RecreateCleanDatabase();
        await Helpers.PopulateDatabase();

        using var context = new CustomersContext();

        #region OrderBy
        await context.Customers
            .Select(
                e => new
                {
                    e.Id,
                    FirstOrder = e.Orders.Where(i => i.Id == 1).ToList()
                })
            .ToListAsync();
        #endregion

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
                    Orders =
                    {
                        new()
                        {
                            OrderDate = new DateTime(2021, 7, 31)
                        },
                        new()
                        {
                            OrderDate = new DateTime(2021, 8, 1)
                        },
                        new()
                        {
                            OrderDate = new DateTime(2021, 8, 2)
                        }
                    }
                });

            await context.SaveChangesAsync();
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public ICollection<Order> Orders { get; } = new List<Order>();
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
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
