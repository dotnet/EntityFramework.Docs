---
title: Writing a Database Provider
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 1165e2ec-e421-43fc-92ab-d92f9ab3c494
ms.prod: entity-framework
uid: core/miscellaneous/internals/writing-a-provider
---
# Writing a Database Provider

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

EF Core is designed to be extensible. It provides general purpose building blocks that are intended for use in multiple providers. The purpose of this article is to provide basic guidance on creating a new provider that is compatible with EF Core.

> [!TIP]
> [EF Core source code is open-source](https://github.com/aspnet/EntityFramework). The best source of information is the code itself.

> [!TIP]
> This article shows snippets from an empty EF provider. You can view the [full stubbed-out provider](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Internals/WritingAProvider) on GitHub.

<a name=entry-point></a>

## DbContext Initialization

A user's interaction with EF begins with the `DbContext` constructor. Before the context is available for use, it initializes **options** and **services**. We will example both of these to understand what they represent and how EF configures itself to use different providers.

### Options

`Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptions` is the API surface for **users** to configure `DbContext`. Provider writers are responsible for creating API to configure options and to make services responsive to these options. For example, most providers require a connection string. These options are typically created using `DbContextOptionsBuilder`.

### Services

`System.IServiceProvider` is the main interface used for interaction with services. EF makes heavy use of [dependency injection (DI)](https://wikipedia.org/wiki/Dependency_injection). The `ServiceProvider` contains a collection of services available for injection. Initialization uses `DbContextOptions` to add additional services if needed and select a scoped set of services that all EF operations will use during execution.

See also [Understanding EF Services](services.md).

> [!NOTE]
> EF uses [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) to implement dependency injection. Documentation for this project [is available on docs.asp.net](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html).

## Plugging in a Provider

As explained above, EF uses options and services. Each provider must create API so users to add provider-specific options and services. This API is best created by using extension methods.

> [!TIP]
> When defining an extension method, define it in the namespace of the object being extended so Visual Studio auto-complete will include the extension method as a possible completion.

### The *Use* Method

By convention, providers define a `UseX()` extension on `DbContextOptionsBuilder`. This configures **options** which it typically takes as arguments to method.

<!-- literal_block"xml:space": "preserve", "classes  "backrefs  "names  "dupnames   -->
````
optionsBuilder.UseMyProvider("Server=contoso.com")
````

The `UseX()` extension method creates a provider-specific implementation of `IDbContextOptionsExtension` which is added to the collection of extensions stored within `DbContextOptions`. This is done by a call to the API `IDbContextOptionsBuilderInfrastructure.AddOrUpdateExtension`.

An example implementation of the "Use" method

<!-- [!code-csharp[Main](samples/core/internals/Miscellaneous/Internals/WritingAProvider/EntityFrameworkCore.ProviderStarter/Extensions/MyProviderDbContextOptionsExtensions.cs)] -->
````csharp
public static class MyProviderDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseMyProvider(this DbContextOptionsBuilder optionsBuilder,
        string connectionString)
    {
        ((IDbContextOptionsBuilderInfrastructure) optionsBuilder).AddOrUpdateExtension(
            new MyProviderOptionsExtension
            {
                ConnectionString = connectionString
            });

        return optionsBuilder;
    }
}
````

> [!TIP]
> The `UseX()` method can also be used to return a special wrapper around `DbContextOptionsBuilder` that allows users to configure multiple options with chained calls. See `SqlServerDbContextOptionsBuilder` as an example.

### The *Add* Method

By convention, providers define a `AddX()` extension on `EntityFrameworkServicesBuilder`. This configures **services** and does not take arguments.

`EntityFrameworkServicesBuilder` is a wrapper around `ServiceCollection` which is accessible by calling `GetInfrastructure()`. The `AddX()` method should register services in this collection to be available for dependency injection.

In some cases, users may call the *Add* method directly. This is done when users are configuring a service provider manually and use this service provider to resolve an instance of `DbContext`. In other cases, the *Add* method is called by EF upon service initialization. For more details on service initialization, see [Understanding EF Services](services.md).

A provider *must register* an implementation of `IDatabaseProvider`. Implementing this in-turn requires configuring several more required services. Read more about working with services in [Understanding EF Services](services.md).

EF provides many complete or partial implementations of the required services to make it easier for provider-writers. For example, EF includes a class `DatabaseProvider<TProviderServices, TOptionsExtension>` which can be used in service registration to hook up a provider.

An example implementation of the "Add" method

<!-- [!code-csharp[Main](samples/core/internals/Miscellaneous/Internals/WritingAProvider/EntityFrameworkCore.ProviderStarter/Extensions/MyProviderServiceCollectionExtensions.cs)] -->
````csharp
public static class MyProviderServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkMyProvider(this IServiceCollection services)
    {
        services.AddEntityFramework();

        services.TryAddEnumerable(ServiceDescriptor
            .Singleton<IDatabaseProvider, DatabaseProvider<MyDatabaseProviderServices, MyProviderOptionsExtension>>());

        services.TryAdd(new ServiceCollection()
            // singleton services
            .AddSingleton<MyModelSource>()
            .AddSingleton<MyValueGeneratorCache>()
            // scoped services
            .AddScoped<MyDatabaseProviderServices>()
            .AddScoped<MyDatabaseCreator>()
            .AddScoped<MyDatabase>()
            .AddScoped<MyEntityQueryableExpressionVisitorFactory>()
            .AddScoped<MyEntityQueryModelVisitorFactory>()
            .AddScoped<MyQueryContextFactory>()
            .AddScoped<MyTransactionManager>());

        return services;
    }
}
````

## Next Steps

With these two extensibility APIs now defined, users can now configure their "DbContext" to use your provider. To make your provider functional, you will need to implement other services.

Reading the source code of other providers is an excellent way to learn how to create a new EF provider. See [Database Providers](../../providers/index.md) for a list of current EF providers and to find links to their source code (if applicable).

`Microsoft.EntityFrameworkCore.Relational` includes an extensive library of services designed for relational providers. In many cases, these services need little or no modification to work for multiple relational databases.
