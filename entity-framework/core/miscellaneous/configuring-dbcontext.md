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

This article shows patterns for configuring a `DbContext` via a `DbContextOptions` to connect to a data store using a specific EF Core provider and optional behaviors.

While any pattern that provides the necessary configuration information to the `DbContext` can work at run-time, tools that require using a `DbContext` at design-time such as the [migrations](xref:core/managing-schemas/migrations/index) can only recognize a limited number of patterns.

## Configuring DbContextOptions

`DbContext` must have an instance of `DbContextOptions` in order to perform any work. The `DbContextOptions` can be supplied to the `DbContext` by overriding the `OnConfiguring` method or externally via a constructor argument.

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
> The base constructor of DbContext also accepts the non-generic version of `DbContextOptions`. Using the non-generic version is not recommended for applications with multiple context types.

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

Application code to initialize a `DbContext` with `OnConfiguring`:

``` csharp
using (var context = new BloggingContext())
{
  // do stuff
}
```

> [!TIP]
> This approach does not lend itself to testing, unless the tests target the full database.

## Using DbContext with dependency injection

EF supports using `DbContext` with a dependency injection container. Your DbContext type can be added to the service container by using `AddDbContext<TContext>`.

`AddDbContext` will make both your DbContext type, `TContext`, and `DbContextOptions<TContext>` available for injection from the service container.

See [more reading](#more-reading) below for information on dependency injection.

Adding the `Dbcontext` to dependency injection:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<BloggingContext>(options => options.UseSqlite("Data Source=blog.db"));
}
```

This requires adding a [constructor argument](#constructor-argument) to your DbContext type that accepts `DbContextOptions`.

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

## Design-time discovery and configuration

EF Core design-time tools such as [migrations](xref:core/managing-schemas/migrations/index) need to be able to discover and create a working instance of a `DbContext` type in order to gather details about the application's entity types and how they are mapped to the database schema. This process can work automatically as long as the tool can create the `DbContext` in a way that it will be configured similarly to how it would be configured at runt-time. Concretely, this translates to three  common patterns:

### Constructor without parameters

A `DbContext` with a constructor with no parameters, in which the `DbContextOptions` are supplied in the `OnConfiguring` method can be discovered and instantiated by design-time tools.

### Using dependency injection in ASP.NET Core applications

The `DbContext` has to itself be registered as a service and take dependencies in its constructor other services that can be resolved from the application's dependency injection container. One of the dependencies will typically be the `DbContextOptions<TContext>`, which together with the `DbContext` can be registered in DI using the `AddDbContext<TContext>()` method.

This pattern also requires the tool to obtain an instance of the application's dependency injection container at design-time. Starting with EF Core 2.0 and ASP.NET Core 2.0, the recommended pattern to achieve that is to have a static `Program.BuildWebHost` method to enable design-tools to access the application's service provider at design time.

If you create an ASP.NET Core 2.0 application, this hook is included in the default templates. If you are upgrading an ASP.NET Core 1.x application to 2.0, you will need to modify you `Program` class to resemble the following code:

``` csharp
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCoreDotNetCore.Application
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

In previous versions, the tools would try to invoke `Startup.ConfigureServices()` directly in order to access the application's service provider.

### Using IDesignTimeDbContextFactory<TContext>

As an alternative to the options above, you may provide an implementation of `IDesignTimeDbContextFactory<TContext>` to enable design-time services for context types that do not have a public default constructor and take additional parameters are not registered in DI, or if you are not using DI at all. EF Core tools can use this factory to create an instance of your DbContext.

Design-time services will automatically discover implementations of this interface that are in the same assembly as the derived context.

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
