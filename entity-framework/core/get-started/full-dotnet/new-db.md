---
title: Getting Started on .NET Framework - New Database - EF Core
author: rowanmiller
ms.author: divega
ms.date: 08/06/2018
ms.assetid: 52b69727-ded9-4a7b-b8d5-73f3acfbbad3
ms.technology: entity-framework-core
uid: core/get-started/full-dotnet/new-db
---

# Getting started with EF Core on .NET Framework with a New Database

In this tutorial, you build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework. You use migrations to create the database from your model.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/FullNet/ConsoleApp.NewDb).

## Prerequisites

Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) - at least version 15.7.

## Create a new project

* Open Visual Studio 2017

* **File > New > Project...**

* From the left menu select **Installed > Visual C# > Windows Desktop**

* Select the **Console App (.NET Framework)** project template

* Ensure you are targeting **.NET Framework 4.6.1** or later

* Name the project *ConsoleApp.NewDb* and click **OK**

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This tutorial uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* Tools > NuGet Package Manager > Package Manager Console

* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

Later in this tutorial you use some Entity Framework Tools to maintain the database. So install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Create your model

Now it's time to define a context and entity classes that make up your model.

* **Project > Add Class...**

* Enter *Model.cs* as the name and click **OK**

* Replace the contents of the file with the following code

[!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.NewDb/Model.cs)] 

> [!TIP]  
> In a real application you would put each class in a separate file and put the connection string in a configuration file or environment variable. For the sake of simplicity, everything is in a single code file for this tutorial.

## Create your database

Now that you have a model, you can use migrations to create a database for you.

* **Tools > NuGet Package Manager > Package Manager Console**

* Run `Add-Migration InitialCreate` to scaffold a migration to create the initial set of tables for your model.

* Run `Update-Database` to apply the new migration to the database. Because your database doesn't exist yet, it will be created for you before the migration is applied.

> [!TIP]  
> If you make future changes to your model, you can use the `Add-Migration` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the `Update-Database` command to apply the changes to the database.
>
> EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

## Use your model

You can now use your model to perform data access.

* Open *Program.cs*

* Replace the contents of the file with the following code

[!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.NewDb/Program.cs)]

* **Debug > Start Without Debugging**

You see that one blog is saved to the database and then the details of all blogs are printed to the console.

![image](_static/output-new-db.png)
