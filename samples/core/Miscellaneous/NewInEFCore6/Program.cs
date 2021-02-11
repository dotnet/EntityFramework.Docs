using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Accessing Tracked Entities_");
        Console.WriteLine();

        EntityTypeConfigurationAttributeSample.Using_EntityTypeConfigurationAttribute();
        UnicodeAttributeSample.Using_UnicodeAttribute();
        PrecisionAttributeSample.Using_PrecisionAttribute();
        ToStringTranslationSample.Using_ToString_in_queries();
        RandomFunctionSample.Call_EF_Functions_Random();
        SparseColumnsSample.Use_sparse_columns();
        InMemoryRequiredPropertiesSample.Required_properties_validated_with_in_memory_database();
        InMemoryRequiredPropertiesSample.Required_property_validation_with_in_memory_database_can_be_disabled();
        SqliteSamples.SavepointsApi();
        IsNullOrWhitespaceSample.Translate_IsNullOrWhitespace();

        // Did not make it for preview 1
        // StringConcatSample.Concat_with_multiple_args();
    }
}
