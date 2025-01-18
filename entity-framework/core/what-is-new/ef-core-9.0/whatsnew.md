---
title: What's New in EF Core 9
description: Overview of new features in EF Core 9
author: SamMonoRT
ms.date: 10/21/2024
uid: core/what-is-new/ef-core-9.0/whatsnew
---

# What's New in EF Core 9

EF Core 9 (EF9) is the next release after EF Core 8 and is scheduled for release in November 2024.

EF9 is available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF9 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF9 targets .NET 8, and can therefore be used with either [.NET 8 (LTS)](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0).

> [!TIP]
> The _What's New_ docs are updated for each preview. All the samples are set up to use the [EF9 daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md), which usually have several additional weeks of completed work compared to the latest preview. We strongly encourage use of the daily builds when testing new features so that you're not doing your testing against stale bits.

<a name="cosmos"></a>

## Azure Cosmos DB for NoSQL

EF 9.0 brings substantial improvements to the EF Core provider for Azure Cosmos DB; significant parts of the provider have been rewritten to provide new functionality, allow new forms of queries, and better align the provider with Azure Cosmos DB best practices. The main high-level improvements are listed below; for a full list, [see this epic issue](https://github.com/dotnet/efcore/issues/33033).

> [!WARNING]
> As part of the improvements going into the provider, a number of high-impact breaking changes had to be made; if you are upgrading an existing application, please read the [breaking changes section](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-breaking-changes) carefully.

### Improvements querying with partition keys and document IDs

Each document stored in an Azure Cosmos DB database has a unique resource ID. In addition, each document can contain a "partition key" which determines the logical partitioning of data such that the database can be effectively scaled. More information on choosing partition keys can be found in [_Partitioning and horizontal scaling in Azure Cosmos DB_](/azure/cosmos-db/partitioning-overview).

In EF 9.0, the Azure Cosmos DB provider is significantly better at identifying partition key comparisons in your LINQ queries, and extracting them out to ensure your queries are only sent to the relevant partition; this can greatly improve the performance of your queries and reduce RU charges. For example:

```csharp
var sessions = await context.Sessions
    .Where(b => b.PartitionKey == "someValue" && b.Username.StartsWith("x"))
    .ToListAsync();
```

In this query, the provider automatically recognizes the comparison on `PartitionKey`; if we examine the logs, we'll see the following:

```console
Executed ReadNext (189.8434 ms, 2.8 RU) ActivityId='8cd669ed-2ca5-4f2b-8923-338899071361', Container='test', Partition='["someValue"]', Parameters=[]
SELECT VALUE c
FROM root c
WHERE STARTSWITH(c["Username"], "x")
```

Note that the `WHERE` clause does not contain `PartitionKey`: that comparison has been "lifted" out and is used to execute the query only against the relevant partition. In previous versions, the comparison was left in the `WHERE` clause in many situations, causing the query to be executed against all partitions and resulting in increased costs and reduced performance.

In addition, if your query also provides a value for the document's ID property, and doesn't include any other query operations, the provider can apply an additional optimization:

```csharp
var somePartitionKey = "someValue";
var someId = 8;
var sessions = await context.Sessions
    .Where(b => b.PartitionKey == somePartitionKey && b.Id == someId)
    .SingleAsync();
```

The logs show the following for this query:

```console
Executed ReadItem (73 ms, 1 RU) ActivityId='13f0f8b8-d481-47f0-bf41-67f7deb008b2', Container='test', Id='8', Partition='["someValue"]'
```

Here, no SQL query is sent at all. Instead, the provider performs an an extremely efficient _point read_ (`ReadItem` API), which directly fetches the document given the partition key and ID. This is the most efficient and cost-effective kind of read you can perform in Azure Cosmos DB; [see the Azure Cosmos DB documentation](/azure/cosmos-db/nosql/how-to-dotnet-read-item) for more information about point reads.

To learn more about querying with partition keys and point reads, [see the querying documentation page](xref:core/providers/cosmos/querying).

### Hierarchical partition keys

> [!TIP]
> The code shown here comes from [HierarchicalPartitionKeysSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9.Cosmos/HierarchicalPartitionKeysSample.cs).

Azure Cosmos DB originally supported a single partition key, but has since expanded partitioning capabilities to also support [subpartitioning through the specification of up to three levels of hierarchy in the partition key](/azure/cosmos-db/hierarchical-partition-keys). EF Core 9 brings full support for hierarchical partition keys, allowing you take advantage of the better performance and cost savings associated with this feature.

Partition keys are specified using the model building API, typically in <xref:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating*?displayProperty=nameWithType>. There must be a mapped property in the entity type for each level of the partition key. For example, consider a `UserSession` entity type:

<!--
    public class UserSession
    {
        // Item ID
        public Guid Id { get; set; }

        // Partition Key
        public string TenantId { get; set; } = null!;
        public Guid UserId { get; set; }
        public int SessionId { get; set; }

        // Other members
        public string Username { get; set; } = null!;
    }
-->
[!code-csharp[UserSession](../../../../samples/core/Miscellaneous/NewInEFCore9.Cosmos/HierarchicalPartitionKeysSample.cs?name=UserSession)]

The following code specifies a three-level partition key using the `TenantId`, `UserId`, and `SessionId` properties:

<!--
            modelBuilder
                .Entity<UserSession>()
                .HasPartitionKey(e => new { e.TenantId, e.UserId, e.SessionId });
-->
[!code-csharp[HasPartitionKey](../../../../samples/core/Miscellaneous/NewInEFCore9.Cosmos/HierarchicalPartitionKeysSample.cs?name=HasPartitionKey)]

> [!TIP]
> This partition key definition follows the example given in [_Choose your hierarchical partition keys_](/azure/cosmos-db/hierarchical-partition-keys#choose-your-hierarchical-partition-keys) from the Azure Cosmos DB documentation.

Notice how, starting with EF Core 9, properties of any mapped type can be used in the partition key. For `bool` and numeric types, like the `int SessionId` property, the value is used directly in the partition key. Other types, like the `Guid UserId` property, are automatically converted to strings.

When querying, EF automatically extracts the partition key values from queries and applies them to the Azure Cosmos DB query API to ensure the queries are constrained appropriately to the fewest number of partitions possible. For example, consider the following LINQ query that supplies all three partition key values in the hierarchy:

<!--
            var tenantId = "Microsoft";
            var sessionId = 7;
            var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");

            var sessions = await context.Sessions
                .Where(
                    e => e.TenantId == tenantId
                         && e.UserId == userId
                         && e.SessionId == sessionId
                         && e.Username.Contains("a"))
                .ToListAsync();
-->
[!code-csharp[FullPartitionKey](../../../../samples/core/Miscellaneous/NewInEFCore9.Cosmos/HierarchicalPartitionKeysSample.cs?name=FullPartitionKey)]

When executing this query, EF Core will extract the values of the `tenantId`, `userId`, and `sessionId` parameters, and pass them to the Azure Cosmos DB query API as the partition key value. For example, see the logs from executing the query above:

```output
info: 6/10/2024 19:06:00.017 CosmosEventId.ExecutingSqlQuery[30100] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executing SQL query for container 'UserSessionContext' in partition '["Microsoft","99a410d7-e467-4cc5-92de-148f3fc53f4c",7.0]' [Parameters=[]]
      SELECT c
      FROM root c
      WHERE ((c["Discriminator"] = "UserSession") AND CONTAINS(c["Username"], "a"))
```

Notice that the partition key comparisons have been removed from the `WHERE` clause, and are instead used as the partition key for efficient execution: `["Microsoft","99a410d7-e467-4cc5-92de-148f3fc53f4c",7.0]`.

For more information, see the documentation on [querying with partition keys](xref:core/providers/cosmos/querying#partition-keys).

### Significantly improved LINQ querying capabilities

In EF 9.0, the LINQ translation capabilities of the the Azure Cosmos DB provider have been greatly expanded, and the provider can now execute significantly more query types. The full list of query improvements is too long to list, but here are the main highlights:

* Full support for EF's primitive collections, allowing you to perform LINQ querying on collections of e.g. ints or strings. See [What's new in EF8: primitive collections](xref:core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) for more information.
* Support for arbitrary querying over non-primitive collections.
* Lots of additional LINQ operators are now supported: indexing into collections, `Length`/`Count`, `ElementAt`, `Contains`, and many others.
* Support for aggregate operators such as `Count` and `Sum`.
* Additional function translations (see the [function mappings documentation](xref:core/providers/cosmos/querying#function-mappings) for the full list of supported translations):
  * Translations for `DateTime` and `DateTimeOffset` component members (`DateTime.Year`, `DateTimeOffset.Month`...).
  * `EF.Functions.IsDefined` and `EF.Functions.CoalesceUndefined` now allow dealing with `undefined` values.
  * `string.Contains`, `StartsWith` and `EndsWith` now support `StringComparison.OrdinalIgnoreCase`.

For the full list of querying improvements, see [this issue](https://github.com/dotnet/efcore/issues/33033):

### Improved modeling aligned to Azure Cosmos DB and JSON standards

EF 9.0 maps to Azure Cosmos DB documents in more natural ways for a JSON-based document database, and helps interoperate with other systems accessing your documents. Although this entails breaking changes, APIs exist which allow reverting back to the pre-9.0 behavior in all cases.

#### Simplified `id` properties without discriminators

First, previous versions of EF inserted the discriminator value into the JSON `id` property, producing documents such as the following:

```json
{
    "id": "Blog|1099",
    ...
}
```

This was done in order to allow for documents of different types (e.g. Blog and Post) and the same key value (1099) to exist within the same container partition. Starting with EF 9.0, the `id` property contains contains only the key value:

```json
{
    "id": 1099,
    ...
}
```

This is a more natural way to map to JSON, and makes it easier for external tools and systems to interact with EF-generated JSON documents; such external systems aren't generally aware of the EF discriminator values, which are by default derived from .NET types.

Note this is a breaking change, since EF will no longer be able to query existing documents with the old `id` format. An API has been introduced to revert to the previous behavior, see the [breaking change note](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-id-property-changes) and the [the documentation](xref:core/providers/cosmos/modeling#Discriminators) for more details.

#### Discriminator property renamed to `$type`

The default discriminator property was previously named `Discriminator`. EF 9.0 changes the default to `$type`:

```json
{
    "id": 1099,
    "$type": "Blog",
    ...
}
```

This follows the emerging standard for JSON polymorphism, allowing better interoperability with other tools. For example, .NET's System.Text.Json also supports polymorphism, using `$type` as its default discriminator property name ([docs](/dotnet/standard/serialization/system-text-json/polymorphism#customize-the-type-discriminator-name)).

Note this is a breaking change, since EF will no longer be able to query existing documents with the old discriminator property name. See the [breaking change note](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-discriminator-name-change) for details on how to revert to the previous naming.

### Vector similarity search (preview)

Azure Cosmos DB now offers preview support for vector similarity search. Vector search is a fundamental part of some application types, including AI, semantic search and others. Azure Cosmos DB allows you to store vectors directly in your documents alongside the rest of your data, meaning you can perform all of your queries against a single database. This can considerably simplify your architecture and remove the need for an additional, dedicated vector database solution in your stack. To learn more about Azure Cosmos DB vector search, [see the documentation](/azure/cosmos-db/nosql/vector-search).

Once your Azure Cosmos DB container is properly set up, using vector search via EF is a simple matter of adding a vector property and configuring it:

```c#
public class Blog
{
    ...

    public float[] Vector { get; set; }
}

public class BloggingContext
{
    ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Embeddings)
            .IsVector(DistanceFunction.Cosine, dimensions: 1536);
    }
}
```

Once that's done, use the `EF.Functions.VectorDistance()` function in LINQ queries to perform vector similarity search:

```c#
var blogs = await context.Blogs
    .OrderBy(s => EF.Functions.VectorDistance(s.Vector, vector))
    .Take(5)
    .ToListAsync();
```

For more information, see the [documentation on vector search](xref:core/providers/cosmos/vector-search).

### Pagination support

The Azure Cosmos DB provider now allows for paginating through query results via _continuation tokens_, which is far more efficient and cost-effective than the traditional use of `Skip` and `Take`:

```c#
var firstPage = await context.Posts
    .OrderBy(p => p.Id)
    .ToPageAsync(pageSize: 10, continuationToken: null);

var continuationToken = firstPage.ContinuationToken;
foreach (var post in page.Values)
{
    // Display/send the posts to the user
}
```

The new `ToPageAsync` operator returns a `CosmosPage`, which exposes a continuation token that can be used to efficiently resume the query at a later point, fetching the next 10 items:

```c#
var nextPage = await context.Sessions.OrderBy(s => s.Id).ToPageAsync(10, continuationToken);
```

For more information, [see the documentation section on pagination](xref:core/providers/cosmos/querying#pagination).

### FromSql for safer SQL querying

The Azure Cosmos DB provider has allowed SQL querying via <xref:Microsoft.EntityFrameworkCore.CosmosQueryableExtensions.FromSqlRaw*>. However, that API can be susceptible to SQL injection attacks when user-provided data is interpolated or concatenated into the SQL. In EF 9.0, you can now use the new `FromSql` method, which always integrates parameterized data as a parameter outside the SQL:

```c#
var maxAngle = 8;
_ = await context.Blogs
    .FromSql($"SELECT VALUE c FROM root c WHERE c.Angle1 <= {maxAngle}")
    .ToListAsync();
```

For more information, [see the documentation section on pagination](xref:core/providers/cosmos/querying#pagination).

### Role-based access

Azure Cosmos DB for NoSQL includes a [built-in role-based access control (RBAC) system](/azure/cosmos-db/role-based-access-control). This is now supported by EF9 for all data plane operations. However, Azure Cosmos DB SDK does not support RBAC for management plane operations in Azure Cosmos DB. Use Azure Management API instead of `EnsureCreatedAsync` with RBAC.

### Synchronous I/O is now blocked by default

Azure Cosmos DB for NoSQL does not support synchronous (blocking) APIs from application code. Previously, EF masked this by blocking for you on async calls. However, this both encourages synchronous I/O use, which is bad practice, and [may cause deadlocks](https://blog.stephencleary.com/2012/07/dont-block-on-async-code.html). Therefore, starting with EF 9, an exception is thrown when synchronous access is attempted. For example:

Synchronous I/O can still be used for now by configuring the warning level appropriately. For example, in `OnConfiguring` on your `DbContext` type:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.ConfigureWarnings(b => b.Ignore(CosmosEventId.SyncNotSupported));
```

Note, however, that we plan to fully remove sync support in EF 11, so start updating to use async methods like `ToListAsync` and `SaveChangesAsync` as soon as possible!

## AOT and pre-compiled queries

> [!WARNING]
> NativeAOT and query precompilation are highly experimental features, and are not yet suited for production use. The support described below should be viewed as infrastructure towards the final feature, which will likely be released with EF 10. We encourage you to experiment with the current support and report on your experiences, but recommend against deploying EF NativeAOT applications in production.

EF 9.0 brings initial, experimental support for [.NET NativeAOT](/dotnet/core/deploying/native-aot), allowing the publishing of ahead-of-time compiled applications which make use of EF to access databases. To support LINQ queries in NativeAOT mode, EF relies on _query precompilation_: this mechanism statically identifies EF LINQ queries and generates C# [_interceptors_](/dotnet/csharp/whats-new/csharp-12#interceptors), which contain code to execute each specific query. This can significantly cut down on your application's startup time, as the heavy lifting of processing and compiling your LINQ queries into SQL no longer happens every time your application starts up. Instead, each query's interceptor contains the finalized SQL for that query, as well as optimized code to materialize database results as .NET objects.

For example, given a program with the following EF query:

```c#
var blogs = await context.Blogs.Where(b => b.Name == "foo").ToListAsync();
```

EF will generate a C# interceptor into your project, which will take over the query execution. Instead of processing the query and translating it to SQL every time the program starts, the interceptor has the SQL embedded right into it (for SQL Server in this case), allowing your program to start up much faster:

```c#
var relationalCommandTemplate = ((IRelationalCommandTemplate)(new RelationalCommand(materializerLiftableConstantContext.CommandBuilderDependencies, "SELECT [b].[Id], [b].[Name]\nFROM [Blogs] AS [b]\nWHERE [b].[Name] = N'foo'", new IRelationalParameter[] { })));
```

In addition, the same interceptor contains code to materialize your .NET object from database results:

```c#
var instance = new Blog();
UnsafeAccessor_Blog_Id_Set(instance) = dataReader.GetInt32(0);
UnsafeAccessor_Blog_Name_Set(instance) = dataReader.GetString(1);
```

This uses another new .NET feature - [unsafe accessors](/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute), to inject data from the database into your object's private fields.

If you're interested in NativeAOT and like to experiment with cutting-edge features, give this a try! Just be aware that the feature should be considered unstable, and currently has many limitations; we expect to stabilize it and make it more suitable for production usage in EF 10.

See the [NativeAOT documentation page](xref:core/performance/nativeaot-and-precompiled-queries) for more details.

## LINQ and SQL translation

Like with every release, EF9 includes a large number of improvements to the LINQ querying capabilities. New queries can be translated, and many SQL translations for supported scenarios have been improved, for both better performance and readability.

The number of improvements is too great to list them all here. Below, some of the more important improvements are highlighted; see [this issue](https://github.com/dotnet/efcore/issues/34151) for a more complete listing of the work done in 9.0.

We'd like to call out Andrea Canciani ([@ranma42](https://github.com/ranma42)) for his numerous, high-quality contributions to optimizing the SQL that gets generated by EF Core!

<a name="complex-types"></a>

### Complex types: GroupBy and ExecuteUpdate support

#### GroupBy

> [!TIP]
> The code shown here comes from [ComplexTypesSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/ComplexTypesSample.cs).

EF9 supports grouping by a complex type instance. For example:

<!--
        var groupedAddresses = await context.Stores
            .GroupBy(b => b.StoreAddress)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();
-->
[!code-csharp[GroupByComplexType](../../../../samples/core/Miscellaneous/NewInEFCore9/ComplexTypesSample.cs?name=GroupByComplexType)]

EF translates this as grouping by each member of the complex type, which aligns with the semantics of complex types as value objects. For example, on Azure SQL:

```sql
SELECT [s].[StoreAddress_City], [s].[StoreAddress_Country], [s].[StoreAddress_Line1], [s].[StoreAddress_Line2], [s].[StoreAddress_PostCode], COUNT(*) AS [Count]
FROM [Stores] AS [s]
GROUP BY [s].[StoreAddress_City], [s].[StoreAddress_Country], [s].[StoreAddress_Line1], [s].[StoreAddress_Line2], [s].[StoreAddress_PostCode]
```

#### ExecuteUpdate

> [!TIP]
> The code shown here comes from [ExecuteUpdateSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs).

Similarly, in EF9 `ExecuteUpdate` has also been improved to accept complex type properties. However, each member of the complex type must be specified explicitly. For example:

<!--
            #region UpdateComplexType
            var newAddress = new Address("Gressenhall Farm Shop", null, "Beetley", "Norfolk", "NR20 4DR");

            await context.Stores
                .Where(e => e.Region == "Germany")
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.StoreAddress, newAddress));
-->
[!code-csharp[UpdateComplexType](../../../../samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs?name=UpdateComplexType)]

This generates SQL that updates each column mapped to the complex type:

```sql
UPDATE [s]
SET [s].[StoreAddress_City] = @__complex_type_newAddress_0_City,
    [s].[StoreAddress_Country] = @__complex_type_newAddress_0_Country,
    [s].[StoreAddress_Line1] = @__complex_type_newAddress_0_Line1,
    [s].[StoreAddress_Line2] = NULL,
    [s].[StoreAddress_PostCode] = @__complex_type_newAddress_0_PostCode
FROM [Stores] AS [s]
WHERE [s].[Region] = N'Germany'
```

Previously, you had to manually list out the different properties of the complex type in your `ExecuteUpdate` call.

<a name="prune"></a>

### Prune unneeded elements from SQL

Previously, EF sometimes produced SQL which contained elements that weren't actually needed; in most cases, these were possibly needed at an earlier stage of SQL processing, and were left behind. EF9 now prunes most such elements, resulting in more compact and, in some cases, more efficient SQL.

#### Table pruning

As a first example, the SQL generated by EF sometimes contained JOINs to tables which weren't actually needed in the query. Consider the following model, which uses [table-per-type (TPT) inheritance mapping](xref:core/modeling/inheritance#table-per-type-configuration):

```csharp
public class Order
{
    public int Id { get; set; }
    ...

    public Customer Customer { get; set; }
}

public class DiscountedOrder : Order
{
    public double Discount { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    ...

    public List<Order> Orders { get; set; }
}

public class BlogContext : DbContext
{
    ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().UseTptMappingStrategy();
    }
}
```

If we then execute the following query to get all Customers with at least one Order:

```csharp
var customers = await context.Customers.Where(o => o.Orders.Any()).ToListAsync();
```

EF8 generated the following SQL:

```sql
SELECT [c].[Id], [c].[Name]
FROM [Customers] AS [c]
WHERE EXISTS (
    SELECT 1
    FROM [Orders] AS [o]
    LEFT JOIN [DiscountedOrders] AS [d] ON [o].[Id] = [d].[Id]
    WHERE [c].[Id] = [o].[CustomerId])
```

Note that the query contained a join to the `DiscountedOrders` table even though no columns were referenced on it. EF9 generates a pruned SQL without the join:

```c#
SELECT [c].[Id], [c].[Name]
FROM [Customers] AS [c]
WHERE EXISTS (
    SELECT 1
    FROM [Orders] AS [o]
    WHERE [c].[Id] = [o].[CustomerId])
```

#### Projection pruning

Similarly, let's examine the following query:

```csharp
var orders = await context.Orders
    .Where(o => o.Amount > 10)
    .Take(5)
    .CountAsync();
```

On EF8, this query generated the following SQL:

```sql
SELECT COUNT(*)
FROM (
    SELECT TOP(@__p_0) [o].[Id]
    FROM [Orders] AS [o]
    WHERE [o].[Amount] > 10
) AS [t]
```

Note that the `[o].[Id]` projection isn't needed in the subquery, since the outer SELECT expression simply counts the rows. EF9 generates the following instead:

```sql
SELECT COUNT(*)
FROM (
    SELECT TOP(@__p_0) 1 AS empty
    FROM [Orders] AS [o]
    WHERE [o].[Amount] > 10
) AS [s]
```

... and the projection is empty. This may not seem like much, but it can significantly simplify the SQL in some cases; you're welcome to scroll through some of the [SQL changes in the tests](https://github.com/dotnet/efcore/pull/32672/files#diff-95664269d9a59fe9627612bf1d3e1704e76f6065e329edeecd14a8bf436db058L4011) to see the effect.

<a name="greatest"></a>

### Translations involving GREATEST/LEAST

> [!TIP]
> The code shown here comes from [LeastGreatestSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/LeastGreatestSample.cs).

Several new translations have been introduced that use the `GREATEST` and `LEAST` SQL functions.

> [!IMPORTANT]
> The `GREATEST` and `LEAST` functions were [introduced to SQL Server/Azure SQL databases in the 2022 version](https://techcommunity.microsoft.com/t5/azure-sql-blog/introducing-the-greatest-and-least-t-sql-functions/ba-p/2281726). Visual Studio 2022 installs SQL Server 2019 by default. We recommend installing [SQL Server Developer Edition 2022](https://www.microsoft.com/sql-server/sql-server-downloads) to try out these new translations in EF9.

For example, queries using `Math.Max` or `Math.Min` are now translated for Azure SQL using `GREATEST` and `LEAST` respectively. For example:

<!--
        #region MathMin
        var walksUsingMin = await context.Walks
            .Where(e => Math.Min(e.DaysVisited.Count, e.ClosestPub.Beers.Length) > 4)
            .ToListAsync();
-->
[!code-csharp[MathMin](../../../../samples/core/Miscellaneous/NewInEFCore9/LeastGreatestSample.cs?name=MathMin)]

This query is translated to the following SQL when using EF9 executing against SQL Server 2022:

```sql
SELECT [w].[Id], [w].[ClosestPubId], [w].[DaysVisited], [w].[Name], [w].[Terrain]
FROM [Walks] AS [w]
INNER JOIN [Pubs] AS [p] ON [w].[ClosestPubId] = [p].[Id]
WHERE LEAST((
    SELECT COUNT(*)
    FROM OPENJSON([w].[DaysVisited]) AS [d]), (
    SELECT COUNT(*)
    FROM OPENJSON([p].[Beers]) AS [b])) >
```

`Math.Min` and `Math.Max` can also be used on the values of a primitive collection. For example:

<!--
        #region MathMinPrimitive
        var pubsInlineMax = await context.Pubs
            .SelectMany(e => e.Counts)
            .Where(e => Math.Max(e, threshold) > top)
            .ToListAsync();
-->
[!code-csharp[MathMinPrimitive](../../../../samples/core/Miscellaneous/NewInEFCore9/LeastGreatestSample.cs?name=MathMinPrimitive)]

This query is translated to the following SQL when using EF9 executing against SQL Server 2022:

```sql
SELECT [c].[value]
FROM [Pubs] AS [p]
CROSS APPLY OPENJSON([p].[Counts]) WITH ([value] int '$') AS [c]
WHERE GREATEST([c].[value], @__threshold_0) > @__top_1
```

Finally, `RelationalDbFunctionsExtensions.Least` and `RelationalDbFunctionsExtensions.Greatest` can be used to directly invoke the `Least` or `Greatest` function in SQL. For example:

<!--
        #region Least
        var leastCount = await context.Pubs
            .Select(e => EF.Functions.Least(e.Counts.Length, e.DaysVisited.Count, e.Beers.Length))
            .ToListAsync();
-->
[!code-csharp[Least](../../../../samples/core/Miscellaneous/NewInEFCore9/LeastGreatestSample.cs?name=Least)]

This query is translated to the following SQL when using EF9 executing against SQL Server 2022:

```sql
SELECT LEAST((
    SELECT COUNT(*)
    FROM OPENJSON([p].[Counts]) AS [c]), (
    SELECT COUNT(*)
    FROM OPENJSON([p].[DaysVisited]) AS [d]), (
    SELECT COUNT(*)
    FROM OPENJSON([p].[Beers]) AS [b]))
FROM [Pubs] AS [p]
```

<a name="parameterization"></a>

### Force or prevent query parameterization

> [!TIP]
> The code shown here comes from [QuerySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs).

Except in some special cases, EF Core parameterizes variables used in a LINQ query, but includes constants in the generated SQL. For example, consider the following query method:

<!--
        #region DefaultParameterization
        async Task<List<Post>> GetPosts(int id)
            => await context.Posts
                .Where(e => e.Title == ".NET Blog" && e.Id == id)
                .ToListAsync();
-->
[!code-csharp[DefaultParameterization](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=DefaultParameterization)]

This translates to the following SQL and parameters when using Azure SQL:

```output
Executed DbCommand (1ms) [Parameters=[@__id_0='1'], CommandType='Text', CommandTimeout='30']
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] = @__id_0
```

Notice that EF created a constant in the SQL for ".NET Blog" because this value will not change from query to query. Using a constant allows this value to be examined by the database engine when creating a query plan, potentially resulting in a more efficient query.

On the other hand, the value of `id` is parameterized, since the same query may be executed with many different values for `id`. Creating a constant in this case would result in pollution of the query cache with lots of queries that differ only in `id` values. This is very bad for overall performance of the database.

Generally speaking, these defaults should not be changed. However, EF Core 8.0.2 introduces an `EF.Constant` method which forces EF to use a constant even if a parameter would be used by default. For example:

<!--
        #region ForceConstant
        async Task<List<Post>> GetPostsForceConstant(int id)
            => await context.Posts
                .Where(e => e.Title == ".NET Blog" && e.Id == EF.Constant(id))
                .ToListAsync();
-->
[!code-csharp[ForceConstant](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ForceConstant)]

The translation now contains a constant for the `id` value:

```output
Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] = 1
```

#### The `EF.Parameter` method

EF9 introduces the `EF.Parameter` method to do the opposite. That is, force EF to use a parameter even if the value is a constant in code. For example:

<!--
        #region ForceParameter
        async Task<List<Post>> GetPostsForceParameter(int id)
            => await context.Posts
                .Where(e => e.Title == EF.Parameter(".NET Blog") && e.Id == id)
                .ToListAsync();
-->
[!code-csharp[ForceParameter](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ForceParameter)]

The translation now contains a parameter for the ".NET Blog" string:

```output
Executed DbCommand (1ms) [Parameters=[@__p_0='.NET Blog' (Size = 4000), @__id_1='1'], CommandType='Text', CommandTimeout='30']
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] = @__p_0 AND [p].[Id] = @__id_1
```

<a name="parameterized-collections"></a>

#### Parameterized primitive collections

EF8 changed the way [some queries that use primitive collections are translated](xref:core/what-is-new/ef-core-8.0/whatsnew#queries-with-primitive-collections). When a LINQ query contains a parameterized primitive collection, EF converts its contents to JSON and pass it as a single parameter value the query:

<!--
#region DefaultParameterizationPrimitiveCollection
async Task<List<Post>> GetPostsPrimitiveCollection(int[] ids)
    => await context.Posts
        .Where(e => e.Title == ".NET Blog" && ids.Contains(e.Id))
        .ToListAsync();
-->
[!code-csharp[ForceParameter](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=DefaultParameterizationPrimitiveCollection)]

This will result in the following translation on SQL Server:

```output
Executed DbCommand (5ms) [Parameters=[@__ids_0='[1,2,3]' (Size = 4000)], CommandType='Text', CommandTimeout='30']
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Rating], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] IN (
    SELECT [i].[value]
    FROM OPENJSON(@__ids_0) WITH ([value] int '$') AS [i]
)
```

This allows having the same SQL query for different parameterized collections (only the parameter value changes), but in some situations it can lead to performance issues as the database isn't able to optimally plan for the query. The `EF.Constant` method can be used to revert to the previous translation.

The following query uses `EF.Constant` to that effect:

<!--
#region ForceConstantPrimitiveCollection
async Task<List<Post>> GetPostsForceConstantCollection(int[] ids)
    => await context.Posts
        .Where(e => e.Title == ".NET Blog" && EF.Constant(ids).Contains(e.Id))
        .ToListAsync();
-->
[!code-csharp[ForceParameter](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ForceConstantPrimitiveCollection)]

The resulting SQL is as follows:

```sql
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Rating], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] IN (1, 2, 3)
```

Moreover, EF9 introduces `TranslateParameterizedCollectionsToConstants` [context option](/ef/core/dbcontext-configuration/#dbcontextoptions) that can be used to prevent primitive collection parameterization for all queries. We also added a complementing `TranslateParameterizedCollectionsToParameters` which forces parameterization of primitive collections explicitly (this is the default behavior).

> [!TIP]
> The `EF.Parameter` method overrides the context option. If you want to prevent parameterization of primitive collections for most of your queries (but not all), you can set the context option `TranslateParameterizedCollectionsToConstants` and use `EF.Parameter` for the queries or individual variables that you want to parameterize.

<a name="inlinedsubs"></a>

### Inlined uncorrelated subqueries

> [!TIP]
> The code shown here comes from [QuerySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs).

In EF8, an IQueryable referenced in another query may be executed as a separate database roundtrip. For example, consider the following LINQ query:

<!--
        #region InlinedSubquery
        var dotnetPosts = context
            .Posts
            .Where(p => p.Title.Contains(".NET"));

        var results = dotnetPosts
            .Where(p => p.Id > 2)
            .Select(p => new { Post = p, TotalCount = dotnetPosts.Count() })
            .Skip(2).Take(10)
            .ToArray();
-->
[!code-csharp[InlinedSubquery](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=InlinedSubquery)]

In EF8, the query for `dotnetPosts` is executed as one round trip, and then the final results are executed as second query. For example, on SQL Server:

```sql
SELECT COUNT(*)
FROM [Posts] AS [p]
WHERE [p].[Title] LIKE N'%.NET%'

SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE [p].[Title] LIKE N'%.NET%' AND [p].[Id] > 2
ORDER BY (SELECT 1)
OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
```

In EF9, the `IQueryable` in the `dotnetPosts` is inlined, resulting in a single database round trip:

```sql
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata], (
    SELECT COUNT(*)
    FROM [Posts] AS [p0]
    WHERE [p0].[Title] LIKE N'%.NET%')
FROM [Posts] AS [p]
WHERE [p].[Title] LIKE N'%.NET%' AND [p].[Id] > 2
ORDER BY (SELECT 1)
OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
```

<a name="aggregate-over-subquery"></a>

### Aggregate functions over subqueries and aggregates on SQL Server

EF9 improves the translation of some complex queries using aggregate functions composed over subqueries or other aggregate functions.
Below is an example of such query:

<!--
var latestPostsAverageRatingByLanguage = await context.Blogs.
    Select(x => new
    {
        x.Language,
        LatestPostRating = x.Posts.OrderByDescending(xx => xx.PublishedOn).FirstOrDefault().Rating
    })
    .GroupBy(x => x.Language)
    .Select(x => x.Average(xx => xx.LatestPostRating))
    .ToListAsync();
-->
[!code-csharp[AggregateOverSubquery](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=AggregateOverSubquery)]

First, `Select` computes `LatestPostRating` for each `Post` which requires a subquery when translating to SQL. Later in the query these results are aggregated using `Average` operation. The resulting SQL looks as follows when run on SQL Server:

```sql
SELECT AVG([s].[Rating])
FROM [Blogs] AS [b]
OUTER APPLY (
    SELECT TOP(1) [p].[Rating]
    FROM [Posts] AS [p]
    WHERE [b].[Id] = [p].[BlogId]
    ORDER BY [p].[PublishedOn] DESC
) AS [s]
GROUP BY [b].[Language]
```

In previous versions EF Core would generate invalid SQL for similar queries, trying to apply the aggregate operation directly over the subquery. This is not allowed on SQL Server and results in an exception.
Same principle applies to queries using aggregate over another aggregate:

<!--
var topRatedPostsAverageRatingByLanguage = await context.Blogs.
    Select(x => new
    {
        x.Language,
        TopRating = x.Posts.Max(x => x.Rating)
    })
    .GroupBy(x => x.Language)
    .Select(x => x.Average(xx => xx.TopRating))
    .ToListAsync();
-->
[!code-csharp[AggregateOverAggregate](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=AggregateOverAggregate)]

> [!NOTE]
> This change doesn't affect Sqlite, which supports aggregates over subqueries (or other aggregates) and it does not support `LATERAL JOIN` (`APPLY`). Below is the SQL for the first query running on Sqlite:
>
> ```sql
> SELECT ef_avg((
>     SELECT "p"."Rating"
>     FROM "Posts" AS "p"
>     WHERE "b"."Id" = "p"."BlogId"
>     ORDER BY "p"."PublishedOn" DESC
>     LIMIT 1))
> FROM "Blogs" AS "b"
> GROUP BY "b"."Language"
> ```

<a name="count-not-zero"></a>

### Queries using Count != 0 are optimized

> [!TIP]
> The code shown here comes from [QuerySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs).

In EF8, the following LINQ query was translated to use the SQL `COUNT` function:

<!--
        var blogsWithPost = await context.Blogs
            .Where(b => b.Posts.Count > 0)
            .ToListAsync();
-->
[!code-csharp[NormalizeCount](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=NormalizeCount)]

EF9 now generates a more efficient translation using `EXISTS`:

```sql
SELECT "b"."Id", "b"."Name", "b"."SiteUri"
FROM "Blogs" AS "b"
WHERE EXISTS (
    SELECT 1
    FROM "Posts" AS "p"
    WHERE "b"."Id" = "p"."BlogId")
```

<a name="comparison-null-semantics"></a>

### C# semantics for comparison operations on nullable values

In EF8 comparisons between nullable elements were not performed correctly for some scenarios. In C#, if one or both operands are null, the result of a comparison operation is false; otherwise, the contained values of operands are compared. In EF8 we used to translate comparisons using database null semantics. This would produce results different than similar query using LINQ to Objects.
Moreover, we would produce different results when comparison was done in filter vs projection. Some queries would also produce different results between Sql Server and Sqlite/Postgres.

For example, the query:

<!--
var negatedNullableComparisonFilter = await context.Entities
    .Where(x => !(x.NullableIntOne > x.NullableIntTwo))
    .Select(x => new { x.NullableIntOne, x.NullableIntTwo }).ToListAsync();
-->
[!code-csharp[NegatedNullableComparisonFilter](../../../../samples/core/Miscellaneous/NewInEFCore9/NullSemanticsSample.cs?name=NegatedNullableComparisonFilter)]

would generate the following SQL:

```sql
SELECT [e].[NullableIntOne], [e].[NullableIntTwo]
FROM [Entities] AS [e]
WHERE NOT ([e].[NullableIntOne] > [e].[NullableIntTwo])
```

which filters out entities whose `NullableIntOne` or `NullableIntTwo` are set to null.

In EF9 we produce:

```sql
SELECT [e].[NullableIntOne], [e].[NullableIntTwo]
FROM [Entities] AS [e]
WHERE CASE
    WHEN [e].[NullableIntOne] > [e].[NullableIntTwo] THEN CAST(0 AS bit)
    ELSE CAST(1 AS bit)
END = CAST(1 AS bit)
```

Similar comparison performed in a projection:

<!--
var negatedNullableComparisonProjection = await context.Entities.Select(x => new
{
    x.NullableIntOne,
    x.NullableIntTwo,
    Operation = !(x.NullableIntOne > x.NullableIntTwo)
}).ToListAsync();
-->
[!code-csharp[NegatedNullableComparisonProjection](../../../../samples/core/Miscellaneous/NewInEFCore9/NullSemanticsSample.cs?name=NegatedNullableComparisonProjection)]

resulted in the following SQL:

```sql
SELECT [e].[NullableIntOne], [e].[NullableIntTwo], CASE
    WHEN NOT ([e].[NullableIntOne] > [e].[NullableIntTwo]) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Operation]
