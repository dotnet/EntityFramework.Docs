---
title: What's New in EF Core 10
description: Overview of new features in EF Core 10
author: maumar
ms.date: 01/05/2025
uid: core/what-is-new/ef-core-10.0/whatsnew
---

# What's New in EF Core 10

EF Core 10 (EF10) is the next release after EF Core 9 and is scheduled for release in November 2025.

EF10 is available as a preview. See [.NET 10 release notes](https://github.com/dotnet/core/tree/main/release-notes/10.0) to get information about the latest preview. This article will be updated as new preview releases are made available.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF10 requires the .NET 10 SDK to build and requires the .NET 10 runtime to run. EF10 will not run on earlier .NET versions, and will not run on .NET Framework.

<a name="cosmos"></a>

## Named query filters

EF's [global query filters](xref:core/querying/filters) feature has long enabled users to configuring filters to entity types which apply to all queries by default. This has simplified implementing common patterns and scenarios such as soft deletion, multitenancy and others. However, up to now EF has only supported a single query filter per entity type, making it difficult to have multiple filters and selectively disabling only some of them in specific queries.

EF 10 introduces *named query filters*, which allow attaching names to query filter and managing each one separately:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#FilterConfiguration)]

This notably allows disabling only certain filters in a specific LINQ query:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#DisableSoftDeletionFilter)]

For more information on named query filters, see the [documentation](xref:core/querying/filters).

This feature was contributed by [@bittola](https://github.com/bittola).

## Azure Cosmos DB for NoSQL

<a name="full-text-search-support"></a>

### Full-text search support

Azure Cosmos DB now offers support for [full-text search](/azure/cosmos-db/gen-ai/full-text-search). It enables efficient and effective text searches, as well as evaluating the relevance of documents to a given search query. It can be used in combination with vector search to improve the accuracy of responses in some AI scenarios.
EF Core 10 is adding support for this feature allowing for modeling the database with full-text search enabled properties and using full-text search functions inside queries targeting Azure Cosmos DB.

Here is a basic EF model configuration enabling full-text search on one of the properties:

```c#
public class Blog
{
    ...

    public string Contents { get; set; }
}

public class BloggingContext
{
    ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(b =>
        {
            b.Property(x => x.Contents).EnableFullTextSearch();
            b.HasIndex(x => x.Contents).IsFullTextIndex();
        });
    }
}
```

Once the model is configured, we can use full-text search operations in queries using methods provided in `EF.Functions`:

```c#
var cosmosBlogs = await context.Blogs.Where(x => EF.Functions.FullTextContains(x.Contents, "cosmos")).ToListAsync();
```

The following full-text operations are currently supported: [`FullTextContains`](/azure/cosmos-db/nosql/query/fulltextcontains), [`FullTextContainsAll`](/azure/cosmos-db/nosql/query/fulltextcontainsall), [`FullTextContainsAny`](/azure/cosmos-db/nosql/query/fulltextcontainsany), [`FullTextScore`](/azure/cosmos-db/nosql/query/fulltextscore).

For more information on Cosmos full-text search, see the [docs](xref:core/providers/cosmos/full-text-search).

### Hybrid search

EF Core now supports [`RRF`](/azure/cosmos-db/nosql/query/rrf) (Reciprocal Rank Fusion) function, which combines vector similarity search and full-text search (i.e. hybrid search). Here is an example query using hybrid search:

```c#
float[] myVector = /* generate vector data from text, image, etc. */
var hybrid = await context.Blogs.OrderBy(x => EF.Functions.Rrf(
        EF.Functions.FullTextScore(x.Contents, "database"), 
        EF.Functions.VectorDistance(x.Vector, myVector)))
    .Take(10)
    .ToListAsync();
```

For more information on Cosmos hybrid search, see the [docs](xref:core/providers/cosmos/full-text-search?#hybrid-search).

### Vector similarity search exits preview

In EF9 we added experimental support for [vector similarity search](xref:core/what-is-new/ef-core-9.0/whatsnew#vector-similarity-search-preview). In EF Core 10, vector similarity search support is no longer experimental. We have also made some improvements to the feature:

- EF Core can now generate containers with vector properties defined on owned reference entities. Containers with vector properties defined on owned collections still have to be created by other means. However, they can be used in queries.
- Model building APIs have been renamed. A vector property can now be configured using the `IsVectorProperty` method, and vector index can be configured using the `IsVectorIndex` method.

For more information on Cosmos vector search, see the [docs](xref:core/providers/cosmos/vector-search).

<a name="improved-model-evolution"></a>

### Improved experience when evolving the model

In previous versions of EF Core, evolving the model when using Azure Cosmos DB was quite painful. Specifically, when adding a new required property to the entity, EF would no longer be able to materialize that entity. The reason was that EF expected a value for the new property (since it was required), but the document created before the change didn't contain those values. The workaround was to mark the property as optional first, manually add default values for the property, and only then change it to required.

In EF 10 we improved this experience - EF will now materialize a default value for a required property, if no data is present for it in the document, rather than throw.

<a name="linq-and-sql-translation"></a>

## LINQ and SQL translation

<a name="parameterized-collection-translation"></a>

### Improved translation for parameterized collection

A notoriously difficult problem with relational databases is queries that involve *parameterized collections*:

```c#
int[] ids = [1, 2, 3];
var blogs = await context.Blogs.Where(b => ids.Contains(b.Id)).ToListAsync();
```

In the above query, `ids` is a parameterized collection: the same query can be executed many times, with `ids` containing different values each time.

Since relational databases don't typically support sending a collection directly as a parameter, EF version up to 8.0 simply inlined the collection contents into the SQL as constants:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (1, 2, 3)
```

While this works, it has the unfortunate consequence of generating different SQLs for different collections, causing database plan cache misses and bloat, and creating various performance problems (see [#13617](https://github.com/dotnet/efcore/issues/13617), which was the most highly-voted issue in the repo at the time). As a result, EF 8.0 leveraged the introduction of extensive JSON support and changed the translation of parameterized collections to use JSON arrays ([release notes](xref:core/what-is-new/ef-core-8.0/whatsnew#queries-with-primitive-collections
)):

```sql
@__ids_0='[1,2,3]'

SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (
    SELECT [i].[value]
    FROM OPENJSON(@__ids_0) WITH ([value] int '$') AS [i]
)
```

Here, the collection is encoded as a string containing a JSON array, sent as a single parameter and then unpacked using the SQL Server [`OPENJSON`](/sql/t-sql/functions/openjson-transact-sql) function (other databases use similar mechanisms). Since the collection is now parameterized, the SQL stays the same and a single query plan no matter what values the collection contains. Unfortunately, while elegant, this translation also deprives the database query planner of important information on the cardinality (or length) of the collection, and can cause a plan to be chosen that works well for a small - or large - number of elements. As a result, EF Core 9.0 introduced the ability to control which translation strategy to use ([release notes](xref:core/what-is-new/ef-core-9.0/whatsnew#parameterized-primitive-collections)).

EF 10.0 introduces a new default translation mode for parameterized collections, where each value in the collection is translated into its own scalar parameter:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (@ids1, @ids2, @ids3)
```

This allows the collection values to change without resulting in different SQLs - solving the plan cache problem - but at the same time provides the query planner with information on the collection cardinality. Since different cardinalities still cause different SQLs to be generated, the final version of EF 10 will include a "bucketization" feature, where e.g. 10 parameters are sent even when the user only specifies 8, to reduce the SQL variations and optimize the query cache.

Unfortunately, parameterized collections are a case where EF simply cannot always make the right choice: selecting between multiple parameters (the new default), a single JSON array parameter or multiple inlined constants can require knowledge about the data in your database, and different choices may work better for different queries. As a result, EF exposes full control to the user to control the translation strategy, both at the global configuration level:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer("<CONNECTION STRING>", o => o.UseParameterizedCollectionMode(ParameterTranslationMode.Constant));
```

... and at the per-query level:

```c#
var blogs = await context.Blogs.Where(b => EF.Constant(ids).Contains(b.Id)).ToListAsync();
```

### Support for the .NET 10 `LeftJoin` and `RightJoin` operators

`LEFT JOIN` is a common and useful operation when working with EF Core. In previous versions, implementing `LEFT JOIN` in LINQ was quite complicated, requiring `SelectMany`, `GroupJoin` and `DefaultIfEmpty` operations [in a particular configuration](/dotnet/csharp/linq/standard-query-operators/join-operations#perform-left-outer-joins).

.NET 10 adds first-class LINQ support for `LeftJoin` method, making those queries much simpler to write. EF Core recognizes the new method, so it can be used in EF LINQ queries instead of the old construct:

```C#
var query = context.Students
    .LeftJoin(
        context.Departments,
        student => student.DepartmentID,
        department => department.ID,
        (student, department) => new 
        { 
            student.FirstName,
            student.LastName,
            Department = department.Name ?? "[NONE]"
        });
```

> [!NOTE]
> EF 10 also supports the analogous `RightJoin` operator, which keeps all the data from the second collection and only the matching data from the first collection. EF 10 translates this to `RIGHT JOIN` operation in the database.

Unfortunately, C# query syntax (`from x select x.Id`) doesn't yet support expressing left/right join operations in this way.

See [#12793](https://github.com/dotnet/efcore/issues/12793) and [#35367](https://github.com/dotnet/efcore/issues/35367) for more details.

<a name="other-query-improvements"></a>

### Other query improvements

#### New translations

- Translate [DateOnly.ToDateTime()](/dotnet/api/system.dateonly.todatetime) ([#35194](https://github.com/dotnet/efcore/pull/35194), contributed by [@mseada94](https://github.com/mseada94)).
- Translate [DateOnly.DayNumber](/dotnet/api/system.dateonly.daynumber) and `DayNumber` subtraction for SQL Server and SQLite ([#36183](https://github.com/dotnet/efcore/issues/36183)).
- Translate date/time functions using `DatePart.Microsecond` and `DatePart.Nanosecond` arguments ([#34861](https://github.com/dotnet/efcore/pull/34861)).
- Translate `COALESCE` as `ISNULL` on SQL Server, for most cases ([#34171](https://github.com/dotnet/efcore/pull/34171), contributed by [@ranma42](https://github.com/ranma42)).
- Support some string functions taking `char` as arguments ([#34999](https://github.com/dotnet/efcore/pull/34999), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
- Support `MAX`/`MIN`/`ORDER BY` using `decimal` on SQLite ([#35606](https://github.com/dotnet/efcore/pull/35606), contributed by [@ranma42](https://github.com/ranma42)).
- Support projecting different navigations (but same type) via conditional operator ([#34589](https://github.com/dotnet/efcore/issues/34589), contributed by [@ranma42](https://github.com/ranma42)).

#### Bug fixes and optimizations

- Fix Microsoft.Data.Sqlite behavior around `DateTime`, `DateTimeOffset` and UTC, [see breaking change notes](xref:core/what-is-new/ef-core-10.0/breaking-changes#DateTimeOffset-read) ([#36195](https://github.com/dotnet/efcore/issues/36195)).
- Fix translation of `DefaultIfEmpty` in various scenarios ([#19095](https://github.com/dotnet/efcore/issues/19095), [#33343](https://github.com/dotnet/efcore/issues/33343), [#36208](https://github.com/dotnet/efcore/issues/36208)).
- Optimize multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384), contributed by [@ranma42](https://github.com/ranma42)).
- Optimize use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
- Optimize `MIN`/`MAX` over `DISTINCT` ([#34699](https://github.com/dotnet/efcore/pull/34699), contributed by [@ranma42](https://github.com/ranma42)).
- Simplify parameter names (e.g. from `@__city_0` to `@city`) ([#35200](https://github.com/dotnet/efcore/pull/35200)).

## ExecuteUpdateAsync now accepts a regular, non-expression lambda

The <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdateAsync*> can be used to express arbitrary update operations in the database. In previous versions, the changes to be performed on the database rows were provided via an expression tree parameter; this made it quite difficult to build those changes dynamically. For example, let's assume we want to update a Blog's Views, but conditionally also its Name. Since the setters argument was an expression tree, code such as the following needed to be written:

```c#
// Base setters - update the Views only
Expression<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>> setters =
    s => s.SetProperty(b => b.Views, 8);

// Conditionally add SetProperty(b => b.Name, "foo") to setters, based on the value of nameChanged
if (nameChanged)
{
    var blogParameter = Expression.Parameter(typeof(Blog), "b");

    setters = Expression.Lambda<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>>(
        Expression.Call(
            instance: setters.Body,
            methodName: nameof(SetPropertyCalls<Blog>.SetProperty),
            typeArguments: [typeof(string)],
            arguments:
            [
                Expression.Lambda<Func<Blog, string>>(Expression.Property(blogParameter, nameof(Blog.Name)), blogParameter),
                Expression.Constant("foo")
            ]),
        setters.Parameters);
}

await context.Blogs.ExecuteUpdateAsync(setters);
```

Manually creating expression trees is complicated and error-prone, and made this common scenario much more difficult than it should have been. Starting with EF 10, you can now write the following instead:

```c#
await context.Blogs.ExecuteUpdateAsync(s =>
{
    s.SetProperty(b => b.Views, 8);
    if (nameChanged)
    {
        s.SetProperty(b => b.Name, "foo");
    }
});
```

Thanks to [@aradalvand](https://github.com/aradalvand) for proposing and pushing for this change (in [#32018](https://github.com/dotnet/efcore/issues/32018)).

<a name="default-constrain-names"></a>

## Custom default constraint names

In previous versions of EF Core, when you specified a default value for a property, EF Core would always let the database automatically generate a constraint name. Now, you can explicitly specify the name for default value constraints for SQL Server, giving you more control over your database schema.

You can now specify a constraint name when defining default values in your model configuration:

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Property(b => b.IsActive)
        .HasDefaultValue(true, "DF_Blog_IsActive");

    modelBuilder.Entity<Post>()
        .Property(p => b.CreatedDate)
        .HasDefaultValueSql("GETDATE()", "DF_Post_CreatedDate");
}

```

You can also call `UseNamedDefaultConstraints` to enable automatic naming of all the default constraints. Note that if you have existing migrations then the next migration you add will rename every single default constraint in your model.

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.UseNamedDefaultConstraints();
}

```

<a name="other-improvements"></a>

## Other improvements

- Make SQL Server scaffolding compatible with Azure Data Explorer ([#34832](https://github.com/dotnet/efcore/pull/34832), contributed by [@barnuri](https://github.com/barnuri)).
- Associate the DatabaseRoot with the scoped options instance and not the singleton options ([#34477](https://github.com/dotnet/efcore/pull/34477), contributed by [@koenigst](https://github.com/koenigst)).
- Redact inlined constants from log when sensitive logging is off ([#35724](https://github.com/dotnet/efcore/pull/35724)).
- Improve LoadExtension to work correctly with dotnet run and lib* named libs ([#35617](https://github.com/dotnet/efcore/pull/35617), contributed by [@krwq](https://github.com/krwq)).
- Changes to AsyncLocal usage for better lazy loading performance ([#35835](https://github.com/dotnet/efcore/pull/35835), contributed by [@henriquewr](https://github.com/henriquewr)).
