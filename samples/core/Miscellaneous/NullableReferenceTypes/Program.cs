﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NullableReferenceTypes
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new NullableReferenceTypesContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Add(new Customer("John", "Doe"));

                context.Add(
                    new Order
                    {
                        ShippingAddress = new Address("London", "Downing"),
                        Product = new Product("Cooking stove"),
                        OptionalInfo = new OptionalOrderInfo("Some additional info")
                        {
                            ExtraAdditionalInfo = new ExtraOptionalOrderInfo("Some extra additional info")
                        }
                    });

                context.SaveChanges();
            }

            using (var context = new NullableReferenceTypesContext())
            {
                var john = context.Customers.First(c => c.FirstName == "John");
                Console.WriteLine("John's last name: " + john.LastName);

                #region Including
                var order = context.Orders
                    .Include(o => o.OptionalInfo!)
                    .ThenInclude(op => op.ExtraAdditionalInfo)
                    .Single();
                #endregion

                // The following would be a programming bug: we forgot to include ShippingAddress above. It would throw InvalidOperationException.
                // Console.WriteLine(order.ShippingAddress.City);
                // The following would be a programming bug: we forgot to include Product above; will throw NullReferenceException. It would throw NullReferenceException.
                // Console.WriteLine(order.Product.Name);
            }
        }
    }
}
