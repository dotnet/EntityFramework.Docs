---
title: DbContext Pooling
description: DbContext pooling in Entity Framework Core
author: rick-anderson
ms.author: riande
ms.date: 9/19/2020
uid: core/miscellaneous/context-pooling
---
# DbContext pooling

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

## Limitations

Apps should be profiled and tested to show that context initialization is a significant cost.

`AddDbContextPool` has a few limitations on what can be done in the `OnConfiguring` method of the context.

> [!WARNING]
> Avoid using context pooling in apps that maintain state. For example, private fields in the context that shouldn't be shared across requests. EF Core only resets the state that it is aware of before adding a context instance to the pool.

Context pooling works by reusing the same context instance across requests. This means that it's effectively registered as a [Singleton](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) in terms of the instance itself so that it's able to persist.

Context pooling is intended for scenarios where the context configuration, which includes services resolved, is fixed between requests. For cases where [Scoped](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) services are required, or configuration needs to be changed, don't use pooling. The performance gain from pooling is usually negligible except in highly optimized scenarios.
