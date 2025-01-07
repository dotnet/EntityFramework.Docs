using System;
using System.Threading.Tasks;
using ExplicitKeys;
using ExplicitKeysRequired;
using GeneratedKeys;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Change Tracking in EF Core_");
        Console.WriteLine();

        await GeneratedKeysSamples.Simple_query_and_update_1();
        await GeneratedKeysSamples.Simple_query_and_update_2();
        await GeneratedKeysSamples.Query_then_insert_update_and_delete_1();

        await ExplicitKeysSamples.Inserting_new_entities_1();
        await ExplicitKeysSamples.Inserting_new_entities_2();
        await GeneratedKeysSamples.Inserting_new_entities_3();

        await ExplicitKeysSamples.Attaching_existing_entities_1();
        await ExplicitKeysSamples.Attaching_existing_entities_2();
        await GeneratedKeysSamples.Attaching_existing_entities_3();

        await ExplicitKeysSamples.Updating_existing_entities_1();
        await ExplicitKeysSamples.Updating_existing_entities_2();
        await GeneratedKeysSamples.Updating_existing_entities_3();

        await ExplicitKeysSamples.Deleting_existing_entities_1();
        await ExplicitKeysSamples.Deleting_dependent_child_entities_1();
        await ExplicitKeysSamples.Deleting_dependent_child_entities_2();
        await ExplicitKeysSamples.Deleting_principal_parent_entities_1();
        await ExplicitKeysRequiredSamples.Deleting_principal_parent_entities_1();

        await GeneratedKeysSamples.Custom_tracking_with_TrackGraph_1();
    }
}
