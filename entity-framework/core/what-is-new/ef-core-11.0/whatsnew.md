---
title: What's New in EF Core 11
description: Overview of new features in EF Core 11
author: roji
ms.date: 02/02/2026
uid: core/what-is-new/ef-core-11.0/whatsnew
---

# What's New in EF Core 11

EF Core 11.0 (EF11) is the next release after EF Core 10.0 and is currently in development.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF11 requires the .NET 11 SDK to build and requires the .NET 11 runtime to run. EF11 will not run on earlier .NET versions, and will not run on .NET Framework.

## Complex types

<a name="complex-types-tpt-tpc"></a>

### Complex types and JSON columns on entity types with TPT/TPC inheritance

EF Core has supported complex types and JSON columns for several versions, allowing you to model and persist nested, structured data within your entities. However, until now these features could not be used on entities with TPT (table-per-type) or TPC (table-per-concrete-type) inheritance.

Starting with EF 11, you can now use complex types and JSON columns on entity types with TPT and TPC inheritance mappings. For example, consider the following entity types with a TPT inheritance strategy:

```csharp
public abstract class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public required AnimalDetails Details { get; set; }
}

public class Dog : Animal
{
    public string Breed { get; set; }
}

public class Cat : Animal
{
    public bool IsIndoor { get; set; }
}

[ComplexType]
public class AnimalDetails
{
    public DateTime BirthDate { get; set; }
    public string? Veterinarian { get; set; }
}
```

With the following TPT mapping configuration:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Animal>().UseTptMappingStrategy();
}
```

EF 11 now properly supports this scenario, creating the `Details_BirthDate` and `Details_Veterinarian` columns on the `Animal` table for the complex type properties.

Similarly, JSON columns are now supported with TPT/TPC. The following configuration maps the `Details` property to a JSON column:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Animal>()
        .UseTptMappingStrategy()
        .ComplexProperty(a => a.Details, b => b.ToJson());
}
```

This enhancement removes a significant limitation when modeling complex domain hierarchies, allowing you to combine the flexibility of TPT/TPC inheritance with the power of complex types and JSON columns.

For more information on inheritance mapping strategies, see [Inheritance](xref:core/modeling/inheritance).

<a name="complex-types-cosmos"></a>

### Complex types in Azure Cosmos DB

