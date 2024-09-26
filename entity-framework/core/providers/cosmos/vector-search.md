---
title: Vector Search - Azure Cosmos DB Provider - EF Core
description: Vector search with the Azure Cosmos DB EF Core Provider
author: roji
ms.date: 09/20/2024
uid: core/providers/cosmos/vector-search
---
# Vector search

> [!WARNING]
> Azure Cosmos DB vector search is currently in preview. As a result, using EF's vector search APIs will generate an "experimental API" warning (`EF9103`) which must be suppressed. The APIs and capabilities may change in breaking ways in the future.

Azure Cosmos DB now offers preview support for vector similarity search. Vector search is a fundamental part of some application types, including AI, semantic search and others. Azure Cosmos DB allows you to store vectors directly in your documents alongside the rest of your data, meaning you can perform all of your queries against a single database. This can considerably simplify your architecture and remove the need for an additional, dedicated vector database solution in your stack. To learn more about Azure Cosmos DB vector search, [see the documentation](/azure/cosmos-db/nosql/vector-search).

To use vector search, you must first [enroll in the preview feature](/azure/cosmos-db/nosql/vector-search#enroll-in-the-vector-search-preview-feature). Then, [define vector policies on your container](/azure/cosmos-db/nosql/vector-search#container-vector-policies) to identify which JSON properties in your documents contain vectors and vector-related information for those properties (dimensions, data type, distance function).

Once your container is properly set up, add a vector property to your model in the path you defined in the container policy, and configure it with EF as a vector:

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
        modelBuilder.Entity<Blog>()
            .Property(b => b.Embeddings)
            .IsVector(DistanceFunction.Cosine, dimensions: 1536);
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
