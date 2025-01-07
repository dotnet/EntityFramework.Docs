using System;
using System.Threading.Tasks;
using Graphs;
using Updates;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Identity Resolution in EF Core_");
        Console.WriteLine();

        await IdentityResolutionSamples.Identity_Resolution_in_EF_Core_1();
        await IdentityResolutionSamples.Updating_an_entity_1();
        await IdentityResolutionSamples.Updating_an_entity_2();
        await IdentityResolutionSamples.Updating_an_entity_3();
        await IdentityResolutionSamples.Updating_an_entity_4();
        await IdentityResolutionSamples.Updating_an_entity_5();
        await IdentityResolutionSamples.Updating_an_entity_6();

        await SerializedGraphExamples.Attaching_a_serialized_graph_1();
        await SerializedGraphExamples.Attaching_a_serialized_graph_2();
        await SerializedGraphExamples.Attaching_a_serialized_graph_3();
        await SerializedGraphExamples.Attaching_a_serialized_graph_4();
        await SerializedGraphExamples.Attaching_a_serialized_graph_5();

        IdentityResolutionSamples.Failing_to_set_key_values_1();
    }
}