FROM [Entities] AS [e]
```

which returns `false` for entities whose `NullableIntOne` or `NullableIntTwo` are set to null (rather than `true` expected in C#). Running the same scenario on Sqlite generated:

```sql
SELECT "e"."NullableIntOne", "e"."NullableIntTwo", NOT ("e"."NullableIntOne" > "e"."NullableIntTwo") AS "Operation"
FROM "Entities" AS "e"
```

which results in `Nullable object must have a value` exception, as translation produces `null` value for cases where `NullableIntOne` or `NullableIntTwo` are null.

EF9 now properly handles these scenarios, producing results consistent with LINQ to Objects and across different providers.

This enhancement was contributed by [@ranma42](https://github.com/ranma42). Many thanks!

<a name="order-operator"></a>

### Translation of `Order` and `OrderDescending` LINQ operators

EF9 enables the translation of LINQ simplified ordering operations (`Order` and `OrderDescending`). These work similar to `OrderBy`/`OrderByDescending` but don't require an argument. Instead, they apply default ordering - for entities this means ordering based on primary key values and for other types, ordering based on the values themselves.

Below is an example query which takes advantage of the simplified ordering operators:

<!--
var orderOperation = await context.Blogs
    .Order()
    .Select(x => new
    {
        x.Name,
        OrderedPosts = x.Posts.OrderDescending().ToList(),
        OrderedTitles = x.Posts.Select(xx => xx.Title).Order().ToList()
    })
    .ToListAsync();
-->
[!code-csharp[OrderOrderDescending](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=OrderOrderDescending)]

This query is equivalent to the following:

<!--
var orderByEquivalent = await context.Blogs
    .OrderBy(x => x.Id)
    .Select(x => new
    {
        x.Name,
        OrderedPosts = x.Posts.OrderByDescending(xx => xx.Id).ToList(),
        OrderedTitles = x.Posts.Select(xx => xx.Title).OrderBy(xx => xx).ToList()
    })
    .ToListAsync();
-->
[!code-csharp[OrderByEquivalent](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=OrderByEquivalent)]

and produces the following SQL:

```sql
SELECT [b].[Name], [b].[Id], [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Rating], [p].[Title], [p].[PromoText], [p].[Metadata], [p0].[Title], [p0].[Id]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
LEFT JOIN [Posts] AS [p0] ON [b].[Id] = [p0].[BlogId]
ORDER BY [b].[Id], [p].[Id] DESC, [p0].[Title]
```

> [!NOTE]
> `Order` and `OrderDescending` methods are only supported for collections of entities, complex types or scalars - they will not work on more complex projections, e.g. collections of anonymous types containing multiple properties.

This enhancement was contributed by the EF Team alumnus [@bricelam](https://github.com/bricelam). Many thanks!

<a name="improved-negation"></a>

### Improved translation of logical negation operator (!)

EF9 brings many optimizations around SQL `CASE/WHEN`, `COALESCE`, negation, and various other constructs; most of these were contributed by Andrea Canciani ([@ranma42](https://github.com/ranma42)) - many thanks for all of these! Below, we'll detail just a few of these optimizations around logical negation.

Let's examine the following query:

<!--
var negatedContainsSimplification = await context.Posts
    .Where(p => !p.Content.Contains("Announcing"))
    .Select(p => new { p.Content }).ToListAsync();
-->
[!code-csharp[NegatedContainsImprovements](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=NegatedContainsImprovements)]

In EF8 we would produce the following SQL:

```sql
SELECT "p"."Content"
FROM "Posts" AS "p"
WHERE NOT (instr("p"."Content", 'Announcing') > 0)
```

In EF9 we "push" `NOT` operation into the comparison:

```sql
SELECT "p"."Content"
FROM "Posts" AS "p"
WHERE instr("p"."Content", 'Announcing') <= 0
```

Another example, applicable to SQL Server, is a negated conditional operation.

<!--
var caseSimplification = await context.Blogs
    .Select(b => !(b.Id > 5 ? false : true))
    .ToListAsync();
-->
[!code-csharp[CaseTranslationImprovements](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=CaseTranslationImprovements)]

In EF8 used to result in nested `CASE` blocks:

```sql
SELECT CASE
    WHEN CASE
        WHEN [b].[Id] > 5 THEN CAST(0 AS bit)
        ELSE CAST(1 AS bit)
    END = CAST(0 AS bit) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [Blogs] AS [b]
