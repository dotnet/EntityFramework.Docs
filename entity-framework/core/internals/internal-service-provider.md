---
title: The internal service provider - EF Core
author: ajcvickers
ms.date: 01/25/2020
uid: core/internals/internal-service-provider
---

# The internal service provider

## Introduction

Internally, EF Core is built around a [dependency injection]() container.
This container exposes an extensible set of services that define the behavior of EF Core.
Service implementations can come from the application, plug-ins, extensions, database providers, and EF Core itself.

This container is exposed as an [`IServiceProvider`]() and implemented using [Microsoft.Extensions.DependencyInjection]().
It is therefore known as the _internal service provider_.

## DbContext and the internal service provider

[`DbContext`]() instances are created by the application using:
* Explicit construction with [`new`]()
* The application's own dependency injection container
This container is known as the [_application service provider_]().

The DbContext instance then locates the internal service provider to use.<sup>[(1)](#footnote-1)</sup>
The service provider is most commonly created automatically when needed.
It is then [cached and reused]().

An application can instead explicitly configure the internal service provider to use.
The application must then handle caching.

### Scopes and lifetimes

A new [service provider scope]() is created for each DbContext instance.
This means that:
* [Singleton]() services are shared between DbContext instances
* [Scoped]() services are shared within, but not between, DbContext instances

A DbContext instance typically represents a [session]() or [unit-of-work]().
This means scoped services work together to orchestrate the DbContext session.

### Multiple internal service providers

Most singleton services behave the same way between different context instances.
That is, changes in the context configuration do not affect these services.

However, some changes in context configuration require singleton services to behave differently.
For example, [sensitive data logging]() requires singleton services to change the way they log.
Likewise, some changes in context configuration require different service implementations.
For example, [changing the database provider]() requires different implementations for many services.

These additional service providers are cached automatically.
An [error/warning]() is generated if this cache gets too large.

Applications managing the internal service provider must supply a container consistent with the context configuration.

## Building the internal service provider

An internal service provider must be configured with one and only one database provider.
This database provider supplies the entry point (e.g. [`AddEntityFrameworkSqlServer`]()) to configure the provider.
However, calling AddEntityFramework... explicitly is rare.
Usually a method like [`UseSqlServer`]() adds a provider-specific [OptionsExtension]() when configuring the context.
This extension then calls the provider-specific AddEntityFramework... method as part of its [ApplyServices]() implementation.

### Service definitions

[`EntityFrameworkServicesBuilder`]() defines a set of [`CoreServices`]().
The public surface of each service is defined by an interface.
All interaction between services uses this public surface.
A service must only depend on the interfaces of other services.
A service must never depend on the specific implementation of another service.

In addition, all EF Core services have a well-defined [lifetime]() and a flag indicating whether or not multiple registrations are allowed.
These service characteristics are used for any implementation of the service, regardless of where it comes from.

### Registration of services

[`EntityFrameworkServicesBuilder.TryAddCoreServices`] is used by database providers to register default service implementations.
As the name suggests, TryAddCoreServices only register a default implementation if some other implementation has not already been registered.
This means database providers can override service implementations by registering an implementation before calling TryAddCoreServices.

Database providers register overrides using [`EntityFrameworkServicesBuilder.TryAdd`]() methods.
These methods ensure that provider registrations use the service characteristics (notably, lifetime) defined by EF Core.

For example, [`AddEntityFrameworkCosmos`]() looks something like this:

```CSharp
public static IServiceCollection AddEntityFrameworkCosmos([NotNull] this IServiceCollection serviceCollection)
{
    var builder = new EntityFrameworkServicesBuilder(serviceCollection)
        .TryAdd<LoggingDefinitions, CosmosLoggingDefinitions>()
        .TryAdd<IDatabaseProvider, DatabaseProvider<CosmosOptionsExtension>>()
        // ... more Cosmos service registrations.
    
    builder.TryAddCoreServices();
    
    return serviceCollection;
}
```

### Relational database providers

Database providers for relational databases share many common services.
These services are defined by [`EntityFrameworkRelationalServicesBuilder`](), which inherits from EntityFrameworkServicesBuilder.
EntityFrameworkRelationalServicesBuilder overrides TryAddCoreServices to register relational implementations before calling the base method to register EF Core defaults.

A relational database provider uses the same code shown above, except using a EntityFrameworkRelationalServicesBuilder instance.
For example, see [`AddEntityFrameworkSqlServer`()].
This means that:
* Provider-specific services are registered first.
* Relational-specific services are then registered unless the provider has already registered a provider-specific implementation.
* Default services are then registered only when a relational-specific or provider-specific implementation has not been already registered.

### Extending services

Sometimes it is useful for a provider or relational-specific service to extend the core service interface.
For example, [`IRelationalTypeMappingSource`]() inherits from [`ITypeMappingSource`]() adding relational-specific mapping methods.

Non-relational code will resolve this service using ITypeMappingSource.
Relational code that needs the relational-specific methods will resolve the same service using IRelationalTypeMappingSource.
Both these resolutions must return _the same service instance_.
This is achieved redirecting resolution for ITypeMappingSource to use IRelationalTypeMappingSource:

```CSharp
TryAdd<ITypeMappingSource>(p => p.GetService<IRelationalTypeMappingSource>());
```

## Resolving services

Service implementations can be public or [public but internal]().
Internal service implementations use [constructor-injection]() for dependencies in the normal way.
For example:

```CSharp
public MigrationsModelDiffer(
    IRelationalTypeMappingSource typeMappingSource,
    IMigrationsAnnotationProvider migrationsAnnotations,
    ...)
{
    TypeMappingSource = typeMappingSource;
    MigrationsAnnotations = migrationsAnnotations;
    ...
}
```

Adding a new dependency to this constructor will break subclasses.
This is [generally not acceptable]() for public classes.
Therefore, public service implementations obtain their dependencies through sealed _dependency objects_.
See [Dependency objects]() for important considerations and rules when using dependency objects.

## Footnotes

<a name="footnote-1"></a>
<sup>(1)</sup> Isn't that backwards? Surely the container should be created first and then the DbContext resolved from it.

We debated this question at the outset of EF Core.
Several things influenced the decision:
* Not everyone wants to use dependency injection
* Reduction in the number concepts that must be understood to get started
* Easy transition from EF6 DbContext patterns

It has also become apparent that application service provider is the best place to create DbContext instances.
This is because the application configures the context and controls its lifetime.
That being said, [resolving DbContext instances from a container]() is something we plan to improve.

