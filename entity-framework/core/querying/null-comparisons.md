---
title: Comparisons with null values in queries
description: Information on how Entity Framework Core handles null comparisons in queries 
author: maumar
ms.date: 11/11/2020
uid: core/querying/null-comparisons
---
# Query null semantics

## Introduction

SQL databases operate on 3-valued logic (`true`, `false`, `null`) when performing comparisons, as opposed to the boolean logic of C#. When translating LINQ queries to SQL, EF Core tries to compensate for the difference by introducing additional null checks for some elements of the query.
To illustrate this, let's define the following entity:

[!code-csharp[Main](../../../samples/core/Querying/NullSemantics/NullSemanticsEntity.cs#Entity)]

and issue several queries:

[!code-csharp[Main](../../../samples/core/Querying/NullSemantics/Program.cs#BasicExamples)]

The first two queries produce simple comparisons. In the first query, both columns are non-nullable so null checks are not needed. In the second query, `NullableInt` could contain `null`, but `Id` is non-nullable; comparing `null` to non-null yields `null` as a result, which would be filtered out by `WHERE` operation. So no additional terms are needed either.

```sql
SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE [e].[Id] = [e].[Int]

SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE [e].[Id] = [e].[NullableInt]
```

The third query introduces a null check. When `NullableInt` is `null` the comparison `Id <> NullableInt` yields `null`, which would be filtered out by `WHERE` operation. However, from the boolean logic perspective this case should be returned as part of the result. Hence EF Core adds the necessary check to ensure that.

```sql
SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE ([e].[Id] <> [e].[NullableInt]) OR [e].[NullableInt] IS NULL
```

Queries four and five show the pattern when both columns are nullable. It's worth noting that the `<>` operation produces more complicated (and potentially slower) query than the `==` operation.

```sql
SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE ([e].[String1] = [e].[String2]) OR ([e].[String1] IS NULL AND [e].[String2] IS NULL)

SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE (([e].[String1] <> [e].[String2]) OR ([e].[String1] IS NULL OR [e].[String2] IS NULL)) AND ([e].[String1] IS NOT NULL OR [e].[String2] IS NOT NULL)
```

## Treatment of nullable values in functions

Many functions in SQL can only return a `null` result if some of their arguments are `null`. EF Core takes advantage of this to produce more efficient queries.
The query below illustrates the optimization:

[!code-csharp[Main](../../../samples/core/Querying/NullSemantics/Program.cs#Functions)]

The generated SQL is as follows (we don't need to evaluate the `SUBSTRING` function since it will be only null when either of the arguments to it is null.):

```sql
SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE [e].[String1] IS NULL OR [e].[String2] IS NULL
```

The optimization can also be used for user-defined functions. See [user defined function mapping](xref:core/querying/user-defined-function-mapping#configuring-nullability-of-user-defined-function-based-on-its-arguments) page for more details.

## Writing performant queries

- Comparing non-nullable columns is simpler and faster than comparing nullable columns. Consider marking columns as non-nullable whenever possible.

- Checking for equality (`==`) is simpler and faster than checking for non-equality (`!=`), because query doesn't need to distinguish between `null` and `false` result. Use equality comparison whenever possible. However, simply negating `==` comparison is effectively the same as `!=`, so it doesn't result in performance improvement.

- In some cases, it is possible to simplify a complex comparison by filtering out `null` values from a column explicitly - for example when no `null` values are present or these values are not relevant in the result. Consider the following example:

[!code-csharp[Main](../../../samples/core/Querying/NullSemantics/Program.cs#ManualOptimization)]

These queries produce the following SQL:

```sql
SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE ((([e].[String1] <> [e].[String2]) OR ([e].[String1] IS NULL OR [e].[String2] IS NULL)) AND ([e].[String1] IS NOT NULL OR [e].[String2] IS NOT NULL)) OR ((CAST(LEN([e].[String1]) AS int) = CAST(LEN([e].[String2]) AS int)) OR ([e].[String1] IS NULL AND [e].[String2] IS NULL))

SELECT [e].[Id], [e].[Int], [e].[NullableInt], [e].[String1], [e].[String2]
FROM [Entities] AS [e]
WHERE ([e].[String1] IS NOT NULL AND [e].[String2] IS NOT NULL) AND (([e].[String1] <> [e].[String2]) OR (CAST(LEN([e].[String1]) AS int) = CAST(LEN([e].[String2]) AS int)))
```

In the second query, `null` results are filtered out from `String1` column explicitly. EF Core can safely treat the `String1` column as non-nullable during comparison, resulting in a simpler query.

## Using relational null semantics

It's possible to disable the null comparison compensation and use relational null semantics directly. This can be done by calling `UseRelationalNulls(true)` method on the options builder inside `OnConfiguring` method:

[!code-csharp[Main](../../../samples/core/Querying/NullSemantics/NullSemanticsContext.cs#UseRelationalNulls)]

> [!WARNING]
> When using relational null semantics, your LINQ queries no longer have the same meaning as they do in C#, and may yield different results than expected. Exercise caution when using this mode.