```

In EF9 we removed the nesting:

```sql
SELECT CASE
    WHEN [b].[Id] > 5 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [Blogs] AS [b]
```

On SQL Server, when projecting a negated bool property:

<!--
var negatedBoolProjection = await context.Posts.Select(x => new { x.Title, Active = !x.Archived }).ToListAsync();
-->
[!code-csharp[NegatedBoolProjection](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=NegatedBoolProjection)]

 EF8 would generate a `CASE` block because comparisons can't appear in the projection directly in SQL Server queries:

 ```sql
SELECT [p].[Title], CASE
    WHEN [p].[Archived] = CAST(0 AS bit) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Active]
FROM [Posts] AS [p]
 ```

In EF9, this translation has been simplified and now uses bitwise NOT (`~`):

```sql
SELECT [p].[Title], ~[p].[Archived] AS [Active]
FROM [Posts] AS [p]
```

<a name="azuresql-azuresynapse"></a>

### Better support for Azure SQL and Azure Synapse

EF9 allows for more flexibility when specifying the type of SQL Server which is being targeted. Instead of configuring EF with `UseSqlServer`, you can now specify `UseAzureSql` or `UseAzureSynapse`.
This allows EF to produce better SQL when using Azure SQL or Azure Synapse. EF can take advantage of the database specific features (e.g. [dedicated type for JSON on Azure SQL](/sql/t-sql/data-types/json-data-type)), or work around its limitations (e.g. [`ESCAPE` clause is not available when using `LIKE` on Azure Synapse](/sql/t-sql/language-elements/like-transact-sql#syntax)).

### Other query improvements

* The primitive collections querying support [introduced in EF8](xref:core/what-is-new/ef-core-8.0/whatsnew#queries-with-primitive-collections) has been extended to support all `ICollection<T>` types. Note that this applies only to parameter and inline collections - primitive collections that are part of entities are still limited to arrays, lists and [in EF9 also read-only arrays/lists](#read-only-primitive-collections).
* New `ToHashSetAsync` functions to return the results of a query as a `HashSet` ([#30033](https://github.com/dotnet/efcore/issues/30033), contributed by [@wertzui](https://github.com/wertzui)).
* `TimeOnly.FromDateTime` and `FromTimeSpan` are now translated on SQL Server ([#33678](https://github.com/dotnet/efcore/issues/33678)).
* `ToString` over enums is now translated ([#33706](https://github.com/dotnet/efcore/pull/33706), contributed by [@Danevandy99](https://github.com/Danevandy99)).
* `string.Join` now translates to [CONCAT_WS](/sql/t-sql/functions/concat-ws-transact-sql) in non-aggregate context on SQL Server ([#28899](https://github.com/dotnet/efcore/issues/28899)).
* `EF.Functions.PatIndex` now translates to the SQL Server [`PATINDEX`](/sql/t-sql/functions/patindex-transact-sql) function, which returns the starting position of the first occurrence of a pattern ([#33702](https://github.com/dotnet/efcore/issues/33702), [@smnsht](https://github.com/smnsht)).
* `Sum` and `Average` now work for decimals on SQLite ([#33721](https://github.com/dotnet/efcore/pull/33721), contributed by [@ranma42](https://github.com/ranma42)).
* Fixes and optimizations to `string.StartsWith` and `EndsWith` ([#31482](https://github.com/dotnet/efcore/pull/31482)).
* `Convert.To*` methods can now accept argument of type `object` ([#33891](https://github.com/dotnet/efcore/pull/33891), contributed by [@imangd](https://github.com/imangd)).
* Exclusive-Or (XOR) operation is now translated on SQL Server ([#34071](https://github.com/dotnet/efcore/pull/34071), contributed by [@ranma42](https://github.com/ranma42)).
* Optimizations around nullability for `COLLATE` and `AT TIME ZONE` operations ([#34263](https://github.com/dotnet/efcore/pull/34263), contributed by [@ranma42](https://github.com/ranma42)).
* Optimizations for `DISTINCT` over `IN`, `EXISTS` and set operations ([#34381](https://github.com/dotnet/efcore/pull/34381), contributed by [@ranma42](https://github.com/ranma42)).

The above were only some of the more important query improvements in EF9; see [this issue](https://github.com/dotnet/efcore/issues/34151) for a more complete listing.

## Migrations

<a name="concurrent-migrations"></a>

### Protection against concurrent migrations

EF9 introduces a locking mechanism to protect against multiple migration executions happening simultaneously, as that could leave the database in a corrupted state. This doesn't happen when migrations are deployed to the production environment using [recommended methods](/ef/core/managing-schemas/migrations/applying#sql-scripts), but can happen if migrations are applied at runtime using the [`DbContext.Database.MigrateAsync()`](/dotnet/api/microsoft.entityframeworkcore.relationaldatabasefacadeextensions.migrate) method. We recommend applying migrations at deployment, rather than as part of application startup, but that can result in more complicated application architectures (e.g. [when using .NET Aspire projects](/dotnet/aspire/database/ef-core-migrations)).

> [!NOTE]
> If you are using Sqlite database, see [potential issues associated with this feature](/ef/core/providers/sqlite/limitations#concurrent-migrations-protection).

<a name="warn-when-no-transaction"></a>

### Warn when multiple migration operations can't be run inside a transaction

The majority of operations performed during migrations are protected by a transaction. This ensures that if for some reason migration fails, the database does not end up in a corrupted state. However, some operations are not wrapped in a transaction (e.g. [operations on SQL Server memory-optimized tables](/sql/relational-databases/in-memory-oltp/unsupported-sql-server-features-for-in-memory-oltp#scenarios-not-supported), or database altering operations like modifying the database collation). To avoid corrupting the database in case of migration failure, it is recommended that these operations are performed in isolation using a separate migration. EF9 now detects a scenario when a migration contains multiple operations, one of which can't be wrapped in a transaction, and issues a warning.

### Improved data seeding

EF9 introduced a convenient way to perform data seeding, that is populating the database with initial data. `DbContextOptionsBuilder` now contains `UseSeeding` and `UseAsyncSeeding` methods which get executed when the DbContext is initialized (as part of `EnsureCreatedAsync`).

> [!NOTE]
> If the application had ran previously, the database may already contain the sample data (which would have been added on the first initialization of the context). As such, `UseSeeding` `UseAsyncSeeding` should check if data exists before attempting to populate the database. This can be achieved by issuing a simple EF query.

Here is an example of how these methods can be used:

[!code-csharp[ContextOptionSeeding](../../../../samples/core/Modeling/DataSeeding/DataSeedingContext.cs?name=ContextOptionSeeding)]

More information can be found [here](/ef/core/modeling/data-seeding#use-seeding-method).

### Other migration improvements

* When changing an existing table into a SQL Server temporal table, the migration code size has been significantly reduced.

## Model building

<a name="auto-compiled-models"></a>

### Auto-compiled models

> [!TIP]
> The code shown here comes from the [NewInEFCore9.CompiledModels](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9.CompiledModels/) sample.

Compiled models can improve startup time for applications with large models--that is entity type counts in the 100s or 1000s. In previous versions of EF Core, a compiled model had to be generated manually, using the command line. For example:

```dotnetcli
dotnet ef dbcontext optimize
```

After running the command, a line like, `.UseModel(MyCompiledModels.BlogsContextModel.Instance)` must be added to `OnConfiguring` to tell EF Core to use the compiled model.

Starting with EF9, this `.UseModel` line is no longer needed when the application's `DbContext` type is in the same project/assembly as the compiled model. Instead, the compiled model will be detected and used automatically. This can be seen by having EF log whenever it is building the model. Running a simple application then shows EF building the model when the application starts:

```output
Starting application...
>> EF is building the model...
Model loaded with 2 entity types.
```

The output from running `dotnet ef dbcontext optimize` on the model project is:

```output
PS D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\Model> dotnet ef dbcontext optimize

