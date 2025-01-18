---
title: DbContext Lifetime, Configuration, and Initialization - EF Core
description: Patterns for creating and managing DbContext instances with or without dependency injection
author: SamMonoRT
ms.date: 11/07/2020
uid: core/dbcontext-configuration/index
---

# DbContext Lifetime, Configuration, and Initialization

This article shows basic patterns for initialization and configuration of a <xref:Microsoft.EntityFrameworkCore.DbContext> instance.

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

## The DbContext lifetime

The lifetime of a `DbContext` begins when the instance is created and ends when the instance is [disposed](/dotnet/standard/garbage-collection/unmanaged). A `DbContext` instance is designed to be used for a _single_ [unit-of-work](https://www.martinfowler.com/eaaCatalog/unitOfWork.html). This means that the lifetime of a `DbContext` instance is usually very short.

> [!TIP]
> To quote Martin Fowler from the link above, "A Unit of Work keeps track of everything you do during a business transaction that can affect the database. When you're done, it figures out everything that needs to be done to alter the database as a result of your work."

A typical unit-of-work when using Entity Framework Core (EF Core) involves:

- Creation of a `DbContext` instance
- Tracking of entity instances by the context. Entities become tracked by
  - Being [returned from a query](xref:core/querying/tracking)
  - Being [added or attached to the context](xref:core/saving/disconnected-entities)
- Changes are made to the tracked entities as needed to implement the business rule
- <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*> is called. EF Core detects the changes made and writes them to the database.
- The `DbContext` instance is disposed

> [!IMPORTANT]
>
> - It is important to dispose the <xref:Microsoft.EntityFrameworkCore.DbContext> after use. This ensures any:
>   - Unmanaged resources are freed.
>   - Events or other hooks are unregistered. Unregistering prevents memory leaks when the instance remains referenced.
> - [DbContext is **Not thread-safe**](#avoiding-dbcontext-threading-issues). Don't share contexts between threads. Make sure to [await](/dotnet/csharp/language-reference/operators/await) all async calls before continuing to use the context instance.
> - An <xref:System.InvalidOperationException> thrown by EF Core code can put the context into an unrecoverable state. Such exceptions indicate a program error and are not designed to be recovered from.

## DbContext in dependency injection for ASP.NET Core

In many web applications, each HTTP request corresponds to a single unit-of-work. This makes tying the context lifetime to that of the request a good default for web applications.

ASP.NET Core applications are [configured using dependency injection](/aspnet/core/fundamentals/startup). EF Core can be added to this configuration using <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> in `Program.cs`. For example:

<!--
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer("name=DefaultConnection"));
        }
-->

[!code-csharp[snippet_1](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp9/Program9.cs?name=snippet_1)]

The preceding code registers `ApplicationDbContext`, a subclass of `DbContext`, as a scoped service in the ASP.NET Core app service provider. The service provider is also known as the dependency injection container. The context is configured to use the SQL Server database provider and reads the connection string from [ASP.NET Core configuration](xref:core/miscellaneous/connection-strings#aspnet-core).

The `ApplicationDbContext` class must expose a public constructor with a `DbContextOptions<ApplicationDbContext>` parameter. This is how context configuration from `AddDbContext` is passed to the `DbContext`. For example:

<!--
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
-->

[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp/ApplicationDbContext.cs?name=ApplicationDbContext)]

`ApplicationDbContext` can be used in ASP.NET Core controllers or other services through constructor injection:

<!--
    public class MyController
    {
        private readonly ApplicationDbContext _context;

        public MyController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
-->
[!code-csharp[MyController](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp/Controllers/MyController.cs?name=MyController)]

The final result is an `ApplicationDbContext` instance created for each request and passed to the controller to perform a unit-of-work before being disposed when the request ends.

Read further in this article to learn more about configuration options. See [Dependency injection in ASP.NET Core](/aspnet/core/fundamentals/dependency-injection) for more information.

<!-- See also [Using Dependency Injection](TODO) for advanced dependency injection configuration with EF Core. -->

## Basic DbContext initialization with 'new'

`DbContext` instances can be constructed with `new` in C#. Configuration can be performed by overriding the `OnConfiguring` method, or by passing options to the constructor. For example:

<!--
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithNew/ApplicationDbContext.cs?name=ApplicationDbContext)]

This pattern also makes it easy to pass configuration like the connection string via the `DbContext` constructor. For example:

