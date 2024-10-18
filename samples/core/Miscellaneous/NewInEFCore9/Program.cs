using NewInEfCore9;

public class Program
{
    public static async Task Main()
    {
        await PrimitiveCollectionsSample.Queries_using_readonly_primitive_collections();
        await PrimitiveCollectionsSample.Queries_using_readonly_primitive_collections_SQLite();

        await ComplexTypesSample.GropupBy_complex_type_instances();
        await ComplexTypesSample.GropupBy_complex_type_instances_on_SQLite();

        await QuerySample.Query_improvements_in_EF9();
        await QuerySample.Query_improvements_in_EF9_on_SQLite();

        // Note that SQL Server 2022 is required for Greater and Least queries.
        // await LeastGreatestSample.Queries_using_Least_and_Greatest();
        await LeastGreatestSample.Queries_using_Least_and_Greatest_on_SQLite();

        await NullSemanticsSample.Null_semantics_improvements_in_EF9();
        await NullSemanticsSample.Null_semantics_improvements_in_EF9_on_SQLite();

        await CustomConventionsSample.Conventions_enhancements_in_EF9();

        await JsonColumnsSample.Columns_from_JSON_are_pruned_when_needed();
        await JsonColumnsSample.Columns_from_JSON_are_pruned_when_needed_on_SQLite();

        await ExecuteUpdateSample.ExecuteUpdate_for_complex_type_instances();
        await ExecuteUpdateSample.ExecuteUpdate_for_complex_type_instances_on_SQLite();

        await HierarchyIdSample.SQL_Server_HierarchyId();

        await ModelBuildingSample.Model_building_improvements_in_EF9();

        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQLite();
        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server();
        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server_with_JSON();
    }
}
