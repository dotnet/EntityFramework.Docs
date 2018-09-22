---
title: EF Core tools reference (Package Manager Console) - EF Core
author: bricelam
ms.author: bricelam
ms.date: 09/18/2018
uid: core/miscellaneous/cli/powershell
---

# Entity Framework Core tools reference - Package Manager Console in Visual Studio

The Package Manager Console (PMC) tools for Entity Framework Core perform design-time development tasks. For example, they create migrations, apply migrations, and scaffold a model based on an existing database. The commands run inside of Visual Studio using the [Package Manager Console](https://docs.microsoft.com/nuget/tools/package-manager-console). These tools work with both .NET Framework and .NET Core projects.

If you aren't using Visual Studio, we recommend the [EF Core Command-line Tools](dotnet.md) instead. The CLI tools are cross-platform and run inside a command prompt.

## Installing the tools

The procedures for installing and updating the tools differ between ASP.NET Core 2.1+ and earlier versions or other project types.

## ASP.NET Core version 2.1 and later

You don't need to do anything to install the tools. They are automatically included in the project because the `Microsoft.EntityFrameworkCore.Tools` package is included in the [Microsoft.AspNetCore.App metapackage](/aspnet/core/fundamentals/metapackage-app).

To update the tools:
* Install the latest .NET Core SDK.
* Update Visual Studio to the latest version.
* Edit your *.csproj* file and add a line specifying the latest tools package. For example, the *.csproj* file might include an `ItemGroup` that looks like this:

  ```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.1.4" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.1.1" />
  </ItemGroup>
  ```

## Other versions and project types

Install the Package Manager Console tools by running the following command in **Package Manager Console**:

``` powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
```

Update the tools by running the following command in **Package Manager Console**.

``` powershell
Update-Package Microsoft.EntityFrameworkCore.Tools
```

## Verify the installation

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

<A list of available commands follows, omitted here for brevity.>
```

## Using the tools

Before using the tools:
* Understand the difference between target and startup project.
* Learn how to use the tools with .NET Standard class libraries.
* For ASP.NET Core projects, set the environment.

### Target and startup project

The commands refer to *target project* and *startup project*:

* The target project is where the EF context and entity classes reside. It's also where any files are added or removed by the commands. The target project defaults to the **Default project** selected in **Package Manager Console**. It can also be specified by using the `-Project` parameter.

* The *startup project* is the one that the tools build and run. The startup project defaults to the project that is designated
**Startup Project** in **Solution Explorer**. It can be specified by using the `-StartupProject` parameter.

One project may be both target project and startup project. A typical scenario where they are separate projects is the following:

* EF Core context and entity classes are in a class library.
* A console app or web app references the class library.

In this scenario, if the command prompt runs in the class library project folder, you would specify the startup project as in the following example:

```console
Add-Migration InitialCreate -StartupProject MyConsoleApplication
```

### .NET Standard class libraries

The **Package Manager Console** tools work only with .NET Framework and .NET Core projects. If your project targets a framework other than these (for example, Universal Windows Platform or Xamarin), you have to use EF Core in a .NET Standard class library. In that case, you have to create a startup project that targets .NET Core so that the tooling has a concrete target platform into which it can load your class library. This startup project can be a dummy project with no real code &mdash; it is only needed to provide a target for the tooling.

For projects that target .NET Framework or .NET Core, you don't have to use a .NET Standard class library. In that case, it will be easier to work with the EF Core tools if you use a .NET Core or .NET Framework class library.

### ASP.NET Core environment

To specify the environment for ASP.NET Core projects, set **env:ASPNETCORE_ENVIRONMENT** before running commands.

## Common parameters

The following table shows parameters that are common to all of the EF Core commands:

| Parameter                 | Description                                                                                                                                                                                                          |
|---------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| :-----------              | :------------                                                                                                                                                                                                        |
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
|-----------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| :-----------                      | :------------                                                                                                           |
| <nobr>-Name \<String><nobr>       | The name of the migration. This is a positional parameter and is required.                                              |
| <nobr>-OutputDir \<String></nobr> | The directory (and sub-namespace) to use. Paths are relative to the target project directory. Defaults to "Migrations". |

## Drop-Database

Drops the database.

Parameters:

| Parameter | Description                                                |
|-----------|------------------------------------------------------------|
| :-------- | :--------------------------------------------------------- |
| -WhatIf   | Show which database would be dropped, but don't drop it.   |

## Get-DbContext

Lists available `DbContext` types.

## Remove-Migration

Removes the last migration.

Parameters:

| Parameter | Description                                                       |
|-----------|-------------------------------------------------------------------|
| :-------  | :-------------------------------------------------------------    |
| -Force    | Revert the migration even if it has been applied to the database. |

## Scaffold-DbContext

Scaffolds a `DbContext` and entity types for a database.

Parameters:

| Parameter                                  | Description                                                                                                                                                                                                                                                             |
|--------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| :----------------------------------------- | :-------------------------------------------------------------------------------------------------                                                                                                                                                                      |
| <nobr>-Connection \<String></nobr>         | The connection string to the database. For ASP.NET Core 2.x projects, the value can be *name=\<name of connection string>*. In that case the name comes from the configuration sources that are set up for the project. This is a positional parameter and is required. |
| <nobr>-Provider \<String></nobr>           | The provider to use. Typically this is the name of the NuGet package, for example: `Microsoft.EntityFrameworkCore.SqlServer`. This is a positional parameter and is required.                                                                                           |
| -OutputDir \<String>                       | The directory to put files in. Paths are relative to the project directory.                                                                                                                                                                                             |
| -ContextDir \<String>                      | The directory to put the `DbContext` file in. Paths are relative to the project directory.                                                                                                                                                                              |
| -Context \<String>                         | The name of the `DbContext` class to generate.                                                                                                                                                                                                                          |
| -Schemas \<String[]>                       | The schemas of tables to generate entity types for. If this parameter is omitted, all schemas are included.                                                                                                                                                             |
| -Tables \<String[]>                        | The tables to generate entity types for. If this parameter is omitted, all tables are included.                                                                                                                                                                         |
| -DataAnnotations                           | Use attributes to configure the model (where possible). If this parameter is omitted, only the fluent API is used.                                                                                                                                                      |
| -UseDatabaseNames                          | Use table and column names exactly as they appear in the database. If this parameter is omitted, database names are changed to more closely conform to C# name style conventions.                                                                                       |
| -Force                                     | Overwrite existing files.                                                                                                                                                                                                                                               |

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

| Parameter           | Description                                                                                                                                                                                                                |
|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| :------------------ | :-------------------------------------------------------------------                                                                                                                                                       |
| *-From* \<String>   | The starting migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration*. Defaults to 0.                                                              |
| *-To* \<String>     | The ending migration. Defaults to the last migration.                                                                                                                                                                      |
| -Idempotent         | Generate a script that can be used on a database at any migration.                                                                                                                                                         |
| -Output \<String>   | The file to write the result to. IF this parameter is omitted, the file is created with a generated name in the same folder as the app's runtime files are created, for example: */obj/Debug/netcoreapp2.1/ghbkztfz.sql/*. |

> [!TIP]
> The To, From, and Output parameters support tab-expansion.

The following examples create scripts for the InitialCreate migration and for all migrations after it.

```powershell
Script-Migration -To InitialCreate
Script-Migration -From 20180904195021_InitialCreate
```

## Update-Database

Updates the database to the last migration or to a specified migration.

| Parameter                             | Description                                                                                                                                                                                                                                                     |
|---------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| :------------------------------------ | :-----------------------------------------------------------------------------------------------                                                                                                                                                                |
| <nobr>*-Migration* \<String></nobr>   | The target migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration* and causes all migrations to be reverted. If no migration is specified, the command defaults to the last migration. |

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