Build succeeded in 0.3s

Build succeeded in 0.3s
Build started...
Build succeeded.
>> EF is building the model...
>> EF is building the model...
Successfully generated a compiled model, it will be discovered automatically, but you can also call 'options.UseModel(BlogsContextModel.Instance)'. Run this command again when the model is modified.
PS D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\Model> 
```

Notice that the log output indicates that the _model was built when running the command_. If we now run the application again, after rebuilding but without making any code changes, then the output is:

```output
Starting application...
Model loaded with 2 entity types.
```

Notice that the model was not built when starting the application because the compiled model was detected and used automatically.

### MSBuild integration

With the above approach, the compiled model still needs to be regenerated manually when the entity types or `DbContext` configuration is changed. However, EF9 ships with an MSBuild task package that can automatically update the compiled model when the model project is built! To get started, install the [Microsoft.EntityFrameworkCore.Tasks](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tasks/) NuGet package. For example:

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Tasks --version 9.0.0
```

> [!TIP]
> Use the package version in the command above that matches the version of EF Core that you are using.

Then enable the integration by setting the `EFOptimizeContext` and `EFScaffoldModelStage` properties in your `.csproj` file. For example:

```xml
<PropertyGroup>
    <EFOptimizeContext>true</EFOptimizeContext>
    <EFScaffoldModelStage>build</EFScaffoldModelStage>
</PropertyGroup>
```

