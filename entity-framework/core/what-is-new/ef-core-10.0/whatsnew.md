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

## Azure SQL and SQL Server

### Vector search support

EF 10 brings full support for the recently-introduced [vector data type](/sql/t-sql/data-types/vector-data-type) and its supporting [`VECTOR_DISTANCE()`](/sql/t-sql/functions/vector-distance-transact-sql) function, available on Azure SQL Database and on SQL Server 2025. The vector data type allows storing *embeddings*, which are representation of meaning that can be efficiently searched over for similarity, powering AI workloads such as semantic search and retrieval-augmented generation (RAG).

To use the `vector` data type, simply add a .NET property of type `SqlVector<float>` to your entity type:

```c#
public class Blog
{
    // ...

    [Column(TypeName = "vector(1536)")]
    public SqlVector<float> Embedding { get; set; }
}
```

Then, insert embedding data by populating the Embedding property and calling `SaveChangesAsync()` as usual:

```c#
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = /* Set up your preferred embedding generator */;

var embedding = await embeddingGenerator.GenerateVectorAsync("Some text to be vectorized");
context.Blogs.Add(new Blog
{
    Name = "Some blog",
    Embedding = new SqlVector<float>(embedding)
});
await context.SaveChangesAsync();
```

Finally, use the [`EF.Functions.VectorDistance()`](/sql/t-sql/functions/vector-distance-transact-sql) function in your LINQ queries to perform similarity search for a given user query:

```c#
var sqlVector = /* some user query which we should search for similarity */;
var topSimilarBlogs = context.Blogs
    .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, sqlVector))
    .Take(3)
    .ToListAsync();
```

For more information on vector search, [see the documentation](xref:core/providers/sql-server/vector-search).

### JSON type support

EF 10 also fully supports the new [json data type](/sql/t-sql/data-types/json-data-type), also available on Azure SQL Database and on SQL Server 2025. While SQL Server has included JSON functionality for several versions, the data itself was stored in plain textual columns in the database; the new data type provides significant efficiency improvements and a safer way to store and interact with JSON.

With EF 10, if you've configured EF with `UseAzureSql()` or with a compatibility level of 170 or higher (SQL Server 2025), EF automatically defaults to using the new JSON data type. For example, the following entity type has a primitive collection (Tags, an array of strings) and Details (mapped as a complex type):

```c#
public class Blog
{
    public int Id { get; set; }

    public string[] Tags { get; set; }
    public required BlogDetails Details { get; set; }
}

public class BlogDetails
{
    public string? Description { get; set; }
    public int Viewers { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().ComplexProperty(b => b.Details, b => b.ToJson());
}
```

EF 10 creates the following table for the above:

```sql
CREATE TABLE [Blogs] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Tags] json NOT NULL,
    [Details] json NOT NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([Id])
);
```

LINQ querying is fully supported as well. For example, the following filters for blogs with more than 3 viewers:

```c#
var highlyViewedBlogs = await context.Blogs.Where(b => b.Details.Viewers > 3).ToListAsync();
```

This produces the following SQL, making use of the new `JSON_VALUE()` `RETURNING` clause:

```sql
SELECT [b].[Id], [b].[Name], [b].[Tags], [b].[Details]
FROM [Blogs] AS [b]
WHERE JSON_VALUE([b].[Details], '$.Viewers' RETURNING int) > 3
```

Note that if your EF application already uses JSON via `nvarchar` columns, these columns will be automatically changed to `json` with the first migration. You can opt out of this by manually setting the column type to `nvarchar(max)`, or configuring a compatibility level lower than 170.

<a name="default-constraint-names"></a>

### Custom default constraint names

EF 10 now allows you to specify a name for default constraints, rather than letting the database generate them:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .Property(p => b.CreatedDate)
        .HasDefaultValueSql("GETDATE()", "DF_Post_CreatedDate");
}
```

You can also enable automatic naming by EF of all default constraints:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.UseNamedDefaultConstraints();
}
```

> [!NOTE]
> If you have existing migrations, the next migration you add will rename every single default constraint in your model.

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

For more information on Cosmos full-text search, see the [documentation](xref:core/providers/cosmos/full-text-search).

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

