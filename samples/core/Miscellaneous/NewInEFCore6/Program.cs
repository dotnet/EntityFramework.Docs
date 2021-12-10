public class Program
{
    public static void Main()
    {
        EntityTypeConfigurationAttributeSample.Using_EntityTypeConfigurationAttribute();
        UnicodeAttributeSample.Using_UnicodeAttribute();
        PrecisionAttributeSample.Using_PrecisionAttribute();
        ToStringTranslationSample.Using_ToString_in_queries();
        RandomFunctionSample.Call_EF_Functions_Random();
        SparseColumnsSample.Use_sparse_columns();
        InMemoryRequiredPropertiesSample.Required_properties_validated_with_in_memory_database();
        InMemoryRequiredPropertiesSample.Required_property_validation_with_in_memory_database_can_be_disabled();
        IsNullOrWhitespaceSample.Translate_IsNullOrWhitespace();
        StringConcatSample.Concat_with_multiple_args();
        ArrayParametersSample.Array_parameters_are_logged_in_readable_form();
        TemporaryValuesSample.Explicit_temporary_values_can_be_stored_in_entity_instance_1();
        TemporaryValuesSample.Explicit_temporary_values_can_be_stored_in_entity_instance_2();
        TrailingUnderscoresSample.Backing_fields_with_trailing_underscores_are_matched();
        OptionalDependentsSample.Optional_dependents_with_a_required_property();
        OptionalDependentsSample.Optional_dependents_without_a_required_property();
        OptionalDependentsSample.Handling_optional_dependents_sharing_table_with_principal_1();
        OptionalDependentsSample.Handling_optional_dependents_sharing_table_with_principal_2();
        OptionalDependentsSample.Handling_required_dependents_sharing_table_with_principal();
        OptionalDependentsSample.Handling_nested_optional_dependents_sharing_table_with_principal();
        OptionalDependentsSample.Handling_nested_required_dependents_sharing_table_with_principal();
        PublicPooledDbContextFactorySample.Can_create_pooled_DbContext_factory();
        DbContextFactorySample.Ignore_parameterless_constructor_when_creating_DbContext_from_factory();
        DbContextFactorySample.AddDbContextFactory_also_registers_scoped_DbContext_instance();
        SplitQuerySample.Split_query_for_non_navigation_collections();
        SplitQuerySample.Last_column_in_ORDER_BY_removed_when_joining_for_collection();
        SubstringTranslationSample.Translate_Substring_with_single_parameter();
        BoolToStringTranslationSample.Translate_bool_to_string_on_SQL_Server();
        TagWithFileAndLineSample.Queries_can_be_tagged_with_filename_and_line_number();
        PreConventionModelConfigurationSample.Configure_property_types_and_value_converter_in_one_place();
        MathFTranslationSample.Translate_MathF_methods();
        TemporalTablesSample.Use_SQL_Server_temporal_tables();
        GroupBySample.Translate_GroupBy_followed_by_FirstOrDefault_over_group();
        HasConversionSample.Can_set_value_converter_type_using_generic_method();
        MinimalApiSample.Add_a_DbContext_and_provider();
        ToInMemoryQuerySample.Can_query_keyless_types_from_in_memory_database();
        CommandSourceSample.Interceptors_get_the_source_of_the_command();
        ScaffoldingSample.Reverse_engineer_from_database();
        ManyToManyConfigurationSample.Many_to_many_relationships_may_need_less_configuration();
        ConvertNullsSample.Value_converters_can_convert_nulls();
        ColumnOrderSample.Can_use_ColumnAttribute_to_set_column_order();

        SqliteSamples.SavepointsApi();
        SqliteSamples.ConnectionPooling();
        SqliteSamples.DateOnly_and_TimeOnly();

        // Note: this sample requires a full version of SQL Server. It does not work with LocalDb
        // ContainsFreeTextSample.Contains_with_non_string();
    }
}
