---
title: Querying - Azure Cosmos DB Provider - EF Core
description: Querying with the Azure Cosmos DB EF Core Provider
author: roji
ms.date: 09/19/2024
uid: core/providers/cosmos/querying
---
# Querying with the EF Core Azure Cosmos DB Provider

## Querying basics

[EF Core LINQ queries](xref:core/querying/index) can be executed against Azure Cosmos DB in the same way as for other database providers. For example:

```csharp
public class Session
{
    public Guid Id { get; set; }
    public string Category { get; set; }

    public string TenantId { get; set; } = null!;
    public Guid UserId { get; set; }
    public int SessionId { get; set; }
}

var stringResults = await context.Sessions
    .Where(
        e => e.Category.Length > 4
            && e.Category.Trim().ToLower() != "disabled"
            && e.Category.TrimStart().Substring(2, 2).Equals("xy", StringComparison.OrdinalIgnoreCase))
    .ToListAsync();
```

> [!NOTE]
> The Azure Cosmos DB provider does not translate the same set of LINQ queries as other providers.
> For example, the EF `Include()` operator isn't supported on Azure Cosmos DB, since cross-document queries aren't supported in the database.

## Partition keys

The advantage of partitioning is to have your queries execute only against the partition where the relevant data is found, saving costs and ensuring faster result speed. Queries which don't specify partition keys are executed on all the partitions, which can be quite costly.

Starting with EF 9.0, EF automatically detects and extracts partition key comparisons in your LINQ query's `Where` operators. Let's assume we execute the following query against our `Session` entity type, which is configured with a hierarchical partition key:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Session>()
        .HasPartitionKey(b => new { b.TenantId, b.UserId, b.SessionId })
}

var tenantId = "Microsoft";
var userId = new Guid("99A410D7-E467-4CC5-92DE-148F3FC53F4C");
var username = "scott";

var sessions = await context.Sessions
    .Where(
        e => e.TenantId == tenantId
             && e.UserId == userId
             && e.SessionId > 0
             && e.Username == username)
    .ToListAsync();
```

Examining the logs generated by EF, we see this query executed as follows:

```sql
Executed ReadNext (166.6985 ms, 2.8 RU) ActivityId='312da0d2-095c-4e73-afab-27072b5ad33c', Container='test', Partition='["Microsoft","99a410d7-e467-4cc5-92de-148f3fc53f4c"]', Parameters=[]
SELECT VALUE c
FROM root c
WHERE ((c["SessionId"] > 0) AND CONTAINS(c["Username"], "a"))
```

In these logs, we notice the following:

* The first two comparisons - on `TenantId` and `UserId` - have been lifted out, and appear in the `ReadNext` "Partition" rather than in the `WHERE` clause; this means that query will only execute on the subpartitions for those values.
* `SessionId` is also part of the hierarchical partition key, but instead of an equality comparison, it uses a greater-than operator (`>`), and therefore cannot be lifted out. It is part of the `WHERE` clause like any regular property.
* `Username` is a regular property - not part of the partition key - and therefore remains in the `WHERE` clause as well.

Note that even though some of the partition key values are not provided, hierarchical partition keys still allow targeting only the subpartitions which correspond to the first two properties. While this isn't as efficient as targeting a single partition (as identified by all three properties), it's still much more efficient than targeting all partitions.

Rather than referencing partition key properties in a `Where` operator, you can explicitly specify them by using the <xref:Microsoft.EntityFrameworkCore.CosmosQueryableExtensions.WithPartitionKey%2A> operator:

```c#
var sessions = await context.Sessions
    .WithPartitionKey(tenantId, userId)
    .Where(e => e.SessionId > 0 && e.Username.Contains("a"))
    .ToListAsync();