For more information on Cosmos hybrid search, [see the documentation](xref:core/providers/cosmos/full-text-search?#hybrid-search).

### Vector similarity search exits preview

In EF9 we added experimental support for [vector similarity search](xref:core/what-is-new/ef-core-9.0/whatsnew#vector-similarity-search-preview). In EF Core 10, vector similarity search support is no longer experimental. We have also made some improvements to the feature:

- EF Core can now generate containers with vector properties defined on owned reference entities. Containers with vector properties defined on owned collections still have to be created by other means. However, they can be used in queries.
- Model building APIs have been renamed. A vector property can now be configured using the `IsVectorProperty` method, and vector index can be configured using the `IsVectorIndex` method.

For more information on Cosmos vector search, [see the documentation](xref:core/providers/cosmos/vector-search).

<a name="improved-model-evolution"></a>

### Improved experience when evolving the model

In previous versions of EF Core, evolving the model when using Azure Cosmos DB was quite painful. Specifically, when adding a new required property to the entity, EF would no longer be able to materialize that entity. The reason was that EF expected a value for the new property (since it was required), but the document created before the change didn't contain those values. The workaround was to mark the property as optional first, manually add default values for the property, and only then change it to required.

In EF 10 we improved this experience - EF will now materialize a default value for a required property, if no data is present for it in the document, rather than throw.

<a name="named-query-filters"></a>

## Named query filters

EF's [global query filters](xref:core/querying/filters) feature has long enabled users to configuring filters to entity types which apply to all queries by default. This has simplified implementing common patterns and scenarios such as soft deletion, multitenancy and others. However, up to now EF has only supported a single query filter per entity type, making it difficult to have multiple filters and selectively disabling only some of them in specific queries.

EF 10 introduces *named query filters*, which allow attaching names to query filter and managing each one separately:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#FilterConfiguration)]

This notably allows disabling only certain filters in a specific LINQ query:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#DisableSoftDeletionFilter)]

For more information on named query filters, [see the documentation](xref:core/querying/filters).

This feature was contributed by [@bittola](https://github.com/bittola).

<a name="linq-and-sql-translation"></a>

## LINQ and SQL translation

<a name="support-left-join"></a>

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

See [#12793](https://github.com/dotnet/efcore/issues/12793) and [#35367](https://github.com/dotnet/efcore/issues/35367) for more details.

<a name="other-query-improvements"></a>

### Other query improvements

- Translate [DateOnly.ToDateTime()](/dotnet/api/system.dateonly.todatetime) ([#35194](https://github.com/dotnet/efcore/pull/35194), contributed by [@mseada94](https://github.com/mseada94)).
- Translate [DateOnly.DayNumber](/dotnet/api/system.dateonly.daynumber) and `DayNumber` subtraction for SQL Server and SQLite ([#36183](https://github.com/dotnet/efcore/issues/36183)).
- Optimize multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384), contributed by [@ranma42](https://github.com/ranma42)).
- Optimize use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
- Optimize `MIN`/`MAX` over `DISTINCT` ([#34699](https://github.com/dotnet/efcore/pull/34699), contributed by [@ranma42](https://github.com/ranma42)).
- Translate date/time functions using `DatePart.Microsecond` and `DatePart.Nanosecond` arguments ([#34861](https://github.com/dotnet/efcore/pull/34861)).
- Simplify parameter names (e.g. from `@__city_0` to `@city`) ([#35200](https://github.com/dotnet/efcore/pull/35200)).
- Translate `COALESCE` as `ISNULL` on SQL Server, for most cases ([#34171](https://github.com/dotnet/efcore/pull/34171), contributed by [@ranma42](https://github.com/ranma42)).
- Support some string functions taking `char` as arguments ([#34999](https://github.com/dotnet/efcore/pull/34999), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
- Support `MAX`/`MIN`/`ORDER BY` using `decimal` on SQLite ([#35606](https://github.com/dotnet/efcore/pull/35606), contributed by [@ranma42](https://github.com/ranma42)).
- Support projecting different navigations (but same type) via conditional operator ([#34589](https://github.com/dotnet/efcore/issues/34589), contributed by [@ranma42](https://github.com/ranma42)).

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

<a name="other-improvements"></a>

## Other improvements

- Make SQL Server scaffolding compatible with Azure Data Explorer ([#34832](https://github.com/dotnet/efcore/pull/34832), contributed by [@barnuri](https://github.com/barnuri)).
- Associate the DatabaseRoot with the scoped options instance and not the singleton options ([#34477](https://github.com/dotnet/efcore/pull/34477), contributed by [@koenigst](https://github.com/koenigst)).
- Redact inlined constants from log when sensitive logging is off ([#35724](https://github.com/dotnet/efcore/pull/35724)).
- Improve LoadExtension to work correctly with dotnet run and lib* named libs ([#35617](https://github.com/dotnet/efcore/pull/35617), contributed by [@krwq](https://github.com/krwq)).
- Changes to AsyncLocal usage for better lazy loading performance ([#35835](https://github.com/dotnet/efcore/pull/35835), contributed by [@henriquewr](https://github.com/henriquewr)).
