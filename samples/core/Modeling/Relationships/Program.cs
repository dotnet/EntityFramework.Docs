using System.Threading.Tasks;

namespace EFModeling.Relationships;

public class Program
{
    private static async Task Main()
    {
        OneToMany.Required.BuildModels();
        OneToMany.Optional.BuildModels();
        OneToMany.RequiredWithShadowFk.BuildModels();
        OneToMany.OptionalWithShadowFk.BuildModels();
        OneToMany.RequiredNoNavigationToPrincipal.BuildModels();
        OneToMany.OptionalNoNavigationToPrincipal.BuildModels();
        OneToMany.RequiredWithShadowFkAndNoNavigationToPrincipal.BuildModels();
        OneToMany.OptionalWithShadowFkAndNoNavigationToPrincipal.BuildModels();
        OneToMany.RequiredNoNavigationToDependents.BuildModels();
        OneToMany.OptionalNoNavigationToDependents.BuildModels();
        OneToMany.RequiredWithShadowFkAndNoNavigationToDependents.BuildModels();
        OneToMany.OptionalWithShadowFkAndNoNavigationToDependents.BuildModels();
        OneToMany.RequiredNoNavigations.BuildModels();
        OneToMany.OptionalNoNavigations.BuildModels();
        OneToMany.RequiredWithShadowFkAndNoNavigations.BuildModels();
        OneToMany.OptionalWithShadowFkAndNoNavigations.BuildModels();
        OneToMany.RequiredWithAlternateKey.BuildModels();
        OneToMany.OptionalWithAlternateKey.BuildModels();
        OneToMany.RequiredWithShadowFkWithAlternateKey.BuildModels();
        OneToMany.OptionalWithShadowFkWithAlternateKey.BuildModels();
        OneToMany.RequiredWithCompositeKey.BuildModels();
        OneToMany.OptionalWithCompositeKey.BuildModels();
        OneToMany.RequiredWithShadowFkWithCompositeKey.BuildModels();
        OneToMany.OptionalWithShadowFkWithCompositeKey.BuildModels();
        OneToMany.SelfReferencing.BuildModels();
        OneToMany.RequiredWithoutCascadeDelete.BuildModels();

        OneToOne.Required.BuildModels();
        OneToOne.Optional.BuildModels();
        OneToOne.RequiredPkToPk.BuildModels();
        OneToOne.RequiredWithShadowFk.BuildModels();
        OneToOne.OptionalWithShadowFk.BuildModels();
        OneToOne.RequiredNoNavigationToPrincipal.BuildModels();
        OneToOne.OptionalNoNavigationToPrincipal.BuildModels();
        OneToOne.RequiredWithShadowFkAndNoNavigationToPrincipal.BuildModels();
        OneToOne.OptionalWithShadowFkAndNoNavigationToPrincipal.BuildModels();
        OneToOne.RequiredNoNavigationToDependents.BuildModels();
        OneToOne.OptionalNoNavigationToDependents.BuildModels();
        OneToOne.RequiredWithShadowFkAndNoNavigationToDependents.BuildModels();
        OneToOne.OptionalWithShadowFkAndNoNavigationToDependents.BuildModels();
        OneToOne.RequiredNoNavigations.BuildModels();
        OneToOne.OptionalNoNavigations.BuildModels();
        OneToOne.RequiredWithShadowFkAndNoNavigations.BuildModels();
        OneToOne.OptionalWithShadowFkAndNoNavigations.BuildModels();
        OneToOne.RequiredWithAlternateKey.BuildModels();
        OneToOne.OptionalWithAlternateKey.BuildModels();
        OneToOne.RequiredWithShadowFkWithAlternateKey.BuildModels();
        OneToOne.OptionalWithShadowFkWithAlternateKey.BuildModels();
        OneToOne.RequiredWithCompositeKey.BuildModels();
        OneToOne.OptionalWithCompositeKey.BuildModels();
        OneToOne.RequiredWithShadowFkWithCompositeKey.BuildModels();
        OneToOne.OptionalWithShadowFkWithCompositeKey.BuildModels();
        OneToOne.SelfReferencing.BuildModels();
        OneToOne.RequiredWithoutCascadeDelete.BuildModels();

        ManyToMany.BasicManyToMany.BuildModels();
        ManyToMany.UnidirectionalManyToMany.BuildModels();
        ManyToMany.ManyToManyNamedJoinTable.BuildModels();
        ManyToMany.ManyToManyNamedForeignKeyColumns.BuildModels();
        ManyToMany.ManyToManyWithJoinClass.BuildModels();
        ManyToMany.ManyToManyWithNavsToJoinClass.BuildModels();
        ManyToMany.ManyToManyWithNavsToAndFromJoinClass.BuildModels();
        ManyToMany.ManyToManyWithNamedFksAndNavsToAndFromJoinClass.BuildModels();
        ManyToMany.ManyToManyAlternateKeys.BuildModels();
        ManyToMany.ManyToManyWithNavsAndAlternateKeys.BuildModels();
        ManyToMany.ManyToManyWithJoinClassHavingPrimaryKey.BuildModels();
        ManyToMany.ManyToManyWithPrimaryKeyInJoinEntity.BuildModels();
        ManyToMany.ManyToManyWithPayloadAndNavsToJoinClass.BuildModels();
        ManyToMany.ManyToManyWithNoCascadeDelete.BuildModels();
        ManyToMany.SelfReferencingManyToMany.BuildModels();
        ManyToMany.SelfReferencingUnidirectionalManyToMany.BuildModels();
        ManyToMany.ManyToManyWithCustomSharedTypeEntityType.BuildModels();
        await ManyToMany.DirectJoinTableNoManyToMany.BuildModels();
        await ManyToMany.FullMappingWithJoinEntity.BuildModels();

        RelationshipConventions.ReferenceNavigations.BuildModels();
        RelationshipConventions.CollectionNavigations.BuildModels();

        RelationshipConventions.OneToManySingleRelationship.BuildModels();
        RelationshipConventions.OneToOneSingleRelationship.BuildModels();
        RelationshipConventions.ManyToManySingleRelationship.BuildModels();
        RelationshipConventions.NavigationPrincipalKeyFKName.BuildModels();
        RelationshipConventions.NavigationIdFKName.BuildModels();
        RelationshipConventions.PrincipalTypePrincipalKeyFKName.BuildModels();
        RelationshipConventions.PrincipalTypeIdFKName.BuildModels();

        ForeignAndPrincipalKeys.ForeignKeyConfigurationByLambda.BuildModels();
        ForeignAndPrincipalKeys.CompositeForeignKeyConfigurationByLambda.BuildModels();
        ForeignAndPrincipalKeys.ForeignKeyConfigurationByString.BuildModels();
        ForeignAndPrincipalKeys.CompositeForeignKeyConfigurationByString.BuildModels();
        ForeignAndPrincipalKeys.RequiredForeignKey.BuildModels();
        ForeignAndPrincipalKeys.ShadowForeignKey.BuildModels();
        ForeignAndPrincipalKeys.ForeignKeyConstraintName.BuildModels();
        ForeignAndPrincipalKeys.AlternateKeyConfigurationByLambda.BuildModels();
        ForeignAndPrincipalKeys.CompositeAlternateKeyConfigurationByLambda.BuildModels();
        ForeignAndPrincipalKeys.AlternateKeyConfigurationByString.BuildModels();
        ForeignAndPrincipalKeys.CompositeAlternateKeyConfigurationByString.BuildModels();
        ForeignAndPrincipalKeys.ForeignKeyInKeylessType.BuildModels();
        ForeignAndPrincipalKeys.ManyToManyForeignKeyConstraintNames.BuildModels();
        ForeignAndPrincipalKeys.NavigationConfiguration.BuildModels();

        MappingAttributes.RequiredOnForeignKey.BuildModels();
        MappingAttributes.RequiredOnDependentNavigation.BuildModels();
        MappingAttributes.RequiredOnDependentNavigationShadowFk.BuildModels();
        MappingAttributes.ForeignKeyOnProperty.BuildModels();
        MappingAttributes.ForeignKeyOnDependentNavigation.BuildModels();
        MappingAttributes.ForeignKeyOnPrincipalNavigation.BuildModels();
        MappingAttributes.ForeignKeyOnDependentNavigationShadowFk.BuildModels();
        MappingAttributes.ForeignKeyOnPrincipalNavigationShadowFk.BuildModels();
        MappingAttributes.InverseOnDependentNavigation.BuildModels();
        MappingAttributes.InverseOnPrincipalNavigation.BuildModels();
        MappingAttributes.DeleteBehaviorOnDependentNavigation.BuildModels();
        MappingAttributes.DeleteBehaviorOnPrincipalNavigation.BuildModels();
    }
}
