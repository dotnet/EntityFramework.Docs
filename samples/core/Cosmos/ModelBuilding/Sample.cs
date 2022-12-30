using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cosmos.ModelBuilding;

public static class Sample
{
    public static async Task Run()
    {
        Console.WriteLine();
        Console.WriteLine("Getting started with Cosmos:");
        Console.WriteLine();

        #region HelloCosmos
        using (var context = new OrderContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Add(
                new Order
                {
                    Id = 1, ShippingAddress = new StreetAddress { City = "London", Street = "221 B Baker St" }, PartitionKey = "1"
                });

            await context.SaveChangesAsync();
        }

        using (var context = new OrderContext())
        {
            var order = await context.Orders.FirstAsync();
            Console.WriteLine($"First order will ship to: {order.ShippingAddress.Street}, {order.ShippingAddress.City}");
            Console.WriteLine();
        }
        #endregion

        #region PartitionKey
        using (var context = new OrderContext())
        {
            context.Add(
                new Order
                {
                    Id = 2, ShippingAddress = new StreetAddress { City = "New York", Street = "11 Wall Street" }, PartitionKey = "2"
                });

            await context.SaveChangesAsync();
        }

        using (var context = new OrderContext())
        {
            var order = await context.Orders.WithPartitionKey("2").LastAsync();
            Console.WriteLine($"Last order will ship to: {order.ShippingAddress.Street}, {order.ShippingAddress.City}");
            Console.WriteLine();
        }
        #endregion

        #region OwnedCollection
        var distributor = new Distributor
        {
            Id = 1,
            ShippingCenters = new HashSet<StreetAddress>
            {
                new StreetAddress { City = "Phoenix", Street = "500 S 48th Street" },
                new StreetAddress { City = "Anaheim", Street = "5650 Dolly Ave" }
            }
        };

        using (var context = new OrderContext())
        {
            context.Add(distributor);

            await context.SaveChangesAsync();
        }
        #endregion

        #region ImpliedProperties
        using (var context = new OrderContext())
        {
            var firstDistributor = await context.Distributors.FirstAsync();
            Console.WriteLine($"Number of shipping centers: {firstDistributor.ShippingCenters.Count}");

            var addressEntry = context.Entry(firstDistributor.ShippingCenters.First());
            var addressPKProperties = addressEntry.Metadata.FindPrimaryKey().Properties;

            Console.WriteLine(
                $"First shipping center PK: ({addressEntry.Property(addressPKProperties[0].Name).CurrentValue}, {addressEntry.Property(addressPKProperties[1].Name).CurrentValue})");
            Console.WriteLine();
        }
        #endregion

        #region Attach
        using (var context = new OrderContext())
        {
            var distributorEntry = context.Add(distributor);
            distributorEntry.State = EntityState.Unchanged;

            distributor.ShippingCenters.Remove(distributor.ShippingCenters.Last());

            await context.SaveChangesAsync();
        }

        using (var context = new OrderContext())
        {
            var firstDistributor = await context.Distributors.FirstAsync();
            Console.WriteLine($"Number of shipping centers is now: {firstDistributor.ShippingCenters.Count}");

            var distributorEntry = context.Entry(firstDistributor);
            var idProperty = distributorEntry.Property<string>("__id");
            Console.WriteLine($"The distributor 'id' is: {idProperty.CurrentValue}");
        }
        #endregion
    }
}