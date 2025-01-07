using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.DynamicModel;

public class Program
{
    private static async Task Main()
    {
        // Note that because this sample uses InMemory as its provider, each model gets it's own separate store.

        using (var context = new DynamicContext { UseIntProperty = true })
        {
            context.Entities.Add(new ConfigurableEntity { IntProperty = 44, StringProperty = "Aloha" });
            await context.SaveChangesAsync();
        }

        using (var context = new DynamicContext { UseIntProperty = false })
        {
            context.Entities.Add(new ConfigurableEntity { IntProperty = 43, StringProperty = "Hola" });
            await context.SaveChangesAsync();
        }

        using (var context = new DynamicContext { UseIntProperty = true })
        {
            var entity = await context.Entities.SingleAsync();

            // Writes 44 and an empty string
            Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
        }

        using (var context = new DynamicContext { UseIntProperty = false })
        {
            var entity = await context.Entities.SingleAsync();

            // Writes 0 and an "Hola"
            Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
        }
    }
}
