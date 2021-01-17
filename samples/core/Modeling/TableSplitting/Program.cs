using System;
using System.Linq;

namespace EFModeling.TableSplitting
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            #region Usage
            using (var context = new TableSplittingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(
                    new Order
                    {
                        Status = OrderStatus.Pending,
                        DetailedOrder = new DetailedOrder
                        {
                            Status = OrderStatus.Pending,
                            ShippingAddress = "221 B Baker St, London",
                            BillingAddress = "11 Wall Street, New York"
                        }
                    });

                context.SaveChanges();
            }

            using (var context = new TableSplittingContext())
            {
                var pendingCount = context.Orders.Count(o => o.Status == OrderStatus.Pending);
                Console.WriteLine($"Current number of pending orders: {pendingCount}");
            }

            using (var context = new TableSplittingContext())
            {
                var order = context.DetailedOrders.First(o => o.Status == OrderStatus.Pending);
                Console.WriteLine($"First pending order will ship to: {order.ShippingAddress}");
            }
            #endregion
        }
    }
}