Now, if we build the project, we can see logging at build time indicating that the compiled model is being built:

```output
Optimizing DbContext...
dotnet exec --depsfile D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\App\bin\Release\net8.0\App.deps.json
  --additionalprobingpath G:\packages 
  --additionalprobingpath "C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages" 
  --runtimeconfig D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\App\bin\Release\net8.0\App.runtimeconfig.json G:\packages\microsoft.entityframeworkcore.tasks\9.0.0-preview.4.24205.3\tasks\net8.0\..\..\tools\netcoreapp2.0\ef.dll dbcontext optimize --output-dir D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\Model\obj\Release\net8.0\ 
  --namespace NewInEfCore9 
  --suffix .g 
  --assembly D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\Model\bin\Release\net8.0\Model.dll
  --project-dir D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\Model 
  --root-namespace NewInEfCore9 
  --language C# 
  --nullable 
  --working-dir D:\code\EntityFramework.Docs\samples\core\Miscellaneous\NewInEFCore9.CompiledModels\App 
  --verbose 
  --no-color 
  --prefix-output 
```

And running the application shows that the compiled model has been detected and hence the model is not built again:

```output
Starting application...
Model loaded with 2 entity types.
```

Now, whenever the model changes, the compiled model will be automatically rebuilt as soon as the project is built.

