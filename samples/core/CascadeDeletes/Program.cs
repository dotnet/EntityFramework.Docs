using System;
using System.Threading.Tasks;
using DatabaseCycles;
using IntroOptional;
using IntroRequired;
using Optional;
using Required;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Cascade Delete_");
        Console.WriteLine();

        await IntroRequiredSamples.Deleting_principal_parent_1();
        await IntroRequiredSamples.Severing_a_relationship_1();
        await IntroRequiredSamples.Severing_a_relationship_2();
        await IntroRequiredSamples.Where_cascading_behaviors_happen_1();

        await WithDatabaseCycleSamples.Database_cascade_limitations_1();
        await WithDatabaseCycleSamples.Database_cascade_limitations_2();

        await IntroOptionalSamples.Deleting_principal_parent_1b();
        await IntroOptionalSamples.Severing_a_relationship_1b();
        await IntroOptionalSamples.Severing_a_relationship_2b();

        await RequiredDependentsSamples.Required_relationship_with_dependents_children_loaded();
        await RequiredDependentsSamples.Required_relationship_with_dependents_children_not_loaded();
        await OptionalDependentsSamples.Optional_relationship_with_dependents_children_loaded();
        await OptionalDependentsSamples.Optional_relationship_with_dependents_children_not_loaded();
    }
}
