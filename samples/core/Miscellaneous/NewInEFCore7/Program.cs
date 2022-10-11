using NewInEfCore7;

public class Program
{
    public static async Task Main()
    {
        await TpcInheritanceSample.Inheritance_with_TPH();
        await TpcInheritanceSample.Inheritance_with_TPT();
        await TpcInheritanceSample.Inheritance_with_TPC();
        await TpcInheritanceSample.Inheritance_with_TPC_using_HiLo();
        await TpcInheritanceSample.Inheritance_with_TPC_using_Identity();

        await ExecuteDeleteSample.ExecuteDelete();
        await ExecuteDeleteSample.ExecuteDeleteTpt();
        await ExecuteDeleteSample.ExecuteDeleteTpc();
        await ExecuteDeleteSample.ExecuteDeleteSqlite();

        await ExecuteUpdateSample.ExecuteUpdate();
        await ExecuteUpdateSample.ExecuteUpdateTpt();
        await ExecuteUpdateSample.ExecuteUpdateTpc();
        await ExecuteUpdateSample.ExecuteUpdateSqlite();

        await JsonColumnsSample.Json_columns_with_TPH();

        await ModelBuildingConventionsSample.No_foreign_key_index_convention();
        await ModelBuildingConventionsSample.Discriminator_length_convention();
        await ModelBuildingConventionsSample.Max_string_length_convention();
        await ModelBuildingConventionsSample.Map_members_explicitly_by_attribute_convention();
        await ModelBuildingConventionsSample.Custom_model_validation_convention();
        await ModelBuildingConventionsSample.No_cascade_delete_convention();

        await StoredProcedureMappingSample.Insert_Update_and_Delete_using_stored_procedures_with_TPH();
        await StoredProcedureMappingSample.Insert_Update_and_Delete_using_stored_procedures_with_TPT();
        await StoredProcedureMappingSample.Insert_Update_and_Delete_using_stored_procedures_with_TPC();

        await SimpleMaterializationSample.Simple_actions_on_entity_creation();

        await QueryInterceptionSample.LINQ_expression_tree_interception();

        await OptimisticConcurrencyInterceptionSample.Optimistic_concurrency_interception();

        await InjectLoggerSample.Injecting_services_into_entities();

        await LazyConnectionStringSample.Lazy_initialization_of_a_connection_string();

        await QueryStatisticsLoggerSample.Executing_commands_after_consuming_a_result_set();

        await UngroupedColumnsQuerySample.Subqueries_dont_reference_ungrouped_columns_from_outer_query_SqlServer();

        await GroupByEntityTypeSample.GroupBy_entity_type_Sqlite();
        await GroupByEntityTypeSample.GroupBy_entity_type_SqlServer();
        await GroupByEntityTypeSample.GroupBy_entity_type_InMemory();

        await GroupByFinalOperatorSample.GroupBy_final_operator_SqlServer();
        await GroupByFinalOperatorSample.GroupBy_final_operator_Sqlite();

        await GroupJoinFinalOperatorSample.GroupJoin_final_operator_SqlServer();
        await GroupJoinFinalOperatorSample.GroupJoin_final_operator_InMemory();

        await ReadOnlySetQuerySample.Use_Contains_with_IReadOnlySet_SqlServer();
        await ReadOnlySetQuerySample.Use_Contains_with_IReadOnlySet_Sqlite();
        await ReadOnlySetQuerySample.Use_Contains_with_IReadOnlySet_InMemory();

        await StringAggregateFunctionsSample.Translate_string_Concat_and_string_Join();

        await SpatialAggregateFunctionsSample.Translate_spatial_aggregate_functions_SqlServer();
        await SpatialAggregateFunctionsSample.Translate_spatial_aggregate_functions_InMemory();

        await StatisticalAggregateFunctionsSample.Translate_statistical_aggregate_functions();

        await MiscellaneousTranslationsSample.Translate_string_IndexOf();

        await SaveChangesPerformanceSample.SaveChanges_SQL_generation_samples_SqlServer();
        await SaveChangesPerformanceSample.SaveChanges_SQL_generation_samples_Sqlite();

        await DbContextApiSample.Find_siblings();
        await DbContextApiSample.Get_entry_for_shared_type_entity_type();
        await DbContextApiSample.Use_IEntityEntryGraphIterator();

        await ModelBuildingSample.Indexes_can_be_ordered();
        await ModelBuildingSample.Property_can_be_mapped_to_different_column_names_TPT();
        await ModelBuildingSample.Property_can_be_mapped_to_different_column_names_TPC();
        await ModelBuildingSample.Unidirectional_many_to_many();
        await ModelBuildingSample.Entity_splitting();
        await ModelBuildingSample.Temporal_tables_with_owned_types();

        await ValueGenerationSample.Can_use_value_generation_with_converted_types();

        // // Requires the Cosmos emulator
        // await CosmosQueriesSample.Cosmos_translations_for_RegEx_Match();
    }
}
