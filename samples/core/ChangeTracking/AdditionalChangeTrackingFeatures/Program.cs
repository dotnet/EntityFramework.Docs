using System;
using System.Threading.Tasks;
using DefaultValues;
using Optional;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Identity Resolution in EF Core_");
        Console.WriteLine();

        await Samples.DbContext_versus_DbSet_methods_1();
        await Samples.Temporary_values_1();
        await Samples.Temporary_values_2();

        await DefaultValueSamples.Working_with_default_values_1();
        await DefaultValueSamples.Working_with_default_values_2();
        await DefaultValueSamples.Working_with_default_values_3();
        await DefaultValueSamples.Working_with_default_values_4();
        await DefaultValueSamples.Working_with_default_values_5();
    }
}
