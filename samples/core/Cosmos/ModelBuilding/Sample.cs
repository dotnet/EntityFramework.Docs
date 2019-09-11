using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.ModelBuilding
{
    public static class Sample
    {
        public static async Task Run()
        {
            Console.WriteLine();
            Console.WriteLine("Getting started with Cosmos:");
            Console.WriteLine();

            #region HelloCosmos
            var londonOrder = new Order
            {
                Id = 1,
                ShippingAddress = new StreetAddress { City = "London", Street = "221 B Baker St" }
            };

            using (var context = new OrderContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(londonOrder);

                await context.SaveChangesAsync();
            }

            using (var context = new OrderContext())
            {
                var order = await context.Orders.FirstAsync();
                Console.WriteLine($"First order will ship to: {order.ShippingAddress.Street}, {order.ShippingAddress.City}");
            }
            #endregion

            #region PartitionKey
            //using (var context = new OrderContext())
            //{
            //    context.Add(new Order
            //    {
            //        Id = 2,
            //        ShippingAddress = new StreetAddress { City = "New York", Street = "11 Wall Street" },
            //        PartitionKey = "1"
            //    });

            //    context.SaveChangesAsync();
            //}

            //using (var context = new OrderContext())
            //{
            //    var order = context.Orders.LastAsync();
            //    Console.WriteLine($"Last order will ship to: {order.ShippingAddress.Street}, {order.ShippingAddress.City}");
            //}
            #endregion

            #region OwnedCollection
            using (var context = new OrderContext())
            {
                context.Add(new Distributor
                {
                    Id = 1,
                    ShippingCenters = new HashSet<StreetAddress> {
                        new StreetAddress { City = "Phoenix", Street = "500 S 48th Street" },
                        new StreetAddress { City = "Anaheim", Street = "5650 Dolly Ave" }
                    }
                });

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

                Console.WriteLine($"First shipping center PK: ({addressEntry.Property(addressPKProperties[0].Name).CurrentValue}, {addressEntry.Property(addressPKProperties[1].Name).CurrentValue})");
            }
            #endregion

            #region Attach
            using (var context = new OrderContext())
            {
                var orderEntry = context.Add(londonOrder);
                orderEntry.State = EntityState.Unchanged;

                londonOrder.ShippingAddress.Street = "3 Abbey Road";

                await context.SaveChangesAsync();
            }

            using (var context = new OrderContext())
            {
                var order = await context.Orders.FirstAsync();
                Console.WriteLine($"First order will now ship to: {order.ShippingAddress.Street}, {order.ShippingAddress.City}");

                var orderEntry = context.Entry(order);
                var idProperty = orderEntry.Property<string>("id");
                Console.WriteLine($"The order 'id' is: {idProperty.CurrentValue}");
            }
            #endregion
        }
    }
}
