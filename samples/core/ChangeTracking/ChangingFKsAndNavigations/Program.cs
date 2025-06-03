using System;
using System.Threading.Tasks;
using JoinEntity;
using JoinEntityWithPayload;
using JoinEntityWithSkips;
using JoinEntityWithStringPayload;
using Optional;
using Required;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Changing Foreign Keys and Navigations_");
        Console.WriteLine();

        await OptionalRelationshipsSamples.Relationship_fixup_1();
        await OptionalRelationshipsSamples.Relationship_fixup_2();

        await OptionalRelationshipsSamples.Changing_relationships_using_navigations_1();
        await OptionalRelationshipsSamples.Changing_relationships_using_navigations_2();

        await OptionalRelationshipsSamples.Changing_relationships_using_foreign_key_values_1();

        await OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_1();
        await OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_2();
        await OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_3();
        await RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_4();
        await RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_5();
        await RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_6();
        await OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_7();
        await RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_8();

        await OptionalRelationshipsSamples.Deleting_an_entity_1();
        await RequiredRelationshipsSamples.Deleting_an_entity_2();

        await ExplicitJoinEntitySamples.Many_to_many_relationships_1();
        await ExplicitJoinEntitySamples.Many_to_many_relationships_2();
        await ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_3();
        await ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_4();
        await ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_5();
        await OptionalRelationshipsSamples.Many_to_many_relationships_6();
        await ExplicitJoinEntityWithPayloadSamples.Many_to_many_relationships_7();
        await ExplicitJoinEntityWithStringPayloadSamples.Many_to_many_relationships_8();
        await ExplicitJoinEntityWithStringPayloadSamples.Many_to_many_relationships_9();
    }
}