Complex types are now fully supported in the Azure Cosmos DB provider, embedded as nested JSON objects or arrays. For more information, [see the Cosmos DB section below](#cosmos-complex-types).

<a name="complex-types-stabilization"></a>

### Stabilization and bug fixes

Significant effort has gone into making sure that complex type support is stable and bug-free, to unblock using complex types as an alternative to the owned entity mapping approach. Bugs fixed include:

* [Error querying on complex type whose container is mapped to a table and a view](https://github.com/dotnet/efcore/issues/34706)
* [Problem with ComplexProperty in EF9, when using the TPT approach](https://github.com/dotnet/efcore/issues/35392)
* [Comparison of complex types does not compare properties within nested complex types](https://github.com/dotnet/efcore/issues/37391)
* [Assignment of complex type does not assign nested properties correctly (ExecuteUpdate)](https://github.com/dotnet/efcore/issues/37395)
* [Map two classes with same nullable complex properties to same column → NullReferenceException](https://github.com/dotnet/efcore/issues/37335)
* [Complex property stored as JSON marked non-nullable in TPH class hierarchy](https://github.com/dotnet/efcore/issues/37404)
* [EntityEntry.ReloadAsync throws when nullable complex property is null](https://github.com/dotnet/efcore/issues/37559)
* [Unnecessary columns in SQL with Complex Types + object closures in projections](https://github.com/dotnet/efcore/issues/37551)

## LINQ and SQL translation

<a name="linq-to-one-join-pruning"></a>

### Better SQL for to-one joins

EF Core 11 generates better SQL when querying with to-one (reference) navigation includes in two ways.

First, when using split queries (`AsSplitQuery()`), EF previously added unnecessary joins to reference navigations in the SQL generated for collection queries. For example, consider the following query:

```csharp
var blogs = context.Blogs
    .Include(b => b.BlogType)
    .Include(b => b.Posts)
    .AsSplitQuery()
    .ToList();
```

EF Core previously generated a split query for `Posts` that unnecessarily joined `BlogType`:

```sql
-- Before EF Core 11
SELECT [p].[Id], [p].[BlogId], [p].[Title], [b].[Id], [b0].[Id]
FROM [Blogs] AS [b]
INNER JOIN [BlogType] AS [b0] ON [b].[BlogTypeId] = [b0].[Id]
INNER JOIN [Post] AS [p] ON [b].[Id] = [p].[BlogId]
ORDER BY [b].[Id], [b0].[Id]
```

In EF Core 11, the unneeded join is pruned:

```sql
-- EF Core 11
SELECT [p].[Id], [p].[BlogId], [p].[Title], [b].[Id]
FROM [Blogs] AS [b]
INNER JOIN [Post] AS [p] ON [b].[Id] = [p].[BlogId]
ORDER BY [b].[Id]
```

Second, EF no longer adds redundant keys from reference navigations to `ORDER BY` clauses. Because a reference navigation's key is functionally determined by the parent entity's key (via the foreign key), it does not need to appear separately. For example:

```csharp
var blogs = context.Blogs
    .Include(b => b.Owner)
    .Include(b => b.Posts)
    .ToList();
```

EF Core previously included `[p].[PersonId]` in the `ORDER BY`, even though `[b].[BlogId]` already uniquely identifies the row:

```sql
-- Before EF Core 11
ORDER BY [b].[BlogId], [p].[PersonId]
```

In EF Core 11, the redundant column is omitted:

```sql
-- EF Core 11
ORDER BY [b].[BlogId]
```

Both optimizations can have a significant positive impact on query performance, especially when multiple reference navigations are included. A simple, common split query scenario showed a **29% improvement in querying performance**, as the database no longer has to perform the to-one join; single queries are also significantly improved by the removal of the ORDER BY, even if a bit less: one scenario showed a **22% improvement**.

More details on the benchmark are available [here](https://github.com/dotnet/efcore/issues/29182#issuecomment-4231140289), and as always, actual performance in your application will vary based on your schema, data and a variety of other factors.

<a name="linq-maxby-minby"></a>

### MaxBy and MinBy

EF Core now supports translating the LINQ `MaxByAsync` and `MinByAsync` methods (and their sync counterparts). These methods allow you to find the element with the maximum or minimum value for a given key selector, rather than just the maximum or minimum value itself.

For example, to find the blog with the most posts:

```csharp
var blogWithMostPosts = await context.Blogs.MaxByAsync(b => b.Posts.Count());
```

This translates to the following SQL:

```sql
SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
ORDER BY (
    SELECT COUNT(*)
    FROM [Posts] AS [p]
    WHERE [b].[Id] = [p].[BlogId]) DESC
```

Similarly, `MinByAsync` orders ascending and returns the element with the minimum value for the key selector.

### EF.Functions.JsonPathExists()

EF Core 11 introduces `EF.Functions.JsonPathExists()`, which checks whether a given JSON path exists in a JSON document. On SQL Server, this translates to the [`JSON_PATH_EXISTS`](/sql/t-sql/functions/json-path-exists-transact-sql) function (available since SQL Server 2022).

The following query filters blogs to those whose JSON data contains an `OptionalInt` property:

```csharp
var blogs = await context.Blogs
    .Where(b => EF.Functions.JsonPathExists(b.JsonData, "$.OptionalInt"))
    .ToListAsync();
```

This generates the following SQL:

```sql
SELECT [b].[Id], [b].[Name], [b].[JsonData]
FROM [Blogs] AS [b]
WHERE JSON_PATH_EXISTS([b].[JsonData], N'$.OptionalInt') = 1
```

`EF.Functions.JsonPathExists()` accepts a JSON value and a JSON path to check for. It can be used with scalar string properties, complex types, and owned entity types mapped to JSON columns.

For the full `JSON_PATH_EXISTS` SQL Server documentation, see [`JSON_PATH_EXISTS`](/sql/t-sql/functions/json-path-exists-transact-sql).

## SQL Server

<a name="sqlserver-vector-search"></a>

### VECTOR_SEARCH() and vector indexes

> [!WARNING]
> `VECTOR_SEARCH()` and vector indexes are currently experimental features in SQL Server and are subject to change. The APIs in EF Core for these features are also subject to change.

In EF Core 10, we introduced translation for `EF.Functions.VectorDistance()`, which is a scalar function that computes the distance between two vectors. This function can be used in LINQ queries for vector similarity search, allowing you to find the most similar embeddings to a given embedding. However, `VectorDistance()` computes an _exact_ distance between the given vectors.

When querying large datasets, SQL Server 2025 also supports performing _approximate_ search over a [vector index](/sql/t-sql/statements/create-vector-index-transact-sql), which provides much better performance at the expense of returning items that are approximately similar - rather than exactly similar - to the query. EF 11 now supports creating vector indexes through migrations:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasVectorIndex(b => b.Embedding, "cosine");
}
```

Once you have a vector index, you can use the `VectorSearch()` extension method on your `DbSet` to perform an approximate search:

```csharp
var blogs = await context.Blogs
    .VectorSearch(b => b.Embedding, embedding, "cosine", topN: 5)
    .ToListAsync();
```

This translates to the SQL Server [`VECTOR_SEARCH()`](/sql/t-sql/functions/vector-search-transact-sql) table-valued function, which performs an approximate search over the vector index. The `topN` parameter specifies the number of results to return.

`VectorSearch()` returns `VectorSearchResult<TEntity>`, allowing you to access the distance alongside the entity.

For more information, see the [full documentation on vector search](xref:core/providers/sql-server/vector-search).

<a name="sqlserver-vector-not-auto-loaded"></a>

### Vector properties not loaded by default

EF Core 11 changes how vector properties are loaded: `SqlVector<T>` columns are no longer included in `SELECT` statements when materializing entities. Since vectors can be quite large—containing hundreds or thousands of floating-point numbers—this avoids unnecessary data transfer in the common case where vectors are ingested and used for search but not read back.

```csharp
// Vector column is excluded from the projected entity
var blogs = await context.Blogs.OrderBy(b => b.Name).ToListAsync();
// Generates: SELECT [b].[Id], [b].[Name] FROM [Blogs] AS [b] ...

// Explicit projection still loads the vector
var embeddings = await context.Blogs
    .Select(b => new { b.Id, b.Embedding })
    .ToListAsync();
```

Vector properties can still be used in `WHERE` and `ORDER BY` clauses—including with `VectorDistance()` and `VectorSearch()`—and EF will correctly include them in the SQL, just not in the entity projection.

This optimization can have a dramatic impact on application speed: a minimal benchmark showed an almost 9x (that's nine-fold) increase in performance, and that's while running against a local database. The optimization is even more impactful as latency to the database grows: when executing against a remote Azure SQL database, an improvement of around 22x was observed. More details on the benchmark are available [here](https://github.com/dotnet/efcore/issues/37279#issuecomment-4232243062), and as always, actual performance in your application will vary based on your entity, vector properties and latency.

<a name="sqlserver-full-text"></a>

### Full-text search improvements

#### Full-text search catalog and index creation

SQL Server's [full-text search](/sql/relational-databases/search/full-text-search) requires a full-text catalog and index to be set up on your database before you can use it. EF 11 now allows configuring full-text catalogs and indexes in your model, so that [EF migrations](xref:core/managing-schemas/migrations/index) can automatically create and manage them for you:

```csharp
// In your OnModelCreating:
modelBuilder.HasFullTextCatalog("ftCatalog");

modelBuilder.Entity<Blog>()
    .HasFullTextIndex(b => b.FullName)
    .UseKeyIndex("PK_Blogs")
    .UseCatalog("ftCatalog");
```

This generates the following SQL in a migration:

```sql
CREATE FULLTEXT CATALOG [ftCatalog];
CREATE FULLTEXT INDEX ON [Blogs]([FullName]) KEY INDEX [PK_Blogs] ON [ftCatalog];
```

Previously, full-text catalog and index creation had to be managed manually by adding SQL to migrations. For full details on setting up full-text catalogs and indexes, see the [full-text search documentation](xref:core/providers/sql-server/full-text-search#setting-up-full-text-search).

<a name="sqlserver-full-text-tvf"></a>

#### Full-text search table-valued functions

EF Core has long provided support for SQL Server's full-text search predicates `FREETEXT()` and `CONTAINS()`, via `EF.Functions.FreeText()` and `EF.Functions.Contains()`. These predicates can be used in LINQ `Where()` clauses to filter results based on search criteria.

However, SQL Server also has table-valued function versions of these functions, [`FREETEXTTABLE()`](/sql/relational-databases/system-functions/freetexttable-transact-sql) and [`CONTAINSTABLE()`](/sql/relational-databases/system-functions/containstable-transact-sql), which also return a ranking score along with the results, providing additional flexibility over the predicate versions. EF 11 now supports these table-valued functions:

```csharp
// Using FreeTextTable with a search query
var results = await context.Blogs
    .FreeTextTable("John", b => b.FullName)
    .Select(r => new { Blog = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();

// Using ContainsTable with a search query
var results = await context.Blogs
    .ContainsTable("John", b => b.FullName)
    .Select(r => new { Blog = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

Both methods return `FullTextSearchResult<TEntity>`, giving you access to both the entity and the ranking value from SQL Server's full-text engine. This allows for more sophisticated result ordering and filtering based on relevance scores.

For more information, see the [full documentation on full-text search](xref:core/providers/sql-server/full-text-search).

<a name="sql-server-json-contains"></a>

### Contains operations using JSON_CONTAINS

SQL Server 2025 introduced the [`JSON_CONTAINS`](/sql/t-sql/functions/json-contains-transact-sql) function, which checks whether a value exists in a JSON document. Starting with EF Core 11, when targeting SQL Server 2025, LINQ `Contains` queries over primitive (or scalar) collections stored as JSON are translated to use this function, replacing the previous, less efficient `OPENJSON`-based translation.

The following query checks whether a blog's `Tags` collection contains a specific tag:

```csharp
var blogs = await context.Blogs
    .Where(b => b.Tags.Contains("ef-core"))
    .ToListAsync();
```

Before EF 11 - or when targeting older SQL Server versions - this generates the following SQL:

```sql
SELECT [b].[Id], [b].[Name], [b].[Tags]
FROM [Blogs] AS [b]
WHERE N'ef-core' IN (
    SELECT [t].[value]
    FROM OPENJSON([b].[Tags]) WITH ([value] nvarchar(max) '$') AS [t]
)
```

With EF 11, configure EF to target SQL Server 2025 by setting the compatibility level as follows:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer("<CONNECTION STRING>", o => o.UseCompatibilityLevel(170));
```

At that point, EF will instead generate the following SQL:

```sql
SELECT [b].[Id], [b].[Name], [b].[Tags]
FROM [Blogs] AS [b]
WHERE JSON_CONTAINS([b].[Tags], 'ef-core') = 1
```

`JSON_CONTAINS()` can notably make use of a [JSON index](/sql/t-sql/statements/create-json-index-transact-sql), if one is defined.

> [!NOTE]
> `JSON_CONTAINS` does not support searching for null values. As a result, this translation is only applied when EF can determine that at least one side is non-nullable — either the item being searched for (e.g. a non-null constant or a non-nullable column), or the collection's elements. When this cannot be determined, EF falls back to the previous `OPENJSON`-based translation.

#### EF.Functions.JsonContains()

In the section above, EF Core automatically translates LINQ `Contains` queries over primitive collections to use the SQL Server `JSON_CONTAINS` function. In some cases, however, you may want to directly invoke `JSON_CONTAINS` yourself, for example to search for a value at a specific path, or to specify a search mode. For these cases, EF Core 11 introduces `EF.Functions.JsonContains()`.

The following query checks whether a blog's JSON data contains a specific value at a given path:

```csharp
var blogs = await context.Blogs
    .Where(b => EF.Functions.JsonContains(b.JsonData, 8, "$.Rating") == 1)
    .ToListAsync();
```

This generates the following SQL:

```sql
SELECT [b].[Id], [b].[Name], [b].[JsonData]
FROM [Blogs] AS [b]
WHERE JSON_CONTAINS([b].[JsonData], 8, N'$.Rating') = 1
```

`EF.Functions.JsonContains()` accepts the JSON value to search in, the value to search for, and optionally a JSON path and a search mode. It can be used with scalar string properties, complex types, and owned entity types mapped to JSON columns.

For the full `JSON_CONTAINS` SQL Server documentation, see [`JSON_CONTAINS`](/sql/t-sql/functions/json-contains-transact-sql).

<a name="sqlserver-temporal-clr-properties"></a>

### Temporal period properties mapped to CLR properties

Previously, the period properties (`PeriodStart`/`PeriodEnd`) on temporal tables were required to be [shadow properties](xref:core/modeling/shadow-properties), meaning they existed only in the EF model and not as CLR properties on your .NET entity types. Starting with EF Core 11, period properties can now be mapped to regular CLR properties on the entity type, making it easier to access their values directly without using `EF.Property`.

To map period columns to CLR properties, add `DateTime` properties to your entity type and configure them with the lambda-based `HasPeriodStart`/`HasPeriodEnd` overloads:

```csharp
public class Employee
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

modelBuilder
    .Entity<Employee>()
    .ToTable(
        "Employees",
        b => b.IsTemporal(
            b =>
            {
                b.HasPeriodStart(e => e.PeriodStart);
                b.HasPeriodEnd(e => e.PeriodEnd);
            }));
```

Once period properties are mapped to CLR properties, you can reference them directly in LINQ queries — for example, in `OrderBy`, `Select`, or `Where` clauses — without needing `EF.Property`:

```csharp
var history = context.Employees
    .TemporalAll()
    .OrderBy(e => e.PeriodStart)
    .Select(e => new { e.Name, e.PeriodStart, e.PeriodEnd })
    .ToList();
```

Period properties remain configured with `ValueGenerated.OnAddOrUpdate`, so their values are always generated by SQL Server and excluded from INSERT and UPDATE statements.

For more information, see the [full documentation on temporal tables](xref:core/providers/sql-server/temporal-tables#mapping-period-columns-to-clr-properties).

<a name="sqlserver-datetimeoffset-translations"></a>

### Additional DateTimeOffset and date/time translations

EF Core 11 adds several new SQL Server translations for `DateTimeOffset`. Properties such as `DateTime`, `UtcDateTime`, and `LocalDateTime` are now translated, allowing you to extract a `DateTime` from a `DateTimeOffset` in different time zones directly in your queries. `Offset.TotalMinutes` is also translated, giving access to the offset component, and `ToOffset()` allows converting a `DateTimeOffset` to a different offset via SQL Server's `SWITCHOFFSET`.

You can also now construct a `DateTimeOffset` from a `DateTime` directly in LINQ queries, using `new DateTimeOffset(dateTime)` or `new DateTimeOffset(dateTime, offset)`, which translates to SQL Server's `TODATETIMEOFFSET` function.

In addition, `EF.Functions.DateTrunc()` is now available for truncating `DateTime`, `DateTimeOffset`, `DateOnly` and `TimeOnly` values to a specified precision (e.g. day, hour, minute), translating to SQL Server's [`DATETRUNC`](/sql/t-sql/functions/datetrunc-transact-sql) function.

For the complete list of date/time function translations, see the [SQL Server function mappings page](xref:core/providers/sql-server/functions).

## Cosmos DB

<a name="cosmos-complex-types"></a>

### Complex types

EF Core [complex types](xref:core/what-is-new/ef-core-10.0/whatsnew#complex-types) are now fully supported in the Azure Cosmos DB provider; they are embedded as nested JSON objects (or arrays, for collections) within the owning document, with support for queries, inserts, and updates.

For example, the following configures `ShippingAddress` as a complex type:

#### [Fluent API](#tab/fluent-api)

```csharp
modelBuilder.Entity<Order>()
    .ComplexProperty(o => o.ShippingAddress);
```

#### [Data Annotations](#tab/data-annotations)

```csharp
[ComplexType]
public class ShippingAddress
{
    public required string Street { get; set; }
    public required string City { get; set; }
}
```

***

Complex types are generally a better fit than owned types when mapping to JSON documents: unlike owned types, complex types have value semantics and no identity, which means they work better in scenarios such as comparing two embedded objects within the same document.

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

<a name="cosmos-transactional-batches"></a>

### Transactional batches

Azure Cosmos DB supports [transactional batches](/azure/cosmos-db/transactional-batch), which allow multiple operations to be executed atomically and in a single roundtrip against a single partition. Starting with EF Core 11, the provider leverages transactional batches by default, providing best-effort atomicity and improved performance when saving changes.

The batching behavior can be controlled via the <xref:Microsoft.EntityFrameworkCore.AutoTransactionBehavior> property:

* **Auto** (default): Operations are grouped into transactional batches by container and partition. Batches are executed sequentially; if a batch fails, subsequent batches are not executed.
* **Never**: All operations are performed individually and sequentially (the pre-11 behavior).
* **Always**: Requires all operations to fit in a single transactional batch; throws if they cannot.

For more information, [see the documentation](xref:core/providers/cosmos/saving#transactionality-and-transactional-batches).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

<a name="cosmos-bulk-execution"></a>

### Bulk execution

Azure Cosmos DB supports _bulk execution_, which allows multiple document operations to be executed in parallel and across DbContext instances, significantly improving throughput when saving many entities at once. EF Core now supports enabling bulk execution:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.BulkExecutionEnabled());
```

For more information, [see the documentation](xref:core/providers/cosmos/saving#bulk-execution).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

<a name="cosmos-session-tokens"></a>

### Session token management

Azure Cosmos DB uses session tokens to track read-your-writes consistency within a session. When running in an environment with multiple instances (e.g., with round-robin load balancing), you may need to manually manage session tokens to ensure consistency across requests.

EF Core now provides APIs to retrieve and set session tokens on a `DbContext`. To enable manual session token management, configure the `SessionTokenManagementMode()`:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.SessionTokenManagementMode(SessionTokenManagementMode.SemiAutomatic));
```

You can then retrieve and use session tokens:

```csharp
// After performing operations, retrieve the session token
var sessionToken = context.Database.GetSessionToken();

// Later, in a different context instance, apply the session token before reading
context.Database.UseSessionToken(sessionToken);
var result = await context.Documents.FindAsync(id);
```

For more information, [see the documentation](xref:core/providers/cosmos/saving#session-token-management).

This feature was contributed by [@JoasE](https://github.com/JoasE) - many thanks!

## Migrations

<a name="migrations-exclude-fk"></a>

### Excluding foreign key constraints from migrations

It is now possible to configure a foreign key relationship in the EF model while preventing the corresponding database constraint from being created by migrations. This is useful for legacy databases without existing constraints, or in data synchronization scenarios where referential integrity constraints might conflict with the synchronization order:

```csharp
modelBuilder.Entity<Blog>()
    .HasMany(e => e.Posts)
    .WithOne(e => e.Blog)
    .HasForeignKey(e => e.BlogId)
    .ExcludeForeignKeyFromMigrations();
```

The relationship is fully supported in EF for queries, change tracking, etc. Only the foreign key constraint in the database is suppressed; a database index is still created on the foreign key column.

For more information, see [Excluding foreign key constraints from migrations](xref:core/modeling/relationships/foreign-and-principal-keys#excluding-foreign-key-constraints-from-migrations).

<a name="migrations-snapshot-latest-id"></a>

### Latest migration ID recorded in model snapshot

When working in team environments, it's common for multiple developers to create migrations on separate branches. When these branches are merged, the migration trees can diverge, leading to issues that are sometimes difficult to detect.

Starting with EF Core 11, the model snapshot now records the ID of the latest migration. When two developers create migrations on divergent branches, both branches will modify this value in the model snapshot, causing a source control merge conflict. This conflict alerts the team that they need to resolve the divergence - typically by discarding one of the migration trees and creating a new, unified migration.

For more information on managing migrations in team environments, see [Migrations in Team Environments](xref:core/managing-schemas/migrations/teams).

<a name="migrations-add-and-apply"></a>

### Create and apply migrations in one step

The `dotnet ef database update` command now supports creating and applying a migration in a single step using the new `--add` option. This uses Roslyn to compile the migration at runtime, enabling scenarios like .NET Aspire and containerized applications where recompilation isn't possible:

```dotnetcli
dotnet ef database update InitialCreate --add
```

This command scaffolds a new migration named `InitialCreate`, compiles it using Roslyn, and immediately applies it to the database. The migration files are still saved to disk for source control and future recompilation. The same options available for `dotnet ef migrations add` can be used:

```dotnetcli
dotnet ef database update AddProducts --add --output-dir Migrations/Products --namespace MyApp.Migrations
```

In PowerShell, use the `-Add` parameter:

```powershell
Update-Database -Migration InitialCreate -Add
```

<a name="migrations-remove-connection-offline"></a>

### Connection and offline options for migrations remove

The `dotnet ef migrations remove` and `database drop` commands now accept `--connection` parameters, allowing you to specify the database connection string directly without needing to configure a default connection in your `DbContext`. Additionally, `migrations remove` supports the new `--offline` option to remove a migration without connecting to the database:

```console
# Remove migration with specific connection
dotnet ef migrations remove --connection "Server=prod;Database=MyDb;..."

# Remove migration without connecting to database (offline mode)
dotnet ef migrations remove --offline

# Revert and remove applied migration
dotnet ef migrations remove --force

# Drop specific database by connection string
dotnet ef database drop --connection "Server=test;Database=MyDb;..." --force
```

The `--offline` option skips the database connection check entirely, which is useful when the database is inaccessible or when you're certain the migration hasn't been applied. Note that `--offline` and `--force` cannot be used together, since `--force` requires a database connection to check if the migration has been applied before reverting it.

In PowerShell, use the `-Connection` and `-Offline` parameters:

```powershell
Remove-Migration -Connection "Server=prod;Database=MyDb;..."
Remove-Migration -Offline
Drop-Database -Connection "Server=test;Database=MyDb;..." -Force
```

## Other improvements

* The EF command-line tool now writes all logging and status messages to standard error, reserving standard output only for the command's actual expected output. For example, when generating a migration SQL script with `dotnet ef migrations script`, only the SQL is written to standard output.
