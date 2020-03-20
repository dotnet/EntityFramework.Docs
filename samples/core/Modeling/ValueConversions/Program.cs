using System;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.ValueConversions
{
    /// <summary>
    /// Samples for value conversions and comparisons.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            new MappingImmutableClassProperty().Run();
            new MappingImmutableStructProperty().Run();
            new MappingListProperty().Run();
            new OverridingByteArrayComparisons().Run();
        }

        protected static void ConsoleWriteLines(params string[] values)
        {
            Console.WriteLine();
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine();
        }

        protected static void CleanDatabase(DbContext context)
        {
            ConsoleWriteLines("Deleting and re-creating database...");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            ConsoleWriteLines("Done. Database is clean and fresh.");
        }
    }
}
