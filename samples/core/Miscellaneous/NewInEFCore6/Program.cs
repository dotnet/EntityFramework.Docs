using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        await EntityTypeConfigurationAttributeSample.Using_EntityTypeConfigurationAttribute();
        await UnicodeAttributeSample.Using_UnicodeAttribute();
        await PrecisionAttributeSample.Using_PrecisionAttribute();
        await ToStringTranslationSample.Using_ToString_in_queries();
        await RandomFunctionSample.Call_EF_Functions_Random();
        await SparseColumnsSample.Use_sparse_columns();
        await InMemoryRequiredPropertiesSample.Required_properties_validated_with_in_memory_database();
        await InMemoryRequiredPropertiesSample.Required_property_validation_with_in_memory_database_can_be_disabled();
        await IsNullOrWhitespaceSample.Translate_IsNullOrWhitespace();
        await StringConcatSample.Concat_with_multiple_args();
        await ArrayParametersSample.Array_parameters_are_logged_in_readable_form();
        await TemporaryValuesSample.Explicit_temporary_values_can_be_stored_in_entity_instance_1();
        await TemporaryValuesSample.Explicit_temporary_values_can_be_stored_in_entity_instance_2();
        await TrailingUnderscoresSample.Backing_fields_with_trailing_underscores_are_matched();
        await OptionalDependentsSample.Optional_dependents_with_a_required_property();
        await OptionalDependentsSample.Optional_dependents_without_a_required_property();
        await OptionalDependentsSample.Handling_optional_dependents_sharing_table_with_principal_1();
        await OptionalDependentsSample.Handling_optional_dependents_sharing_table_with_principal_2();
        await OptionalDependentsSample.Handling_required_dependents_sharing_table_with_principal();
        await OptionalDependentsSample.Handling_nested_optional_dependents_sharing_table_with_principal();
        await OptionalDependentsSample.Handling_nested_required_dependents_sharing_table_with_principal();
        PublicPooledDbContextFactorySample.Can_create_pooled_DbContext_factory();
        DbContextFactorySample.Ignore_parameterless_constructor_when_creating_DbContext_from_factory();
        DbContextFactorySample.AddDbContextFactory_also_registers_scoped_DbContext_instance();
        await SplitQuerySample.Split_query_for_non_navigation_collections();
        await SplitQuerySample.Last_column_in_ORDER_BY_removed_when_joining_for_collection();
        await SubstringTranslationSample.Translate_Substring_with_single_parameter();
        await BoolToStringTranslationSample.Translate_bool_to_string_on_SQL_Server();
        await TagWithFileAndLineSample.Queries_can_be_tagged_with_filename_and_line_number();
        await PreConventionModelConfigurationSample.Configure_property_types_and_value_converter_in_one_place();
        await MathFTranslationSample.Translate_MathF_methods();
        await TemporalTablesSample.Use_SQL_Server_temporal_tables();
        await GroupBySample.Translate_GroupBy_followed_by_FirstOrDefault_over_group();
        await HasConversionSample.Can_set_value_converter_type_using_generic_method();
        MinimalApiSample.Add_a_DbContext_and_provider();
        await ToInMemoryQuerySample.Can_query_keyless_types_from_in_memory_database();
        await CommandSourceSample.Interceptors_get_the_source_of_the_command();
        await ScaffoldingSample.Reverse_engineer_from_database();
        await ManyToManyConfigurationSample.Many_to_many_relationships_may_need_less_configuration();
        await ConvertNullsSample.Value_converters_can_convert_nulls();
        await ColumnOrderSample.Can_use_ColumnAttribute_to_set_column_order();

        await SqliteSamples.SavepointsApi();
        await SqliteSamples.ConnectionPooling();
        await SqliteSamples.DateOnly_and_TimeOnly();

        // Note: this sample requires a full version of SQL Server. It does not work with LocalDb
        // ContainsFreeTextSample.Contains_with_non_string();
    }
}