<!--
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithNewAndArgs/ApplicationDbContext.cs?name=ApplicationDbContext)]

Alternately, `DbContextOptionsBuilder` can be used to create a `DbContextOptions` object that is then passed to the `DbContext` constructor. This allows a `DbContext` configured for dependency injection to also be constructed explicitly. For example, when using `ApplicationDbContext` defined for ASP.NET Core web apps above:

<!--
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp/ApplicationDbContext.cs?name=ApplicationDbContext)]

The `DbContextOptions` can be created and the constructor can be called explicitly:

<!--
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test")
                .Options;

            using var context = new ApplicationDbContext(contextOptions);
-->
[!code-csharp[UseNewForWebApp](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp/UseNewForWebApp.cs?name=UseNewForWebApp)]

## Use a DbContext factory

Some application types (e.g. [ASP.NET Core Blazor](/aspnet/core/blazor/)) use dependency injection but do not create a service scope that aligns with the desired `DbContext` lifetime. Even where such an alignment does exist, the application may need to perform multiple units-of-work within this scope. For example, multiple units-of-work within a single HTTP request.

In these cases, <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory*> can be used to register a factory for creation of `DbContext` instances. For example:

<!--
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test"));
        }
-->
[!code-csharp[ConfigureServices](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithContextFactory/FactoryServicesExample.cs?name=ConfigureServices)]

The `ApplicationDbContext` class must expose a public constructor with a `DbContextOptions<ApplicationDbContext>` parameter. This is the same pattern as used in the traditional ASP.NET Core section above.

<!--
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WebApp/ApplicationDbContext.cs?name=ApplicationDbContext)]

The `DbContextFactory` factory can then be used in other services through constructor injection. For example:

<!--
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public MyController(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
-->
[!code-csharp[Construct](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithContextFactory/MyController.cs?name=Construct)]

The injected factory can then be used to construct DbContext instances in the service code. For example:

<!--
        public void DoSomething()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                // ...
            }
        }
-->
[!code-csharp[DoSomething](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithContextFactory/MyController.cs?name=DoSomething)]

Notice that the `DbContext` instances created in this way are **not** managed by the application's service provider and therefore must be disposed by the application.

See [ASP.NET Core Blazor Server with Entity Framework Core](/aspnet/core/blazor/blazor-server-ef-core) for more information on using EF Core with Blazor.

## DbContextOptions

The starting point for all `DbContext` configuration is <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder>. There are three ways to get this builder:

- In `AddDbContext` and related methods
- In `OnConfiguring`
- Constructed explicitly with `new`

Examples of each of these are shown in the preceding sections. The same configuration can be applied regardless of where the builder comes from. In addition, `OnConfiguring` is always called regardless of how the context is constructed. This means `OnConfiguring` can be used to perform additional configuration even when `AddDbContext` is being used.

### Configuring the database provider

Each `DbContext` instance must be configured to use one and only one database provider. (Different instances of a `DbContext` subtype can be used with different database providers, but a single instance must only use one.) A database provider is configured using a specific `Use*` call. For example, to use the SQL Server database provider:

<!--
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/WithNew/ApplicationDbContext.cs?name=ApplicationDbContext)]

These `Use*` methods are extension methods implemented by the database provider. This means that the database provider NuGet package must be installed before the extension method can be used.

> [!TIP]
> EF Core database providers make extensive use of [extension methods](/dotnet/csharp/programming-guide/classes-and-structs/extension-methods). If the compiler indicates that a method cannot be found, then make sure that the provider's NuGet package is installed and that you have `using Microsoft.EntityFrameworkCore;` in your code.

The following table contains examples for common database providers.

| Database system            | Example configuration                        | NuGet package                                                                                                      |
|:---------------------------|----------------------------------------------|--------------------------------------------------------------------------------------------------------------------|
| SQL Server or Azure SQL    | `.UseSqlServer(connectionString)`            | [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/) |
| Azure Cosmos DB            | `.UseCosmos(connectionString, databaseName)` | [Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Cosmos/)       |
| SQLite                     | `.UseSqlite(connectionString)`               | [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)       |
| EF Core in-memory database | `.UseInMemoryDatabase(databaseName)`         | [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/)   |
| PostgreSQL*                | `.UseNpgsql(connectionString)`               | [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/)     |
| MySQL/MariaDB*             | `.UseMySql(connectionString)`                | [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)               |
| Oracle*                    | `.UseOracle(connectionString)`               | [Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Oracle.EntityFrameworkCore/)                           |

