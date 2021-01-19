using System;
using System.Linq;

namespace EFModeling.OwnedEntities
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new OwnedEntityContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(
                    new DetailedOrder
                    {
                        Status = OrderStatus.Pending,
                        OrderDetails = new OrderDetails
                        {
                            ShippingAddress = new StreetAddress { City = "London", Street = "221 B Baker St" },
                            BillingAddress = new StreetAddress { City = "New York", Street = "11 Wall Street" }
                        }
                    });

                context.SaveChanges();
            }

            using (var context = new OwnedEntityContext())
            {
                #region DetailedOrderQuery
                var order = context.DetailedOrders.First(o => o.Status == OrderStatus.Pending);
                Console.WriteLine($"First pending order will ship to: {order.OrderDetails.ShippingAddress.City}");
                #endregion
            }
        }
    }
}