For more information see [MSBuild integration](xref:core/cli/msbuild).

<a name="read-only-primitives"></a>

### Read-only primitive collections

> [!TIP]
> The code shown here comes from [PrimitiveCollectionsSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/PrimitiveCollectionsSample.cs).

EF8 introduced support for [mapping arrays and mutable lists of primitive types](xref:core/what-is-new/ef-core-8.0/whatsnew#primitive-collections). This has been expanded in EF9 to include read-only collections/lists. Specifically, EF9 supports collections typed as `IReadOnlyList`, `IReadOnlyCollection`, or `ReadOnlyCollection`. For example, in the following code, `DaysVisited` will be mapped by convention as a primitive collection of dates:

```csharp
public class DogWalk
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ReadOnlyCollection<DateOnly> DaysVisited { get; set; }
}
```

The read-only collection can be backed by a normal, mutable collection if desired. For example, in the following code, `DaysVisited` can be mapped as a primitive collection of dates, while still allowing code in the class to manipulate the underlying list.

```csharp
    public class Pub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IReadOnlyCollection<string> Beers { get; set; }

        private List<DateOnly> _daysVisited = new();
        public IReadOnlyList<DateOnly> DaysVisited => _daysVisited;
    }
```

These collections can then be used in queries in the normal way. For example, this LINQ query:

<!--
        var walksWithADrink = await context.Walks.Select(
            w => new
            {
                WalkName = w.Name,
                PubName = w.ClosestPub.Name,
                Count = w.DaysVisited.Count(v => w.ClosestPub.DaysVisited.Contains(v)),
                TotalCount = w.DaysVisited.Count
            }).ToListAsync();
-->
[!code-csharp[WalksWithADrink](../../../../samples/core/Miscellaneous/NewInEFCore9/PrimitiveCollectionsSample.cs?name=WalksWithADrink)]

Which translates to the following SQL on SQLite:

```sql
SELECT "w"."Name" AS "WalkName", "p"."Name" AS "PubName", (
    SELECT COUNT(*)
    FROM json_each("w"."DaysVisited") AS "d"
    WHERE "d"."value" IN (
        SELECT "d0"."value"
        FROM json_each("p"."DaysVisited") AS "d0"
    )) AS "Count", json_array_length("w"."DaysVisited") AS "TotalCount"
FROM "Walks" AS "w"
INNER JOIN "Pubs" AS "p" ON "w"."ClosestPubId" = "p"."Id"
```

<a name="sequence-caching"></a>

### Specify fill-factor for keys and indexes

> [!TIP]
> The code shown here comes from [ModelBuildingSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/ModelBuildingSample.cs).

EF9 supports specification of the [SQL Server fill-factor](/sql/relational-databases/indexes/specify-fill-factor-for-an-index) when using EF Core Migrations to create keys and indexes. From the SQL Server docs, "When an index is created or rebuilt, the fill-factor value determines the percentage of space on each leaf-level page to be filled with data, reserving the remainder on each page as free space for future growth."

The fill-factor can be set on a single or composite primary and alternate keys and indexes. For example:

<!--
            #region FillFactor
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id)
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasAlternateKey(e => new { e.Region, e.Ssn })
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Name })
                .HasFillFactor(80);

            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Region, e.Tag })
                .HasFillFactor(80);
