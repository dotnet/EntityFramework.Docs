---
title: Query Tags - EF Core
description: Using query tags to help identify specific queries in log messages emitted by Entity Framework Core
author: smitpatel
ms.date: 11/14/2018
uid: core/querying/tags
---

# Query tags

Query tags help correlate LINQ queries in code with generated SQL queries captured in logs.
You annotate a LINQ query using the new `TagWith()` method:

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying/Tags) on GitHub.

[!code-csharp[Main](../../../samples/core/Querying/Tags/Program.cs#BasicQueryTag)]

This LINQ query is translated to the following SQL statement:

```sql
-- This is my spatial query!

SELECT TOP(@__p_1) [p].[Id], [p].[Location]
FROM [People] AS [p]
ORDER BY [p].[Location].STDistance(@__myLocation_0) DESC
```

It's possible to call `TagWith()` many times on the same query.
Query tags are cumulative.
For example, given the following methods:

[!code-csharp[Main](../../../samples/core/Querying/Tags/Program.cs#QueryableMethods)]

The following query:

[!code-csharp[Main](../../../samples/core/Querying/Tags/Program.cs#ChainedQueryTags)]

Translates to:

```sql
-- GetNearestPeople

-- Limit

SELECT TOP(@__p_1) [p].[Id], [p].[Location]
FROM [People] AS [p]
ORDER BY [p].[Location].STDistance(@__myLocation_0) DESC
```

It's also possible to use multi-line strings as query tags.
For example:

[!code-csharp[Main](../../../samples/core/Querying/Tags/Program.cs#MultilineQueryTag)]

Produces the following SQL:

```sql
-- GetNearestPeople

-- Limit

-- This is a multi-line
-- string

SELECT TOP(@__p_1) [p].[Id], [p].[Location]
FROM [People] AS [p]
ORDER BY [p].[Location].STDistance(@__myLocation_0) DESC
```

## Known limitations

**Query tags aren't parameterizable:**
EF Core always treats query tags in the LINQ query as string literals that are included in the generated SQL.
Compiled queries that take query tags as parameters aren't allowed.
