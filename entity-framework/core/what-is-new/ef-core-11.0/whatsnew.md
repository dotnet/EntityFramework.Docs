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

## LINQ and SQL translation

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

## Cosmos DB

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
    .VectorSearch(b => b.Embedding, "cosine", embedding, topN: 5)
    .ToListAsync();
```

This translates to the SQL Server [`VECTOR_SEARCH()`](/sql/t-sql/functions/vector-search-transact-sql) table-valued function, which performs an approximate search over the vector index. The `topN` parameter specifies the number of results to return.

`VectorSearch()` returns `VectorSearchResult<TEntity>`, allowing you to access the distance alongside the entity.

For more information, see the [full documentation on vector search](xref:core/providers/sql-server/vector-search).

<a name="sqlserver-full-text-tvf"></a>

### Full-text search table-valued functions

EF Core has long provided support for SQL Server's full-text search predicates `FREETEXT()` and `CONTAINS()`, via `EF.Functions.FreeText()` and `EF.Functions.Contains()`. These predicates can be used in LINQ `Where()` clauses to filter results based on search criteria.

However, SQL Server also has table-valued function versions of these functions, [`FREETEXTTABLE()`](/sql/relational-databases/system-functions/freetexttable-transact-sql) and [`CONTAINSTABLE()`](/sql/relational-databases/system-functions/containstable-transact-sql), which also return a ranking score along with the results, providing additional flexibility over the predicate versions. EF 11 now supports these table-valued functions:

```csharp
// Using FreeTextTable with a search query
var results = await context.Blogs
    .FreeTextTable(b => b.FullName, "John")
    .Select(r => new { Blog = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();

// Using ContainsTable with a search query
var results = await context.Blogs
    .ContainsTable(b => b.FullName, "John")
    .Select(r => new { Blog = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

Both methods return `FullTextSearchResult<TEntity>`, giving you access to both the entity and the ranking value from SQL Server's full-text engine. This allows for more sophisticated result ordering and filtering based on relevance scores.

For more information, see the [full documentation on full-text search](xref:core/providers/sql-server/full-text-search).

<a name="sql-server-json-contains"></a>

### Translate Contains over primitive collections using JSON_CONTAINS

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
