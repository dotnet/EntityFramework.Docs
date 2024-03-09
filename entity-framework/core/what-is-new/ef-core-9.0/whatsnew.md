---
title: What's New in EF Core 9
description: Overview of new features in EF Core 9
author: ajcvickers
ms.date: 03/07/2024
uid: core/what-is-new/ef-core-9.0/whatsnew
---

# What's New in EF Core 9

EF Core 9 (EF9) is the next release after EF Core 8 and is scheduled for release in November 2024. See [_Plan for Entity Framework Core 9_](xref:core/what-is-new/ef-core-9.0/plan) for details.

EF9 is available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF9 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF9 targets .NET 8, and can therefore be used with either [.NET 8 (LTS)](https://dotnet.microsoft.com/download/dotnet/8.0) or a .NET 9 preview.

> [!TIP]
> The _What's New_ docs are updated for each preview. All the samples are set up to use the [EF9 daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md), which usually have several additional weeks of completed work compared to the latest preview. We strongly encourage use of the daily builds when testing new features so that you're not doing your testing against stale bits.

## LINQ and SQL translation

The team is working on some significant architecture changes to the query pipeline in EF Core 9 as part of our continued improvements to JSON mapping and document databases. This means we need to get **people like you** to run your code on these new internals. (If you're reading a "What's New" doc at this point in the release, then you're a really engaged part of the community; thank you!) We have over 120,000 tests, but it's not enough! We need you, people running real code on our bits, in order to find issues and ship a solid release!

<a name="prune"></a>

### Prune columns passed to OPENJSON's WITH clause

> [!TIP]
> The code shown here comes from [JsonColumnsSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/JsonColumnsSample.cs).

EF9 removes unnecessary columns when calling `OPENJSON WITH`. For example, consider a query that obtains a count from a JSON collection using a predicate:

<!--
        #region PruneJSON
        var results = await context.Posts
            .Where(p => p.Metadata!.Updates.Count(e => e.UpdatedOn >= date) == 1)
            .ToListAsync();
-->
[!code-csharp[PruneJSON](../../../../samples/core/Miscellaneous/NewInEFCore9/JsonColumnsSample.cs?name=PruneJSON)]

In EF8, this query generates the following SQL when using the Azure SQL database provider:

```sql
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON([p].[Metadata], '$.Updates') WITH (
        [PostedFrom] nvarchar(45) '$.PostedFrom',
        [UpdatedBy] nvarchar(max) '$.UpdatedBy',
        [UpdatedOn] date '$.UpdatedOn',
        [Commits] nvarchar(max) '$.Commits' AS JSON
    ) AS [u]
    WHERE [u].[UpdatedOn] >= @__date_0) = 1
```

Notice that the `UpdatedBy`, and `Commits` are not needed in this query. Starting with EF9, these columns are now pruned away:

```sql
SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
FROM [Posts] AS [p]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON([p].[Metadata], '$.Updates') WITH (
        [PostedFrom] nvarchar(45) '$.PostedFrom',
        [UpdatedOn] date '$.UpdatedOn'
    ) AS [u]
    WHERE [u].[UpdatedOn] >= @__date_0) = 1
```

In some scenarios, this results in complete removal of the `WITH` clause. For example:

<!--
        #region PruneJSONPrimitive
        var tagsWithCount = await context.Tags.Where(p => p.Text.Length == 1).ToListAsync();
-->
[!code-csharp[PruneJSONPrimitive](../../../../samples/core/Miscellaneous/NewInEFCore9/JsonColumnsSample.cs?name=PruneJSONPrimitive)]

In EF8, this query translates to the following SQL:

```sql
SELECT [t].[Id], [t].[Text]
FROM [Tags] AS [t]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON([t].[Text]) WITH ([value] nvarchar(max) '$') AS [t0]) = 1
```

In EF9, this has been improved to:

```sql
SELECT [t].[Id], [t].[Text]
FROM [Tags] AS [t]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON([t].[Text]) AS [t0]) = 1
```

<a name="greatest"></a>

### Translations involving GREATEST/LEAST

> [!TIP]
> The code shown here comes from [LeastGreatestSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/LeastGreatestSample.cs).

Several new translations have been introduced that use the `GREATEST` and `LEAST` SQL functions.

> [!IMPORTANT]
> The `GREATEST` and `LEAST` functions wre [introduced to SQL Server/Azure SQL databases in the 2022 version](https://techcommunity.microsoft.com/t5/azure-sql-blog/introducing-the-greatest-and-least-t-sql-functions/ba-p/2281726). Visual Studio 2022 installs SQL Server 2019 by default. We recommend installing [SQL Server Developer Edition 2022](https://www.microsoft.com/sql-server/sql-server-downloads) to try out these new translations in EF9.

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
                .Where(
                    e => e.Title == ".NET Blog" && e.Id == id)
                .ToListAsync();
-->
[!code-csharp[DefaultParameterization](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=DefaultParameterization)]

This translates to the following SQL and parameters when using Azure SQL:

```output
info: 2/5/2024 15:43:13.789 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (1ms) [Parameters=[@__id_0='1'], CommandType='Text', CommandTimeout='30']
      SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
      FROM [Posts] AS [p]
      WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] = @__id_0
```

Notice that EF created a constant in the SQL for ".NET Blog" because this value will not change from query to query. Using a constant allows this value to be examined by the database engine when creating a query plan, potentially resulting in a more efficient query.

On the other hand, the value of `id` is parameterized, since the same query may be executed with many different values for `id`. Creating a constant in this case results in pollution of the query cache with lots of queries that differ only in parameter values. This is very bad for overall performance of the database.

Generally speaking, these defaults should not be changed. However, EF Core 8.0.2 introduces an `EF.Constant` method which forces EF to use a constant even if a parameter would be used by default. For example:

<!--
        #region ForceConstant
        async Task<List<Post>> GetPostsForceConstant(int id)
            => await context.Posts
                .Where(
                    e => e.Title == ".NET Blog" && e.Id == EF.Constant(id))
                .ToListAsync();
-->
[!code-csharp[ForceConstant](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ForceConstant)]

The translation now contains a constant for the `id` value:

```output
info: 2/5/2024 15:43:13.812 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
      FROM [Posts] AS [p]
      WHERE [p].[Title] = N'.NET Blog' AND [p].[Id] = 1
```

EF9 introduces the `EF.Parameter` method to do the opposite. That is, force EF to use a parameter even if the value is a constant in code. For example:

<!--
        #region ForceParameter
        async Task<List<Post>> GetPostsForceParameter(int id)
            => await context.Posts
                .Where(
                    e => e.Title == EF.Parameter(".NET Blog") && e.Id == id)
                .ToListAsync();
-->
[!code-csharp[ForceParameter](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ForceParameter)]

The translation now contains a parameter for the ".NET Blog" string:

```output
info: 2/5/2024 15:43:13.803 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (1ms) [Parameters=[@__p_0='.NET Blog' (Size = 4000), @__id_1='1'], CommandType='Text', CommandTimeout='30']
      SELECT [p].[Id], [p].[Archived], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Discriminator], [p].[PublishedOn], [p].[Title], [p].[PromoText], [p].[Metadata]
      FROM [Posts] AS [p]
      WHERE [p].[Title] = @__p_0 AND [p].[Id] = @__id_1
```

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

In EF9, the `IQueryable` in the `dotnetPosts` is inlined, resulting in a single round trip:

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

<a name="hashsetasync"></a>

### New `ToHashSetAsync<T>` methods

> [!TIP]
> The code shown here comes from [QuerySample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs).

The <xref:System.Linq.Enumerable.ToHashSet%2A?displayProperty=nameWithType> methods have existed since .NET Core 2.0. In EF9, the equivalent async methods have been added. For example:

<!--
        #region ToHashSetAsync
        var set1 = await context.Posts
            .Where(p => p.Tags.Count > 3)
            .ToHashSetAsync();

        var set2 = await context.Posts
            .Where(p => p.Tags.Count > 3)
            .ToHashSetAsync(ReferenceEqualityComparer<Post>.Instance);
-->
[!code-csharp[ToHashSetAsync](../../../../samples/core/Miscellaneous/NewInEFCore9/QuerySample.cs?name=ToHashSetAsync)]

This enhancement was contributed by [@wertzui](https://github.com/wertzui). Many thanks!

## ExecuteUpdate and ExecuteDelete

<a name="executecomplex"></a>

### Allow passing complex type instances to ExecuteUpdate

> [!TIP]
> The code shown here comes from [ExecuteUpdateSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs).

The `ExecuteUpdate` API was introduced in EF7 to perform immediate, direct updates to the database without tracking or `SaveChanges`. For example:

<!--
        #region NormalExecuteUpdate
        await context.Stores
            .Where(e => e.Region == "Germany")
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.Region, "Deutschland"));
-->
[!code-csharp[NormalExecuteUpdate](../../../../samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs?name=NormalExecuteUpdate)]

Running this code executes the following query to update the `Region` to "Deutschland":

```sql
UPDATE [s]
SET [s].[Region] = N'Deutschland'
FROM [Stores] AS [s]
WHERE [s].[Region] = N'Germany'
```

In EF8 `ExecuteUpdate` can also be used to update values of complex type properties. However, each member of the complex type must be specified explicitly. For example:

<!--
            #region UpdateComplexTypeByMember
            var newAddress = new Address("Gressenhall Farm Shop", null, "Beetley", "Norfolk", "NR20 4DR");

            await context.Stores
                .Where(e => e.Region == "Deutschland")
                .ExecuteUpdateAsync(
                    s => s.SetProperty(b => b.StoreAddress.Line1, newAddress.Line1)
                        .SetProperty(b => b.StoreAddress.Line2, newAddress.Line2)
                        .SetProperty(b => b.StoreAddress.City, newAddress.City)
                        .SetProperty(b => b.StoreAddress.Country, newAddress.Country)
                        .SetProperty(b => b.StoreAddress.PostCode, newAddress.PostCode));
-->
[!code-csharp[UpdateComplexTypeByMember](../../../../samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs?name=UpdateComplexTypeByMember)]

Running this code results in the following query execution:

```sql
UPDATE [s]
SET [s].[StoreAddress_PostCode] = @__newAddress_PostCode_4,
    [s].[StoreAddress_Country] = @__newAddress_Country_3,
    [s].[StoreAddress_City] = @__newAddress_City_2,
    [s].[StoreAddress_Line2] = NULL,
    [s].[StoreAddress_Line1] = @__newAddress_Line1_0
FROM [Stores] AS [s]
WHERE [s].[Region] = N'Deutschland'
```

In EF9, the same update can be performed by passing the complex type instance itself. That is, each member does not need to be explicitly specified. For example:

<!--
            #region UpdateComplexType
            var newAddress = new Address("Gressenhall Farm Shop", null, "Beetley", "Norfolk", "NR20 4DR");

            await context.Stores
                .Where(e => e.Region == "Germany")
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.StoreAddress, newAddress));
-->
[!code-csharp[UpdateComplexType](../../../../samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs?name=UpdateComplexType)]

