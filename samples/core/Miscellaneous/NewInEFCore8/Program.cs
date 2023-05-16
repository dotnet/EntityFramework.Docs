using NewInEfCore8;

public class Program
{
    public static async Task Main()
    {
        await JsonColumnsSample.Json_columns_with_TPH();

        // https://github.com/dotnet/efcore/issues/30886
        // await JsonColumnsSample.Json_columns_with_TPH_on_SQLite();

        await RawSqlSample.SqlQuery_for_unmapped_types();
        await LazyLoadingSample.Lazy_loading_for_no_tracking_queries();
        await InheritanceSample.Discriminator_length_TPH();
        await LookupByKeySample.Lookup_tracked_entities_by_key();
        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQLite();

        // https://github.com/dotnet/efcore/issues/30885
        // await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server();
        // await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server_with_JSON();

        await HierarchyIdSample.SQL_Server_HierarchyId();
        await PrimitiveCollectionsSample.Queries_using_primitive_collections();
        await PrimitiveCollectionsSample.Queries_using_primitive_collections_SQLite();
        await PrimitiveCollectionsInJsonSample.Queries_using_primitive_collections_in_JSON_documents();
        await PrimitiveCollectionsInJsonSample.Queries_using_primitive_collections_in_JSON_documents_SQLite();
        await PrimitiveCollectionToTableSample.Queries_against_a_table_wrapping_a_primitive_type();
    }
}
