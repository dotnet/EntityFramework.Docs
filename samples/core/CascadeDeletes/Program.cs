using System;
using DatabaseCycles;
using IntroOptional;
using IntroRequired;
using Optional;
using Required;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Cascade Delete_");
        Console.WriteLine();

        IntroRequiredSamples.Deleting_principal_parent_1();
        IntroRequiredSamples.Severing_a_relationship_1();
        IntroRequiredSamples.Severing_a_relationship_2();
        IntroRequiredSamples.Where_cascading_behaviors_happen_1();

        WithDatabaseCycleSamples.Database_cascade_limitations_1();
        WithDatabaseCycleSamples.Database_cascade_limitations_2();

        IntroOptionalSamples.Deleting_principal_parent_1b();
        IntroOptionalSamples.Severing_a_relationship_1b();
        IntroOptionalSamples.Severing_a_relationship_2b();

        RequiredDependentsSamples.Required_relationship_with_dependents_children_loaded();
        RequiredDependentsSamples.Required_relationship_with_dependents_children_not_loaded();
        OptionalDependentsSamples.Optional_relationship_with_dependents_children_loaded();
        OptionalDependentsSamples.Optional_relationship_with_dependents_children_not_loaded();
    }
}
