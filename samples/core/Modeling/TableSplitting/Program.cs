using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.TableSplitting;

public static class Program
{
    private static async Task Main(string[] args)
    {
        #region Usage
        using (var context = new TableSplittingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

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

            await context.SaveChangesAsync();
        }

        using (var context = new TableSplittingContext())
        {
            var pendingCount = await context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            Console.WriteLine($"Current number of pending orders: {pendingCount}");
        }

        using (var context = new TableSplittingContext())
        {
            var order = await context.DetailedOrders.FirstAsync(o => o.Status == OrderStatus.Pending);
            Console.WriteLine($"First pending order will ship to: {order.ShippingAddress}");
        }
        #endregion
    }
}
