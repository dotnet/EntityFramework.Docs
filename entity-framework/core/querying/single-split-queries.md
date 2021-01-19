---
title: Single vs. Split Queries - EF Core
description: Translating query into single and split queries in SQL with Entity Framework Core
author: smitpatel
ms.date: 10/03/2019
uid: core/querying/single-split-queries
---
# Split queries

## Single queries

In relational databases, all related entities are loaded by introducing JOINs in single query.

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId], [p].[PostId]
```

If a typical blog has multiple related posts, rows for these posts will duplicate the blog's information. This duplication leads to the so-called "cartesian explosion" problem. As more one-to-many relationships are loaded, the amount of duplicated data may grow and adversely affect the performance of your application.

## Split queries

> [!NOTE]
> This feature was introduced in EF Core 5.0. It only works when using `Include`. [This issue](https://github.com/dotnet/efcore/issues/21234) is tracking support for split query when loading related data in projection without `Include`.

EF allows you to specify that a given LINQ query should be *split* into multiple SQL queries. Instead of JOINs, split queries generate an additional SQL query for each included collection navigation:

[!code-csharp[Main](../../../samples/core/Querying/RelatedData/Program.cs?name=AsSplitQuery&highlight=5)]

It will produce the following SQL:

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
ORDER BY [b].[BlogId]

SELECT [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title], [b].[BlogId]
FROM [Blogs] AS [b]
INNER JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId]
```

> [!NOTE]
> One-to-one related entities are always loaded via JOINs in the same query, as it has no performance impact.

## Enabling split queries globally

You can also configure split queries as the default for your application's context:

[!code-csharp[Main](../../../samples/core/Querying/RelatedData/SplitQueriesBloggingContext.cs?name=QuerySplittingBehaviorSplitQuery&highlight=6)]

When split queries are configured as the default, it's still possible to configure specific queries to execute as single queries:

[!code-csharp[Main](../../../samples/core/Querying/RelatedData/Program.cs?name=AsSingleQuery&highlight=5)]

EF Core uses single query mode by default in the absence of any configuration. Since it may cause performance issues, EF Core generates a warning whenever following conditions are met:

- EF Core detects that the query loads multiple collections.
- User hasn't configured query splitting mode globally.
- User hasn't used `AsSingleQuery`/`AsSplitQuery` operator on the query.

To turn off the warning, configure query splitting mode globally or at the query level to an appropriate value.

## Characteristics of split queries

While split query avoids the performance issues associated with JOINs and cartesian explosion, it also has some drawbacks:

- While most databases guarantee data consistency for single queries, no such guarantees exist for multiple queries. If the database is updated concurrently when executing your queries, resulting data may not be consistent. You can mitigate it by wrapping the queries in a serializable or snapshot transaction, although doing so may create performance issues of its own. For more information, see your database's documentation.
- Each query currently implies an additional network roundtrip to your database. Multiple network roundtrips can degrade performance, especially where latency to the database is high (for example, cloud services).
- While some databases allow consuming the results of multiple queries at the same time (SQL Server with MARS, Sqlite), most allow only a single query to be active at any given point. So all results from earlier queries must be buffered in your application's memory before executing later queries, which leads to increased memory requirements.

Unfortunately, there isn't one strategy for loading related entities that fits all scenarios. Carefully consider the advantages and disadvantages of single and split queries to select the one that fits your needs.
