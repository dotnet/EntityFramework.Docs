---
title: Microsoft SQL Server Database Provider - Vector Search - EF Core
description: Using vectors and embeddings to perform similarity search with the Entity Framework Core Microsoft SQL Server database provider
author: roji
ms.date: 08/17/2025
uid: core/providers/sql-server/vector-search
---
# Vector search in the SQL Server EF Core Provider

## Vector search

> [!NOTE]
> Vector support was introduced in EF Core 10.0, and is only supported with SQL Server 2025 and above.

The SQL Server vector data type allows storing *embeddings*, which are representation of meaning that can be efficiently searched over for similarity, powering AI workloads such as semantic search and retrieval-augmented generation (RAG).

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

Once you've chosen your embedding generator and set it up, use it to generate embeddings and insert them as follows

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

Finally, use the [`EF.Functions.VectorDistance()`](/sql/t-sql/functions/vector-distance-transact-sql) function to perform similarity search for a given user query:

```c#
var sqlVector = new SqlVector<float>(await embeddingGenerator.GenerateVectorAsync("Some user query to be vectorized"));
var topSimilarBlogs = context.Blogs
    .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, sqlVector))
    .Take(3)
    .ToListAsync();
```

> [!NOTE]
> The built-in support in EF 10 replaces the previous [EFCore.SqlServer.VectorSearch](https://github.com/efcore/EFCore.SqlServer.VectorSearch) extension, which allowed performing vector search before the `vector` data type was introduced. As part of upgrading to EF 10, remove the extension from your projects.
>
> The [`VECTOR_SEARCH()`](/sql/t-sql/functions/vector-search-transact-sql) function (in preview) for approximate search with DiskANN is currently unsupported.
