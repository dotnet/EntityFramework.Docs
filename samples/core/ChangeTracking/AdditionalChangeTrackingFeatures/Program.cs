using System;
using DefaultValues;
using Optional;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Identity Resolution in EF Core_");
        Console.WriteLine();

        Samples.DbContext_versus_DbSet_methods_1();
        Samples.Temporary_values_1();
        Samples.Temporary_values_2();

        DefaultValueSamples.Working_with_default_values_1();
        DefaultValueSamples.Working_with_default_values_2();
        DefaultValueSamples.Working_with_default_values_3();
        DefaultValueSamples.Working_with_default_values_4();
        DefaultValueSamples.Working_with_default_values_5();
    }
}
