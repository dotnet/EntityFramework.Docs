---
title: Complex Query Operators - EF Core
description: In-depth information on the more complex LINQ query operators when using Entity Framework Core
author: smitpatel
ms.date: 10/03/2019
uid: core/querying/complex-query-operators
---
# Complex Query Operators

Language Integrated Query (LINQ) contains many complex operators, which combine multiple data sources or does complex processing. Not all LINQ operators have suitable translations on the server side. Sometimes, a query in one form translates to the server but if written in a different form doesn't translate even if the result is the same. This page describes some of the complex operators and their supported variations. In future releases, we may recognize more patterns and add their corresponding translations. It's also important to keep in mind that translation support varies between providers. A particular query, which is translated in SqlServer, may not work for SQLite databases.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Querying/ComplexQuery) on GitHub.

## Join

The LINQ Join operator allows you to connect two data sources based on the key selector for each source, generating a tuple of values when the key matches. It naturally translates to `INNER JOIN` on relational databases. While the LINQ Join has outer and inner key selectors, the database requires a single join condition. So EF Core generates a join condition by comparing the outer key selector to the inner key selector for equality.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#Join)]

```sql
SELECT [p].[PersonId], [p].[Name], [p].[PhotoId], [p0].[PersonPhotoId], [p0].[Caption], [p0].[Photo]
FROM [PersonPhoto] AS [p0]
INNER JOIN [Person] AS [p] ON [p0].[PersonPhotoId] = [p].[PhotoId]
```

Further, if the key selectors are anonymous types, EF Core generates a join condition to compare equality component-wise.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#JoinComposite)]

```sql
SELECT [p].[PersonId], [p].[Name], [p].[PhotoId], [p0].[PersonPhotoId], [p0].[Caption], [p0].[Photo]
FROM [PersonPhoto] AS [p0]
INNER JOIN [Person] AS [p] ON ([p0].[PersonPhotoId] = [p].[PhotoId] AND ([p0].[Caption] = N'SN'))
```

## GroupJoin

The LINQ GroupJoin operator allows you to connect two data sources similar to Join, but it creates a group of inner values for matching outer elements. Executing a query like the following example generates a result of `Blog` & `IEnumerable<Post>`. Since databases (especially relational databases) don't have a way to represent a collection of client-side objects, GroupJoin doesn't translate to the server in many cases. It requires you to get all of the data from the server to do GroupJoin without a special selector (first query below). But if the selector is limiting data being selected then fetching all of the data from the server may cause performance issues (second query below). That's why EF Core doesn't translate GroupJoin.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#GroupJoin)]

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#GroupJoinComposed)]

## SelectMany

The LINQ SelectMany operator allows you to enumerate over a collection selector for each outer element and generate tuples of values from each data source. In a way, it's a join but without any condition so every outer element is connected with an element from the collection source. Depending on how the collection selector is related to the outer data source, SelectMany can translate into various different queries on the server side.

### Collection selector doesn't reference outer

When the collection selector isn't referencing anything from the outer source, the result is a cartesian product of both data sources. It translates to `CROSS JOIN` in relational databases.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#SelectManyConvertedToCrossJoin)]

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
CROSS JOIN [Posts] AS [p]
```

### Collection selector references outer in a where clause

When the collection selector has a where clause, which references the outer element, then EF Core translates it to a database join and uses the predicate as the join condition. Normally this case arises when using collection navigation on the outer element as the collection selector. If the collection is empty for an outer element, then no results would be generated for that outer element. But if `DefaultIfEmpty` is applied on the collection selector then the outer element will be connected with a default value of the inner element. Because of this distinction, this kind of queries translates to `INNER JOIN` in the absence of `DefaultIfEmpty` and `LEFT JOIN` when `DefaultIfEmpty` is applied.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#SelectManyConvertedToJoin)]

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
INNER JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId]

SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId]
```

### Collection selector references outer in a non-where case

