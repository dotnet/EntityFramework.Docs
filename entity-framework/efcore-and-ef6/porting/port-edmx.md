---
title: Porting an EDMX-Based Model (Model First & Database First)
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 63003709-f1ec-4bdc-8083-65a60c4826d2
ms.prod: entity-framework-
uid: efcore-and-ef6/porting/port-edmx
---
# Porting an EDMX-Based Model (Model First & Database First)

EF Core does not support the EDMX file format for models. The best option to port these models, is to generate a new code-based model from the database for your application.

## Install EF Core NuGet packages

Install the `Microsoft.EntityFrameworkCore.Tools` NuGet package.

You also need to install the **design time** NuGet package for the database provider you want to use. This is typically the runtime database provider package name with `.Design` post-fixed. For example, when targeting SQL Server, you would install `Microsoft.EntityFrameworkCore.SqlServer.Design`. See [Database Providers](../../core/providers/index.md) for details.

## Regenerate the model

You can now use the reverse engineer functionality to create a model based on your existing database.

Run the following command in Package Manager Console (Tools –> NuGet Package Manager –> Package Manager Console). See [Package Manager Console (Visual Studio)](../../core/miscellaneous/cli/powershell.md) for command options to scaffold a subset of tables etc.

<!-- literal_block"ids  "classes  "xml:space": "preserve", "backrefs  "linenos": true, "dupnames  : "csharp",", highlight_args}, "names": [] -->
````text
Scaffold-DbContext "<connection string>" <database provider name>
````

For example, here is the command to scaffold a model from the Blogging database on your SQL Server LocalDB instance.

<!-- literal_block"ids  "classes  "xml:space": "preserve", "backrefs  "linenos": true, "dupnames  : "csharp",", highlight_args}, "names": [] -->
````text
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
````

## Remove EF6.x model

You would now remove the EF6.x model from your application.

It is fine to leave the EF6.x NuGet package (EntityFramework) installed, as EF Core and EF6.x can be used side-by-side in the same application. However, if you aren't intending to use EF6.x in any areas of your application, then uninstalling the package will help give compile errors on pieces of code that need attention.

## Update your code

At this point, it's a matter of addressing compilation errors and reviewing code to see if the behavior changes between EF6.x and EF Core will impact you.

## Test the port

Just because your application compiles, does not mean it is successfully ported to EF Core. You will need to test all areas of your application to ensure that none of the behavior changes have adversely impacted your application.
