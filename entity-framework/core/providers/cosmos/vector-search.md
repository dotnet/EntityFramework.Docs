---
title: Vector Search - Azure Cosmos DB Provider - EF Core
description: Vector search with the Azure Cosmos DB EF Core Provider
author: roji
ms.date: 09/20/2024
uid: core/providers/cosmos/vector-search
---
# Vector search

Azure Cosmos DB now offers support for vector similarity search. Vector search is a fundamental part of some application types, including AI, semantic search and others. Azure Cosmos DB allows you to store vectors directly in your documents alongside the rest of your data, meaning you can perform all of your queries against a single database. This can considerably simplify your architecture and remove the need for an additional, dedicated vector database solution in your stack. To learn more about Azure Cosmos DB vector search, [see the documentation](/azure/cosmos-db/nosql/vector-search).

Vector property can be configured inside `OnModelCreating`:

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
        modelBuilder.Entity<Blog>(b =>
        {
            b.Property(b => b.Vector).IsVectorProperty(DistanceFunction.Cosine, dimensions: 1536);
            b.HasIndex(x => x.Vector).IsVectorIndex(VectorIndexType.Flat);
        });
    }
}
```

At this point your model is configured. Insertion of vector data is done just like any other data type with EF:

```c#
float[] vector = /* generate vector data from text, image, etc. */
context.Add(new Blog { Vector = vector });
await context.SaveChangesAsync();
```

Finally, use the `EF.Functions.VectorDistance()` function in LINQ queries to perform vector similarity search:

```c#
float[] anotherVector = /* generate vector data from text, image, etc. */
var blogs = await context.Blogs
    .OrderBy(s => EF.Functions.VectorDistance(s.Vector, anotherVector))
    .Take(5)
    .ToListAsync();
```

This will returns the top five Blogs, based on the similarity of their `Vector` property and the externally-provided `anotherVector` data.

## Hybrid search

Vector similarity search can be used with full-text search in the same query (i.e. hybrid search), by combining results of `VectorDistance` and `FullTextScore` functions using the [`RRF`](/azure/cosmos-db/nosql/query/rrf) (Reciprocal Rank Fusion) function.

See [documentation](xref:core/providers/cosmos/full-text-search?#hybrid-search) to learn how to enable full-text search support in EF model and how to use hybrid search in queries.
