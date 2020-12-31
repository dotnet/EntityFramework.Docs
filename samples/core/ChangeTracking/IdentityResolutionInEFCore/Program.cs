using System;
using Graphs;
using Updates;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Identity Resolution in EF Core_");
        Console.WriteLine();

        IdentityResolutionSamples.Identity_Resolution_in_EF_Core_1();
        IdentityResolutionSamples.Updating_an_entity_1();
        IdentityResolutionSamples.Updating_an_entity_2();
        IdentityResolutionSamples.Updating_an_entity_3();
        IdentityResolutionSamples.Updating_an_entity_4();
        IdentityResolutionSamples.Updating_an_entity_5();
        IdentityResolutionSamples.Updating_an_entity_6();

        SerializedGraphExamples.Attaching_a_serialized_graph_1();
        SerializedGraphExamples.Attaching_a_serialized_graph_2();
        SerializedGraphExamples.Attaching_a_serialized_graph_3();
        SerializedGraphExamples.Attaching_a_serialized_graph_4();
        SerializedGraphExamples.Attaching_a_serialized_graph_5();

        IdentityResolutionSamples.Failing_to_set_key_values_1();
    }
}