*These database providers are not shipped by Microsoft. See [Database Providers](xref:core/providers/index) for more information about database providers.

> [!WARNING]
> The EF Core in-memory database is not designed for production use. In addition, it may not be the best choice even for testing. See [Testing Code That Uses EF Core](xref:core/testing/index) for more information.

See [Connection Strings](xref:core/miscellaneous/connection-strings) for more information on using connection strings with EF Core.

Optional configuration specific to the database provider is performed in an additional provider-specific builder. For example, using <xref:Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder.EnableRetryOnFailure*> to configure retries for connection resiliency when connecting to Azure SQL:

<!--
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Test",
                    providerOptions =>
                        {
                            providerOptions.EnableRetryOnFailure();
                        });
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/AdditionalProviderConfiguration/ApplicationDbContext.cs?name=ApplicationDbContext)]

> [!TIP]
> The same database provider is used for SQL Server and Azure SQL. However, it is recommended that [connection resiliency](xref:core/miscellaneous/connection-resiliency) be used when connecting to SQL Azure.

See [Database Providers](xref:core/providers/index) for more information on provider-specific configuration.

### Other DbContext configuration

Other `DbContext` configuration can be chained either before or after (it makes no difference which) the `Use*` call. For example, to turn on sensitive-data logging:

<!--
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/AdditionalConfiguration/ApplicationDbContext.cs?name=ApplicationDbContext)]

The following table contains examples of common methods called on `DbContextOptionsBuilder`.

| DbContextOptionsBuilder method                                                             | What it does                                                | Learn more
|:-------------------------------------------------------------------------------------------|-------------------------------------------------------------|--------------
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseQueryTrackingBehavior*>   | Sets the default tracking behavior for queries              | [Query Tracking Behavior](xref:core/querying/tracking)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.LogTo*>                      | A simple way to get EF Core logs                            | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseLoggerFactory*>           | Registers an `Microsoft.Extensions.Logging` factory         | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.EnableSensitiveDataLogging*> | Includes application data in exceptions and logging         | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.EnableDetailedErrors*>       | More detailed query errors (at the expense of performance)  | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.ConfigureWarnings*>          | Ignore or throw for warnings and other events               | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.AddInterceptors*>            | Registers EF Core interceptors                              | [Logging, Events, and Diagnostics](xref:core/logging-events-diagnostics/index)
| <xref:Microsoft.EntityFrameworkCore.ProxiesExtensions.UseLazyLoadingProxies*>            | Use dynamic proxies for lazy-loading                        | [Lazy Loading](xref:core/querying/related-data/lazy)
| <xref:Microsoft.EntityFrameworkCore.ProxiesExtensions.UseChangeTrackingProxies*>         | Use dynamic proxies for change-tracking                     | Coming soon...

> [!NOTE]
> <xref:Microsoft.EntityFrameworkCore.ProxiesExtensions.UseLazyLoadingProxies*> and <xref:Microsoft.EntityFrameworkCore.ProxiesExtensions.UseChangeTrackingProxies*> are extension methods from the [Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies/) NuGet package. This kind of ".UseSomething()" call is the recommended way to configure and/or use EF Core extensions contained in other packages.

### `DbContextOptions` versus `DbContextOptions<TContext>`

Most `DbContext` subclasses that accept a `DbContextOptions` should use the [generic](/dotnet/csharp/programming-guide/generics/) `DbContextOptions<TContext>` variation. For example:

<!--
    public sealed class SealedApplicationDbContext : DbContext
    {
        public SealedApplicationDbContext(DbContextOptions<SealedApplicationDbContext> contextOptions)
            : base(contextOptions)
        {
        }
    }
-->
[!code-csharp[SealedApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/InheritDbContext/ApplicationDbContext.cs?name=SealedApplicationDbContext)]

This ensures that the correct options for the specific `DbContext` subtype are resolved from dependency injection, even when multiple `DbContext` subtypes are registered.

> [!TIP]
> Your DbContext does not need to be sealed, but sealing is best practice to do so for classes not designed to be inherited from.

However, if the `DbContext` subtype is itself intended to be inherited from, then it should expose a protected constructor taking a non-generic `DbContextOptions`. For example:

<!--
    public abstract class ApplicationDbContextBase : DbContext
    {
        protected ApplicationDbContextBase(DbContextOptions contextOptions)
            : base(contextOptions)
        {
        }
    }
