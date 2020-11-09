---
title: Configuring a DbContext - EF Core
description: Strategies for configuring DbContexts with Entity Framework Core
author: ajcvickers
ms.date: 10/27/2016
uid: core/dbcontext-configuration/index
---
# Configuring a DbContext

This article shows basic patterns for configuring a `DbContext` via a `DbContextOptions` to connect to a database using a specific EF Core provider and optional behaviors.

## Design-time DbContext configuration

EF Core design-time tools such as [migrations](xref:core/managing-schemas/migrations/index) need to be able to discover and create a working instance of a `DbContext` type in order to gather details about the application's entity types and how they map to a database schema. This process can be automatic as long as the tool can easily create the `DbContext` in such a way that it will be configured similarly to how it would be configured at run-time.

While any pattern that provides the necessary configuration information to the `DbContext` can work at run-time, tools that require using a `DbContext` at design-time can only work with a limited number of patterns. These are covered in more detail in the [Design-Time Context Creation](xref:core/cli/dbcontext-creation) section.

## Configuring DbContextOptions

`DbContext` must have an instance of `DbContextOptions` in order to perform any work. The `DbContextOptions` instance carries configuration information such as:

- The database provider to use, typically selected by invoking a method such as `UseSqlServer` or `UseSqlite`. These extension methods require the corresponding provider package, such as `Microsoft.EntityFrameworkCore.SqlServer` or `Microsoft.EntityFrameworkCore.Sqlite`. The methods are defined in the `Microsoft.EntityFrameworkCore` namespace.
- Any necessary connection string or identifier of the database instance, typically passed as an argument to the provider selection method mentioned above
- Any provider-level optional behavior selectors, typically also chained inside the call to the provider selection method
- Any general EF Core behavior selectors, typically chained after or before the provider selector method

The following example configures the `DbContextOptions` to use the SQL Server provider, a connection contained in the `connectionString` variable, a provider-level command timeout, and an EF Core behavior selector that makes all queries executed in the `DbContext` [no-tracking](xref:core/querying/tracking#no-tracking-queries) by default:

```csharp
optionsBuilder
    .UseSqlServer(connectionString, providerOptions=>providerOptions.CommandTimeout(60))
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
```

> [!NOTE]
> Provider selector methods and other behavior selector methods mentioned above are extension methods on `DbContextOptions` or provider-specific option classes. In order to have access to these extension methods you may need to have a namespace (typically `Microsoft.EntityFrameworkCore`) in scope and include additional package dependencies in the project.

The `DbContextOptions` can be supplied to the `DbContext` by overriding the `OnConfiguring` method or externally via a constructor argument.

If both are used, `OnConfiguring` is applied last and can overwrite options supplied to the constructor argument.

### Constructor argument

Your constructor can simply accept a `DbContextOptions` as follows:

```csharp
public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options)
        : base(options)
    { }

    public DbSet<Blog> Blogs { get; set; }
}
```

> [!TIP]
> The base constructor of DbContext also accepts the non-generic version of `DbContextOptions`, but using the non-generic version is not recommended for applications with multiple context types.

Your application can now pass the `DbContextOptions` when instantiating a context, as follows:

```csharp
var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
optionsBuilder.UseSqlite("Data Source=blog.db");

using (var context = new BloggingContext(optionsBuilder.Options))
{
  // do stuff
}
```

### OnConfiguring

You can also initialize the `DbContextOptions` within the context itself. While you can use this technique for basic configuration, you will typically still need to get certain configuration details from the outside, e.g. a database connection string. This can be done with a configuration API or any other means.

To initialize `DbContextOptions` within the context, override the `OnConfiguring` method and call the methods on the provided `DbContextOptionsBuilder`:

```csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=blog.db");
    }
}
```

An application can simply instantiate such a context without passing anything to its constructor:

```csharp
using (var context = new BloggingContext())
{
  // do stuff
}
```

> [!TIP]
> This approach does not lend itself to testing, unless the tests target the full database.

### Using DbContext with dependency injection

EF Core supports using `DbContext` with a dependency injection container. Your DbContext type can be added to the service container by using the `AddDbContext<TContext>` method.

`AddDbContext<TContext>` will make both your DbContext type, `TContext`, and the corresponding `DbContextOptions<TContext>` available for injection from the service container.

See [more reading](#more-reading) below for additional information on dependency injection.

Adding the `DbContext` to dependency injection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<BloggingContext>(options => options.UseSqlite("Data Source=blog.db"));
}
```

This requires adding a [constructor argument](#constructor-argument) to your DbContext type that accepts `DbContextOptions<TContext>`.

Context code:

```csharp
public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options)
      :base(options)
    { }

    public DbSet<Blog> Blogs { get; set; }
}
```

Application code (in ASP.NET Core):

```csharp
public class MyController
{
    private readonly BloggingContext _context;

    public MyController(BloggingContext context)
    {
      _context = context;
    }

    ...
}
```

Application code (using ServiceProvider directly, less common):

```csharp
using (var context = serviceProvider.GetService<BloggingContext>())
{
  // do stuff
}

var options = serviceProvider.GetService<DbContextOptions<BloggingContext>>();
```

## Avoiding DbContext threading issues

Entity Framework Core does not support multiple parallel operations being run on the same `DbContext` instance. This includes both parallel execution of async queries and any explicit concurrent use from multiple threads. Therefore, always `await` async calls immediately, or use separate `DbContext` instances for operations that execute in parallel.

When EF Core detects an attempt to use a `DbContext` instance concurrently, you'll see an `InvalidOperationException` with a message like this:

> A second operation started on this context before a previous operation completed. This is usually caused by different threads using the same instance of DbContext, however instance members are not guaranteed to be thread safe.

When concurrent access goes undetected, it can result in undefined behavior, application crashes and data corruption.

There are common mistakes that can inadvertently cause concurrent access on the same `DbContext` instance:

### Forgetting to await the completion of an asynchronous operation before starting any other operation on the same DbContext

Asynchronous methods enable EF Core to initiate operations that access the database in a non-blocking way. But if a caller does not await the completion of one of these methods, and proceeds to perform other operations on the `DbContext`, the state of the `DbContext` can be, (and very likely will be) corrupted.

Always await EF Core asynchronous methods immediately.

### Implicitly sharing DbContext instances across multiple threads via dependency injection

The [`AddDbContext`](/dotnet/api/microsoft.extensions.dependencyinjection.entityframeworkservicecollectionextensions.adddbcontext) extension method registers `DbContext` types with a [scoped lifetime](/aspnet/core/fundamentals/dependency-injection#service-lifetimes) by default.

This is safe from concurrent access issues in most ASP.NET Core applications because there is only one thread executing each client request at a given time, and because each request gets a separate dependency injection scope (and therefore a separate `DbContext` instance). For Blazor Server hosting model, one logical request is used for maintaining the Blazor user circuit, and thus only one scoped DbContext instance is available per user circuit if the default injection scope is used.

Any code that explicitly executes multiple threads in parallel should ensure that `DbContext` instances aren't ever accessed concurrently.

Using dependency injection, this can be achieved by either registering the context as scoped, and creating scopes (using `IServiceScopeFactory`) for each thread, or by registering the `DbContext` as transient (using the overload of `AddDbContext` which takes a `ServiceLifetime` parameter).

## More reading

- Read [Dependency Injection](/aspnet/core/fundamentals/dependency-injection) to learn more about using DI.
- Read [Testing](xref:core/testing/index) for more information.
