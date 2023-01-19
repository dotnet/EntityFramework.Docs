using NewInEfCore8;

public class Program
{
    public static async Task Main()
    {
        await JsonColumnsSample.Json_columns_with_TPH();
        await RawSqlSample.SqlQuery_for_unmapped_types();
        await LazyLoadingSample.Lazy_loading_for_no_tracking_queries();
        await InheritanceSample.Discriminator_length_TPH();
        await LookupByKeySample.Lookup_tracked_entities_by_key();
    }
}
