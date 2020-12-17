---
title: Advanced Performance Topics
description: Advanced performance topics for Entity Framework Core
author: rick-anderson
ms.author: riande
ms.date: 12/9/2020
uid: core/performance/advanced-performance-topics
---
# Advanced Performance Topics

## DbContext pooling

`AddDbContextPool` enables pooling of `DbContext` instances. Context pooling can increase throughput in high-scale scenarios such as web servers by reusing context instances, rather than creating new instances for each request.

The typical pattern in an ASP.NET Core app using EF Core involves registering a custom <xref:Microsoft.EntityFrameworkCore.DbContext> type into the [dependency injection](/aspnet/core/fundamentals/dependency-injection) container and obtaining instances of that type through constructor parameters in controllers or Razor Pages. Using constructor injection, a new context instance is created for each request.

<xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool%2A> enables a pool of reusable context instances. To use context pooling, use the `AddDbContextPool` method instead of `AddDbContext` during service registration:

```csharp
services.AddDbContextPool<BloggingContext>(
    options => options.UseSqlServer(connectionString));
```

When `AddDbContextPool` is used, at the time a context instance is requested, EF first checks if there is an instance available in the pool. Once the request processing finalizes, any state on the instance is reset and the instance is itself returned to the pool.

This is conceptually similar to how connection pooling operates in ADO.NET providers and has the advantage of saving some of the cost of initialization of the context instance.

The `poolSize` parameter of <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool%2A> sets the maximum number of instances retained by the pool. Once `poolSize` is exceeded, new context instances are not cached and  EF falls back to the non-pooling behavior of creating instances on demand.

### Limitations

Apps should be profiled and tested to show that context initialization is a significant cost.

`AddDbContextPool` has a few limitations on what can be done in the `OnConfiguring` method of the context.

> [!WARNING]
> Avoid using context pooling in apps that maintain state. For example, private fields in the context that shouldn't be shared across requests. EF Core only resets the state that it is aware of before adding a context instance to the pool.

Context pooling works by reusing the same context instance across requests. This means that it's effectively registered as a [Singleton](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) in terms of the instance itself so that it's able to persist.

Context pooling is intended for scenarios where the context configuration, which includes services resolved, is fixed between requests. For cases where [Scoped](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) services are required, or configuration needs to be changed, don't use pooling. The performance gain from pooling is usually negligible except in highly optimized scenarios.

## Query caching and parameterization

When EF receives a LINQ query tree for execution, it must first "compile" that tree into a SQL query. Because this is a heavy process, EF caches queries by the query tree *shape*: queries with the same structure reuse internally-cached compilation outputs, and can skip repeated compilation. The different queries may still reference different *values*, but as long as these values are properly parameterized, the structure is the same and caching will function properly.

Consider the following two queries:

[!code-csharp[Main](../../../samples/core/Performance/Program.cs#QueriesWithConstants)]

Since the expression trees contains different constants, the expression tree differs and each of these queries will be compiled separately by EF Core. In addition, each query produces a slightly different SQL command:

```sql
SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] = N'blog1'

SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] = N'blog2'
```

Because the SQL differs, your database server will likely also need to produce a query plan for both queries, rather than reusing the same plan.

A small modification to your queries can change things considerably:

[!code-csharp[Main](../../../samples/core/Performance/Program.cs#QueriesWithParameterization)]

Since the blog name is now *parameterized*, both queries have the same tree shape, and EF only needs to be compiled once. The SQL produced is also parameterized, allowing the database to reuse the same query plan:

```sql
SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] = @__blogName_0
```

Note that there is no need to parameterize each and every query: it's perfectly fine to have some queries with constants, and indeed, databases (and EF) can sometimes perform certain optimization around constants which aren't possible when the query is parameterized. See the section on [dynamically-constructed queries](#dynamically-constructed-queries) for an example where proper parameterization is crucial.

> [!NOTE]
> EF Core's [event counters](xref:core/logging-events-diagnostics/event-counters) report the Query Cache Hit Rate. In a normal application, this counter reaches 100% soon after program startup, once most queries have executed at least once. If this counter remains stable below 100%, that is an indication that your application may be doing something which defeats the query cache - it's a good idea to investigate that.

> [!NOTE]
> How the database manages caches query plans is database-dependent. For example, SQL Server implicitly maintains an LRU query plan cache, whereas PostgreSQL does not (but prepared statements can produce a very similar end effect). Consult your database documentation for more details.

## Dynamically-constructed queries

In some situations, it is necessary to dynamically construct LINQ queries rather than specifying them outright in source code. This can happen, for example, in a website which receives arbitrary query details from a client, with open-ended query operators (sorting, filtering, paging...). In principle, if done correctly, dynamically-constructed queries can be just as efficient as regular ones (although it's not possible to use the compiled query optimization with dynamic queries). In practice, however, they are frequently the source of performance issues, since it's easy to accidentally produce expression trees with shapes that differ every time.

The following example uses two techniques to dynamically construct a query; we add a `Where` operator to the query only if the given parameter is not null. Note that this isn't a good use case for dynamically constructing a query - but we're using it for simplicity:

### [With constant](#tab/with-constant)

[!code-csharp[Main](../../../samples/core/Benchmarks/DynamicallyConstructedQueries.cs?name=WithConstant&highlight=14-24)]

### [With parameter](#tab/with-parameter)

[!code-csharp[Main](../../../samples/core/Benchmarks/DynamicallyConstructedQueries.cs?name=WithParameter&highlight=14)]

***

Benchmarking these two techniques gives the following results:

|        Method |       Mean |    Error |    StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |-----------:|---------:|----------:|--------:|-------:|------:|----------:|
|  WithConstant | 1,096.7 us | 12.54 us |  11.12 us | 13.6719 | 1.9531 |     - |  83.91 KB |
| WithParameter |   570.8 us | 42.43 us | 124.43 us |  5.8594 |      - |     - |  37.16 KB |

Even if the sub-millisecond difference seems small, keep in mind that the constant version continuously pollutes the cache and causes other queries to be re-compiled, slowing them down as well.

> [!NOTE]
> Avoid constructing queries with the expression tree API unless you really need to. Aside from the API's complexity, it's very easy to inadvertently cause significant performance issues when using them.
