---
title: Query Tags - EF Core
author: divega
ms.date: 11/14/2018
ms.assetid: 73C7A627-C8E9-452D-9CD5-AFCC8FEFE395
uid: core/querying/tags
---

# Query tags
> [!NOTE]
> This feature is new in EF Core 2.2.

This feature helps correlate LINQ queries in code with generated SQL queries captured in logs.
You annotate a LINQ query using the new `TagWith()` method: 

``` csharp
  var nearestFriends =
      (from f in context.Friends.TagWith("This is my spatial query!")
      orderby f.Location.Distance(myLocation) descending
      select f).Take(5).ToList();
```

This LINQ query is translated to the following SQL statement:

``` sql
-- This is my spatial query!

SELECT TOP(@__p_1) [f].[Name], [f].[Location]
FROM [Friends] AS [f]
ORDER BY [f].[Location].STDistance(@__myLocation_0) DESC
```

It's possible to call `TagWith()` many times on the same query.
Query tags are cumulative.
For example, given the following methods:

``` csharp
IQueryable<Friend> GetNearestFriends(Point myLocation) =>
    from f in context.Friends.TagWith("GetNearestFriends")
    orderby f.Location.Distance(myLocation) descending
    select f;

IQueryable<T> Limit<T>(IQueryable<T> source, int limit) =>
    source.TagWith("Limit").Take(limit);
```

The following query:   

``` csharp
var results = Limit(GetNearestFriends(myLocation), 25).ToList();
```

Translates to:

``` sql
-- GetNearestFriends

-- Limit

SELECT TOP(@__p_1) [f].[Name], [f].[Location]
FROM [Friends] AS [f]
ORDER BY [f].[Location].STDistance(@__myLocation_0) DESC
```

It's also possible to use multi-line strings as query tags.
For example:

``` csharp
var results = Limit(GetNearestFriends(myLocation), 25).TagWith(
@"This is a multi-line
string").ToList();
```

Produces the following SQL:

``` sql
-- GetNearestFriends

-- Limit

-- This is a multi-line
-- string

SELECT TOP(@__p_1) [f].[Name], [f].[Location]
FROM [Friends] AS [f]
ORDER BY [f].[Location].STDistance(@__myLocation_0) DESC
```

## Known limitations
**Query tags aren't parameterizable:**
EF Core always treats query tags in the LINQ query as string literals that are included in the generated SQL.
Compiled queries that take query tags as parameters aren't allowed.