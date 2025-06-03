using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.OwnedEntities;

public static class Program
{
    private static async Task Main(string[] args)
    {
        using (var context = new OwnedEntityContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

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

            await context.SaveChangesAsync();
        }

        using (var context = new OwnedEntityContext())
        {
            #region DetailedOrderQuery
            var order = await context.DetailedOrders.FirstAsync(o => o.Status == OrderStatus.Pending);
            Console.WriteLine($"First pending order will ship to: {order.OrderDetails.ShippingAddress.City}");
            #endregion
        }
    }
}
