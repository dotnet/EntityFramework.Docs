---
title: Configuring a DbContext - EF Core
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: d7a22b5a-4c5b-4e3b-9897-4d7320fcd13f
ms.technology: entity-framework-core

uid: core/miscellaneous/configuring-dbcontext
---
# Configuring a DbContext

This article shows basic patterns for configuring a `DbContext` via a `DbContextOptions` to connect to a database using a specific EF Core provider and optional behaviors.

While any pattern that provides the necessary configuration information to the `DbContext` can work at run-time, tools that require using a `DbContext` at design-time such as the [migrations](xref:core/managing-schemas/migrations/index) can only recognize a limited number of patterns.

## Configuring DbContextOptions

`DbContext` must have an instance of `DbContextOptions` in order to perform any work. The `DbContextOptions` instance carries configuration information such as:

- The database provider to use, typically selected by invoking a method such as `UseSqlServer` or `UseSqlite`
- Any necessary connection string or identifier of the database instance, typically passed as an argument to the provider selection method mentioned above
- Any provider-level optional behavior selectors, typically also chained inside the call to the provider selection method
- Any general EF Core behavior selectors, typically chained after or before the provider selector method

The following example configures the `DbContextOptions` to use the SQL Server provider, a connection contained in the connectionString variable, a provider-level command timeout, and an EF Core behavior selector that makes all queries executed in the `DbContext` [no-tracking](xref:core/querying/tracking#no-tracking-queries) by default:

> [!NOTE]  
> Provider selector methods and other behavior selector methods mentioned above are extension methods on `DbContextOptions` or provider-specific option classes. In order to have access to these methods you may need to have a namespace (typically `Microsoft.EntityFrameworkCore`) in scope and additional package dependencies in the project.


``` csharp
optionsBuilder
    .UseSqlServer(connectionString, providerOptions=>providerOptions.CommandTimeout(60))
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
```

The `DbContextOptions` can be supplied to the `DbContext` by overriding the `OnConfiguring` method or externally via a constructor argument.

If both are used, `OnConfiguring` is applied last and can overwrite options supplied to the constructor argument.

### Constructor argument

Context code with constructor:

``` csharp
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

Application code to initialize from constructor argument:

``` csharp
var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
optionsBuilder.UseSqlite("Data Source=blog.db");

using (var context = new BloggingContext(optionsBuilder.Options))
{
  // do stuff
}
```

### OnConfiguring

Context code with `OnConfiguring`:

``` csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=blog.db");
    }
}
```

Application code to initialize a `DbContext` that uses `OnConfiguring`:

``` csharp
using (var context = new BloggingContext())
{
  // do stuff
}
```

> [!TIP]
> This approach does not lend itself to testing, unless the tests target the full database.

## Using DbContext with dependency injection

EF Core supports using `DbContext` with a dependency injection container. Your DbContext type can be added to the service container by using the `AddDbContext<TContext>` method.

`AddDbContext<TContext>` will make both your DbContext type, `TContext`, and the corresponding `DbContextOptions<TContext>` available for injection from the service container.

See [more reading](#more-reading) below for additional information on dependency injection.

Adding the `Dbcontext` to dependency injection:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<BloggingContext>(options => options.UseSqlite("Data Source=blog.db"));
}
```

This requires adding a [constructor argument](#constructor-argument) to your DbContext type that accepts `DbContextOptions<TContext>`.

Context code:

``` csharp
public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options)
      :base(options)
    { }

    public DbSet<Blog> Blogs { get; set; }
}
```

Application code (in ASP.NET Core):

``` csharp
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

``` csharp
using (var context = serviceProvider.GetService<BloggingContext>())
{
  // do stuff
}

var options = serviceProvider.GetService<DbContextOptions<BloggingContext>>();
```

## Design-time DbContext discovery and configuration

EF Core design-time tools such as [migrations](xref:core/managing-schemas/migrations/index) need to be able to discover and create a working instance of a `DbContext` type in order to gather details about the application's entity types and how they map to a database schema. This process can work automatically as long as the tool can easily create the `DbContext` in such a way that it will be configured similarly to how it would be configured at runt-time. Concretely, this translates to three  common patterns:

### Constructor without parameters

Design-time tools can discover and instantiate a `DbContext` with a constructor with no parameters, in which the `DbContextOptions` will be usually supplied in the [`OnConfiguring`](#OnConfiguring) method.

### Using dependency injection in ASP.NET Core applications

The `DbContext` has to itself be registered as a service and any dependencies in its constructor need to be services that can be resolved from the application's dependency injection container. One of the dependencies will typically be the `DbContextOptions<TContext>`, which together with the `DbContext` can be registered in DI using the `AddDbContext<TContext>` method.

This pattern also requires tools to be able to obtain an instance of the application's dependency injection container at design-time. Starting with EF Core 2.0 and ASP.NET Core 2.0, the recommended pattern to achieve that is to have a static `Program.BuildWebHost` method as follows:

``` csharp
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MyWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
```

When you create an ASP.NET Core 2.0 application, this hook is included from the default templates. If you are upgrading an ASP.NET Core 1.x application to 2.0, you will need to modify your `Program` class to resemble the code above.

In previous versions of EF Core and ASP.NET Core, the tools would try to invoke `Startup.ConfigureServices` directly in order to access the application's service provider, but this pattern no longer works correctly in ASP.NET Core 2.0 applications.

### Using IDesignTimeDbContextFactory<TContext>

As an alternative to the options above, you may provide an implementation of `IDesignTimeDbContextFactory<TContext>` to enable design-time services. This is useful for context types that do not have a public default constructor and take additional parameters are not registered in DI, if you are not using DI at all, or if for some reason you prefer not to have a `BuildWebHost` method in your ASP.NET Core application's `Main` class.

Design-time services will automatically discover implementations of this interface that are in the same assembly as the derived context, and use the factory to create the instance at design-time.

Example:

``` csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MyProject
{
    public class BloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();
            optionsBuilder.UseSqlite("Data Source=blog.db");

            return new BloggingContext(optionsBuilder.Options);
        }
    }
}
```

## More reading

* Read [Getting Started on ASP.NET Core](../get-started/aspnetcore/index.md) for more information on using EF with ASP.NET Core.
* Read [Dependency Injection](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html) to learn more about using DI.
* Read [Testing](testing/index.md) for more information.
