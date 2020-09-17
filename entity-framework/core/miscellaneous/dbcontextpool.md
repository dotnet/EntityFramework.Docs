---
title: Logging - EF Core
description: Configuring logging with Entity Framework Core
author: rick-anderson
ms.author: riande
ms.date: 9/19/2020
uid: core/miscellaneous/dbcontextpool
---
# Connection pooling

The typical pattern for using EF Core in an ASP.NET Core app usually involves registering a custom <xref:Microsoft.EntityFrameworkCore.DbContext> type into the [dependency injection](/aspnet/core/fundamentals/dependency-injection) system and later obtaining instances of that type through constructor parameters in controllers or Razor Pages. Using constructor injection, a new instance of the `DbContext` is created for each request.

<xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool%2A> provides a way to provide a pool of reusable `DbContext` instances. To use `DbContext` pooling, use the `AddDbContextPool` instead of `AddDbContext` during service registration:

``` csharp
services.AddDbContextPool<BloggingContext>(
    options => options.UseSqlServer(connectionString));
```

When `AddDbContextPool` is called, at the time a `DbContext` instance is requested, EF first checks if there is an instance available in the pool. Once the request processing finalizes, any state on the instance is reset and the instance is itself returned to the pool.

This is conceptually similar to how connection pooling operates in ADO.NET providers and has the advantage of saving some of the cost of initialization of the `DbContext` instance.

## Limitations

`AddDbContextPool` has a few limitations on what can be done in the `OnConfiguring` method of the `DbContext`.

> [!WARNING]  
> Avoid using `DbContext` Pooling in apps that maintain state. For example, private fields, in the derived `DbContext` class that shouldn't be shared across requests. EF Core only resets the state that it is aware of before adding a `DbContext` instance to the pool.

Context pooling works by reusing the same context instance across requests. This means that it's effectively registered as a [Singleton](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) in terms of the instance itself so that it's able to persist.
<!-- Review, so what's able tto persist  -->

Context pooling is intended for scenarios where the context configuration, which includes services resolved, is fixed between requests. For cases where configuration needs to be changed, don't use pooling. Note that the performance gain from pooling is usually negligible except in highly-optimized scenarios. <!-- If you find that not to be the case, please create a sample and an issue on the [EF Core GitHub repo](https://github.com/dotnet/efcore). Review: let me know if you want to add this sentence -->