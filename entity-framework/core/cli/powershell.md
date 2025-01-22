---
title: EF Core tools reference (Package Manager Console) - EF Core
description: Reference guide for the Entity Framework Core Visual Studio Package Manager Console
author: SamMonoRT
ms.date: 11/08/2024
uid: core/cli/powershell
---
# Entity Framework Core tools reference - Package Manager Console in Visual Studio

The Package Manager Console (PMC) tools for Entity Framework Core perform design-time development tasks. For example, they create [migrations](/aspnet/core/data/ef-mvc/migrations), apply migrations, and generate code for a model based on an existing database. The commands run inside of Visual Studio using the [Package Manager Console](/nuget/tools/package-manager-console). These tools work with both .NET Framework and .NET Core projects.

If you aren't using Visual Studio, we recommend the [EF Core Command-line Tools](xref:core/cli/dotnet) instead. The .NET Core CLI tools are cross-platform and run inside a command prompt.

[!INCLUDE [managed-identities-test-non-production](~/core/includes/managed-identities-test-non-production.md)]

## Install the tools

Install the Package Manager Console tools by running the following command in **Package Manager Console**:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
```

Update the tools by running the following command in **Package Manager Console**.

```powershell
Update-Package Microsoft.EntityFrameworkCore.Tools
```

### Verify the installation

Verify that the tools are installed by running this command:

```powershell
Get-Help about_EntityFrameworkCore
```

The output looks like this (it doesn't tell you which version of the tools you're using):

```output

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

## Use the tools

Before using the tools:

* Understand the difference between target and startup project.
* Learn how to use the tools with .NET Standard class libraries.
* For ASP.NET Core projects, set the environment.

### Target and startup project

The commands refer to a *project* and a *startup project*.

* The *project* is also known as the *target project* because it's where the commands add or remove files. By default, the **Default project** selected in **Package Manager Console** is the target project. You can specify a different project as target project by using the <nobr>`-Project`</nobr> parameter.

* The *startup project* is the one that the tools build and run. The tools have to execute application code at design time to get information about the project, such as the database connection string and the configuration of the model. By default, the **Startup Project** in **Solution Explorer** is the startup project. You can specify a different project as startup project by using the <nobr>`-StartupProject`</nobr> parameter.

The startup project and target project are often the same project. A typical scenario where they are separate projects is when:

* The EF Core context and entity classes are in a .NET Core class library.
* A .NET Core console app or web app references the class library.

It's also possible to [put migrations code in a class library separate from the EF Core context](xref:core/managing-schemas/migrations/projects).

### Other target frameworks

The Package Manager Console tools work with .NET Core or .NET Framework projects. Apps that have the EF Core model in a .NET Standard class library might not have a .NET Core or .NET Framework project. For example, this is true of Xamarin and Universal Windows Platform apps. In such cases, you can create a .NET Core or .NET Framework console app project whose only purpose is to act as startup project for the tools. The project can be a dummy project with no real code &mdash; it is only needed to provide a target for the tooling.

> [!IMPORTANT]
> Xamarin.Android, Xamarin.iOS, Xamarin.Mac are now integrated directly into .NET (starting with .NET 6) as .NET for Android, .NET for iOS, and .NET for macOS. If you're building with these project types today, they should be upgraded to .NET SDK-style projects for continued support. For more information about upgrading Xamarin projects to .NET, see the [Upgrade from Xamarin to .NET & .NET MAUI](/dotnet/maui/migration) documentation.

Why is a dummy project required? As mentioned earlier, the tools have to execute application code at design time. To do that, they need to use the .NET Core or .NET Framework runtime. When the EF Core model is in a project that targets .NET Core or .NET Framework, the EF Core tools borrow the runtime from the project. They can't do that if the EF Core model is in a .NET Standard class library. The .NET Standard is not an actual .NET implementation; it's a specification of a set of APIs that .NET implementations must support. Therefore .NET Standard is not sufficient for the EF Core tools to execute application code. The dummy project you create to use as startup project provides a concrete target platform into which the tools can load the .NET Standard class library.

### ASP.NET Core environment

You can specify [the environment](/aspnet/core/fundamentals/environments) for ASP.NET Core projects on the command-line. This and any additional arguments are passed into Program.CreateHostBuilder.

```powershell
Update-Database -Args '--environment Production'
```

## Common parameters

The following table shows parameters that are common to all of the EF Core commands:

| Parameter                 | Description                                                                                                                                                                                                          |
|:--------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-Context <String>`</nobr>        | The `DbContext` class to use. Class name only or fully qualified with namespaces.  If this parameter is omitted, EF Core finds the context class. If there are multiple context classes, this parameter is required. |
| <nobr>`-Project <String>`</nobr>        | The target project. If this parameter is omitted, the **Default project** for **Package Manager Console** is used as the target project.                                                                             |
| <nobr>`-StartupProject <String>`</nobr> | The startup project. If this parameter is omitted, the **Startup project** in **Solution properties** is used as the target project.                                                                                 |
| <nobr>`-Args <String>`</nobr>           | Arguments passed to the application.                                                                                                                                                           |
| `-Verbose`                              | Show verbose output.                                                                                                                                                                                                 |

To show help information about a command, use PowerShell's `Get-Help` command.

> [!TIP]
> The `Context`, `Project`, and `StartupProject` parameters support tab-expansion.

## Add-Migration

Adds a new migration.

Parameters:

| Parameter                          | Description                                                                                                             |
|:-----------------------------------|:------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-Name <String>`</nobr>      | The name of the migration. This is a positional parameter and is required.                                              |
| <nobr>`-OutputDir <String>`</nobr> | The directory use to output the files. Paths are relative to the target project directory. Defaults to "Migrations". |
| <nobr>`-Namespace <String>`</nobr> | The namespace to use for the generated classes. Defaults to generated from the output directory.  |

