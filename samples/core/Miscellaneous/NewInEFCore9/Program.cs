using NewInEfCore9;

public class Program
{
    public static async Task Main()
    {
        await QuerySample.Query_improvements_in_EF9();
        await QuerySample.Query_improvements_in_EF9_on_SQLite();

        // Note that SQL Server 2022 is required for Greater and Least queries.
        // await LeastGreatestSample.Queries_using_Least_and_Greatest();
        await LeastGreatestSample.Queries_using_Least_and_Greatest_on_SQLite();

        await ModelBuildingSample.Model_building_enhancements_in_EF9();

        await JsonColumnsSample.Columns_from_JSON_are_pruned_when_needed();
        await JsonColumnsSample.Columns_from_JSON_are_pruned_when_needed_on_SQLite();

        await ExecuteUpdateSample.ExecuteUpdate_for_complex_type_instances();
        await ExecuteUpdateSample.ExecuteUpdate_for_complex_type_instances_on_SQLite();
    }
}
