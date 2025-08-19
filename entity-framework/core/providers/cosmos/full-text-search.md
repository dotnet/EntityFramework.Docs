---
title: Full-Text Search - Azure Cosmos DB Provider - EF Core
description: Full-text search with the Azure Cosmos DB EF Core Provider
author: maumar
ms.date: 04/19/2025
uid: core/providers/cosmos/full-text-search
---
# Full-text search

> [!NOTE]
> Full-text search support for Cosmos was introduced in EF 10.

Azure Cosmos DB now offers support for [full-text search](/azure/cosmos-db/gen-ai/full-text-search). It enables efficient and effective text searches using advanced techniques like stemming, as well as evaluating the relevance of documents to a given search query. It can be used in combination with vector search (i.e. hybrid search) to improve the accuracy of responses in some AI scenarios.
EF Core allows for modeling the database with full-text search enabled properties and using full-text search functions inside queries targeting Azure Cosmos DB.

## Model configuration

A property can be configured inside `OnModelCreating` to use full-text search by enabling it for the property and defining a full-text index:

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

> [!NOTE]
> Configuring the index is not mandatory, but it is recommended as it greatly improves performance of full-text search queries.

Full-text search operations are language specific, using American English (`en-US`) by default. You can customize the language for individual properties as part of `EnableFullTextSearch` call:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>(b =>
    {
        b.Property(x => x.Contents).EnableFullTextSearch();
        b.HasIndex(x => x.Contents).IsFullTextIndex();
        b.Property(x => x.ContentsGerman).EnableFullTextSearch("de-DE");
        b.HasIndex(x => x.ContentsGerman).IsFullTextIndex();
    });
}
```

You can also set a default language for the container - unless overridden in the `EnableFullTextSearch` method, all full-text properties inside the container will use that language.

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>(b =>
    {
        b.HasDefaultFullTextLanguage("de-DE");
        b.Property(x => x.ContentsEnglish).EnableFullTextSearch("en-US");
        b.HasIndex(x => x.ContentsEnglish).IsFullTextIndex();
        b.Property(x => x.ContentsGerman).EnableFullTextSearch();
        b.HasIndex(x => x.ContentsGerman).IsFullTextIndex();
        b.Property(x => x.TagsGerman).EnableFullTextSearch();
        b.HasIndex(x => x.TagsGerman).IsFullTextIndex();
    });
}
```

## Querying

As part of the full-text search feature, Azure Cosmos DB introduced several built-in functions which allow for efficient querying of content inside the full-text search enabled properties. These functions are: [`FullTextContains`](/azure/cosmos-db/nosql/query/fulltextcontains), [`FullTextContainsAll`](/azure/cosmos-db/nosql/query/fulltextcontainsall), [`FullTextContainsAny`](/azure/cosmos-db/nosql/query/fulltextcontainsany), which look for specific keyword or keywords and [`FullTextScore`](/azure/cosmos-db/nosql/query/fulltextscore), which returns [BM25 score](https://en.wikipedia.org/wiki/Okapi_BM25) based on provided keywords.

> [!NOTE]
> `FullTextScore` can only be used inside `OrderBy` to rank the documents based on the score.

EF Core exposes these functions as part of `EF.Functions` so they can be used in queries:

```c#
var cosmosBlogs = await context.Blogs.Where(x => EF.Functions.FullTextContainsAll(x.Contents, "database", "cosmos")).ToListAsync();

var keywords = new string[] { "AI", "agent", "breakthrough" };
var mostInteresting = await context.Blogs.OrderBy(x => EF.Functions.FullTextScore(x.Contents, keywords)).Take(5).ToListAsync();
```

## Hybrid search

Full-text search can be used with [vector search](xref:core/providers/cosmos/vector-search) in the same query; this is sometimes known as "hybrid search", and involves combining the scoring results from multiple searches via the [RRF (Reciprocal Rank Fusion) function](/azure/cosmos-db/nosql/query/rrf). Once you have your vector and full-text search configuration properly set up, you can perform hybrid search as follows:

```c#
float[] myVector = /* generate vector data from text, image, etc. */
var hybrid = await context.Blogs
    .OrderBy(x => EF.Functions.Rrf(
        EF.Functions.FullTextScore(x.Contents, "database"), 
        EF.Functions.VectorDistance(x.Vector, myVector)))
    .Take(10)
    .ToListAsync();
```

The RRF function also allows assigning different weights to each search function, allowing e.g. the vector search to have great weight in the overall results:

```c#
float[] myVector = /* generate vector data from text, image, etc. */
var hybrid = await context.Blogs
    .OrderBy(x => EF.Functions.Rrf(
        new[]
        {
            EF.Functions.FullTextScore(x.Contents, "database"), 
            EF.Functions.VectorDistance(x.Vector, myVector)
        },
        weights: new[] { 1, 2 }))
    .Take(10)
    .ToListAsync();
```

> [!TIP]
> You can combine more than two scoring functions inside `Rrf` call, as well as using only `FullTextScore`, or only `VectorDistance` invocations.