The [common parameters](#common-parameters) are listed above.

## Bundle-Migration

Creates an executable to update the database.

Parameters:

Parameter                              | Description
-------------------------------------- | -----------
`-Output <String>`                     | The path of executable file to create.
`-Force`                               | Overwrite existing files.
`-SelfContained`                       | Also bundle the .NET runtime so it doesn't need to be installed on the machine.
<nobr>`-TargetRuntime <String>`</nobr> | The target runtime to bundle for.
`-Framework <String>`                  | The target framework. Defaults to the first one in the project.

The [common parameters](#common-parameters) are listed above.

## Drop-Database

Drops the database.

Parameters:

| Parameter              | Description                                              |
|:-----------------------|:---------------------------------------------------------|
| <nobr>`-WhatIf`</nobr> | Show which database would be dropped, but don't drop it. |

The [common parameters](#common-parameters) are listed above.

## Get-DbContext

Lists and gets information about available `DbContext` types.

The [common parameters](#common-parameters) are listed above.

## Get-Migration

Lists available migrations.

Parameters:

| Parameter                           | Description                                                                                            |
| ----------------------------------- | ------------------------------------------------------------------------------------------------------ |
| <nobr>`-Connection <String>`</nobr> | The connection string to the database. Defaults to the one specified in AddDbContext or OnConfiguring. |
| <nobr>`-NoConnect`</nobr>           | Don't connect to the database.                                                                         |

The [common parameters](#common-parameters) are listed above.

## Optimize-DbContext

Generates a compiled version of the model used by the `DbContext`.

See [Compiled models](xref:core/performance/advanced-performance-topics#compiled-models) for more information.

Parameters:

| Parameter                           | Description                                                                                                                                                                                                                                                             |
|:------------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-OutputDir <String>`</nobr>  | The directory to put files in. Paths are relative to the project directory.                                                                                                                                                                                             |
| <nobr>`-Namespace <String>`</nobr>  | The namespace to use for all generated classes. Defaults to generated from the root namespace and the output directory plus `CompiledModels`.                                                                                                                           |

The [common parameters](#common-parameters) are listed above.

> [!NOTE]
> The PMC tools currently don't support generating code required for NativeAOT compilation and precompiled queries.

The following example uses the defaults and works if there is only one `DbContext` in the project:

```powershell
Optimize-DbContext
```

The following example optimizes the model for the context with the specified name and places it in a separate folder and namespace:

```powershell
Optimize-DbContext -OutputDir Models -Namespace BlogModels -Context BlogContext
```

## Remove-Migration

Removes the last migration (rolls back the code changes that were done for the migration).

Parameters:

| Parameter             | Description                                                                     |
|:----------------------|:--------------------------------------------------------------------------------|
| <nobr>`-Force`</nobr> | Revert the migration (roll back the changes that were applied to the database). |

The [common parameters](#common-parameters) are listed above.

## Scaffold-DbContext

Generates code for a `DbContext` and entity types for a database. In order for `Scaffold-DbContext` to generate an entity type, the database table must have a primary key.

Parameters:

| Parameter                                 | Description                                                                                                                                                                                                                                                                  |
|:------------------------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-Connection <String>`</nobr>       | The connection string to the database. The value can be *name=\<name of connection string>*. In that case the name comes from the [configuration sources](xref:core/miscellaneous/connection-strings#aspnet-core) that are set up for the project. This is a positional parameter and is required.      |
| <nobr>`-Provider <String>`</nobr>         | The provider to use. Typically this is the name of the NuGet package, for example: `Microsoft.EntityFrameworkCore.SqlServer`. This is a positional parameter and is required.                                                                                                |
| <nobr>`-OutputDir <String>`</nobr>        | The directory to put entity class files in. Paths are relative to the project directory.                                                                                                                                                                                     |
| <nobr>`-ContextDir <String>`</nobr>       | The directory to put the `DbContext` file in. Paths are relative to the project directory.                                                                                                                                                                                   |
| <nobr>`-Namespace <String>`</nobr>        | The namespace to use for all generated classes. Defaults to generated from the root namespace and the output directory.                                                                                                                                                      |
| <nobr>`-ContextNamespace <String>`</nobr> | The namespace to use for the generated `DbContext` class. Note: overrides `-Namespace`.                                                                                                                                                                                      |
| <nobr>`-Context <String>`</nobr>          | The name of the `DbContext` class to generate.                                                                                                                                                                                                                               |
| <nobr>`-Schemas <String[]>`</nobr>        | The schemas of tables and views to generate entity types for. If this parameter is omitted, all schemas are included. If this option is used, then all tables and views in the schemas will be included in the model, even if they are not explicitly included using -Table. |
| <nobr>`-Tables <String[]>`</nobr>         | The tables and views to generate entity types for. Tables or views in a specific schema can be included using the 'schema.table' or 'schema.view' format. If this parameter is omitted, all tables and views are included.                                                   |
| <nobr>`-DataAnnotations`</nobr>           | Use attributes to configure the model (where possible). If this parameter is omitted, only the fluent API is used.                                                                                                                                                           |
| <nobr>`-UseDatabaseNames`</nobr>          | Use table, view, sequence, and column names exactly as they appear in the database. If this parameter is omitted, database names are changed to more closely conform to C# name style conventions.                                                                           |
| <nobr>`-Force`</nobr>                     | Overwrite existing files.                                                                                                                                                                                                                                                    |
| <nobr>`-NoOnConfiguring`</nobr>           | Don't generate `DbContext.OnConfiguring`.                                                                                                                                                                                                                                    |
| <nobr>`-NoPluralize`</nobr>               | Don't use the pluralizer.                                                                                                                                                                                                                                                    |

The [common parameters](#common-parameters) are listed above.

Example:

```powershell
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

Example that scaffolds only selected tables and creates the context in a separate folder with a specified name and namespace:

```powershell
Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Tables "Blog","Post" -ContextDir Context -Context BlogContext -ContextNamespace New.Namespace
```

The following example [reads the connection string using Configuration](xref:core/miscellaneous/connection-strings#aspnet-core).

```powershell
Scaffold-DbContext "Name=ConnectionStrings:Blogging" Microsoft.EntityFrameworkCore.SqlServer
```

## Script-DbContext

Generates a SQL script from the DbContext. Bypasses any migrations.

Parameters:

| Parameter                       | Description                      |
| ------------------------------- | -------------------------------- |
| <nobr>`-Output <String>`</nobr> | The file to write the result to. |

The [common parameters](#common-parameters) are listed above.

## Script-Migration

Generates a SQL script that applies all of the changes from one selected migration to another selected migration.

Parameters:

| Parameter                    | Description                                                                                                                                                                                                                |
|:------------------------------- |:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-From <String>`</nobr>   | The starting migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration*. Defaults to 0.                                                              |
| <nobr>`-To <String>`</nobr>     | The ending migration. Defaults to the last migration.                                                                                                                                                                      |
| <nobr>`-Idempotent`</nobr>      | Generate a script that can be used on a database at any migration.                                                                                                                                                         |
| <nobr>`-NoTransactions`</nobr>  | Don't generate SQL transaction statements.                                                                                                                                                           |
| <nobr>`-Output <String>`</nobr> | The file to write the result to. IF this parameter is omitted, the file is created with a generated name in the same folder as the app's runtime files are created, for example: */obj/Debug/netcoreapp2.1/ghbkztfz.sql/*. |

The [common parameters](#common-parameters) are listed above.

> [!TIP]
> The `To`, `From`, and `Output` parameters support tab-expansion.

The following example creates a script for the InitialCreate migration (from a database without any migrations), using the migration name.

```powershell
Script-Migration 0 InitialCreate
```

The following example creates a script for all migrations after the InitialCreate migration, using the migration ID.

```powershell
Script-Migration 20180904195021_InitialCreate
```

## Update-Database

Updates the database to the last migration or to a specified migration.

| Parameter                           | Description                                                                                                                                                                                                                                                     |
|:------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <nobr>`-Migration <String>`</nobr>  | The target migration. Migrations may be identified by name or by ID. The number 0 is a special case that means *before the first migration* and causes all migrations to be reverted. If no migration is specified, the command defaults to the last migration. |
| <nobr>`-Connection <String>`</nobr> | The connection string to the database. Defaults to the one specified in `AddDbContext` or `OnConfiguring`.                                                                                                                                |

The [common parameters](#common-parameters) are listed above.

> [!TIP]
> The `Migration` parameter supports tab-expansion.

The following example reverts all migrations.

```powershell
Update-Database 0
```

The following examples update the database to a specified migration. The first uses the migration name and the second uses the migration ID and a specified connection:

```powershell
Update-Database InitialCreate
Update-Database 20180904195021_InitialCreate -Connection your_connection_string
```

## Additional resources

* [Migrations](xref:core/managing-schemas/migrations/index)
* [Reverse Engineering](xref:core/managing-schemas/scaffolding)
* [Compiled models](xref:core/performance/advanced-performance-topics#compiled-models)
