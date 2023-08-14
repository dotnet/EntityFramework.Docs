---
title: Getting Started - EF Core
description: Getting started tutorial for Entity Framework Core
author: rick-anderson
ms.date: 09/17/2019
uid: core/get-started/overview/first-app
---

# Getting Started with EF Core

In this tutorial, you create a .NET Core console app that performs data access against a SQLite database using Entity Framework Core.

You can follow the tutorial by using Visual Studio on Windows, or by using the .NET CLI on Windows, macOS, or Linux.

[View this article's sample on GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/GetStarted).

## Prerequisites

Install the following software:

### [.NET CLI](#tab/netcore-cli)

* [.NET SDK](https://dotnet.microsoft.com/en-us/download).

### [Visual Studio](#tab/visual-studio)

* [Visual Studio 2022 version 17.4 or later](https://www.visualstudio.com/downloads/) with this workload:
  * **.NET desktop development** (under **Desktop && Mobile**)

---

## Create a new project

### [.NET CLI](#tab/netcore-cli)

```dotnetcli
dotnet new console -o EFGetStarted
cd EFGetStarted
```

### [Visual Studio](#tab/visual-studio)

* Open Visual Studio
* Click **New project**
* Select **Console App** with the **C#** tag and click **Next**
* Enter **EFGetStarted** for the name and click **Create**

---

## Install Entity Framework Core

To install EF Core, you install the package for the EF Core database provider(s) you want to target. This tutorial uses SQLite because it runs on all platforms that .NET supports. For a list of available providers, see [Database Providers](xref:core/providers/index).

### [.NET Core CLI](#tab/netcore-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### [Visual Studio](#tab/visual-studio)

* **Tools > NuGet Package Manager > Package Manager Console**
* Run the following commands:

  ```powershell
  Install-Package Microsoft.EntityFrameworkCore.Sqlite
  ```

Tip: You can also install packages by right-clicking on the project and selecting **Manage NuGet Packages**

---

## Create the model

Define a context class and entity classes that make up the model.

### [.NET CLI](#tab/netcore-cli)

* In the project directory, create **Model.cs** with the following code

### [Visual Studio](#tab/visual-studio)

* Right-click on the project and select **Add > Class**
* Enter **Model.cs** as the name and click **Add**
* Replace the contents of the file with the following code

---

[!code-csharp[Main](../../../../samples/core/GetStarted/Model.cs)]

EF Core can also [reverse engineer](xref:core/managing-schemas/scaffolding) a model from an existing database.

Tip: This application intentionally keeps things simple for clarity. [Connection strings](xref:core/miscellaneous/connection-strings) should not be stored in the code for production applications. You may also want to split each C# class into its own file.

## Create the database

The following steps use [migrations](xref:core/managing-schemas/migrations/index) to create a database.

### [.NET CLI](#tab/netcore-cli)

* Run the following commands:

  ```dotnetcli
  dotnet tool install --global dotnet-ef
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

  This installs [dotnet ef](xref:core/cli/dotnet) and the design package which is required to run the command on a project. The `migrations` command scaffolds a migration to create the initial set of tables for the model. The `database update` command creates the database and applies the new migration to it.

### [Visual Studio](#tab/visual-studio)

* Run the following commands in **Package Manager Console (PMC)**

  ```powershell
  Install-Package Microsoft.EntityFrameworkCore.Tools
  Add-Migration InitialCreate
  Update-Database
  ```

  This installs the [PMC tools for EF Core](xref:core/cli/powershell). The `Add-Migration` command scaffolds a migration to create the initial set of tables for the model. The `Update-Database` command creates the database and applies the new migration to it.

---

## Create, read, update & delete

* Open *Program.cs* and replace the contents with the following code:

  [!code-csharp[Main](../../../../samples/core/GetStarted/Program.cs)]

## Run the app

### [.NET CLI](#tab/netcore-cli)

```dotnetcli
dotnet run
```

### [Visual Studio](#tab/visual-studio)

**Debug > Start Without Debugging**

---

## Next steps

* Follow the [ASP.NET Core Tutorial](/aspnet/core/data/ef-rp/intro) to use EF Core in a web app
* Learn more about [LINQ query expressions](/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations)
* [Configure your model](xref:core/modeling/index) to specify things like [required](xref:core/modeling/entity-properties#required-and-optional-properties) and [maximum length](xref:core/modeling/entity-properties#maximum-length)
* Use [Migrations](xref:core/managing-schemas/migrations/index) to update the database schema after changing your model
