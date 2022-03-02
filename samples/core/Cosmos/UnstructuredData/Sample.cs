using System;
using System.Linq;
using System.Threading.Tasks;
using Cosmos.ModelBuilding;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Cosmos.UnstructuredData;

public static class Sample
{
    public static async Task Run()
    {
        Console.WriteLine();
        Console.WriteLine("Unstructured data:");
        Console.WriteLine();

        #region Unmapped
        using (var context = new OrderContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var order = new Order
            {
                Id = 1, ShippingAddress = new StreetAddress { City = "London", Street = "221 B Baker St" }, PartitionKey = "1"
            };

            context.Add(order);

            await context.SaveChangesAsync();
        }

        using (var context = new OrderContext())
        {
            var order = await context.Orders.FirstAsync();
            var orderEntry = context.Entry(order);

            var jsonProperty = orderEntry.Property<JObject>("__jObject");
            jsonProperty.CurrentValue["BillingAddress"] = "Clarence House";

            orderEntry.State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        using (var context = new OrderContext())
        {
            var order = await context.Orders.FirstAsync();
            var orderEntry = context.Entry(order);
            var jsonProperty = orderEntry.Property<JObject>("__jObject");

            Console.WriteLine($"First order will be billed to: {jsonProperty.CurrentValue["BillingAddress"]}");
        }
        #endregion

        #region CosmosClient
        using (var context = new OrderContext())
        {
            var cosmosClient = context.Database.GetCosmosClient();
            var database = cosmosClient.GetDatabase("OrdersDB");
            var container = database.GetContainer("Orders");

            var resultSet = container.GetItemQueryIterator<JObject>(new QueryDefinition("select * from o"));
            var order = (await resultSet.ReadNextAsync()).First();

            Console.WriteLine($"First order JSON: {order}");

            order.Remove("TrackingNumber");

            await container.ReplaceItemAsync(order, order["id"].ToString());
        }
        #endregion

        #region MissingProperties
        using (var context = new OrderContext())
        {
            var orders = await context.Orders.ToListAsync();
            var sortedOrders = await context.Orders.OrderBy(o => o.TrackingNumber).ToListAsync();

            Console.WriteLine($"Number of orders: {orders.Count}");
            Console.WriteLine($"Number of sorted orders: {sortedOrders.Count}");
        }
        #endregion
    }
}