```

This executes in the same way as the above query, and can be preferable if you want to make partition keys more explicit in your queries. Using <xref:Microsoft.EntityFrameworkCore.CosmosQueryableExtensions.WithPartitionKey%2A> may be necessary in versions of EF prior to 9.0 - keep an eye on the logs to ensure that queries are using partition keys as expected.

## Point reads

While Azure Cosmos DB allows for powerful querying via SQL, such queries can be quite expensive. Azure Cosmos DB also supports _point reads_, which should be used when retrieving a single document if both the `id` property and the entire partition key are known. Point reads directly identify a specific document in a specific partition, and execute extremely efficiently and with reduced costs compared to retrieving the same document with a query. It's recommended to design your system to leverage point reads as often as possible. To read more, see the [Azure Cosmos DB documentation](/azure/cosmos-db/nosql/how-to-dotnet-read-item).

In the previous section, we saw EF identifying and extracting partition key comparisons from the `Where` clause for more efficient querying, restricting processing only to the relevant partitions. It's possible to go a step further, and provide the `id` property in the query as well. Let's examine the following query:

```c#
var session = await context.Sessions.SingleAsync(
    e => e.Id == someId
         && e.TenantId == tenantId
         && e.UserId == userId
         && e.SessionId == sessionId);
```

In this query, a value for the `Id` property is provided (which is mapped to the Azure Cosmos DB `id` property), as well as values for all the partition key properties. Furthermore, there are no additional components to the query. When all these conditions are met, EF is able to execute the query as a point read:

```console
Executed ReadItem (46 ms, 1 RU) ActivityId='d7391311-2266-4811-ae2d-535904c42c43', Container='test', Id='9', Partition='["Microsoft","99a410d7-e467-4cc5-92de-148f3fc53f4c",10.0]'
```

Note the `ReadItem`, which indicates that the query was executed as an efficient point read - no SQL query is involved.

Note that as with partition key extraction, significant improvements have been made to this mechanism in EF 9.0; older versions do not reliably detect and use point reads.

## Pagination

> [!NOTE]
> This feature was introduced in EF Core 9.0 and is still experimental. Please let us know how it works for you and if you have any feedback.

Pagination refers to retrieving results in pages, rather than all at once; this is typically done for large resultsets, where a user interface is displayed, allowing users to navigate through pages of the results.

A common way to implement pagination with databases is to use the `Skip` and `Take` LINQ operators (`OFFSET` and `LIMIT` in SQL). Given a page size of 10 results, the third page can be fetched with EF Core as follows:

```csharp
var position = 20;
var nextPage = context.Session
    .OrderBy(s => s.Id)
    .Skip(position)
    .Take(10)
    .ToList();
```

Unfortunately, this technique is quite inefficient and can considerably increase querying costs. Azure Cosmos DB provides a special mechanism for paginating through the result of a query, via the use of _continuation tokens_:

```csharp
CosmosPage firstPage = await context.Sessions
    .OrderBy(s => s.Id)
    .ToPageAsync(pageSize: 10, continuationToken: null);

string continuationToken = firstPage.ContinuationToken;
foreach (var session in firstPage.Values)
{
    // Display/send the sessions to the user
}
```

Rather than terminating the LINQ query with `ToListAsync` or similar, we use the `ToPageAsync` method, instructing it to get at most 10 items in every page (note that there may be fewer items in the database). Since this is our first query, we'd like to get results from the beginning, and pass `null` as the continuation token. `ToPageAsync` returns a `CosmosPage`, which exposes a continuation token and the values in the page (up to 10 items). Your program will typically send those values to the client, along with the continuation token; this will allow resuming the query later and fetching more results.

Let's assume the user now clicks on the "Next" button in their UI, asking for the next 10 items. You can then execute the query as follows:

```csharp
CosmosPage nextPage = await context.Sessions.OrderBy(s => s.Id).ToPageAsync(10, continuationToken);
string continuationToken = nextPage.ContinuationToken;
foreach (var session in nextPage.Values)
{
    // Display/send the sessions to the user
}
```

We execute the same query, but this time we pass in the continuation token received from the first execution; this instructs the query engine to continue the query where it left off, and fetch the next 10 items. Once we fetch the last page and there are no more results, the continuation token will be `null`, and the "Next" button can be grayed out. This method of paginating is extremely efficient and cost-effective compared to using `Skip` and `Take`.

To learn more about pagination in Azure Cosmos DB, [see this page](/azure/cosmos-db/nosql/query/pagination).

> [!NOTE]
> Azure Cosmos DB does not support backwards pagination, and does not provide a count of the total pages or items.
>
> `ToPageAsync` is currently annotated as experimental, since it may be replaced with a more generic EF pagination API that isn't Azure Cosmos DB specific. Although using the current API will generate a compilation warning (`EF9102`), doing so should be safe - future changes may require minor tweaks in the API shape.

## `FindAsync`

[`FindAsync`](xref:core/change-tracking/entity-entries#find-and-findasync) is a useful API for getting an entity by its primary key, and avoiding a database roundtrip when the entity has already been loaded and is tracked by the context.

Developers familiar with relational databases are used to the primary key of an entity type consisting e.g. of an `Id` property. When using the EF Azure Cosmos DB provider, the primary key contains the partition key properties in addition to the property mapped to the JSON `id` property; this is the case since Azure Cosmos DB allows different partitions to contain documents with the same JSON `id` property, and so only the combined `id` and partition key uniquely identify a single document in a container:

```csharp
public class Session
{
    public Guid Id { get; set; }
    public string PartitionKey { get; set; }
    ...
}