-->
[!code-csharp[FillFactor](../../../../samples/core/Miscellaneous/NewInEFCore9/ModelBuildingSample.cs?name=FillFactor)]

When applied to existing tables, this will alter the tables to the fill-factor to the constraint:

```sql
ALTER TABLE [User] DROP CONSTRAINT [AK_User_Region_Ssn];
ALTER TABLE [User] DROP CONSTRAINT [PK_User];
DROP INDEX [IX_User_Name] ON [User];
DROP INDEX [IX_User_Region_Tag] ON [User];

ALTER TABLE [User] ADD CONSTRAINT [AK_User_Region_Ssn] UNIQUE ([Region], [Ssn]) WITH (FILLFACTOR = 80);
ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]) WITH (FILLFACTOR = 80);
CREATE INDEX [IX_User_Name] ON [User] ([Name]) WITH (FILLFACTOR = 80);
CREATE INDEX [IX_User_Region_Tag] ON [User] ([Region], [Tag]) WITH (FILLFACTOR = 80);
```

This enhancement was contributed by [@deano-hunter](https://github.com/deano-hunter). Many thanks!

<a name="conventions"></a>

### Make existing model building conventions more extensible

> [!TIP]
> The code shown here comes from [CustomConventionsSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/CustomConventionsSample.cs).

Public model building conventions for applications were [introduced in EF7](xref:core/modeling/bulk-configuration#Conventions). In EF9, we have made it easier to extend some of the existing conventions. For example, [the code to map properties by attribute in EF7](xref:core/what-is-new/ef-core-7.0/whatsnew#model-building-conventions) is this:

```csharp
public class AttributeBasedPropertyDiscoveryConvention : PropertyDiscoveryConvention
{
    public AttributeBasedPropertyDiscoveryConvention(ProviderConventionSetBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
        => Process(entityTypeBuilder);

    public override void ProcessEntityTypeBaseTypeChanged(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionEntityType? newBaseType,
        IConventionEntityType? oldBaseType,
        IConventionContext<IConventionEntityType> context)
    {
        if ((newBaseType == null
             || oldBaseType != null)
            && entityTypeBuilder.Metadata.BaseType == newBaseType)
        {
            Process(entityTypeBuilder);
        }
    }

    private void Process(IConventionEntityTypeBuilder entityTypeBuilder)
    {
        foreach (var memberInfo in GetRuntimeMembers())
        {
            if (Attribute.IsDefined(memberInfo, typeof(PersistAttribute), inherit: true))
            {
                entityTypeBuilder.Property(memberInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo
                     && Dependencies.TypeMappingSource.FindMapping(propertyInfo) != null)
            {
                entityTypeBuilder.Ignore(propertyInfo.Name);
            }
        }

        IEnumerable<MemberInfo> GetRuntimeMembers()
        {
            var clrType = entityTypeBuilder.Metadata.ClrType;

            foreach (var property in clrType.GetRuntimeProperties()
                         .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic))
            {
                yield return property;
            }

            foreach (var property in clrType.GetRuntimeFields())
            {
                yield return property;
            }
        }
    }
}
```

In EF9, this can be simplified down to the following:

<!--
    #region AttributeBasedPropertyDiscoveryConvention
    public class AttributeBasedPropertyDiscoveryConvention(ProviderConventionSetBuilderDependencies dependencies)
        : PropertyDiscoveryConvention(dependencies)
    {
        protected override bool IsCandidatePrimitiveProperty(
            MemberInfo memberInfo, IConventionTypeBase structuralType, out CoreTypeMapping? mapping)
        {
            if (base.IsCandidatePrimitiveProperty(memberInfo, structuralType, out mapping))
            {
                if (Attribute.IsDefined(memberInfo, typeof(PersistAttribute), inherit: true))
                {
                    return true;
                }

                structuralType.Builder.Ignore(memberInfo.Name);
            }

            mapping = null;
            return false;
        }
    }
-->
[!code-csharp[AttributeBasedPropertyDiscoveryConvention](../../../../samples/core/Miscellaneous/NewInEFCore9/CustomConventionsSample.cs?name=AttributeBasedPropertyDiscoveryConvention)]

<a name="non-pub-apply"></a>

### Update ApplyConfigurationsFromAssembly to call non-public constructors

In previous versions of EF Core, the `ApplyConfigurationsFromAssembly` method only instantiated configuration types with a public, parameterless constructors. In EF9, we have both [improved the error messages generated when this fails](https://github.com/dotnet/efcore/pull/32577), and also enabled instantiation by non-public constructor. This is useful when co-locating configuration in a private nested class which should never be instantiated by application code. For example:

```csharp
public class Country
{
    public int Code { get; set; }
    public required string Name { get; set; }

    private class FooConfiguration : IEntityTypeConfiguration<Country>
    {
        private FooConfiguration()
        {
        }

        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(e => e.Code);
        }
    }
}
```

As an aside, some people think this pattern is an abomination because it couples the entity type to the configuration. Other people think it is very useful because it co-locates configuration with the entity type. Let's not debate this here. :-)

## SQL Server HierarchyId

> [!TIP]
> The code shown here comes from [HierarchyIdSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/HierarchyIdSample.cs).

<a name="hierarchyid-path-generation"></a>

### Sugar for HierarchyId path generation

First class support for the SQL Server `HierarchyId` type was [added in EF8](xref:core/what-is-new/ef-core-8.0/whatsnew#hierarchyid). In EF9, a sugar method has been added to make it easier to create new child nodes in the tree structure. For example, the following code queries for an existing entity with a `HierarchyId` property:

<!--
        #region HierarchyIdQuery
        var daisy = await context.Halflings.SingleAsync(e => e.Name == "Daisy");
-->
[!code-csharp[HierarchyIdQuery](../../../../samples/core/Miscellaneous/NewInEFCore9/HierarchyIdSample.cs?name=HierarchyIdQuery)]

This `HierarchyId` property can then be used to create child nodes without any explicit string manipulation. For example:

<!--
        #region HierarchyIdParse1
        var child1 = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 1), "Toast");
        var child2 = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 2), "Wills");
