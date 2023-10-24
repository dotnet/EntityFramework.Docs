using NewInEfCore8;

public class Program
{
    public static async Task Main()
    {
        await ComplexTypesSample.Use_mutable_class_as_complex_type();
        await ImmutableComplexTypesSample.Use_immutable_class_as_complex_type();
        await RecordComplexTypesSample.Use_immutable_record_as_complex_type();
        await StructComplexTypesSample.Use_mutable_struct_as_complex_type();
        await ImmutableStructComplexTypesSample.Use_immutable_struct_as_complex_type();
        await NestedComplexTypesSample.Use_mutable_classes_as_complex_types();

        await ComplexTypesSample.Use_mutable_class_as_complex_type_SQLite();
        await ImmutableComplexTypesSample.Use_immutable_class_as_complex_type_SQLite();
        await RecordComplexTypesSample.Use_immutable_record_as_complex_type_SQLite();
        await StructComplexTypesSample.Use_mutable_struct_as_complex_type_SQLite();
        await ImmutableStructComplexTypesSample.Use_immutable_struct_as_complex_type_SQLite();
        await NestedComplexTypesSample.Use_mutable_classes_as_complex_types_SQLite();

        await JsonColumnsSample.Json_columns_with_TPH();
        await JsonColumnsSample.Json_columns_with_TPH_on_SQLite();

        await RawSqlSample.SqlQuery_for_unmapped_types();

        await LazyLoadingSample.Lazy_loading_for_no_tracking_queries();

        await InheritanceSample.Discriminator_length_TPH();

        await LookupByKeySample.Lookup_tracked_entities_by_key();

        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQLite();
        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server();
        await DateOnlyTimeOnlySample.Can_use_DateOnly_TimeOnly_on_SQL_Server_with_JSON();

        await HierarchyIdSample.SQL_Server_HierarchyId();

        await PrimitiveCollectionsSample.Queries_using_primitive_collections();
        await PrimitiveCollectionsSample.Queries_using_primitive_collections_SQLite();

        await PrimitiveCollectionsInJsonSample.Queries_using_primitive_collections_in_JSON_documents();
        await PrimitiveCollectionsInJsonSample.Queries_using_primitive_collections_in_JSON_documents_SQLite();

        await PrimitiveCollectionToTableSample.Queries_against_a_table_wrapping_a_primitive_type();

        await DefaultConstraintSample.Insert_rows_using_database_default_constraint();
        await DefaultConstraintSample.Insert_rows_using_database_default_constraint_SQLite();

        await ExecuteUpdateDeleteSample.ExecuteUpdate_ExecuteDelete_with_multiple_entities_targeting_one_table();
        await ExecuteUpdateDeleteSample.ExecuteUpdate_ExecuteDelete_with_multiple_entities_targeting_one_table_SQLite();
    }
}
