---
title: ASP.NET Core - New Database | Microsoft Docs
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016
ms.assetid: e153627f-f132-4c11-b13c-6c9a607addce
ms.technology: entity-framework-core
uid: core/get-started/aspnetcore/new-db
---

# ASP.NET Core - New Database

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

> [!IMPORTANT]
> The [.NET Core SDK](https://www.microsoft.com/net/download/core) 1.0.0 no longer supports `project.json` or Visual Studio 2015. Everyone doing .NET Core development is encouraged to [migrate from project.json to csproj](https://docs.microsoft.com/dotnet/articles/core/migration/) and [Visual Studio 2017](https://www.visualstudio.com/downloads/).

In this walkthrough, you will build an ASP.NET Core MVC application that performs basic data access using Entity Framework. You will use migrations to create the database from your model.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb) on GitHub.

## Prerequisites

The following prerequisites are needed to complete this walkthrough:
* [Visual Studio 2017](https://www.visualstudio.com/downloads/)

## Create a new project

* Open Visual Studio 2017
* **File -> New -> Project...**
* From the left menu select **Installed -> Templates -> Visual C# -> Web**
* Select the **ASP.NET Core Web Application (.NET Core)** project template
* Enter **EFGetStarted.AspNetCore.NewDb** as the name and click **OK**
* Wait for the **New ASP.NET Core Web Application** dialog to appear
* Under **ASP.NET Core Templates 1.1** select the **Web Application**
* Ensure that **Authentication** is set to **No Authentication**
* Click **OK**

> [!WARNING]
> If you use **Individual User Accounts** instead of **None** for **Authentication** then an Entity Framework model will be added to your project in `Models\IdentityModel.cs`. Using the techniques you will learn in this walkthrough, you can choose to add a second model, or extend this existing model to contain your entity classes.

## Install Entity Framework

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see [Database Providers](../../providers/index.md).

* **Tools -> NuGet Package Manager -> Package Manager Console**
* Run `Install-Package Microsoft.EntityFrameworkCore.SqlServer`

We will be using some Entity Framework Tools to maintain the database. So we will install the tools package as well.

* Run `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Create your model

Now it's time to define a context and entity classes that make up your model.

* Right-click on the project in **Solution Explorer** and select **Add -> New Folder**
* Enter **Models** as the name of the folder
* Right-click on the **Models** folder and select **Add -> Class...**
* Enter **Model.cs** as the name and click **OK**
* Replace the contents of the file with the following code

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Models/Model.cs)]

> [!NOTE]
> In a real application you would typically put each class from your model in a separate file. For the sake of simplicity, we are putting all the classes in one file for this tutorial.

## Register your context with dependency injection

The concept of dependency injection is central to ASP.NET Core. Services (such as `BloggingContext`) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the [Dependency Injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html) article on the ASP.NET site.

In order for our MVC controllers to make use of `BloggingContext` we are going to register it as a service.

* Open **Startup.cs**
* Add the following `using` statements at the start of the file

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs#AddedUsings)]

Now we can use the `AddDbContext(...)` method to register it as a service.
* Locate the `ConfigureServices(...)` method
* Add the following code to register the context as a service

[!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs?name=ConfigureServices&highlight=7-8)]

> [!NOTE]
> In a real application you would typically put the connection string in a configuration file. For the sake of simplicity, we are defining it in code. For more information, see [Connection Strings](../../miscellaneous/connection-strings.md).

## Create your database

Now that you have a model, you can use migrations to create a database for you.

* **Tools –> NuGet Package Manager –> Package Manager Console**
* Run `Add-Migration MyFirstMigration` to scaffold a migration to create the initial set of tables for your model. If you receive an error stating `The term 'add-migration' is not recognized as the name of a cmdlet`, then close and reopen Visual Studio
* Run `Update-Database` to apply the new migration to the database. Because your database doesn't exist yet, it will be created for you before the migration is applied.

> [!TIP]
> If you make future changes to your model, you can use the `Add-Migration` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the `Update-Database` command to apply the changes to the database.
>
>EF uses a `__EFMigrationsHistory` table in the database to keep track of which migrations have already been applied to the database.

## Create a controller

Next, we'll enable scaffolding in our project.

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add -> Controller...**
* Select **Full Dependencies** and click **Add**
* You can ignore the instructions in the `ScaffoldingReadMe.txt` file that opens

Now that scaffolding is enabled, we can scaffold a controller for the `Blog` entity.

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add -> Controller...**
* Select **MVC Controller with views, using Entity Framework** and click **Ok**
* Set **Model class** to **Blog** and **Data context class** to **BloggingContext**
* Click **Add**

## Run the application

You can now run the application to see it in action.

* **Debug -> Start Without Debugging**
* The application will build and open in a web browser
* Navigate to `/Blogs`
* Click **Create New**
* Enter a **Url** for the new blog and click **Create**

![image](_static/create.png)

![image](_static/index-new-db.png)
