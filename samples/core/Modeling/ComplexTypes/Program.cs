using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.ComplexTypes;

public static class Program
{
    private static async Task Main()
    {
        using (var context = new ComplexTypesContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            #region SaveCustomer
            var customer = new Customer
            {
                Name = "Willow",
                Address = new Address
                {
                    Line1 = "Barking Gate",
                    City = "Walpole St Peter",
                    Country = "UK",
                    PostCode = "PE14 7AV"
                }
            };

            context.Add(customer);
            await context.SaveChangesAsync();
            #endregion

            #region SharedInstance
            // The same Address instance can be assigned to multiple complex properties.
            customer.Orders.Add(
                new Order
                {
                    Contents = "Tesco Tasty Treats",
                    BillingAddress = customer.Address,
                    ShippingAddress = customer.Address
                });

            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new ComplexTypesContext())
        {
            var customer = await context.Customers.FirstAsync(c => c.Name == "Willow");

            #region ChangeImmutableRecord
            // Address is an immutable record, so create a new instance to change a value.
            customer.Address = customer.Address with { Line1 = "Peacock Lodge" };

            await context.SaveChangesAsync();
            #endregion
        }

        using (var context = new ComplexTypesContext())
        {
            var customer = await context.Customers.FirstAsync(c => c.Name == "Willow");

            #region ChangeTracking
            var addressEntry = context.Entry(customer).ComplexProperty(c => c.Address);
            Console.WriteLine($"City is currently: {addressEntry.Property(a => a.City).CurrentValue}");
            Console.WriteLine($"Address was modified: {addressEntry.Property(a => a.City).IsModified}");
            #endregion
        }

        using (var context = new ComplexTypesContext())
        {
            #region QueryComplexMember
            // Filter and project members of a complex property.
            var ukCities = await context.Customers
                .Where(c => c.Address.Country == "UK")
                .Select(c => c.Address.City)
                .ToListAsync();
            #endregion

            Console.WriteLine($"Found {ukCities.Count} customer(s) in the UK.");
        }

        using (var context = new ComplexTypesContext())
        {
            context.Add(
                new Distributor
                {
                    Name = "Acme",
                    ShippingCenters =
                    {
                        new Address { Line1 = "1 Main St", City = "Metropolis", Country = "US", PostCode = "10001" },
                        new Address { Line1 = "2 Side St", City = "Gotham", Country = "US", PostCode = "10002" }
                    }
                });

            await context.SaveChangesAsync();
        }
    }
}
