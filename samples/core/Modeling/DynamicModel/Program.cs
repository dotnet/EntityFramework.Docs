using System;
using System.Linq;

namespace EFModeling.DynamicModel
{
    public class Program
    {
        private static void Main()
        {
            // Note that because this sample uses InMemory as its provider, each model gets it's own separate store.

            using (var context = new DynamicContext { UseIntProperty = true })
            {
                context.Entities.Add(new ConfigurableEntity { IntProperty = 44, StringProperty = "Aloha" });
                context.SaveChanges();
            }

            using (var context = new DynamicContext { UseIntProperty = false })
            {
                context.Entities.Add(new ConfigurableEntity { IntProperty = 43, StringProperty = "Hola" });
                context.SaveChanges();
            }

            using (var context = new DynamicContext { UseIntProperty = true })
            {
                var entity = context.Entities.Single();

                // Writes 44 and an empty string
                Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
            }

            using (var context = new DynamicContext { UseIntProperty = false })
            {
                var entity = context.Entities.Single();

                // Writes 0 and an "Hola"
                Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
            }
        }
    }
}
