---
title: Design-time DbContext Creation - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/27/2017
ms.technology: entity-framework-core
---
Design-time DbContext Creation
==============================
Some of the EF Tools commands require a DbContext instance to be created at design time (for example, when running
Migrations commands). There are various ways the tools will try and do this.

From application services
-------------------------
If your startup project is an ASP.NET Core app, the tools will try to obtain the DbContext object from the application's
service provider. This is done by invoking `Program.BuildWebHost()` and accessing the `IWebHost.Services` property. Any
DbContext registered using `IServiceCollection.AddDbContext<TContext>()` can be found and created this way. This pattern
was [introduced in ASP.NET Core 2.0][1]

Using the default constructor
-----------------------------
If the DbContext can't be obtained from the application service provider, the tools will find the DbContext type inside
the project and try to create it using its default constructor.

From a design-time factory
--------------------------
You can also tell the tools how to create your DbContext by implementing `IDesignTimeDbContextFactory`. If a class
implementing this interface is found inside your project, the tools will bypass the other ways of creating the DbContext
and always use the factory at design time. This is especially useful if you need to configure the DbContext differently
for design time than at runtime.

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

> [!NOTE]
> The `args` parameter is currently unused. There is [an issue][2] tracking the ability to specify design-time arguments
> from the tools.

  [1]: https://docs.microsoft.com/aspnet/core/migration/1x-to-2x/#update-main-method-in-programcs
  [2]: https://github.com/aspnet/EntityFrameworkCore/issues/8332
