---
title: Understanding EF Services
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: a871e3ca-3650-459d-b084-8fde5d7b2e3a
ms.prod: entity-framework
uid: core/miscellaneous/internals/services
---
# Understanding EF Services

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

Entity Framework executes as a collection of services working together. A service is a reusable component. A service is typically an implementation of an interface. Services are available to other services via [dependency injection (DI)](https://wikipedia.org/wiki/Dependency_injection), which is implemented in EF using [Microsoft.Extensions.DependencyInjection](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html).

This article covers some fundamentals principles for understanding how EF uses services and DI.

## Terms

**Service**

   A reusable component. In .NET, a service can be identified by a class or interface. By convention, Entity Framework only uses interfaces to identify services.

**Service lifetime**

   A description of the way in which a service is persisted and disposed across multiple uses of the same service type.

**Service provider**

   The mechanism for storing a collection of services. Also known as a service container.

**Service collection**

   The mechanism for constructing a service provider.

## Categories of Services

Services fall into one or more categories.

**Context services**

   Services that are related to a specific instance of  `DbContext`. They provide functionality for working with the user model and context options.

**Provider services**

   Provider-specific implementations of services. For example, SQLite uses "provider services" to customize the behavior of SQL generation, migrations, and file I/O.

**Design-time services**

   Services used when a developer is creating an application. For example, EF commands uses design-time services to execute migrations and code generation (aka scaffolding).

**User services**

   A user can define custom services to interact with EF. These are written in application code, not provider code. For example, users can provide an implementation of `IModelCustomizer` for controlling how a model is created.

> [!NOTE]
> Service provider is not to be confused with a "provider's services".

## Service Lifetime

EF services can be registered with different lifetime options. The suitable option depends on how the service is used and implemented.

**Transient**

   Transient lifetime services are created each time they are injected into other services. This isolates each instance of the service. For example, `MigrationsScaffolder` should not be reused, therefore it is registered as transient.

**Scoped**

   Scoped lifetime services are created once per `DbContext` instance. This is used to isolate instance of `DbContext`. For example, `StateManager` is added as scoped because it should only track entity states for one context.

**Singleton**

   Singleton lifetime services exists once per service provider and span all scopes. Each time the service is injected, the same instance is used. For example, `IModelCustomizer` is a singleton because it is idempotent, meaning each call to `IModelCustomizer.Customize()` does not change the customizer.

## How AddDbContext works

EF provides an extension method `AddDbContext<TContext>()` for adding using EF into a service collection. This method adds the following into a service collection:

* `TContext` as "scoped"

* `DbContextOptions` as a "singleton"

* `DbContextOptionsFactory<T>` as a "singleton"

`AddDbContext` does not add any context services, provider services, or design-time services to the service collection (except for [special cases](#special-cases)). DbContext constructs its own internal service provider for this.

### Special cases

`AddDbContext` adds `DbContextOptionsFactory<T>` to the service collection AddDbContext was called on (which is used to create the "external" service provider). `DbContextOptionsFactory<T>` acts as a bridge between the external service provider and DbContext's internal service provider. If the external provider has services for `ILoggerFactory` or `IMemoryCache`, these will be added to the internal service provider.

The bridging is done for these common scenarios so users can easily configure logging and memory caching without needing to provide a custom internal service provider.

## DbContext's internal service provider

By default, `DbContext` uses an internal service provider that is **separate** from all other service providers in the application. This internal provider is constructed from an instance of `DbContextOptions`. Methods such as `UseSqlServer()` extend the construction step add specialized services for their database system.

### Providing a custom internal service provider

`DbContextOptionsBuilder` provides an API for giving a custom service provider to DbContext for EF to use internally. This API is `DbContextOptions.UseInternalServiceProvider(IServiceProvider provider)`.

If a custom service provider is provided, DbContext will not use `DbContextOptions` to create its own internal service provider. The custom service provider must already have provider-specific services added.

Database provider writers should provided methods such as AddEntityFrameworkSqlServer" or "AddEntityFrameworkSqlite" to simplify the process of creating a custom service container.

<!-- literal_block"language": "csharp",rp", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````csharp
var services = new ServiceCollection()
    .AddEntityFrameworkSqlServer()
    .AddSingleton<MyCustomService>()
    .BuildServiceProvider();

var options = new DbContextOptionsBuilder();

options
    .UseInternalServiceProvider(services)
    .UseSqlServer(connectionString);

using (var context = new DbContext(options))
{ }
````

### Service provider caching

EF caches this internal service provider with `IDbContextOptions` as the key. This means the service provider is only created once per unique set of options. It is reused when a DbContext is instantiated using a set of options that have already been used during the application lifetime.

## Required Provider Services

EF database providers must register a basic set of services. These required services are defined as properties on `IDatabaseProviderServices`. Provider writers may need to implement some services from scratch. Others have partial or complete implementations in EF's library that can be reused.

For more information on required provider services, see [Writing a Database Provider](writing-a-provider.md).

## Additional Information

EF uses `the Microsoft.Extensions.DependencyInjection library <[https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)>`_ to implement DI. Documentation for this library [is available on docs.asp.net](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html).

[System.IServiceProvider](https://docs.microsoft.com/en-us/dotnet/core/api/system.iserviceprovider) is defined in the .NET base class library.
