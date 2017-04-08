---
title: ASP.NET Core - New database with Visual Studio 2017 | Microsoft Docs
author: rick-anderson
ms.author: riande
ms.author2: tdykstra
ms.date: 04/07/2017
ms.assetid: e153627f-f132-4c11-b13c-6c9a607addce
ms.technology: entity-framework-core
uid: core/get-started/aspnetcore/new-db
---

# ASP.NET Core - New database with Visual Studio 2017

In this walkthrough, you will build an ASP.NET Core MVC application that performs basic data access using Entity Framework Core. You will use migrations to create the database from your model.

This tutorial requires [Visual Studio 2017](https://www.visualstudio.com/downloads/). See [.NET Core - New database with SQLite](xref:core/get-started/netcore/new-db-sqlite) for a cross-platform  console version.

Notes: 

- You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb) on GitHub.
- This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).
- The [.NET Core SDK](https://www.microsoft.com/net/download/core) 1.1.x no longer supports `project.json` or Visual Studio 2015. We recommend you [migrate from project.json to csproj](https://docs.microsoft.com/dotnet/articles/core/migration/). If you are using Visual Studio, we recommend you migrate to [Visual Studio 2017](https://www.visualstudio.com/downloads/).

## Create a new project in Visual Studio 2017

* **File > New > Project**
* From the left menu select **Installed > Templates > Visual C# > .NET Core**.
* Select **ASP.NET Core Web Application (.NET Core)**.
* Enter **EFGetStarted.AspNetCore.NewDb** for the name and click **OK**.
* In the **New ASP.NET Core Web Application** dialog enter:

  * Under **ASP.NET Core Templates 1.1** select the **Web Application**
  * Ensure that **Authentication** is set to **No Authentication**
  * Click **OK**

Warning: If you use **Individual User Accounts** instead of **None** for **Authentication** then an Entity Framework model will be added to your project in `Models\IdentityModel.cs`. Using the techniques you will learn in this walkthrough, you can choose to add a second model, or extend this existing model to contain your entity classes.

## Install Entity Framework

Install the package for the EF Core database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* Open the **Package Manager Console** (PMC):
  **Tools > NuGet Package Manager > Package Manager Console**
* Enter `Install-Package Microsoft.EntityFrameworkCore.SqlServer` in the PMC.

Install the Entity Framework Tools to maintain the database:

* Enter `Install-Package Microsoft.EntityFrameworkCore.Tools` in the PMC.

## Create the model

Define a context and entity classes that make up the model:

* Right-click on the project in **Solution Explorer** and select **Add > New Folder**.
* Enter **Models** as the name of the folder.
* Right-click on the **Models** folder and select **Add > Class**.
* Enter **Model.cs** as the name and click **OK**.
* Replace the contents of the file with the following code:
 [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Models/Model.cs)]

Note: In a real application you would typically put each class from your model in a separate file. For the sake of simplicity, we are putting all the classes in one file for this tutorial.

## Register your context with dependency injection

Services (such as `BloggingContext`) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the [Dependency Injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html) article on the ASP.NET site.

In order for our MVC controllers to make use of `BloggingContext` we will register it as a service.

* Open **Startup.cs**
* Add the following `using` statements:

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs#AddedUsings)]

Add the `AddDbContext` method to register it as a service:

* Add the following code to the `ConfigureServices` method:

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs?name=ConfigureServices&highlight=7-8)]

Note: A real app would gennerally put the connection string in a configuration file. For the sake of simplicity, we are defining it in code. For more information, see [Connection Strings](../../miscellaneous/connection-strings.md).

## Create your database

Once you have a model, you can use [migrations](https://docs.microsoft.com/aspnet/core/data/ef-mvc/migrations#introduction-to-migrations) to create a database.

* Open the PMC:

  **Tools –> NuGet Package Manager –> Package Manager Console**
* Run `Add-Migration InitialCreate` to scaffold a migration to create the initial set of tables for your model. If you receive an error stating `The term 'add-migration' is not recognized as the name of a cmdlet`, close and reopen Visual Studio.
* Run `Update-Database` to apply the new migration to the database. This command creates the database before applying migrations. 

Tip: If you make future changes to your model, you can use the `Add-Migration` command to scaffold a new migration to make the corresponding schema changes to the database. See [Migrations](https://docs.microsoft.com/aspnet/core/data/ef-mvc/migrations#introduction-to-migrations) for more information.

EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

## Create a controller

Enable scaffolding in our project:

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add > Controller**.
* Select **Minimal Dependencies** and click **Add**.
* You can ignore or delete the *ScaffoldingReadMe.txt* file.

Now that scaffolding is enabled, we can scaffold a controller for the `Blog` entity.

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add > Controller**.
* Select **MVC Controller with views, using Entity Framework** and click **Ok**.
* Set **Model class** to **Blog** and **Data context class** to **BloggingContext**.
* Click **Add**.


## Run the application

Press F5 to run and test the app. 

* Navigate to `/Blogs`
* Use the create link to create some blog entries. Test the details and delete links.

![image](_static/create.png)

![image](_static/index-new-db.png)
