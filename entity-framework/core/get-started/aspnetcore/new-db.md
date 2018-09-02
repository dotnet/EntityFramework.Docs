---
title: Getting Started on ASP.NET Core - New database - EF Core
author: rick-anderson
ms.author: riande
ms.date: 08/03/2018
ms.assetid: e153627f-f132-4c11-b13c-6c9a607addce
uid: core/get-started/aspnetcore/new-db
---

# Getting Started with EF Core on ASP.NET Core with a New database

In this tutorial, you build an ASP.NET Core MVC application that performs basic data access using Entity Framework Core. You use migrations to create the database from your EF Core model.

[View this article's sample on GitHub](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb).

## Prerequisites

Install the following software:

# [Visual Studio](#tab/visual-studio)

* [Visual Studio 2017 15.7](https://www.visualstudio.com/downloads/) with these workloads:
  * **ASP.NET and web development** (under **Web & Cloud**)
  * **.NET Core cross-platform development** (under **Other Toolsets**)
* [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core).

# [.NET Core CLI](#tab/netcore-cli)

* [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core).

---

## Create a new project

# [Visual Studio](#tab/visual-studio)

* Open Visual Studio 2017
* **File > New > Project**
* From the left menu select **Installed > Visual C# > .NET Core**.
* Select **ASP.NET Core Web Application**.
* Enter **EFGetStarted.AspNetCore.NewDb** for the name and click **OK**.
* In the **New ASP.NET Core Web Application** dialog:
  * Ensure the options **.NET Core** and **ASP.NET Core 2.1** are selected in the drop down lists
  * Select the **Web Application (Model-View-Controller)** project template
  * Ensure that **Authentication** is set to **No Authentication**
  * Click **OK**

Warning: If you use **Individual User Accounts** instead of **None** for **Authentication** then an Entity Framework Core model will be added to your project in `Models\IdentityModel.cs`. Using the techniques you learn in this tutorial, you can choose to add a second model, or extend this existing model to contain your entity classes.

# [.NET Core CLI](#tab/netcore-cli)

* In the **Terminal**, navigate to a folder that you can use for a new project.

* Run the following command:

   ```cli
   dotnet new mvc -o EFGetStarted.AspNetCore.NewDb
   ```

---

## Install Entity Framework Core

To install EF Core, you install the package for the EF Core database provider(s) you want to target. For a list of available providers see [Database Providers](../../providers/index.md). 

For this tutorial, you don't have to install a provider package because the tutorial uses SQL Server. The SQL Server provider package is included in the [Microsoft.AspnetCore.App metapackage](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/metapackage-app?view=aspnetcore-2.1).

## Create the model

Define a context class and entity classes that make up the model.

# [Visual Studio](#tab/visual-studio)

* Right-click on the **Models** folder and select **Add > Class**.
* Enter **Model.cs** as the name and click **OK**.
* Replace the contents of the file with the following code:

  [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Models/Model.cs)]

# [.NET Core CLI](#tab/netcore-cli)

* In the **Models** folder create **Model.cs** with the following code:

  [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Models/Model.cs)]

---

In a real app you would typically put each class from your model in a separate file. For the sake of simplicity, this tutorial puts all the classes in one file.

## Register your context with dependency injection

Services (such as `BloggingContext`) are registered with [dependency injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html) during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties.

To make `BloggingContext` available to MVC controllers, register it as a service.

* In **Startup.cs** add the following `using` statements:

  [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs#AddedUsings)]

* Add the following highlighted code to the `ConfigureServices` method:

  [!code-csharp[Main](../../../../samples/core/GetStarted/AspNetCore/EFGetStarted.AspNetCore.NewDb/Startup.cs?name=ConfigureServices&highlight=13-14)]

A real app would generally put the connection string in a configuration file or environment variable. For the sake of simplicity, this tutorial defines it in code. See [Connection Strings](../../miscellaneous/connection-strings.md) for more information.

## Create the database

Once you have a model, you can use [migrations](https://docs.microsoft.com/aspnet/core/data/ef-mvc/migrations#introduction-to-migrations) to create a database.

# [Visual Studio](#tab/visual-studio)

* **Tools > NuGet Package Manager > Package Manager Console**
* Run `Add-Migration InitialCreate` to scaffold a migration to create the initial set of tables for your model. If you receive an error stating `The term 'add-migration' is not recognized as the name of a cmdlet`, close and reopen Visual Studio.
* Run `Update-Database` to apply the new migration to the database. This command creates the database before applying migrations.

# [.NET Core CLI](#tab/netcore-cli)

* Run `dotnet ef migrations add InitialCreate` to scaffold a migration to create the initial set of tables for your model.
* Run `dotnet ef database update` to apply the new migration to the database. This command creates the database before applying migrations.

---

## Create a controller

Scaffold a controller and views for the `Blog` entity.

# [Visual Studio](#tab/visual-studio)

* Right-click on the **Controllers** folder in **Solution Explorer** and select **Add > Controller**.
* Select **MVC Controller with views, using Entity Framework** and click **Add**.
* Set **Model class** to **Blog** and **Data context class** to **BloggingContext**.
* Click **Add**.

# [.NET Core CLI](#tab/netcore-cli)

```cli
dotnet aspnet-codegenerator controller -name BlogsController -m Blog -dc BloggingContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries 
```

---

The scaffolding engine creates the following:

* A controller (*Controllers/BlogsController.cs*)
* Razor view files for Create, Delete, Details, Edit, and Index pages (_Views/Movies/*.cshtml_)

## Run the application

# [Visual Studio](#tab/visual-studio)

Press F5 to run and test the app.

# [.NET Core CLI](#tab/netcore-cli)

```cli
dotnet run
```
---

* Navigate to `/Blogs`
* Use the create link to create some blog entries. Test the details and delete links.

![image](_static/create.png)

![image](_static/index-new-db.png)

## Additional Resources

* [EF - New database with SQLite](xref:core/get-started/netcore/new-db-sqlite) -  a cross-platform console EF tutorial.
* [Introduction to ASP.NET Core MVC on Mac or Linux](https://docs.microsoft.com/aspnet/core/tutorials/first-mvc-app-xplat/index)
* [Introduction to ASP.NET Core MVC with Visual Studio](https://docs.microsoft.com/aspnet/core/tutorials/first-mvc-app/index)
* [Getting started with ASP.NET Core and Entity Framework Core using Visual Studio](https://docs.microsoft.com/aspnet/core/data/ef-mvc/index)