-->
[!code-csharp[ApplicationDbContextBase](../../../samples/core/Miscellaneous/ConfiguringDbContext/InheritDbContext/ApplicationDbContext.cs?name=ApplicationDbContextBase)]

This allows multiple concrete subclasses to call this base constructor using their different generic `DbContextOptions<TContext>` instances. For example:

<!--
    public sealed class ApplicationDbContext1 : ApplicationDbContextBase
    {
        public ApplicationDbContext1(DbContextOptions<ApplicationDbContext1> contextOptions)
            : base(contextOptions)
        {
        }
    }

    public sealed class ApplicationDbContext2 : ApplicationDbContextBase
    {
        public ApplicationDbContext2(DbContextOptions<ApplicationDbContext2> contextOptions)
            : base(contextOptions)
        {
        }
    }
-->
[!code-csharp[ApplicationDbContext12](../../../samples/core/Miscellaneous/ConfiguringDbContext/InheritDbContext/ApplicationDbContext.cs?name=ApplicationDbContext12)]

Notice that this is exactly the same pattern as when inheriting from `DbContext` directly. That is, the `DbContext` constructor itself accepts a non-generic `DbContextOptions` for this reason.

A `DbContext` subclass intended to be both instantiated and inherited from should expose both forms of constructor. For example:

<!--
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        protected ApplicationDbContext(DbContextOptions contextOptions)
            : base(contextOptions)
        {
        }
    }
-->
[!code-csharp[ApplicationDbContext](../../../samples/core/Miscellaneous/ConfiguringDbContext/InheritDbContext/ApplicationDbContext.cs?name=ApplicationDbContext)]

## Design-time DbContext configuration

EF Core design-time tools such as those for [EF Core migrations](xref:core/managing-schemas/migrations/index) need to be able to discover and create a working instance of a `DbContext` type in order to gather details about the application's entity types and how they map to a database schema. This process can be automatic as long as the tool can easily create the `DbContext` in such a way that it will be configured similarly to how it would be configured at run-time.

While any pattern that provides the necessary configuration information to the `DbContext` can work at run-time, tools that require using a `DbContext` at design-time can only work with a limited number of patterns. These are covered in more detail in [Design-Time Context Creation](xref:core/cli/dbcontext-creation).

## Avoiding DbContext threading issues

Entity Framework Core does not support multiple parallel operations being run on the same `DbContext` instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads. Therefore, always `await` async calls immediately, or use separate `DbContext` instances for operations that execute in parallel.

When EF Core detects an attempt to use a `DbContext` instance concurrently, you'll see an `InvalidOperationException` with a message like this:

> A second operation started on this context before a previous operation completed. This is usually caused by different threads using the same instance of DbContext, however instance members are not guaranteed to be thread safe.

When concurrent access goes undetected, it can result in undefined behavior, application crashes and data corruption.

There are common mistakes that can inadvertently cause concurrent access on the same `DbContext` instance:

### Asynchronous operation pitfalls

Asynchronous methods enable EF Core to initiate operations that access the database in a non-blocking way. But if a caller does not await the completion of one of these methods, and proceeds to perform other operations on the `DbContext`, the state of the `DbContext` can be, (and very likely will be) corrupted.

Always await EF Core asynchronous methods immediately.

### Implicitly sharing DbContext instances via dependency injection

The [`AddDbContext`](/dotnet/api/microsoft.extensions.dependencyinjection.entityframeworkservicecollectionextensions.adddbcontext) extension method registers `DbContext` types with a [scoped lifetime](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) by default.

This is safe from concurrent access issues in most ASP.NET Core applications because there is only one thread executing each client request at a given time, and because each request gets a separate dependency injection scope (and therefore a separate `DbContext` instance). For Blazor Server hosting model, one logical request is used for maintaining the Blazor user circuit, and thus only one scoped DbContext instance is available per user circuit if the default injection scope is used.

Any code that explicitly executes multiple threads in parallel should ensure that `DbContext` instances aren't ever accessed concurrently.

Using dependency injection, this can be achieved by either registering the context as scoped, and creating scopes (using `IServiceScopeFactory`) for each thread, or by registering the `DbContext` as transient (using the overload of `AddDbContext` which takes a `ServiceLifetime` parameter).

## More reading

- Read [Dependency Injection](/aspnet/core/fundamentals/dependency-injection) to learn more about using DI.
- Read [Testing](xref:core/testing/index) for more information.
