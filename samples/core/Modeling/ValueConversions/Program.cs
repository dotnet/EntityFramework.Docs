using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.ValueConversions;

/// <summary>
///     Samples for value conversions and comparisons.
/// </summary>
public class Program
{
    public static async Task Main()
    {
        await new MappingImmutableClassProperty().Run();
        await new MappingImmutableStructProperty().Run();
        await new MappingListProperty().Run();
        await new MappingListPropertyOld().Run();
        await new OverridingByteArrayComparisons().Run();
        await new EnumToStringConversions().Run();
        await new KeyValueObjects().Run();
        await new SimpleValueObject().Run();
        await new CompositeValueObject().Run();
        await new PrimitiveCollection().Run();
        await new ValueObjectCollection().Run();
        await new ULongConcurrency().Run();
        await new PreserveDateTimeKind().Run();
        await new CaseInsensitiveStrings().Run();
        await new FixedLengthStrings().Run();
        await new EncryptPropertyValues().Run();
        await new WithMappingHints().Run();
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

    protected static async Task CleanDatabase(DbContext context)
    {
        ConsoleWriteLines("Deleting and re-creating database...");
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        ConsoleWriteLines("Done. Database is clean and fresh.");
    }
}
