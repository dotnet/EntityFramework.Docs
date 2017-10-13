using System;
using System.Linq;

namespace EFModeling.Samples.DynamicModel
{
    class Program
    {
        static void Main()
        {
            using (var context = new DynamicContext())
            {
                context.Entities.Add(new ConfigurableEntity {IntProperty = 42, StringProperty = "Hello"});
                context.SaveChanges();
            }

            using (var context = new DynamicContext {IgnoreIntProperty = false})
            {
                context.Entities.Add(new ConfigurableEntity {IntProperty = 43, StringProperty = "Hola"});
                context.SaveChanges();
            }

            using (var context = new DynamicContext {IgnoreIntProperty = true})
            {
                context.Entities.Add(new ConfigurableEntity {IntProperty = 44, StringProperty = "Aloha"});
                context.SaveChanges();
            }

            using (var context = new DynamicContext())
            {
                var entity = context.Entities.Single();

                Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
            }

            using (var context = new DynamicContext {IgnoreIntProperty = false})
            {
                var entity = context.Entities.Single();

                Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
            }

            using (var context = new DynamicContext {IgnoreIntProperty = true})
            {
                var entity = context.Entities.Single();

                Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
            }
        }
    }
}