When the collection selector references the outer element, which isn't in a where clause (as the case above), it doesn't translate to a database join. That's why we need to evaluate the collection selector for each outer element. It translates to `APPLY` operations in many relational databases. If the collection is empty for an outer element, then no results would be generated for that outer element. But if `DefaultIfEmpty` is applied on the collection selector then the outer element will be connected with a default value of the inner element. Because of this distinction, this kind of queries translates to `CROSS APPLY` in the absence of `DefaultIfEmpty` and `OUTER APPLY` when `DefaultIfEmpty` is applied. Certain databases like SQLite don't support `APPLY` operators so this kind of query may not be translated.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#SelectManyConvertedToApply)]

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], ([b].[Url] + N'=>') + [p].[Title] AS [p]
FROM [Blogs] AS [b]
CROSS APPLY [Posts] AS [p]

SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], ([b].[Url] + N'=>') + [p].[Title] AS [p]
FROM [Blogs] AS [b]
OUTER APPLY [Posts] AS [p]
```

## GroupBy

LINQ GroupBy operators create a result of type `IGrouping<TKey, TElement>` where `TKey` and `TElement` could be any arbitrary type. Furthermore, `IGrouping` implements `IEnumerable<TElement>`, which means you can compose over it using any LINQ operator after the grouping. Since no database structure can represent an `IGrouping`, GroupBy operators have no translation in most cases. When an aggregate operator is applied to each group, which returns a scalar, it can be translated to SQL `GROUP BY` in relational databases. The SQL `GROUP BY` is restrictive too. It requires you to group only by scalar values. The projection can only contain grouping key columns or any aggregate applied over a column. EF Core identifies this pattern and translates it to the server, as in the following example:

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#GroupBy)]

```sql
SELECT [p].[AuthorId] AS [Key], COUNT(*) AS [Count]
FROM [Posts] AS [p]
GROUP BY [p].[AuthorId]
```

EF Core also translates queries where an aggregate operator on the grouping appears in a Where or OrderBy (or other ordering) LINQ operator. It uses `HAVING` clause in SQL for the where clause. The part of the query before applying the GroupBy operator can be any complex query as long as it can be translated to server. Furthermore, once you apply aggregate operators on a grouping query to remove groupings from the resulting source, you can compose on top of it like any other query.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#GroupByFilter)]

```sql
SELECT [p].[AuthorId] AS [Key], COUNT(*) AS [Count]
FROM [Posts] AS [p]
GROUP BY [p].[AuthorId]
HAVING COUNT(*) > 0
ORDER BY [p].[AuthorId]
```

The aggregate operators EF Core supports are as follows

| .NET                     | SQL           |
|--------------------------|---------------|
| Average(x => x.Property) | AVG(Property) |
| Count()                  | COUNT(*)      |
| LongCount()              | COUNT(*)      |
| Max(x => x.Property)     | MAX(Property) |
| Min(x => x.Property)     | MIN(Property) |
| Sum(x => x.Property)     | SUM(Property) |

Additional aggregate operators may be supported. Check your provider docs for more function mappings.

Even though there is no database structure to represent an `IGrouping`, in some cases, EF Core 7.0 and newer can create the groupings after the results are returned from the database. This is similar to how the [`Include`](xref:core/querying/related-data/eager) operator works when including related collections. The following LINQ query uses the GroupBy operator to group the results by the value of their Price property.

```csharp
var query = context.Books.GroupBy(s => s.Price);
```

```sql
SELECT [b].[Price], [b].[Id], [b].[AuthorId]
FROM [Books] AS [b]
ORDER BY [b].[Price]
```

In this case, the GroupBy operator doesn't translate directly to a `GROUP BY` clause in the SQL, but instead, EF Core creates the groupings after the results are returned from the server.

## Left Join

While Left Join isn't a LINQ operator, relational databases have the concept of a Left Join which is frequently used in queries. A particular pattern in LINQ queries gives the same result as a `LEFT JOIN` on the server. EF Core identifies such patterns and generates the equivalent `LEFT JOIN` on the server side. The pattern involves creating a GroupJoin between both the data sources and then flattening out the grouping by using the SelectMany operator with DefaultIfEmpty on the grouping source to match null when the inner doesn't have a related element. The following example shows what that pattern looks like and what it generates.

[!code-csharp[Main](../../../samples/core/Querying/ComplexQuery/Program.cs#LeftJoin)]

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url], [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId]
```

The above pattern creates a complex structure in the expression tree. Because of that, EF Core requires you to flatten out the grouping results of the GroupJoin operator in a step immediately following the operator. Even if the GroupJoin-DefaultIfEmpty-SelectMany is used but in a different pattern, we may not identify it as a Left Join.
