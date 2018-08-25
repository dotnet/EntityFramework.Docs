---
title: Getting Started on .NET Framework - New Database - EF Core
author: rowanmiller
ms.date: 08/06/2018
ms.assetid: 52b69727-ded9-4a7b-b8d5-73f3acfbbad3
uid: core/get-started/full-dotnet/new-db
---

# Getting started with EF Core on .NET Framework with a New Database

In this tutorial, you build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework. You use migrations to create the database from a model.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/FullNet/ConsoleApp.NewDb).

## Prerequisites

* [Visual Studio 2017 version 15.7 or later](https://www.visualstudio.com/downloads/)

## Create a new project

* Open Visual Studio 2017

* **File > New > Project...**

* From the left menu select **Installed > Visual C# > Windows Desktop**

* Select the **Console App (.NET Framework)** project template

* Make sure that the project targets **.NET Framework 4.6.1** or later

* Name the project *ConsoleApp.NewDb* and click **OK**

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This tutorial uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* Tools > NuGet Package Manager > Package Manager Console

* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

Later in this tutorial you use some Entity Framework Tools to maintain the database. So install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Create the model

Now it's time to define a context and entity classes that make up the model.

* **Project > Add Class...**

* Enter *Model.cs* as the name and click **OK**

* Replace the contents of the file with the following code

  [!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.NewDb/Model.cs)] 

> [!TIP]  
> In a real application you would put each class in a separate file and put the connection string in a configuration file or environment variable. For the sake of simplicity, everything is in a single code file for this tutorial.

## Create the database

Now that you have a model, you can use migrations to create a database.

* **Tools > NuGet Package Manager > Package Manager Console**

* Run `Add-Migration InitialCreate` to scaffold a migration to create the initial set of tables for the model.

* Run `Update-Database` to apply the new migration to the database. Because the database doesn't exist yet, it will be created before the migration is applied.

> [!TIP]  
> If you make changes to the model, you can use the `Add-Migration` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the `Update-Database` command to apply the changes to the database.
>
> EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

## Use the model

You can now use the model to perform data access.

* Open *Program.cs*

* Replace the contents of the file with the following code

  [!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.NewDb/Program.cs)]

* **Debug > Start Without Debugging**

  You see that one blog is saved to the database and then the details of all blogs are printed to the console.

  ![image](_static/output-new-db.png)

## Additional Resources

* [EF Core on .NET Framework with an existing database](xref:core/get-started/full-dotnet/existing-db)
* [EF Core on .NET Core with a new database - SQLite](xref:core/get-started/netcore/new-db-sqlite) -  a cross-platform console EF tutorial.