Running this code results in the same query execution as the previous example:

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

Multiple updates to both complex type properties and simple properties can be combined in a single call to `ExecuteUdate`. For example:

<!--
        #region UpdateMultipleComplexType
        await context.Customers
            .Where(e => e.Name == name)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                        b => b.CustomerInfo.WorkAddress, new Address("Gressenhall Workhouse", null, "Beetley", "Norfolk", "NR20 4DR"))
                    .SetProperty(b => b.CustomerInfo.HomeAddress, new Address("Gressenhall Farm", null, "Beetley", "Norfolk", "NR20 4DR"))
                    .SetProperty(b => b.CustomerInfo.Tag, "Tog"));
-->
[!code-csharp[UpdateMultipleComplexType](../../../../samples/core/Miscellaneous/NewInEFCore9/ExecuteUpdateSample.cs?name=UpdateMultipleComplexType)]

Running this code results in the same query execution as the previous example:

```sql
UPDATE [c]
SET [c].[CustomerInfo_Tag] = N'Tog',
    [c].[CustomerInfo_HomeAddress_City] = N'Beetley',
    [c].[CustomerInfo_HomeAddress_Country] = N'Norfolk',
    [c].[CustomerInfo_HomeAddress_Line1] = N'Gressenhall Farm',
    [c].[CustomerInfo_HomeAddress_Line2] = NULL,
    [c].[CustomerInfo_HomeAddress_PostCode] = N'NR20 4DR',
    [c].[CustomerInfo_WorkAddress_City] = N'Beetley',
    [c].[CustomerInfo_WorkAddress_Country] = N'Norfolk',
    [c].[CustomerInfo_WorkAddress_Line1] = N'Gressenhall Workhouse',
    [c].[CustomerInfo_WorkAddress_Line2] = NULL,
    [c].[CustomerInfo_WorkAddress_PostCode] = N'NR20 4DR'
FROM [Customers] AS [c]
WHERE [c].[Name] = @__name_0
```

