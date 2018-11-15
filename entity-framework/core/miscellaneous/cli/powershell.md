---
title: EF Core tools reference (Package Manager Console) - EF Core
author: bricelam
ms.author: bricelam
ms.date: 09/18/2018
uid: core/miscellaneous/cli/powershell
---

# Entity Framework Core tools reference - Package Manager Console in Visual Studio

The Package Manager Console (PMC) tools for Entity Framework Core perform design-time development tasks. For example, they create [migrations](/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-2.0#introduction-to-migrations), apply migrations, and generate code for a model based on an existing database. The commands run inside of Visual Studio using the [Package Manager Console](/nuget/tools/package-manager-console). These tools work with both .NET Framework and .NET Core projects.

If you aren't using Visual Studio, we recommend the [EF Core Command-line Tools](dotnet.md) instead. The CLI tools are cross-platform and run inside a command prompt.

## Installing the tools

The procedures for installing and updating the tools differ between ASP.NET Core 2.1+ and earlier versions or other project types.

### ASP.NET Core version 2.1 and later

The tools are automatically included in an ASP.NET Core 2.1+ project because the `Microsoft.EntityFrameworkCore.Tools` package is included in the [Microsoft.AspNetCore.App metapackage](/aspnet/core/fundamentals/metapackage-app).

Therefore, you don't have to do anything to install the tools, but you do have to:
* Restore packages before using the tools in a new project.
* Install a package to update the tools to a newer version.

To make sure that you're getting the latest version of the tools, we recommend that you also do the following step:

* Edit your *.csproj* file and add a line specifying the latest version of the [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/) package. For example, the *.csproj* file might include an `ItemGroup` that looks like this:

  ```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.1.3" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.1.1" />
  </ItemGroup>
  ```

Update the tools when you get a message like the following example:

> The EF Core tools version '2.1.1-rtm-30846' is older than that of the runtime '2.1.3-rtm-32065'. Update the tools for the latest features and bug fixes.

To update the tools:
* Install the latest .NET Core SDK.
* Update Visual Studio to the latest version.
* Edit the *.csproj* file so that it includes a package reference to the latest tools package, as shown earlier.

### Other versions and project types

Install the Package Manager Console tools by running the following command in **Package Manager Console**:

``` powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
```

Update the tools by running the following command in **Package Manager Console**.

``` powershell
Update-Package Microsoft.EntityFrameworkCore.Tools
```

### Verify the installation

Verify that the tools are installed by running this command:

``` powershell
Get-Help about_EntityFrameworkCore
```

The output looks like this (it doesn't tell you which version of the tools you're using):

```console

                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

TOPIC
    about_EntityFrameworkCore

SHORT DESCRIPTION
    Provides information about the Entity Framework Core Package Manager Console Tools.

<A list of available commands follows, omitted here.>
```

## Using the tools

Before using the tools:
* Understand the difference between target and startup project.
* Learn how to use the tools with .NET Standard class libraries.
* For ASP.NET Core projects, set the environment.

### Target and startup project

The commands refer to a *project* and a *startup project*.

* The *project* is also known as the *target project* because it's where the commands add or remove files. By default, the **Default project** selected in **Package Manager Console** is the target project. You can specify a different project as target project by using the <nobr>`--project`</nobr> option.

* The *startup project* is the one that the tools build and run. The tools have to execute application code at design time to get information about the project, such as the database connection string and the configuration of the model. By default, the **Startup Project** in **Solution Explorer** is the startup project. You can specify a different project as startup project by using the <nobr>`--startup-project`</nobr> option.

The startup project and target project are often the same project. A typical scenario where they are separate projects is when:

* The EF Core context and entity classes are in a .NET Core class library.
* A .NET Core console app or web app references the class library.

It's also possible to [put migrations code in a class library separate from the EF Core context](xref:core/managing-schemas/migrations/projects).

### Other target frameworks

The Package Manager Console tools work with .NET Core or .NET Framework projects. Apps that have the EF Core model in a .NET Standard class library might not have a .NET Core or .NET Framework project. For example, this is true of Xamarin and Universal Windows Platform apps. In such cases, you can create a .NET Core or .NET Framework console app project whose only purpose is to act as startup project for the tools. The project can be a dummy project with no real code &mdash; it is only needed to provide a target for the tooling.

Why is a dummy project required? As mentioned earlier, the tools have to execute application code at design time. To do that, they need to use the .NET Core or .NET Framework runtime. When the EF Core model is in a project that targets .NET Core or .NET Framework, the EF Core tools borrow the runtime from the project. They can't do that if the EF Core model is in a .NET Standard class library. The .NET Standard is not an actual .NET implementation; it's a specification of a set of APIs that .NET implementations must support. Therefore .NET Standard is not sufficient for the EF Core tools to execute application code. The dummy project you create to use as startup project provides a concrete target platform into which the tools can load the .NET Standard class library.

### ASP.NET Core environment

To specify the environment for ASP.NET Core projects, set **env:ASPNETCORE_ENVIRONMENT** before running commands.

## Common parameters

The following table shows parameters that are common to all of the EF Core commands:

| Parameter                 | Description                                                                                                                                                                                                          |
|:--------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| -Context \<String>        | The `DbContext` class to use. Class name only or fully qualified with namespaces.  If this parameter is omitted, EF Core finds the context class. If there are multiple context classes, this parameter is required. |
| -Project \<String>        | The target project. If this parameter is omitted, the **Default project** for **Package Manager Console** is used as the target project.                                                                             |
| -StartupProject \<String> | The startup project. If this parameter is omitted, the **Startup project** in **Solution properties** is used as the target project.                                                                                 |
| -Verbose                  | Show verbose output.                                                                                                                                                                                                 |

To show help information about a command, use PowerShell's `Get-Help` command.

> [!TIP]
> The Context, Project, and StartupProject parameters support tab-expansion.

## Add-Migration

Adds a new migration.

Parameters:

| Parameter                         | Description                                                                                                             |
|:----------------------------------|:------------------------------------------------------------------------------------------------------------------------|
| <nobr>-Name \<String><nobr>       | The name of the migration. This is a positional parameter and is required.                                              |
| <nobr>-OutputDir \<String></nobr> | The directory (and sub-namespace) to use. Paths are relative to the target project directory. Defaults to "Migrations". |

## Drop-Database

Drops the database.

Parameters:

| Parameter | Description                                              |
|:----------|:---------------------------------------------------------|
| -WhatIf   | Show which database would be dropped, but don't drop it. |

## Get-DbContext

Lists available `DbContext` types.

## Remove-Migration

Removes the last migration (rolls back the code changes that were done for the migration).

Parameters:

| Parameter | Description                                                                     |
|:----------|:--------------------------------------------------------------------------------|
| -Force    | Revert the migration (roll back the changes that were applied to the database). |

## Scaffold-DbContext

Generates code for a `DbContext` and entity types for a database. In order for `Scaffold-DbContext` to generate an entity type, the database table must have a primary key.

Parameters:

| Parameter                          | Description                                                                                                                                                                                                                                                             |
|:-----------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>-Connection \<String></nobr> | The connection string to the database. For ASP.NET Core 2.x projects, the value can be *name=\<name of connection string>*. In that case the name comes from the configuration sources that are set up for the project. This is a positional parameter and is required. |
| <nobr>-Provider \<String></nobr>   | The provider to use. Typically this is the name of the NuGet package, for example: `Microsoft.EntityFrameworkCore.SqlServer`. This is a positional parameter and is required.                                                                                           |
| -OutputDir \<String>               | The directory to put files in. Paths are relative to the project directory.                                                                                                                                                                                             |
| -ContextDir \<String>              | The directory to put the `DbContext` file in. Paths are relative to the project directory.                                                                                                                                                                              |
| -Context \<String>                 | The name of the `DbContext` class to generate.                                                                                                                                                                                                                          |
| -Schemas \<String[]>               | The schemas of tables to generate entity types for. If this parameter is omitted, all schemas are included.                                                                                                                                                             |
| -Tables \<String[]>                | The tables to generate entity types for. If this parameter is omitted, all tables are included.                                                                                                                                                                         |
| -DataAnnotations                   | Use attributes to configure the model (where possible). If this parameter is omitted, only the fluent API is used.                                                                                                                                                      |
| -UseDatabaseNames                  | Use table and column names exactly as they appear in the database. If this parameter is omitted, database names are changed to more closely conform to C# name style conventions.                                                                                       |
| -Force                             | Overwrite existing files.                                                                                                                                                                                                                                               |

Example:

```powershell
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

Example that scaffolds only selected tables and creates the context in a separate folder with a specified name:

```powershell
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Tables "Blog","Post" -ContextDir Context -Context BlogContext
```

## Script-Migration

Generates a SQL script that applies all of the changes from one selected migration to another selected migration.

Parameters:

| Parameter                | Description                                                                                                                                                                                                                |
|:-------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| *-From* \<String>        | The starting migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration*. Defaults to 0.                                                              |
| *-To* \<String>          | The ending migration. Defaults to the last migration.                                                                                                                                                                      |
| <nobr>-Idempotent</nobr> | Generate a script that can be used on a database at any migration.                                                                                                                                                         |
| -Output \<String>        | The file to write the result to. IF this parameter is omitted, the file is created with a generated name in the same folder as the app's runtime files are created, for example: */obj/Debug/netcoreapp2.1/ghbkztfz.sql/*. |

> [!TIP]
> The To, From, and Output parameters support tab-expansion.

The following example creates a script for the InitialCreate migration, using the migration name.

```powershell
Script-Migration -To InitialCreate
```

The following example creates a script for all migrations after the InitialCreate migration, using the migration ID.

```powershell
Script-Migration -From 20180904195021_InitialCreate
```

## Update-Database

Updates the database to the last migration or to a specified migration.

| Parameter                           | Description                                                                                                                                                                                                                                                     |
|:------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>*-Migration* \<String></nobr> | The target migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration* and causes all migrations to be reverted. If no migration is specified, the command defaults to the last migration. |

> [!TIP]
> The Migration parameter supports tab-expansion.

The following example reverts all migrations.

```powershell
Update-Database -Migration 0
```

The following examples update the database to a specified migration. The first uses the migration name and the second uses the migration ID:

```powershell
Update-Database -Migration InitialCreate
Update-Database -Migration 20180904195021_InitialCreate
```

## Additional resources

* [Migrations](xref:core/managing-schemas/migrations/index)
* [Reverse Engineering](xref:core/managing-schemas/scaffolding)
