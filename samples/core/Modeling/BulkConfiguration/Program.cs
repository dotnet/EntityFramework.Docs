using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.BulkConfiguration;

internal class Program
{
    private static async Task Main()
    {
        Console.WriteLine("Samples showing bulk configuration of the model");
        Console.WriteLine();

        using (var context = new MetadataAPIContext())
        {
            RoundtripValue(context);
        }

        using (var context = new PreConventionContext())
        {
            RoundtripValue(context);
        }

        await ModelBuildingConventionsSample.No_foreign_key_index_convention();
        await ModelBuildingConventionsSample.Discriminator_length_convention();
        await ModelBuildingConventionsSample.Max_string_length_convention();
        await ModelBuildingConventionsSample.Map_members_explicitly_by_attribute_convention();

        Console.WriteLine();
        Console.WriteLine("Sample finished.");
    }

    private static void RoundtripValue(DbContext context)
    {
        Console.WriteLine("Using " + context.GetType().Name);
        Console.WriteLine("Deleting and re-creating database...");
        Console.WriteLine();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        Console.WriteLine();
        Console.WriteLine("Done. Database is clean and fresh.");

        var product = new Product { Price = new Currency(3.99m) };
        context.Add(product);

        Console.WriteLine("Save a new product with price: " + product.Price.Amount);
        Console.WriteLine();

        context.SaveChanges();

        Console.WriteLine();
        Console.WriteLine("Read the entity back with price: " + context.Set<Product>().Single().Price.Amount);
        Console.WriteLine();
    }
}