-->
[!code-csharp[HierarchyIdParse1](../../../../samples/core/Miscellaneous/NewInEFCore9/HierarchyIdSample.cs?name=HierarchyIdParse1)]

If `daisy` has a `HierarchyId` of `/4/1/3/1/`, then, `child1` will get the `HierarchyId` "/4/1/3/1/1/", and `child2` will get the `HierarchyId` "/4/1/3/1/2/".

To create a node between these two children, an additional sub-level can be used. For example:

<!--
        #region HierarchyIdParse2
        var child1b = new Halfling(HierarchyId.Parse(daisy.PathFromPatriarch, 1, 5), "Toast");
-->
[!code-csharp[HierarchyIdParse2](../../../../samples/core/Miscellaneous/NewInEFCore9/HierarchyIdSample.cs?name=HierarchyIdParse2)]

This creates a node with a `HierarchyId` of `/4/1/3/1/1.5/`, putting it between `child1` and `child2`.

This enhancement was contributed by [@Rezakazemi890](https://github.com/Rezakazemi890). Many thanks!

## Tooling

<a name="fewer-rebuilds"></a>

### Fewer rebuilds

The [`dotnet ef` command line tool](xref:core/cli/dotnet) by default builds your project before executing the tool. This is because not rebuilding before running the tool is a common source of confusion when things don't work. Experienced developers can use the `--no-build` option to avoid this build, which may be slow. However, even the `--no-build` option could cause the project to be re-built the next time it is built outside of the EF tooling.

We believe a [community contribution](https://github.com/dotnet/efcore/pull/32860) from [@Suchiman](https://github.com/Suchiman) has fixed this. However, we're also conscious that tweaks around MSBuild behaviors have a tendency to have unintended consequences, so we're asking people like you to try this out and report back on any negative experiences you have.
