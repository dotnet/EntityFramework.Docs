---
title: Single vs. Split Queries - EF Core
description: Translating LINQ queries into single and split SQL queries with Entity Framework Core
author: roji
ms.date: 1/9/2023
uid: core/querying/single-split-queries
---
# Single vs. Split Queries

## Performance issues with single queries

When working against relational databases, EF loads related entities by introducing JOINs into a single query. While JOINs are quite standard when using SQL, they can create significant performance issues if used improperly. This page describes these performance issues, and shows an alternative way to load related entities which works around them.

### Cartesian explosion

Let's examine the following LINQ query and its translated SQL equivalent:

```c#
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .Include(b => b.Contributors)
    .ToListAsync();
```

```sql
SELECT [b].[Id], [b].[Name], [p].[Id], [p].[BlogId], [p].[Title], [c].[Id], [c].[BlogId], [c].[FirstName], [c].[LastName]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
LEFT JOIN [Contributors] AS [c] ON [b].[Id] = [c].[BlogId]
ORDER BY [b].[Id], [p].[Id]
```

In this example, since both `Posts` and `Contributors` are collection navigations of `Blog` - they're at the same level - relational databases return a *cross product*: each row from `Posts` is joined with each row from `Contributors`. This means that if a given blog has 10 posts and 10 contributors, the database returns 100 rows for that single blog. This phenomenon - sometimes called *cartesian explosion* - can cause huge amounts of data to unintentionally get transferred to the client, especially as more sibling JOINs are added to the query; this can be a major performance issue in database applications.

Note that cartesian explosion does not occur when the two JOINs aren't at the same level:

```c#
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .ThenInclude(p => p.Comments)
    .ToListAsync();
```

```sql
SELECT [b].[Id], [b].[Name], [t].[Id], [t].[BlogId], [t].[Title], [t].[Id0], [t].[Content], [t].[PostId]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
LEFT JOIN [Comment] AS [c] ON [p].[Id] = [c].[PostId]
ORDER BY [b].[Id], [t].[Id]
```

In this query, `Comments` is a collection navigation of `Post`, unlike `Contributors` in the previous query, which was a collection navigation of `Blog`. In this case, a single row is returned for each comment that a blog has (through its posts), and a cross product does not occur.

### Data duplication

JOINs can create another type of performance issue. Let's examine the following query, which only loads a single collection navigation:

```c#
var blogs = await ctx.Blogs
    .Include(b => b.Posts)
    .ToListAsync();
```

```sql
SELECT [b].[Id], [b].[Name], [b].[HugeColumn], [p].[Id], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Posts] AS [p] ON [b].[Id] = [p].[BlogId]
ORDER BY [b].[Id]
```

Examining at the projected columns, each row returned by this query contains properties from both the `Blogs` and `Posts` tables; this means that the blog properties are duplicated for each post that the blog has. While this is usually normal and causes no issues, if the `Blogs` table happens to have a very big column (e.g. binary data, or a huge text), that column would get duplicated and sent back to the client multiple times. This can significantly increase network traffic and adversely affect your application's performance.

If you don't actually need the huge column, it's easy to simply not query for it:

```c#
var blogs = await ctx.Blogs
    .Select(b => new
    {
        b.Id,
        b.Name,
        b.Posts
    })
    .ToListAsync();
```

By using a projection to explicitly choose which columns you want, you can omit big columns and improve performance; note that this is a good idea regardless of data duplication, so consider doing it even when not loading a collection navigation. However, since this projects the blog to an anonymous type, the blog isn't tracked by EF and changes to it can't be saved back as usual.

It's worth noting that unlike cartesian explosion, the data duplication caused by JOINs isn't typically significant, as the duplicated data size is negligible; this typically is something to worry about only if you have big columns in your principal table.

## Split queries

To work around the performance issues described above, EF allows you to specify that a given LINQ query should be *split* into multiple SQL queries. Instead of JOINs, split queries generate an additional SQL query for each included collection navigation:

[!code-csharp[Main](../../../samples/core/Querying/RelatedData/Program.cs?name=AsSplitQuery&highlight=5)]

It will produce the following SQL:

```sql
SELECT [b].[BlogId], [b].[OwnerId], [b].[Rating], [b].[Url]
FROM [Blogs] AS [b]
ORDER BY [b].[BlogId]

SELECT [p].[PostId], [p].[AuthorId], [p].[BlogId], [p].[Content], [p].[Rating], [p].[Title], [b].[BlogId]
FROM [Blogs] AS [b]
INNER JOIN [Posts] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId]
```

> [!WARNING]
> When using split queries with Skip/Take on EF versions prior to 10, pay special attention to making your query ordering fully unique; not doing so could cause incorrect data to be returned. For example, if results are ordered only by date, but there can be multiple results with the same date, then each one of the split queries could each get different results from the database. Ordering by both date and ID (or any other unique property or combination of properties) makes the ordering fully unique and avoids this problem. Note that relational databases do not apply any ordering by default, even on the primary key.

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
- When including reference navigations as well as collection navigations, each one of the split queries will include joins to the reference navigations. This can degrade performance, particularly if there are many reference navigations. Please upvote [#29182](https://github.com/dotnet/efcore/issues/29182) if this is something you'd like to see fixed.

Unfortunately, there isn't one strategy for loading related entities that fits all scenarios. Carefully consider the advantages and disadvantages of single and split queries to select the one that fits your needs.
