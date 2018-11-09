---
title: Getting Started on .NET Framework - Existing Database - EF Core
author: rowanmiller
ms.date: 08/06/2018
ms.assetid: a29a3d97-b2d8-4d33-9475-40ac67b3b2c6
uid: core/get-started/full-dotnet/existing-db
---

# Getting started with EF Core on .NET Framework with an Existing Database

In this tutorial, you build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework. You create an Entity Framework model by reverse engineering an existing database.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb).

## Prerequisites

* [Visual Studio 2017 version 15.7 or later](https://www.visualstudio.com/downloads/)

## Create Blogging database

This tutorial uses a **Blogging** database on the LocalDb instance as the existing database. If you have already created the **Blogging** database as part of another tutorial, skip these steps.

* Open Visual Studio

* **Tools > Connect to Database...**

* Select **Microsoft SQL Server** and click **Continue**

* Enter **(localdb)\mssqllocaldb** as the **Server Name**

* Enter **master** as the **Database Name** and click **OK**

* The master database is now displayed under **Data Connections** in **Server Explorer**

* Right-click on the database in **Server Explorer** and select **New Query**

* Copy the script listed below into the query editor

* Right-click on the query editor and select **Execute**

[!code-sql[Main](../_shared/create-blogging-database-script.sql)]

## Create a new project

* Open Visual Studio 2017

* **File > New > Project...**

* From the left menu select **Installed > Visual C# > Windows Desktop**

* Select the **Console App (.NET Framework)** project template

* Make sure that the project targets **.NET Framework 4.6.1** or later

* Name the project *ConsoleApp.ExistingDb* and click **OK**

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This tutorial uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* **Tools > NuGet Package Manager > Package Manager Console**

* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

In the next step, you use some Entity Framework Tools to reverse engineer the database. So install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Reverse engineer the model

Now it's time to create the EF model based on an existing database.

* **Tools –> NuGet Package Manager –> Package Manager Console**

* Run the following command to create a model from the existing database

  ``` powershell
  Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
  ```

> [!TIP]  
> You can specify the tables to generate entities for by adding the `-Tables` argument to the command above. For example, `-Tables Blog,Post`.

The reverse engineer process created entity classes (`Blog` and `Post`) and a derived context (`BloggingContext`) based on the schema of the existing database.

The entity classes are simple C# objects that represent the data you will be querying and saving. Here are the `Blog` and `Post` entity classes:

 [!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/Blog.cs)]

[!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/Post.cs)]

> [!TIP]  
> To enable lazy loading, you can make navigation properties `virtual` (Blog.Post and Post.Blog).

The context represents a session with the database. It has methods that you can use to query and save instances of the entity classes.

[!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/BloggingContext.cs)]

## Use the model

You can now use the model to perform data access.

* Open *Program.cs*

* Replace the contents of the file with the following code

  [!code-csharp[Main](../../../../samples/core/GetStarted/FullNet/ConsoleApp.ExistingDb/Program.cs)] 

* Debug > Start Without Debugging

  You see that one blog is saved to the database and then the details of all blogs are printed to the console.

  ![image](_static/output-existing-db.png)

## Next steps

For more information about how to scaffold a context and entity classes, see the following articles:
* [Reverse Engineering](xref:core/managing-schemas/scaffolding)
* [Entity Framework Core tools reference - .NET CLI](xref:core/miscellaneous/cli/dotnet#dotnet-ef-dbcontext-scaffold)
* [Entity Framework Core tools reference - Package Manager Console](xref:core/miscellaneous/cli/powershell#scaffold-dbcontext)