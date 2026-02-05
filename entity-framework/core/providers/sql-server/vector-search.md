---
title: Microsoft SQL Server Database Provider - Vector Search - EF Core
description: Using vectors and embeddings to perform similarity search with the Entity Framework Core Microsoft SQL Server database provider
author: roji
ms.date: 08/17/2025
uid: core/providers/sql-server/vector-search
---
# Vector search in the SQL Server EF Core Provider

> [!NOTE]
> Vector support was introduced in EF Core 10.0, and is only supported with SQL Server 2025 and above.

The SQL Server vector data type allows storing *embeddings*, which are representation of meaning that can be efficiently searched over for similarity, powering AI workloads such as semantic search and retrieval-augmented generation (RAG).

## Setting up vector properties

To use the `vector` data type, simply add a .NET property of type `SqlVector<float>` to your entity type, specifying the dimensions as follows:

### [Data Annotations](#tab/data-annotations)

```c#
public class Blog
{
    // ...

    [Column(TypeName = "vector(1536)")]
    public SqlVector<float> Embedding { get; set; }
}
```

### [Fluent API](#tab/fluent-api)

```c#
public class Blog
{
    // ...

    public SqlVector<float> Embedding { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Property(b => b.Embedding)
        .HasColumnType("vector(1536)");
}
```

***

