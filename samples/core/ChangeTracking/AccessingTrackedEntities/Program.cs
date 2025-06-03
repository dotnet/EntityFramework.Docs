using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Accessing Tracked Entities_");
        Console.WriteLine();

        await Samples.Using_DbContext_Entry_and_EntityEntry_instances_1();
        await Samples.Work_with_the_entity_1();
        await Samples.Work_with_the_entity_2();
        await Samples.Work_with_a_single_property_1();
        await Samples.Work_with_a_single_navigation_1();
        await Samples.Work_with_a_single_navigation_2();
        await Samples.Work_with_all_properties_of_an_entity_1();
        await Samples.Work_with_all_navigations_of_an_entity_1();
        await Samples.Work_with_all_members_of_an_entity_1();

        await Samples.Find_and_FindAsync_1();
        await Samples.Find_and_FindAsync_2();

        await Samples.Using_ChangeTracker_Entries_to_access_all_tracked_entities_1();

        await Samples.Using_DbSet_Local_to_query_tracked_entities_1();
        await Samples.Using_DbSet_Local_to_query_tracked_entities_2();
        await Samples.Using_DbSet_Local_to_query_tracked_entities_3();
        await Samples.Using_DbSet_Local_to_query_tracked_entities_4();
    }
}
