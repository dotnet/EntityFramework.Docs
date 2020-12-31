using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Accessing Tracked Entities_");
        Console.WriteLine();

        Samples.Using_DbContext_Entry_and_EntityEntry_instances_1();
        Samples.Work_with_the_entity_1();
        Samples.Work_with_the_entity_2();
        Samples.Work_with_a_single_property_1();
        Samples.Work_with_a_single_navigation_1();
        Samples.Work_with_a_single_navigation_2();
        Samples.Work_with_all_properties_of_an_entity_1();
        Samples.Work_with_all_navigations_of_an_entity_1();
        Samples.Work_with_all_members_of_an_entity_1();

        Samples.Find_and_FindAsync_1();
        Samples.Find_and_FindAsync_2();

        Samples.Using_ChangeTracker_Entries_to_access_all_tracked_entities_1();

        Samples.Using_DbSet_Local_to_query_tracked_entities_1();
        Samples.Using_DbSet_Local_to_query_tracked_entities_2();
        Samples.Using_DbSet_Local_to_query_tracked_entities_3();
        Samples.Using_DbSet_Local_to_query_tracked_entities_4();
    }
}
