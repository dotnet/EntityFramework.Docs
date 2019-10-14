using System;
using System.Linq;

namespace EFModeling.OwnedEntities
{
    public static class Program
    {
        static void Main(string[] args)
        {
            using (var context = new OwnedEntityContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(new Order
                {
                    ShippingAddress = new StreetAddress { City = "London", Street = "221 B Baker St" },
                });

                context.SaveChanges();
            }

            using (var context = new OwnedEntityContext())
            {
                #region DetailedOrderQuery
                var order = context.Orders.First();
                Console.WriteLine($"First order will ship to: {order.ShippingAddress.City}");
                #endregion
            }

            using (var context = new OwnedEntityContext())
            {
                // Create a detached order with City changed.
                var order = new Order
                {
                    Id = 1,
                    ShippingAddress = new StreetAddress { City = "Stockholm", Street = "221 B Baker St" },
                };

                // Only update changed properties (not using context.Update(order))
                var orderEntry = context.Set<Order>().Attach(order);
                var orderDbValues = orderEntry.GetDatabaseValues();
                orderEntry.OriginalValues.SetValues(orderDbValues);

                var shippingAddressEntry = context.Entry(order.ShippingAddress);
                var shippingAddressDbValues = shippingAddressEntry.GetDatabaseValues();
                shippingAddressEntry.OriginalValues.SetValues(shippingAddressDbValues);

                context.SaveChanges();
            }
        }
    }
}
