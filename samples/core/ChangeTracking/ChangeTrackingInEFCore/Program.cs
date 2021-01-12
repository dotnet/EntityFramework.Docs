using System;
using ExplicitKeys;
using ExplicitKeysRequired;
using GeneratedKeys;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Change Tracking in EF Core_");
        Console.WriteLine();

        GeneratedKeysSamples.Simple_query_and_update_1();
        GeneratedKeysSamples.Simple_query_and_update_2();
        GeneratedKeysSamples.Query_then_insert_update_and_delete_1();

        ExplicitKeysSamples.Inserting_new_entities_1();
        ExplicitKeysSamples.Inserting_new_entities_2();
        GeneratedKeysSamples.Inserting_new_entities_3();

        ExplicitKeysSamples.Attaching_existing_entities_1();
        ExplicitKeysSamples.Attaching_existing_entities_2();
        GeneratedKeysSamples.Attaching_existing_entities_3();

        ExplicitKeysSamples.Updating_existing_entities_1();
        ExplicitKeysSamples.Updating_existing_entities_2();
        GeneratedKeysSamples.Updating_existing_entities_3();

        ExplicitKeysSamples.Deleting_existing_entities_1();
        ExplicitKeysSamples.Deleting_dependent_child_entities_1();
        ExplicitKeysSamples.Deleting_dependent_child_entities_2();
        ExplicitKeysSamples.Deleting_principal_parent_entities_1();
        ExplicitKeysRequiredSamples.Deleting_principal_parent_entities_1();

        GeneratedKeysSamples.Custom_tracking_with_TrackGraph_1();
    }
}