## Migrations

<a name="temporal-migrations"></a>

### Improved temporal table migrations

The migration created when changing an existing table into a temporal table has been reduced in size for EF9. For example, in EF8 making a single existing table a temporal table results in the following migration:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterTable(
        name: "Blogs")
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

    migrationBuilder.AlterColumn<string>(
        name: "SiteUri",
        table: "Blogs",
        type: "nvarchar(max)",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "nvarchar(max)")
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

    migrationBuilder.AlterColumn<string>(
        name: "Name",
        table: "Blogs",
        type: "nvarchar(max)",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "nvarchar(max)")
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

    migrationBuilder.AlterColumn<int>(
        name: "Id",
        table: "Blogs",
        type: "int",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int")
        .Annotation("SqlServer:Identity", "1, 1")
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
        .OldAnnotation("SqlServer:Identity", "1, 1");

    migrationBuilder.AddColumn<DateTime>(
        name: "PeriodEnd",
        table: "Blogs",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

    migrationBuilder.AddColumn<DateTime>(
        name: "PeriodStart",
        table: "Blogs",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
}
```

In EF9, the same operation now results in a much smaller migration:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterTable(
        name: "Blogs")
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", "BlogsHistory")
        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

    migrationBuilder.AddColumn<DateTime>(
        name: "PeriodEnd",
        table: "Blogs",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

    migrationBuilder.AddColumn<DateTime>(
        name: "PeriodStart",
        table: "Blogs",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);
}
```

## Model building

<a name="sequence-caching"></a>

### Specify caching for sequences

> [!TIP]
> The code shown here comes from [ModelBuildingSample.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/NewInEFCore9/ModelBuildingSample.cs).

EF9 allows setting the [caching options for database sequences](/sql/t-sql/statements/create-sequence-transact-sql) for any relational database provider that supports this. For example, `UseCache` can be used to explicitly turn on caching and set the cache size:

<!--
            #region UseCache
            modelBuilder.HasSequence<int>("MyCachedSequence")
                .HasMin(10).HasMax(255000)
                .IsCyclic()
                .StartsAt(11).IncrementsBy(2)
                .UseCache(20);
-->
[!code-csharp[UseCache](../../../../samples/core/Miscellaneous/NewInEFCore9/ModelBuildingSample.cs?name=UseCache)]

This results in the following sequence definition when using SQL Server:

```sql
CREATE SEQUENCE [MyCachedSequence] AS int START WITH 11 INCREMENT BY 2 MINVALUE 10 MAXVALUE 255000 CYCLE CACHE 3;
```

Similarly, `UseNoCache` explicitly turns off caching:

<!--
            #region UseNoCache
            modelBuilder.HasSequence<int>("MyUncachedSequence")
                .HasMin(10).HasMax(255000)
                .IsCyclic()
                .StartsAt(11).IncrementsBy(2)
                .UseNoCache();
-->
[!code-csharp[UseNoCache](../../../../samples/core/Miscellaneous/NewInEFCore9/ModelBuildingSample.cs?name=UseNoCache)]

```sql
CREATE SEQUENCE [MyUncachedSequence] AS int START WITH 11 INCREMENT BY 2 MINVALUE 10 MAXVALUE 255000 CYCLE NO CACHE;
```

If neither `UseCache` or `UseNoCache` are called, then caching is not specified and the database will use whatever its default is. This may be a different default for different databases.

This enhancement was contributed by [@bikbov](https://github.com/bikbov). Many thanks!

<a name="fill-factor"></a>

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

> [!NOTE]
> There is currently a bug in preview 2 where the fill-factors are not included when the table is created for the first time. This is tracked by [Issue #33269](https://github.com/dotnet/efcore/issues/33269)

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

This creates a node with a `HierarchyId` of `/4/1/3/1/1.5/`, putting it bteween `child1` and `child2`.

This enhancement was contributed by [@Rezakazemi890](https://github.com/Rezakazemi890). Many thanks!

## Tooling

<a name="fewer-rebuilds"></a>

### Fewer rebuilds

The [`dotnet ef` command line tool](xref:core/cli/dotnet) by default builds your project before executing the tool. This is because not rebuilding before running the tool is a common source of confusion when things don't work. Experienced developers can use the `--no-build` option to avoid this build, which may be slow. However, even the `--no-build` option could cause the project to be re-built the next time it is built outside of the EF tooling.

We believe a [community contribution](https://github.com/dotnet/efcore/pull/32860) from [@Suchiman](https://github.com/Suchiman) has fixed this. However, we're also conscious that tweaks around MSBuild behaviors have a tendency to have unintended consequences, so we're asking people like you to try this out and report back on any negative experiences you have.
