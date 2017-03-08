---
title: .NET Core - New Database | Microsoft Docs
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: 099d179e-dd7b-4755-8f3c-fcde914bf50b
ms.technology: entity-framework-core

uid: core/get-started/netcore/new-db-sqlite
---

# .NET Core - New Database

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

In this walkthrough, you will build a .NET Core console application that performs basic data access against a SQLite database using Entity Framework. You will use migrations to create the database from your model.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/NetCore/ConsoleApp.SQLite) on GitHub.

## Prerequisites

The following prerequisites are needed to complete this walkthrough:
* An operating system that supports .NET Core.
* [The .NET Core SDK](https://www.microsoft.com/net/core) provides the `dotnet` command line tool, which will be used to build and run your application.
* A text editor or IDE of your choice.

## Create a new project

* Create a new `ConsoleApp.SQLite` folder for your project and use the `dotnet` command to populate it with a .NET Core application.

```bash
mkdir ConsoleApp.SQLite
cd ConsoleApp.SQLite/
dotnet new
```

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see [Database Providers](../../providers/index.md).

*  Modify the `project.json` file so that it matches the following.

[!code[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/project.json)]

*  Run `dotnet restore` to install the new packages.

## Create your model

Now it's time to define a context and entity classes that make up your model.

* Create a new `Model.cs` file with the following contents.

[!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs)]

> [!TIP]
> In a real application you would put each class in a separate file and put the connection string in a configuration file. For the sake of simplicity, we are putting everything in a single code file for this tutorial.

## Create your database

Now that you have a model, you can use migrations to create a database for you.

* Run `dotnet ef migrations add MyFirstMigration` to scaffold a migration to create the initial set of tables for your model.
* Run `dotnet ef database update` to apply the new migration to the database. Because your database doesn't exist yet, it will be created for you before the migration is applied.

> [!NOTE]
> When using relative paths with SQLite, the path will be relative to the application's main assembly. In this sample, the main binary is `bin/Debug/netcoreapp1.1/ConsoleApp.SQLite.dll`, so the SQLite database will be in `bin/Debug/netcoreapp1.1/Blogging.db`.

> [!TIP]
> If you make future changes to your model, you can use the `dotnet ef migrations add` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the `dotnet ef database update` command to apply the changes to the database.
>
>EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

> [!WARNING]
> Migrations on SQLite do not support more complex schema changes due to limitations in SQLite itself. See [SQLite Limitations](../../providers/sqlite/limitations.md)

## Use your model

You can now use your model to perform data access.

* Open `Program.cs` and replace the contents with the following code.

[!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Program.cs)]

* Run `dotnet run` to execute your application.

You will see that one blog is saved to the database and then the details of all blogs are printed to the console.

```bash
ConsoleApp.SQLite>dotnet run
Project ConsoleApp.SQLite (.NETCoreApp,Version=v1.1) will be compiled because inputs were modified
Compiling ConsoleApp.SQLite for .NETCoreApp,Version=v1.1

Compilation succeeded.
    0 Warning(s)
    0 Error(s)

Time elapsed 00:00:00.8735339


1 records saved to database

All blogs in database:
 - http://blogs.msdn.com/adonet
```
