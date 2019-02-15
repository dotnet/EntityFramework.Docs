---
title: Getting Started on .NET Core - New database - EF Core
author: rick-anderson
ms.author: riande
description: Getting started with .NET Core using Entity Framework Core
ms.date: 08/03/2018
ms.assetid: 099d179e-dd7b-4755-8f3c-fcde914bf50b
uid: core/get-started/netcore/new-db-sqlite
---

# Getting Started with EF Core on .NET Core Console App with a New database

In this tutorial, you create a .NET Core console app that performs data access against a SQLite database using Entity Framework Core. You use migrations to create the database from the model. See [ASP.NET Core - New database](xref:core/get-started/aspnetcore/new-db) for a Visual Studio version using ASP.NET Core MVC.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/NetCore/ConsoleApp.SQLite).

## Prerequisites

* [The .NET Core 2.1 SDK](https://www.microsoft.com/net/core)

## Create a new project

* Create a new console project:

  ``` Console
  dotnet new console -o ConsoleApp.SQLite
  ```
## Change the current directory

In subsequent steps, we need to issue `dotnet` commands against the application.

* We change the current directory to the application's directory like this:

  ``` Console
  cd ConsoleApp.SQLite/
  ```
## Install Entity Framework Core

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see [Database Providers](../../providers/index.md).

* Install Microsoft.EntityFrameworkCore.Sqlite and Microsoft.EntityFrameworkCore.Design

  ```Console
  dotnet add package Microsoft.EntityFrameworkCore.Sqlite
  dotnet add package Microsoft.EntityFrameworkCore.Design
  ```

* Run `dotnet restore` to install the new packages.

## Create the model

Define a context and entity classes that make up your model.

* Create a new *Model.cs* file with the following contents.

  [!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Model.cs)]

Tip: In a real application, you put each class in a separate file and put the connection string in a configuration file or environment variable. To keep the tutorial simple, everything is contained in one file.

## Create the database

Once you have a model, you use [migrations](xref:core/managing-schemas/migrations/index) to create a database.

* Run `dotnet ef migrations add InitialCreate` to scaffold a migration and create the initial set of tables for the model.
* Run `dotnet ef database update` to apply the new migration to the database. This command creates the database before applying migrations.

The *blogging.db* SQLite DB is in the project directory.

## Use the model

* Open *Program.cs* and replace the contents with the following code:

  [!code-csharp[Main](../../../../samples/core/GetStarted/NetCore/ConsoleApp.SQLite/Program.cs)]

* Test the app from the console. See the [Visual Studio note](#vs) to run the app from Visual Studio.

  `dotnet run`

  One blog is saved to the database and the details of all blogs are displayed in the console.

  ```Console
  ConsoleApp.SQLite>dotnet run
  1 records saved to database

  All blogs in database:
   - http://blogs.msdn.com/adonet
  ```

### Changing the model:

- If you make changes to the model, you can use the `dotnet ef migrations add` command to scaffold a new [migration](xref:core/managing-schemas/migrations/index). Once you have checked the scaffolded code (and made any required changes), you can use the `dotnet ef database update` command to apply the schema changes to the database.
- EF Core uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.
- The SQLite database engine doesn't support certain schema changes that are supported by most other relational databases. For example, the `DropColumn` operation is not supported. EF Core Migrations will generate code for these operations. But if you try to apply them to a database or generate a script, EF Core throws exceptions. See [SQLite Limitations](../../providers/sqlite/limitations.md). For new development, consider dropping the database and creating a new one rather than using migrations when the model changes.

<a name="vs"></a>
### Run from Visual Studio

To run this sample from Visual Studio, you must set the working directory manually to be the root of the project. If  you don't set the working directory, the following `Microsoft.Data.Sqlite.SqliteException` is thrown:
`SQLite Error 1: 'no such table: Blogs'`.

To set the working directory:

* In **Solution Explorer**, right click the project and then select **Properties**.
* Select the **Debug** tab in the left pane.
* Set **Working directory** to the project directory.
* Save the changes.

## Additional Resources

* [Tutorial: Get started with EF Core on ASP.NET Core with a new database using SQLite](xref:core/get-started/aspnetcore/new-db)
* [Tutorial: Get started with Razor Pages in ASP.NET Core](https://docs.microsoft.com/aspnet/core/tutorials/razor-pages/razor-pages-start)
* [Tutorial: Razor Pages with Entity Framework Core in ASP.NET Core](https://docs.microsoft.com/aspnet/core/data/ef-rp/intro)
