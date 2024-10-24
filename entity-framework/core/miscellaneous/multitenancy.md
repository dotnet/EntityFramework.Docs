---
title: Multi-tenancy - EF Core
description: Learn several ways to implement multi-tenant databases using Entity Framework Core.
author: jeremylikness
ms.author: jeliknes
ms.date: 03/01/2022
uid: core/miscellaneous/multitenancy
---
# Multi-tenancy

Many line of business applications are designed to work with multiple customers. It is important to secure the data so that customer data isn't "leaked" or seen by other customers and potential competitors. These applications are classified as "multi-tenant" because each customer is considered a tenant of the application with their own set of data.

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

> [!IMPORTANT]
> This document provides examples and solutions "as is." These are not intended to be "best practices" but rather "working practices" for your consideration.

> [!TIP]
> You can view the source code for this [sample on GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Miscellaneous/Multitenancy)

## Supporting multi-tenancy

There are many approaches to implementing multi-tenancy in applications. One common approach (that is sometimes a requirement) is to keep data for each customer in a separate database. The schema is the same but the data is customer-specific. Another approach is to partition the data in an existing database by customer. This can be done by using a column in a table, or having a table in multiple schemas with a schema for each tenant.

|Approach|Column for Tenant?|Schema per Tenant?|Multiple Databases?|EF Core Support|
|:--:|:--:|:--:|:--:|:--:|
|Discriminator (column)|Yes|No|No|Global query filter|
|Database per tenant|No|No|Yes|Configuration|
|Schema per tenant|No|Yes|No|Not supported|

For the database-per-tenant approach, switching to the right database is as simple as providing the correct connection string. When the data is stored in a single database, a [global query filter](/ef/core/querying/filters) can be used to automatically filter rows by the tenant ID column, ensuring that developers don't accidentally write code that can access data from other customers.

These examples should work fine in most app models, including console, WPF, WinForms, and ASP.NET Core apps. Blazor Server apps require special consideration.

### Blazor Server apps and the life of the factory

The recommended pattern for [using Entity Framework Core in Blazor apps](/aspnet/core/blazor/blazor-server-ef-core) is to register the [DbContextFactory](/ef/core/dbcontext-configuration/#using-a-dbcontext-factory-eg-for-blazor), then call it to create a new instance of the `DbContext` each operation. By default, the factory is a _singleton_ so only one copy exists for all users of the application. This is usually fine because although the factory is shared, the individual `DbContext` instances are not.

For multi-tenancy, however, the connection string may change per user. Because the factory caches the configuration with the same lifetime, this means all users must share the same configuration. Therefore, the lifetime should be changed to `Scoped`.

This issue doesn't occur in Blazor WebAssembly apps because the singleton is scoped to the user. Blazor Server apps, on the other hand, present a unique challenge. Although the app is a web app, it is "kept alive" by real-time communication using SignalR. A session is created per user and lasts beyond the initial request. A new factory should be provided per user to allow new settings. The lifetime for this special factory is scoped and a new instance is created per user session.

## An example solution (single database)

A possible solution is to create a simple `ITenantService` service that handles setting the user's current tenant. It provides callbacks so code is notified when the tenant changes. The implementation (with the callbacks omitted for clarity) might look like this:

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/Common/ITenantService.cs":::

The `DbContext` can then manage the multi-tenancy. The approach depends on your database strategy. If you are storing all tenants in a single database, you are likely going to use a query filter. The `ITenantService` is passed to the constructor via dependency injection and used to resolve and store the tenant identifier.

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/SingleDbSingleTable/Data/ContactContext.cs" range="10-13":::

The `OnModelCreating` method is overridden to specify the query filter:

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/SingleDbSingleTable/Data/ContactContext.cs" range="31-33":::

This ensures that every query is filtered to the tenant on every request. There is no need to filter in application code because the global filter will be automatically applied.

The tenant provider and `DbContextFactory` are configured in the application startup like this, using Sqlite as an example:

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/SingleDbSingleTable/Program.cs" range="13-14":::

Notice that the [service lifetime](/dotnet/core/extensions/dependency-injection#service-lifetimes) is configured with `ServiceLifetime.Scoped`. This enables it to take a dependency on the tenant provider.

> [!NOTE]
> Dependencies must always flow towards the singleton. That means a `Scoped` service can depend on another `Scoped` service or a `Singleton` service, but a `Singleton` service can only depend on other `Singleton` services: `Transient => Scoped => Singleton`.

## Multiple schemas

> [!WARNING]
> This scenario is not directly supported by EF Core and is not a recommended solution.

In a different approach, the same database may handle `tenant1` and `tenant2` by using table schemas.

- **Tenant1** - `tenant1.CustomerData`
- **Tenant2** - `tenant2.CustomerData`

If you are not using EF Core to handle database updates with migrations and already have multi-schema tables, you can override the schema in a `DbContext` in `OnModelCreating` like this (the schema for table `CustomerData` is set to the tenant):

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder) =>
    modelBuilder.Entity<CustomerData>().ToTable(nameof(CustomerData), tenant);
```

## Multiple databases and connection strings

The multiple database version is implemented by passing a different connection string for each tenant. This can be configured at startup by resolving the service provider and using it to build the connection string. A connection string by tenant section is added to the `appsettings.json` configuration file.  

:::code language="json" source="../../../samples/core/Miscellaneous/Multitenancy/MultiDb/appsettings.json":::

The service and configuration are both injected into the `DbContext`:

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/MultiDb/ContactContext.cs" range="11-19":::

The tenant is then used to look up the connection string in `OnConfiguring`:

:::code language="csharp" source="../../../samples/core/Miscellaneous/Multitenancy/MultiDb/ContactContext.cs" range="40-45":::

This works fine for most scenarios unless the user can switch tenants during the same session.

### Switching tenants

In the previous configuration for multiple databases, the options are cached at the `Scoped` level. This means that if the user changes the tenant, the options are _not_ reevaluated and so the tenant change isn't reflected in queries.

The easy solution for this when the tenant _can_ change is to set the lifetime to `Transient.` This ensures the tenant is re-evaluated along with the connection string each time a `DbContext` is requested. The user can switch tenants as often as they like. The following table helps you choose which lifetime makes the most sense for your factory.

|**Scenario**|**Single database**|**Multiple databases**|
|:--|:--|:--|
|_User stays in a single tenant_|`Scoped`|`Scoped`|
|_User can switch tenants_|`Scoped`|`Transient`|

The default of `Singleton` still makes sense if your database does not take on user-scoped dependencies.

## Performance notes

EF Core was designed so that `DbContext` instances can be instantiated quickly with as little overhead as possible. For that reason, creating a new `DbContext` per operation should usually be fine. If this approach is impacting the performance of your application, consider using [DbContext pooling](xref:core/performance/advanced-performance-topics).

## Conclusion

This is working guidance for implementing multi-tenancy in EF Core apps. If you have further examples or scenarios or wish to provide feedback, please [open an issue](https://github.com/dotnet/EntityFramework.Docs/issues/new) and reference this document.
