---
title: Design-time DbContext Creation - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/27/2017
uid: core/miscellaneous/cli/dbcontext-creation
---
Design-time DbContext Creation
==============================
Some of the EF Core Tools commands (for example, the [Migrations][1] commands) require a derived `DbContext` instance
to be created at design time in order to gather details about the application's entity types and how they map to a
database schema. In most cases, it is desirable that the `DbContext` thereby created is configured in a similar
way to how it would be [configured at run time][2].

There are various ways the tools try to create the `DbContext`:

From application services
-------------------------
If your startup project is an ASP.NET Core app, the tools try to obtain the DbContext object from the application's
service provider.

The tools first try to obtain the service provider by invoking `Program.BuildWebHost()` and accessing the
`IWebHost.Services` property.

> [!NOTE]
> When you create a new ASP.NET Core 2.0 application, this hook is included by default. In previous versions of EF Core
> and ASP.NET Core, the tools try to invoke `Startup.ConfigureServices` directly in order to obtain the application's
> service provider, but this pattern no longer works correctly in ASP.NET Core 2.0 applications. If you are upgrading an
> ASP.NET Core 1.x application to 2.0, you can [modify your `Program` class to follow the new pattern][3].

The `DbContext` itself and any dependencies in its constructor need to be registered as services in the application's
service provider. This can be easily achieved by having 
[a constructor on the `DbContext` that takes an instance of `DbContextOptions<TContext>` as an argument][4] and using the 
[`AddDbContext<TContext>` method][5].

Using a constructor with no parameters
--------------------------------------
If the DbContext can't be obtained from the application service provider, the tools look for the derived `DbContext`
type inside the project. Then they try to create an instance using a constructor with no parameters. This can be the
default constructor if the `DbContext` is configured using the [`OnConfiguring`][6] method.

From a design-time factory
--------------------------
You can also tell the tools how to create your DbContext by implementing the `IDesignTimeDbContextFactory<TContext>`
interface: If a class implementing this interface is found in either the same project as the derived `DbContext` or in 
the application's startup project, the tools bypass the other ways of creating the DbContext and use the 
design-time factory instead.

``` csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

> [!NOTE]
> The `args` parameter is currently unused. There is [an issue][7] tracking the ability to specify design-time arguments
> from the tools.

A design-time factory can be especially useful if you need to configure the DbContext differently for design time than
at run time, if the `DbContext` constructor takes additional parameters are not registered in DI, if you are not using
DI at all, or if for some reason you prefer not to have a `BuildWebHost` method in your ASP.NET Core application's `Main` class.

  [1]: xref:core/managing-schemas/migrations/index
  [2]: xref:core/miscellaneous/configuring-dbcontext
  [3]: https://docs.microsoft.com/aspnet/core/migration/1x-to-2x/#update-main-method-in-programcs
  [4]: xref:core/miscellaneous/configuring-dbcontext#constructor-argument
  [5]: xref:core/miscellaneous/configuring-dbcontext#using-dbcontext-with-dependency-injection
  [6]: xref:core/miscellaneous/configuring-dbcontext#onconfiguring
  [7]: https://github.com/aspnet/EntityFrameworkCore/issues/8332
