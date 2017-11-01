---
title: Design-time DbContext Creation - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/27/2017
ms.technology: entity-framework-core
---
Design-time DbContext Creation
==============================
Some of the EF Tools commands require a DbContext instance to be created at design time. There are various ways the
tools will try an do this.

From application services
-------------------------
If your startup project is an ASP.NET Core app, the tools will try to obtain the DbContext object from the application's
service provider. This is done by invoking `Program.BuildWebHost()` and accessing the `IWebHost.Services` property. Any
DbContext registered using `IServiceCollection.AddDbContext<TContext>()` can be found and created this way.

Using the default constructor
-----------------------------
If the DbContext can't be obtained from the application service provider, the tools will find any DbContext types inside
the project try to create it using the default constructor.

From a design-time factory
--------------------------
You can also tell the tools how your DbContext should be created by implementing `IDesignTimeDbContextFactory`. If a
class implementing this interface is found inside your project, the tools will bypass the other ways of creating the
DbContext and always use the factory at design time. This is especially useful if you need to configure the DbContext
differently for design time than at runtime.

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
