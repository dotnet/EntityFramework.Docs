---
title: Microsoft SQL Server Database Provider - Full-Text Search - EF Core
description: Using full-text search with the Entity Framework Core Microsoft SQL Server database provider
author: roji
ms.date: 02/05/2026
uid: core/providers/sql-server/full-text-search
---
# Full-Text Search in the SQL Server EF Core Provider

SQL Server provides [full-text search](/sql/relational-databases/search/full-text-search) capabilities that enable sophisticated text search beyond simple `LIKE` patterns. Full-text search supports linguistic matching, inflectional forms, proximity search, and weighted ranking.

EF Core's SQL Server provider supports both full-text search *predicates* (for filtering) and *table-valued functions* (for filtering with ranking).

## Setting up full-text search

Before using full-text search, you must:

1. **Create a full-text catalog** on your database
2. **Create a full-text index** on the columns you want to search

This setup is done at the SQL Server level and is outside the scope of EF Core. For more information, see the [SQL Server full-text search documentation](/sql/relational-databases/search/get-started-with-full-text-search).

## Full-text predicates

EF Core supports the `FREETEXT()` and `CONTAINS()` predicates, which are used in `Where()` clauses to filter results.

### FREETEXT()

`FREETEXT()` performs a less strict matching, searching for words based on their meaning, including inflectional forms (such as verb tenses and noun plurals):

```csharp
var articles = await context.Articles
    .Where(a => EF.Functions.FreeText(a.Contents, "veggies"))
    .ToListAsync();
```

This translates to:

```sql
SELECT [a].[Id], [a].[Title], [a].[Contents]
FROM [Articles] AS [a]
WHERE FREETEXT([a].[Contents], N'veggies')
```

You can optionally specify a language term:

```csharp
var articles = await context.Articles
    .Where(a => EF.Functions.FreeText(a.Contents, "veggies", "English"))
    .ToListAsync();
```

### CONTAINS()

`CONTAINS()` performs more precise matching and supports more sophisticated search criteria, including prefix terms, proximity search, and weighted terms:

```csharp
// Simple search
var articles = await context.Articles
    .Where(a => EF.Functions.Contains(a.Contents, "veggies"))
    .ToListAsync();

// Prefix search (words starting with "vegg")
var articles = await context.Articles
    .Where(a => EF.Functions.Contains(a.Contents, "\"vegg*\""))
    .ToListAsync();

// Phrase search
var articles = await context.Articles
    .Where(a => EF.Functions.Contains(a.Contents, "\"fresh vegetables\""))
    .ToListAsync();
```

This translates to:

```sql
SELECT [a].[Id], [a].[Title], [a].[Contents]
FROM [Articles] AS [a]
WHERE CONTAINS([a].[Contents], N'veggies')
```

For more information on `CONTAINS()` query syntax, see the [SQL Server CONTAINS documentation](/sql/t-sql/queries/contains-transact-sql).

## Full-text table-valued functions

> [!NOTE]
> Full-text table-valued functions are being introduced in EF Core 11.

While the predicates above are useful for filtering, they don't provide ranking information. SQL Server's table-valued functions [`FREETEXTTABLE()`](/sql/relational-databases/system-functions/freetexttable-transact-sql) and [`CONTAINSTABLE()`](/sql/relational-databases/system-functions/containstable-transact-sql) return both matching rows and a ranking score that indicates how well each row matches the search query.

### FreeTextTable()

`FreeTextTable()` is the table-valued function version of `FreeText()`. It returns `FullTextSearchResult<TEntity>`, which includes both the entity and the ranking value:

```csharp
var results = await context.Articles
    .Join(
        context.Articles.FreeTextTable<Article, int>("veggies", topN: 10),
        a => a.Id,
        ftt => ftt.Key,
        (a, ftt) => new { Article = a, ftt.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

Note that you must provide the generic type parameters; `Article` corresponds to the entity type being searched, where `int` is the full-text search key specified when creating the index, and which is returned by `FREETEXTTABLE()`.

The above automatically searches across all columns registered for full-text searching and returns the top 10 matches. You can also provide a specific column to search:

```csharp
var results = await context.Articles
    .Join(
        context.Articles.FreeTextTable<Article, int>(a => a.Contents, "veggies"),
        a => a.Id,
        ftt => ftt.Key,
        (a, ftt) => new { Article = a, ftt.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

... or multiple columns:

```csharp
var results = await context.Articles
    .FreeTextTable(a => new { a.Title, a.Contents }, "veggies")
    .Select(r => new { Article = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

### ContainsTable()

`ContainsTable()` is the table-valued function version of `Contains()`, supporting the same sophisticated search syntax while also providing ranking information:

```csharp
var results = await context.Articles
    .Join(
        context.Articles.ContainsTable<Article, int>( "veggies OR fruits"),
        a => a.Id,
        ftt => ftt.Key,
        (a, ftt) => new { Article = a, ftt.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

### Limiting results

Both table-valued functions support a `topN` parameter to limit the number of results:

```csharp
var results = await context.Articles
    .FreeTextTable(a => a.Contents, "veggies", topN: 10)
    .Select(r => new { Article = r.Value, Rank = r.Rank })
    .OrderByDescending(r => r.Rank)
    .ToListAsync();
```

### Specifying a language

Both table-valued functions support specifying a language term for linguistic matching:

```csharp
var results = await context.Articles
    .FreeTextTable(a => a.Contents, "veggies", languageTerm: "English")
    .Select(r => new { Article = r.Value, Rank = r.Rank })
    .ToListAsync();
```

## When to use predicates vs table-valued functions

Feature                           | Predicates (`FreeText()`, `Contains()`) | Table-valued functions (`FreeTextTable()`, `ContainsTable()`)
--------------------------------- | --------------------------------------- | -------------------------------------------------------------
Provides ranking                  | ❌ No                                   | ✅ Yes
Performance for large result sets | Better for filtering                    | Better for ranking and sorting
Combine with other entities       | Via joins                               | Built-in entity result
Use in `Where()` clause           | ✅ Yes                                  | ❌ No (use as a source)

Use predicates when you simply need to filter results based on full-text search criteria. Use table-valued functions when you need ranking information to order results by relevance or display relevance scores to users.
