---
title: EF Core | Getting Started on .NET Core - New database | Microsoft Docs
author: rick-anderson
ms.author: riande
ms.author2: tdykstra
description: Getting started with .NET Core using Entity Framework Core
keywords: .NET Core, Entity Framework Core, VS Code, Visual Studio Code, Mac, Linux
ms.date: 04/05/2017
ms.assetid: 099d179e-dd7b-4755-8f3c-fcde914bf50b
ms.technology: entity-framework-core
uid: core/get-started/netcore/new-db-sqlite
---

# Getting Started with EF Core on .NET Core Console App with a New database

In this walkthrough, you will create a .NET Core console app that performs basic data access against a SQLite database using Entity Framework Core. You will use migrations to create the database from your model. See [ASP.NET Core - New database](xref:core/get-started/aspnetcore/new-db) for a Visual Studio version using ASP.NET Core MVC.

> [!NOTE] The [.NET Core SDK](https://www.microsoft.com/net/download/core) 1.1.x no longer supports `project.json` or Visual Studio 2015. We recommend you [migrate from project.json to csproj](https://docs.microsoft.com/dotnet/articles/core/migration/). If you are using Visual Studio, we recommend you migrate to [Visual Studio 2017](https://www.visualstudio.com/downloads/).
> [!TIP] You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/NetCore/ConsoleApp.SQLite) on GitHub.

## Prerequisites

The following prerequisites are needed to complete this walkthrough:
* An operating system that supports .NET Core.
* [The .NET Core SDK](https://www.microsoft.com/net/core) 1.1 and later.

## Create a new project

* Create a new `ConsoleApp.SQLite` folder for your project and use the `dotnet` command to populate it with a .NET Core app.

``` console
mkdir ConsoleApp.SQLite
cd ConsoleApp.SQLite/
dotnet new console
```

## Install Entity Framework Core

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see [Database Providers](../../providers/index.md).

* Install Microsoft.EntityFrameworkCore.Sqlite and Microsoft.EntityFrameworkCore.Design

``` console
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

*  Manually edit `ConsoleApp.SQLite.csproj` to add a DotNetCliToolReference to Microsoft.EntityFrameworkCore.Tools.DotNet

 Note: A future version of `dotnet` will support DotNetCliToolReferences via `dotnet add tool`

`ConsoleApp.SQLite.csproj` should now contain the following:

[!code[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/ConsoleApp.SQLite.csproj)]

 Note: The version numbers used above were correct at the time of publishing.

*  Run `dotnet restore` to install the new packages.

## Create the model

Define a context and entity classes that make up your model.

* Create a new *Model.cs* file with the following contents.

[!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs)]

Tip: In a real application you would put each class in a separate file and put the connection string in a configuration file. To keep the tutorial simple, we are putting everything in one file.

## Create the database

Once you have a model, you can use [migrations](https://docs.microsoft.com/aspnet/core/data/ef-mvc/migrations#introduction-to-migrations) to create a database.

* Run `dotnet ef migrations add InitialCreate` to scaffold a migration and create the initial set of tables for the model.
* Run `dotnet ef database update` to apply the new migration to the database. This command creates the database before applying migrations.

> [!NOTE] When using relative paths with SQLite, the path will be relative to the application's main assembly. In this sample, the main binary is `bin/Debug/netcoreapp1.1/ConsoleApp.SQLite.dll`, so the SQLite database will be in `bin/Debug/netcoreapp1.1/blogging.db`.

## Use your model

* Open *Program.cs* and replace the contents with the following code:

 [!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Program.cs)]

* Test the app:

 `dotnet run`

 One blog is saved to the database and the details of all blogs are displayed in the console.

  ``` console
  ConsoleApp.SQLite>dotnet run
  Project ConsoleApp.SQLite (.NETCoreApp,Version=v1.1) will be compiled because
  inputs were modified
  Compiling ConsoleApp.SQLite for .NETCoreApp,Version=v1.1

  Compilation succeeded.
      0 Warning(s)
      0 Error(s)

  Time elapsed 00:00:00.8735339

  1 records saved to database

  All blogs in database:
   - http://blogs.msdn.com/adonet
  ```

### Changing the model:

- If you make changes to your model, you can use the `dotnet ef migrations add` command to scaffold a new [migration](https://docs.microsoft.com/aspnet/core/data/ef-mvc/migrations#introduction-to-migrations)  to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the `dotnet ef database update` command to apply the changes to the database.
- EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.
- SQLite does not support all migrations (schema changes) due to limitations in SQLite. See [SQLite Limitations](../../providers/sqlite/limitations.md). For new development, consider dropping the database and creating a new one rather than using migrations when your model changes.

## Additional Resources

* [.NET Core - New database with SQLite](xref:core/get-started/netcore/new-db-sqlite) -  a cross-platform console EF tutorial.
* [Introduction to ASP.NET Core MVC on Mac or Linux ](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app-xplat/index)
* [Introduction to ASP.NET Core MVC with Visual Studio](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/index)
* [Getting started with ASP.NET Core and Entity Framework Core using Visual Studio](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/index)
