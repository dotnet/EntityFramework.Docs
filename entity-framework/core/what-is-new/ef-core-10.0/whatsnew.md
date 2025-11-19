---
title: What's New in EF Core 10
description: Overview of new features in EF Core 10
author: roji
ms.date: 10/02/2025
uid: core/what-is-new/ef-core-10.0/whatsnew
---

# What's New in EF Core 10

EF Core 10.0 (EF10) was released in November 2025 and is a Long Term Support (LTS) release. EF10 will be supported until November 10, 2028.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF10 requires the .NET 10 SDK to build and requires the .NET 10 runtime to run. EF10 will not run on earlier .NET versions, and will not run on .NET Framework.

The below release notes list the major improvements in the release; [the full list of issues for the release can be found here](https://github.com/dotnet/efcore/issues?q=is%3Aissue%20milestone%3A10.0.0).

<a name="sql-server"></a>

## Azure SQL and SQL Server

<a name="sql-server-vector-search"></a>

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

<a name="sql-server-json-type"></a>

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

<a name="cosmos"></a>

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

<a name="other-cosmos-improvements"></a>

## Other Cosmos improvements

- Use ExecutionStrategy for query execution (for retrying) ([#35692](https://github.com/dotnet/efcore/issues/35692)).

<a name="complex-types"></a>

## Complex types

Complex types are used to model types which are contained within your entity types and have no identity of their own; while entity types are (usually) mapped to a database table, complex types can be mapped to columns in their container table ("table splitting"), or to a single JSON column. Complex types introduce document modeling techniques, which can bring substantial performance benefits as traditional JOINs are avoided, and can make your database modeling much simpler and more natural.

### Table splitting

For example, the following maps a customer's addresses as complex types:

```c#
modelBuilder.Entity<Customer>(b =>
{
    b.ComplexProperty(c => c.ShippingAddress);
    b.ComplexProperty(c => c.BillingAddress);
});
```

On relational database, this causes the addresses to be mapped to additional columns in the main `Customers` table:

```sql
CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [BillingAddress_City] nvarchar(max) NOT NULL,
    [BillingAddress_PostalCode] nvarchar(max) NOT NULL,
    [BillingAddress_Street] nvarchar(max) NOT NULL,
    [BillingAddress_StreetNumber] int NOT NULL,
    [ShippingAddress_City] nvarchar(max) NOT NULL,
    [ShippingAddress_PostalCode] nvarchar(max) NOT NULL,
    [ShippingAddress_Street] nvarchar(max) NOT NULL,
    [ShippingAddress_StreetNumber] int NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
```

Note the difference with the default, traditional relational behavior of mapping the addresses to a separate table and using a foreign key to represent the customer/address relationship.

While the above was already possible since EF 8, EF 10 adds support for **optional** types:

```c#
public class Customer
{
    ...

    public Address ShippingAddress { get; set; }
    public Address? BillingAddress { get; set; }
}
```

Note that optional complex types currently require at least one required property to be defined on the complex type.

### JSON

EF 10 now allows mapping complex types to JSON:

```c#
modelBuilder.Entity<Customer>(b =>
{
    b.ComplexProperty(c => c.ShippingAddress, c => c.ToJson());
    b.ComplexProperty(c => c.BillingAddress, c => c.ToJson());
});
```

This causes EF to map each Address to a single JSON column in the customer table. When using the new SQL Server 2025 JSON column ([see above](#sql-server-json-type)), this causes the following table to be created:

```sql
CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ShippingAddress] json NOT NULL,
    [BillingAddress] json NOT NULL NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
```

Unlike table splitting, JSON mapping allows collections within the mapped type. You can query and update properties inside your JSON documents just like any other non-JSON property, and perform efficient bulk updating on them via `ExecuteUpdateAsync` ([see release note](#execute-update-json)).

### Struct support

Complex types also supports mapping .NET structs instead of classes:

```c#
public struct Address
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string ZipCode { get; set; }
}
```

This aligns well with complex types not having an identity of their own, and only being found within entity types which have an identity. However, collections of structs aren't currently supported.

### Complex and owned entity types

Both table splitting and JSON mapping have been supported before EF 10 via owned entity modeling. However, this modeling created quite a few issues stemming from the fact that owned entity types are entity types, and therefore still operate with reference semantics and an identity behind the scenes.

For example, trying to assign a customer's billing address to be the same as their shipping address fails with owned entity types, since the same entity type can't be referenced more than once:

```c#
var customer = await context.Customers.SingleAsync(c => c.Id == someId);
customer.BillingAddress = customer.ShippingAddress;
await context.SaveChangesAsync(); // ERROR
```

In contrast, since complex types have value semantics, assigning them simply copies their properties over, as expected. For the same reasons, bulk assignment of owned entity types is not supported, whereas complex types fully support `ExecuteUpdateAsync` in EF 10 ([see release note](#execute-update-json)).

Similarly, comparing a customer's shipping and billing addresses in LINQ queries does not work as expected, since entity types are compared by their identities; complex types, on the other hand, are compared by their contents, producing the expected result.

These issues - as well as various others - make complex types the better choice for modeling JSON and table splitting, and users already using owned entity types for these are advised to switch to complex types.

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

This allows the collection values to change without resulting in different SQLs - solving the plan cache problem - but at the same time provides the query planner with information on the collection cardinality.

Since different cardinalities still cause different SQLs to be generated, EF also "pads" parameter list. For example, if your `ids` list contains 8 values, EF generates SQL with 10 parameters:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (@ids1, @ids2, @ids3, @ids4, @ids5, @ids6, @ids7, @ids8, @ids9, @ids10)
```

The last two parameters `@ids9` and `@ids10` are added by EF to reduce the number of SQLs generated, and contain the same value as `@ids8`, so that the query returns the same result.

Unfortunately, parameterized collections are a case where EF simply cannot always make the right choice: selecting between multiple parameters (the new default), a single JSON array parameter (with e.g. SQL Server `OPENJSON`) or multiple inlined constants can require knowledge about the data in your database, and different choices may work better for different queries. As a result, EF exposes full control to the user to control the translation strategy, both at the global configuration level:

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

### More consistent ordering for split queries

Split queries can be essential to avoid performance issues associated with JOINs, such as the so-called "cartesian explosion" effect (see [Single vs. Split Queries](xref:core/querying/single-split-queries) to learn more). However, since split queries loads related data in separate SQL queries, consistency issues can arise, possibly leading to non-deterministic, hard-to-detect data corruption.

For example, consider the following query:

```C#
var blogs = await context.Blogs
    .AsSplitQuery()
    .Include(b => b.Posts)
    .OrderBy(b => b.Name)
    .Take(2)
    .ToListAsync();
```

Prior to EF10, the following two SQL queries were generated:

```sql
SELECT TOP(@__p_0) [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
ORDER BY [b].[Name], [b].[Id]

SELECT [p].[Id], [p].[BlogId], [p].[Title], [b0].[Id]
FROM (
    SELECT TOP(@__p_0) [b].[Id], [b].[Name]
    FROM [Blogs] AS [b]
    ORDER BY [b].[Name]
) AS [b0]
INNER JOIN [Post] AS [p] ON [b0].[Id] = [p].[BlogId]
ORDER BY [b0].[Name], [b0].[Id]
```

Note that the first query (the one reading Blogs) is integrated as a subquery within the second (the one reading Posts). However, the ordering within the subquery omits the `Id` column, leading to possible incorrect data being returned.

EF10 fixes this by ensuring that the ordering is consistent across the queries:

```sql
SELECT [p].[Id], [p].[BlogId], [p].[Title], [b0].[Id]
FROM (
    SELECT TOP(@p) [b].[Id], [b].[Name]
    FROM [Blogs] AS [b]
    ORDER BY [b].[Name], [b].[Id]
) AS [b0]
INNER JOIN [Post] AS [p] ON [b0].[Id] = [p].[BlogId]
ORDER BY [b0].[Name], [b0].[Id]
```

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
- Fix translation of `DefaultIfEmpty` in various scenarios:
  - [DefaultIfEmpty applied on child collection wipes the parent info in query result](https://github.com/dotnet/efcore/issues/19095)
  - [Logic for lifting DefaultIfEmpty out of SelectMany (to LEFT JOIN/OUTER APPLY) is incorrect](https://github.com/dotnet/efcore/issues/33343)
  - [NavigationExpandingExpressionVisitor moves Select() behind DefaultIfEmpty()](https://github.com/dotnet/efcore/issues/36208)
  - [EF Core 9 no longer applies COALESCE in SQL translation for DefaultIfEmpty() which causing an InvalidOperationException](https://github.com/dotnet/efcore/issues/35950)
- Optimize multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384), contributed by [@ranma42](https://github.com/ranma42)).
- Optimize use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
- Optimize `MIN`/`MAX` over `DISTINCT` ([#34699](https://github.com/dotnet/efcore/pull/34699), contributed by [@ranma42](https://github.com/ranma42)).
- Simplify parameter names (e.g. from `@__city_0` to `@city`) ([#35200](https://github.com/dotnet/efcore/pull/35200)).

<a name="execute-update-json"></a>

## ExecuteUpdate support for relational JSON columns

> [!NOTE]
> ExecuteUpdate support for JSON requires mapping your types as complex types ([see above](#json)), and does not work when your types are mapped as owned entities.

Although EF has support JSON columns for some time and allows updating them via `SaveChanges`, `ExecuteUpdate` lacked support for them. EF10 now allows referencing JSON columns and properties within them in `ExecuteUpdate`, allowing efficient bulk updating of document-modeled data within relational databases.

For example, given the following model, mapping the `BlogDetails` type to a complex JSON column in the database:

```c#
public class Blog
{
    public int Id { get; set; }

    public BlogDetails Details { get; set; }
}

public class BlogDetails
{
    public string Title { get; set; }
    public int Views { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().ComplexProperty(b => b.Details, bd => bd.ToJson());
}
```

You can now use `ExecuteUpdate` as usual, referencing properties within `BlogDetails`:

```c#
await context.Blogs.ExecuteUpdateAsync(s =>
    s.SetProperty(b => b.Details.Views, b => b.Details.Views + 1));
```

This generates the following for SQL Server 2025, using the efficient, new [`modify`](/sql/t-sql/data-types/json-data-type#the-modify-method) function to increment the JSON property `Views` by one:

```sql
UPDATE [b]
SET [Details].modify('$.Views', JSON_VALUE([b].[Details], '$.Views' RETURNING int) + 1)
FROM [Blogs] AS [b]
```

Older versions of SQL Server are also supported, where the JSON data is stored in an `nvarchar(max)` column rather than the new JSON data type support. For support with other databases, consult the documentation for your EF provider.

<a name="named-query-filters"></a>

## Named query filters

EF's [global query filters](xref:core/querying/filters) feature has long enabled users to configuring filters to entity types which apply to all queries by default. This has simplified implementing common patterns and scenarios such as soft deletion, multitenancy and others. However, up to now EF has only supported a single query filter per entity type, making it difficult to have multiple filters and selectively disabling only some of them in specific queries.

EF 10 introduces *named query filters*, which allow attaching names to query filter and managing each one separately:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#FilterConfiguration)]

This notably allows disabling only certain filters in a specific LINQ query:

[!code-csharp[Main](../../../../samples/core/Querying/QueryFilters/NamedFilters.cs#DisableSoftDeletionFilter)]

For more information on named query filters, [see the documentation](xref:core/querying/filters).

This feature was contributed by [@bittola](https://github.com/bittola).

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

## Security-related improvements

### Redact inlined constants from logging by default

When logging executed SQL, EF does not log parameter values by default, since these may contain sensitive or personally-identifiable information (PII); <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.EnableSensitiveDataLogging> can be used to enable logging parameter values in diagnostic or debugging scenarios ([see documentation](xref:core/logging-events-diagnostics/extensions-logging#sensitive-data)).

However, EF sometimes inlines parameters into the SQL statement rather than sending them separately; in those scenarios, the potentially sensitive values were logged. EF10 no longer does this, redacting such inlined parameters by default and replacing them with a question mark character (`?`). For example, let's say we have a function that accepts a list of roles, and returns the list of users which have one of those roles:

```c#
Task<List<User>> GetUsersByRoles(BlogContext context, string[] roles)
    => context.Users.Where(b => roles.Contains(b.Role)).ToListAsync();
```

Although the roles here are parameterized (may differ across invocations), we know that the actual sets of roles queried will be quite limited. We can therefore tell EF to *inline* the roles, so that the database's planner can plan execution separately for this query with its specific set of roles:

```c#
Task<List<User>> GetUsersByRoles(BlogContext context, string[] roles)
    => context.Users.Where(b => EF.Constant(roles).Contains(b.Role)).ToListAsync();
```

On previous versions of EF, this produced the following SQL:

```sql
SELECT [u].[Id], [u].[Role]
FROM [Users] AS [u]
WHERE [u].[Role] IN (N'Administrator', N'Manager')
```

EF10 sends the same SQL to the database, but logs the following SQL, where the roles have been redacted:

```sql
SELECT [b].[Id], [b].[Role]
FROM [Blogs] AS [b]
WHERE [b].[Role] IN (?, ?)
```

If the roles represent sensitive information, this prevents that information from leaking into the application logs. As with regular parameters, full logging can be reenabled via <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.EnableSensitiveDataLogging>.

### Warn for string concatenation with raw SQL APIs

SQL injection is one of the most serious security vulnerabilities in database applications; if unsanitized external input is inserted into SQL queries, a malicious user may be able to perform SQL injection, executing arbitrary SQL on your database. For more information about EF's SQL querying APIs and SQL injection, [see the documentation](xref:core/querying/sql-queries#passing-parameters).

While EF users rarely deal with SQL directly, EF does provide SQL-based APIs for the cases where they're needed. Most of these APIs are safe in the face of SQL injection; for example <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSql*> accepts a .NET `FormattableString`, and any parameter will automatically be send as a separate SQL parameter, preventing SQL injection. However, in some scenarios it's necessary to build up a dynamic SQL query by piecing together multiple fragments; <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.FromSqlRaw*> is designed specifically for this, and requires the user to ensure that all fragments are trusted or properly sanitized.

Starting with EF10, EF has an analyzer which warns if concatenation is performed within a "raw" SQL method invocation:

```c#
var users = context.Users.FromSqlRaw("SELECT * FROM Users WHERE [" + fieldName + "] IS NULL");
```

If `fieldName` is trusted or has been properly sanitized, the warning can be safely suppressed.

## Other improvements

- Stop spanning all migrations with a single transaction ([#35096](https://github.com/dotnet/efcore/issues/35096)). This reverts a change done in EF9 which caused issues in various migration scenarios.
- Make SQL Server scaffolding compatible with Azure Data Explorer ([#34832](https://github.com/dotnet/efcore/pull/34832), contributed by [@barnuri](https://github.com/barnuri)).
- Associate the DatabaseRoot with the scoped options instance and not the singleton options ([#34477](https://github.com/dotnet/efcore/pull/34477), contributed by [@koenigst](https://github.com/koenigst)).
- Improve LoadExtension to work correctly with dotnet run and lib* named libs ([#35617](https://github.com/dotnet/efcore/pull/35617), contributed by [@krwq](https://github.com/krwq)).
- Changes to AsyncLocal usage for better lazy loading performance ([#35835](https://github.com/dotnet/efcore/pull/35835), contributed by [@henriquewr](https://github.com/henriquewr)).

[The full list of issues for the release can be found here](https://github.com/dotnet/efcore/issues?q=is%3Aissue%20milestone%3A10.0.0).
