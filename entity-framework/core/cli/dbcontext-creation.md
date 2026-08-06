---
title: Design-time DbContext Creation - EF Core
description: Strategies for creating a design-time DbContext with Entity Framework Core
author: SamMonoRT
ms.date: 08/05/2026
uid: core/cli/dbcontext-creation
---
# Design-time DbContext Creation

Some of the EF Core Tools commands (for example, the [Migrations][1] commands) require a derived `DbContext` instance to be created at design time in order to gather details about the application's entity types and how they map to a database schema. In most cases, it is desirable that the `DbContext` thereby created is configured in a similar way to how it would be [configured at run time][2].

There are various ways the tools try to create the `DbContext`. If an [`IDesignTimeDbContextFactory<TContext>`](#from-a-design-time-factory) is found, the tools use it instead of the other creation patterns. A design-time factory is the recommended pattern for a [separate migrations project](xref:core/managing-schemas/migrations/projects) and for applications whose startup project is platform-specific.

## From application services

If your startup project uses the [ASP.NET Core Web Host][3] or [.NET Core Generic Host][4], the tools try to obtain the DbContext object from the application's service provider.

The tools first try to obtain the service provider by invoking `Program.CreateHostBuilder()`, calling `Build()`, then accessing the `Services` property.

[!code-csharp[Main](../../../samples/core/Miscellaneous/CommandLine/ApplicationService.cs#ApplicationService)]

> [!NOTE]
> When you create a new ASP.NET Core application, this hook is included by default.

The `DbContext` itself and any dependencies in its constructor need to be registered as services in the application's service provider. This can be easily achieved by having [a constructor on the `DbContext` that takes an instance of `DbContextOptions<TContext>` as an argument][5] and using the [`AddDbContext<TContext>` method][6].

## Using a constructor with no parameters

If the DbContext can't be obtained from the application service provider, the tools look for the derived `DbContext` type inside the project. Then they try to create an instance using a constructor with no parameters. This can be the default constructor if the `DbContext` is configured using the [`OnConfiguring`][7] method.

## From a design-time factory

You can also tell the tools how to create your DbContext by implementing the <xref:Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory`1?displayProperty=nameWithType> interface: If a class implementing this interface is found in either the same project as the derived `DbContext` or in the application's startup project, the tools bypass the other ways of creating the DbContext and use the design-time factory instead.

[!code-csharp[Main](../../../samples/core/Miscellaneous/CommandLine/BloggingContextFactory.cs#BloggingContextFactory)]

A design-time factory can be especially useful if the context constructor has dependencies that aren't registered in DI, if you aren't using DI, or if the application startup project can't be executed by the tools.

Configure the same provider, model options, and migrations assembly at design time that the application uses at run time. Otherwise, the tools can scaffold migrations for a model that differs from the model used by the application. Don't assume that an optional command-line argument is present; validate arguments and provide a suitable development default or configuration source.

## Args

Both <xref:Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory`1.CreateDbContext*?displayProperty=nameWithType> and `Program.CreateHostBuilder` accept command line arguments.

You can specify these arguments from the tools:

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update -- --environment Production
```

The `--` token directs `dotnet ef` to treat everything that follows as an argument and not try to parse them as options. Any extra arguments not used by `dotnet ef` are forwarded to the app.

### [Visual Studio](#tab/vs)

```powershell
Update-Database -Args '--environment Production'
```

***

  [1]: xref:core/managing-schemas/migrations/index
  [2]: xref:core/dbcontext-configuration/index
  [3]: /aspnet/core/fundamentals/host/web-host
  [4]: /aspnet/core/fundamentals/host/generic-host
  [5]: xref:core/dbcontext-configuration/index
  [6]: xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*
  [7]: xref:core/dbcontext-configuration/index#basic-dbcontext-initialization-with-new