var mySession = await context.FindAsync(id, pkey);
```

If you have a hierarchical partition key, you must pass all partition key values to `FindAsync`, in the order in which they were configured.

> [!NOTE]
> Use `FindAsync` only when the entity might already be tracked by your context, and you want to avoid the database roundtrip.
> Otherwise, simply use `SingleAsync` - there is no performance difference between the two when the entity needs to be loaded from the database.

## SQL queries

Queries can also be written [directly in SQL](xref:core/querying/sql-queries). For example:

```csharp
var rating = 3;
_ = await context.Blogs
    .FromSql($"SELECT VALUE c FROM root c WHERE c.Rating > {rating}")
    .ToListAsync();
```

This query results in the following query execution:

```sql
SELECT VALUE s
FROM (
    SELECT VALUE c FROM root c WHERE c.Angle1 <= @p0
) s
```

Note that `FromSql` was introduced in EF 9.0. In previous versions, `FromSqlRaw` can be used instead, although note that that method is vulnerable to SQL injection attacks.

For more information on SQL querying, see the [relational documentation on SQL queries](xref:core/querying/sql-queries); most of that content is relevant for the Azure Cosmos DB provider as well.

## Function mappings

This section shows which .NET methods and members are translated into which SQL functions when querying with the Azure Cosmos DB provider.

### Date and time functions

.NET                                       | SQL                                                                               | Added in
------------------------------------------ | --------------------------------------------------------------------------------- | --------
DateTime.UtcNow                            | [GetCurrentDateTime()](/azure/cosmos-db/nosql/query/getcurrentdatetime)
DateTimeOffset.UtcNow                      | [GetCurrentDateTime()](/azure/cosmos-db/nosql/query/getcurrentdatetime)
dateTime.Year<sup>1</sup>                  | [DateTimePart("yyyy", dateTime)](/azure/cosmos-db/nosql/query/datetimepart)       | EF Core 9.0
dateTimeOffset.Year<sup>1</sup>            | [DateTimePart("yyyy", dateTimeOffset)](/azure/cosmos-db/nosql/query/datetimepart) | EF Core 9.0
dateTime.AddYears(years)<sup>1</sup>       | [DateTimeAdd("yyyy", dateTime)](/azure/cosmos-db/nosql/query/datetimeadd)         | EF Core 9.0
dateTimeOffset.AddYears(years)<sup>1</sup> | [DateTimeAdd("yyyy", dateTimeOffset)](/azure/cosmos-db/nosql/query/datetimeadd)   | EF Core 9.0

<sup>1</sup> The other component members are translated as well (Month, Day...).

### Numeric functions

.NET                       | SQL                                                 | Added in
-------------------------- | --------------------------------------------------- | --------
double.DegreesToRadians(x) | [RADIANS(@x)](/azure/cosmos-db/nosql/query/radians) | EF Core 8.0
double.RadiansToDegrees(x) | [DEGREES(@x)](/azure/cosmos-db/nosql/query/degrees) | EF Core 8.0
EF.Functions.Random()      | [RAND()](/azure/cosmos-db/nosql/query/rand)
Math.Abs(value)            | [ABS(@value)](/azure/cosmos-db/nosql/query/abs)
Math.Acos(d)               | [ACOS(@d)](/azure/cosmos-db/nosql/query/acos)
Math.Asin(d)               | [ASIN(@d)](/azure/cosmos-db/nosql/query/asin)
Math.Atan(d)               | [ATAN(@d)](/azure/cosmos-db/nosql/query/atan)
Math.Atan2(y, x)           | [ATN2(@y, @x)](/azure/cosmos-db/nosql/query/atn2)
Math.Ceiling(d)            | [CEILING(@d)](/azure/cosmos-db/nosql/query/ceiling)
Math.Cos(d)                | [COS(@d)](/azure/cosmos-db/nosql/query/cos)
Math.Exp(d)                | [EXP(@d)](/azure/cosmos-db/nosql/query/exp)
Math.Floor(d)              | [FLOOR(@d)](/azure/cosmos-db/nosql/query/floor)
Math.Log(a, newBase)       | [LOG(@a, @newBase)](/azure/cosmos-db/nosql/query/log)
Math.Log(d)                | [LOG(@d)](/azure/cosmos-db/nosql/query/log)
Math.Log10(d)              | [LOG10(@d)](/azure/cosmos-db/nosql/query/log10)
Math.Pow(x, y)             | [POWER(@x, @y)](/azure/cosmos-db/nosql/query/power)
Math.Round(d)              | [ROUND(@d)](/azure/cosmos-db/nosql/query/round)
Math.Sign(value)           | [SIGN(@value)](/azure/cosmos-db/nosql/query/sign)
Math.Sin(a)                | [SIN(@a)](/azure/cosmos-db/nosql/query/sin)
Math.Sqrt(d)               | [SQRT(@d)](/azure/cosmos-db/nosql/query/sqrt)
Math.Tan(a)                | [TAN(@a)](/azure/cosmos-db/nosql/query/atan)
Math.Truncate(d)           | [TRUNC(@d)](/azure/cosmos-db/nosql/query/trunc)

> [!TIP]
> In addition to the methods listed here, corresponding [generic math](/dotnet/standard/generics/math) implementations
> and [MathF](/dotnet/api/system.mathf) methods are also translated. For example, `Math.Sin`, `MathF.Sin`, `double.Sin`,
> and `float.Sin` all map to the `SIN` function in SQL.

### String functions

.NET                                                              | SQL                                                                                | Added in
----------------------------------------------------------------- | ---------------------------------------------------------------------------------- | --------
Regex.IsMatch(input, pattern)                                     | [RegexMatch(@pattern, @input)](/azure/cosmos-db/nosql/query/regexmatch)             | EF Core 7.0
Regex.IsMatch(input, pattern, options)                            | [RegexMatch(@input, @pattern, @options)](/azure/cosmos-db/nosql/query/regexmatch)   | EF Core 7.0
string.Concat(str0, str1)                                         | @str0 + @str1
string.Equals(a, b, StringComparison.Ordinal)                     | [STRINGEQUALS(@a, @b)](/azure/cosmos-db/nosql/query/stringequals)
string.Equals(a, b, StringComparison.OrdinalIgnoreCase)           | [STRINGEQUALS(@a, @b, true)](/azure/cosmos-db/nosql/query/stringequals)
stringValue.Contains(value)                                       | [CONTAINS(@stringValue, @value)](/azure/cosmos-db/nosql/query/contains)
stringValue.Contains(value, StringComparison.Ordinal)             | [CONTAINS(@stringValue, @value, false)](/azure/cosmos-db/nosql/query/contains)     | EF Core 9.0
stringValue.Contains(value, StringComparison.OrdinalIgnoreCase)   | [CONTAINS(@stringValue, @value, true)](/azure/cosmos-db/nosql/query/contains)      | EF Core 9.0
stringValue.EndsWith(value)                                       | [ENDSWITH(@stringValue, @value)](/azure/cosmos-db/nosql/query/endswith)
stringValue.EndsWith(value, StringComparison.Ordinal)             | [ENDSWITH(@stringValue, @value, false)](/azure/cosmos-db/nosql/query/endswith)     | EF Core 9.0
stringValue.EndsWith(value, StringComparison.OrdinalIgnoreCase)   | [ENDSWITH(@stringValue, @value, true)](/azure/cosmos-db/nosql/query/endswith)      | EF Core 9.0
stringValue.Equals(value, StringComparison.Ordinal)               | [STRINGEQUALS(@stringValue, @value)](/azure/cosmos-db/nosql/query/stringequals)
stringValue.Equals(value, StringComparison.OrdinalIgnoreCase)     | [STRINGEQUALS(@stringValue, @value, true)](/azure/cosmos-db/nosql/query/stringequals)
stringValue.FirstOrDefault()                                      | [LEFT(@stringValue, 1)](/azure/cosmos-db/nosql/query/left)
stringValue.IndexOf(value)                                        | [INDEX_OF(@stringValue, @value)](/azure/cosmos-db/nosql/query/index-of)
stringValue.IndexOf(value, startIndex)                            | [INDEX_OF(@stringValue, @value, @startIndex)](/azure/cosmos-db/nosql/query/index-of)
stringValue.LastOrDefault()                                       | [RIGHT(@stringValue, 1)](/azure/cosmos-db/nosql/query/right)
stringValue.Length                                                | [LENGTH(@stringValue)](/azure/cosmos-db/nosql/query/length)
stringValue.Replace(oldValue, newValue)                           | [REPLACE(@stringValue, @oldValue, @newValue)](/azure/cosmos-db/nosql/query/replace)
stringValue.StartsWith(value)                                     | [STARTSWITH(@stringValue, @value)](/azure/cosmos-db/nosql/query/startswith)
stringValue.StartsWith(value, StringComparison.Ordinal)           | [STARTSWITH(@stringValue, @value, false)](/azure/cosmos-db/nosql/query/startswith) | EF Core 9.0
stringValue.StartsWith(value, StringComparison.OrdinalIgnoreCase) | [STARTSWITH(@stringValue, @value, true)](/azure/cosmos-db/nosql/query/startswith)  | EF Core 9.0
stringValue.Substring(startIndex)                                 | [SUBSTRING(@stringValue, @startIndex, LENGTH(@stringValue))](/azure/cosmos-db/nosql/query/substring)
stringValue.Substring(startIndex, length)                         | [SUBSTRING(@stringValue, @startIndex, @length)](/azure/cosmos-db/nosql/query/substring)
stringValue.ToLower()                                             | [LOWER(@stringValue)](/azure/cosmos-db/nosql/query/lower)
stringValue.ToUpper()                                             | [UPPER(@stringValue)](/azure/cosmos-db/nosql/query/upper)
stringValue.Trim()                                                | [TRIM(@stringValue)](/azure/cosmos-db/nosql/query/trim)
stringValue.TrimEnd()                                             | [RTRIM(@stringValue)](/azure/cosmos-db/nosql/query/rtrim)
stringValue.TrimStart()                                           | [LTRIM(@stringValue)](/azure/cosmos-db/nosql/query/ltrim)

### Miscellaneous functions

.NET                                                                                    | SQL                                                                                                           | Notes
--------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- | -----
collection.Contains(item)                                                               | @item IN @collection
EF.Functions.CoalesceUndefined(x, y)<sup>1</sup>                                        | [x ?? y](/azure/cosmos-db/nosql/query/ternary-coalesce-operators#coalesce-operator)                           | Added in EF Core 9.0
EF.Functions.IsDefined(x)                                                               | [IS_DEFINED(x)](/azure/cosmos-db/nosql/query/is-defined)                                                      | Added in EF Core 9.0
EF.Functions.VectorDistance(vector1, vector2)<sup>2</sup>                               | [VectorDistance(vector1, vector2)](/azure/cosmos-db/nosql/query/vectordistance)                               | Added in EF Core 9.0, Experimental
EF.Functions.VectorDistance(vector1, vector2, bruteForce)<sup>2</sup>                   | [VectorDistance(vector1, vector2, bruteForce)](/azure/cosmos-db/nosql/query/vectordistance)                   | Added in EF Core 9.0, Experimental
EF.Functions.VectorDistance(vector1, vector2, bruteForce, distanceFunction)<sup>2</sup> | [VectorDistance(vector1, vector2, bruteForce, distanceFunction)](/azure/cosmos-db/nosql/query/vectordistance) | Added in EF Core 9.0, Experimental

<sup>1</sup> Note that `EF.Functions.CoalesceUndefined` coalesces `undefined`, not `null`. To coalesce `null`, use the regular C# `??` operator.

<sup>2</sup> [See the documentation](xref:core/providers/cosmos/vector-search) for information on using vector search in Azure Cosmos DB, which is experimental. The APIs are subject to change.