Once your property is added and the corresponding column created in the database, you can start inserting embeddings. Embedding generation is done outside of the database, usually via a service, and the details for doing this are out of scope for this documentation. However, [the .NET Microsoft.Extensions.AI libraries](/dotnet/ai/microsoft-extensions-ai) contains [`IEmbeddingGenerator`](/dotnet/ai/microsoft-extensions-ai#create-embeddings), which is an abstraction over embedding generators that has implementations for the major providers.

Once you've chosen your embedding generator and set it up, use it to generate embeddings and insert them as follows:

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

Once you have embeddings saved to your database, you're ready to perform vector similarity search over them.

## Exact search with VECTOR_DISTANCE()

The [`EF.Functions.VectorDistance()`](/sql/t-sql/functions/vector-distance-transact-sql) function computes the *exact* distance between two vectors. Use it to perform similarity search for a given user query:

```c#
var sqlVector = new SqlVector<float>(await embeddingGenerator.GenerateVectorAsync("Some user query to be vectorized"));
var topSimilarBlogs = context.Blogs
    .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, sqlVector))
    .Take(3)
    .ToListAsync();
```

This function computes the distance between the query vector and every row in the table, then returns the closest matches. While this provides perfectly accurate results, it can be slow for large datasets because SQL Server must scan all rows and compute distances for each one.

> [!NOTE]
> The built-in support in EF 10 replaces the previous [EFCore.SqlServer.VectorSearch](https://github.com/efcore/EFCore.SqlServer.VectorSearch) extension, which allowed performing vector search before the `vector` data type was introduced. As part of upgrading to EF 10, remove the extension from your projects.

## Approximate search with VECTOR_SEARCH()

> [!WARNING]
> `VECTOR_SEARCH()` and vector indexes are currently experimental features in SQL Server and are subject to change. The APIs in EF Core for these features are also subject to change.

For large datasets, computing exact distances for every row can be prohibitively slow. SQL Server 2025 introduces support for *approximate* search through a [vector index](/sql/t-sql/statements/create-vector-index-transact-sql), which provides much better performance at the expense of returning items that are approximately similar - rather than exactly similar - to the query.

### Vector indexes

To use `VECTOR_SEARCH()`, you must create a vector index on your vector column. Use the `HasVectorIndex()` method in your model configuration:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasVectorIndex(b => b.Embedding, "cosine");
}
```

This will generate the following SQL migration:

```sql
CREATE VECTOR INDEX [IX_Blogs_Embedding]
    ON [Blogs] ([Embedding])
    WITH (METRIC = COSINE)
```

The following distance metrics are supported for vector indexes:

Metric      | Description
----------- | -----------
`cosine`    | Cosine similarity (angular distance)
`euclidean` | Euclidean distance (L2 norm)
`dot`       | Dot product (negative inner product)

Choose the metric that best matches your embedding model and use case. Cosine similarity is commonly used for text embeddings, while euclidean distance is often used for image embeddings.

### Searching with VECTOR_SEARCH()

Once you have a vector index, use the `VectorSearch()` extension method on your `DbSet`:

```csharp
var blogs = await context.Blogs
    .VectorSearch(b => b.Embedding, "cosine", embedding, topN: 5)
    .ToListAsync();
```

This translates to the following SQL:

```sql
SELECT [v].[Id], [v].[Embedding], [v].[Name]
FROM VECTOR_SEARCH([Blogs], 'Embedding', @__embedding, 'metric = cosine', @__topN)
```

The `topN` parameter specifies the maximum number of results to return.

`VectorSearch()` returns `VectorSearchResult<TEntity>`, which allows you to access both the entity and the computed distance:

```csharp
var searchResults = await context.Blogs
    .VectorSearch(b => b.Embedding, "cosine", embedding, topN: 5)
    .Where(r => r.Distance > 0.005)
    .Select(r => new { Blog = r.Value, Distance = r.Distance })
    .ToListAsync();
```

This allows you to filter on the similarity score, present it users, etc.

## Hybrid search

*Hybrid search* combines vector similarity search with traditional [full-text search](xref:core/providers/sql-server/full-text-search) to deliver more relevant results. Vector search excels at finding semantically similar content, while full-text search is better at exact keyword matching. By combining both approaches and using Reciprocal Rank Fusion (RRF) to merge the results, you can build more intelligent search experiences.

The following example shows how to implement hybrid search using EF Core, combining `FreeTextTable()` and `VectorSearch()` in a single query:

```csharp
const int k = 10;

var results = await context.Articles
    // Perform full-text search
    .FreeTextTable<Article, int>(textualQuery, topN: 10)
    // Perform vector (semantic) search, joining the results of both searches together
    .LeftJoin(
        context.Articles.VectorSearch(b => b.Embedding, queryEmbedding, "cosine", topN: 10),
        fts => fts.Key,
        vs => vs.Value.Id,
        (fts, vs) => new
        {
            Article = vs.Value,
            FullTextRank = fts.Rank,
            VectorDistance = (double?)vs.Distance
        })
    // Apply Reciprocal Rank Fusion (RRF) to combine the results
    .Select(x => new
    {
        x.Article,
        RrfScore = (1.0 / (k + x.FullTextRank)) + (1.0 / (k + x.VectorDistance) ?? 0.0)
    })
    .OrderByDescending(x => x.RrfScore)
    .Take(k)
    .Select(x => x.Article)
    .ToListAsync();
```

This query:

1. Performs a full-text search on `Article`
2. Performs a vector search on `Article` and combines the results to the full-text search results via a LEFT JOIN
3. Calculates the RRF score by combining both the full text and the semantic ranking
4. Orders by RRF score, takes the desired number of results and projects out the original `Article` entities.

> [!NOTE]
> Rather than using a LEFT JOIN, a FULL OUTER JOIN would be more suitable for this scenario; this would allow highly-ranking results from either search side to be included in the final result, even if that result does not appear at all on the other side. With the above LEFT JOIN approach, if a result has a very high vector similarity score, it never gets included in the final result if that result doesn't also have a high full-text score. However, EF doesn't currently support FULL OUTER JOIN; upvote [#37633](https://github.com/dotnet/efcore/issues/37633) if this is something you'd like to see supported.

The query produces the following SQL:

```sql
SELECT TOP(@p3) [a0].[Id], [a0].[Content], [a0].[Embedding], [a0].[Title]
FROM FREETEXTTABLE([Articles], *, @p, @p1) AS [f]
LEFT JOIN VECTOR_SEARCH(
    TABLE = [Articles] AS [a0],
    COLUMN = [Embedding],
    SIMILAR_TO = @p2,
    METRIC = 'cosine',
    TOP_N = @p3
) AS [v] ON [f].[KEY] = [a0].[Id]
ORDER BY 1.0E0 / CAST(10 + [f].[RANK] AS float) + ISNULL(1.0E0 / (10.0E0 + [v].[Distance]), 0.0E0) DESC
```
