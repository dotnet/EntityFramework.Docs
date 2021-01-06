using System;
using JoinEntity;
using JoinEntityWithPayload;
using JoinEntityWithSkips;
using JoinEntityWithStringPayload;
using Optional;
using Required;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Changing Foreign Keys and Navigations_");
        Console.WriteLine();

        OptionalRelationshipsSamples.Relationship_fixup_1();
        OptionalRelationshipsSamples.Relationship_fixup_2();

        OptionalRelationshipsSamples.Changing_relationships_using_navigations_1();
        OptionalRelationshipsSamples.Changing_relationships_using_navigations_2();

        OptionalRelationshipsSamples.Changing_relationships_using_foreign_key_values_1();

        OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_1();
        OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_2();
        OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_3();
        RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_4();
        RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_5();
        RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_6();
        OptionalRelationshipsSamples.Fixup_for_added_or_deleted_entities_7();
        RequiredRelationshipsSamples.Fixup_for_added_or_deleted_entities_8();

        OptionalRelationshipsSamples.Deleting_an_entity_1();
        RequiredRelationshipsSamples.Deleting_an_entity_2();

        ExplicitJoinEntitySamples.Many_to_many_relationships_1();
        ExplicitJoinEntitySamples.Many_to_many_relationships_2();
        ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_3();
        ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_4();
        ExplicitJoinEntityWithSkipsSamples.Many_to_many_relationships_5();
        OptionalRelationshipsSamples.Many_to_many_relationships_6();
        ExplicitJoinEntityWithPayloadSamples.Many_to_many_relationships_7();
        ExplicitJoinEntityWithStringPayloadSamples.Many_to_many_relationships_8();
        ExplicitJoinEntityWithStringPayloadSamples.Many_to_many_relationships_9();
    }
}
