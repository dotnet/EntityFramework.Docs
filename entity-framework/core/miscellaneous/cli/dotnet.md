---
title: .NET Core CLI - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/6/2017
uid: core/miscellaneous/cli/dotnet
---
EF Core .NET Command-line Tools
===============================
The Entity Framework Core .NET Command-line Tools are an extension to the cross-platform **dotnet** command, which is
part of the [.NET Core SDK][2].

> [!TIP]
> If you're using Visual Studio, we recommend [the PMC Tools][1] instead since they provide a more integrated
> experience.

Installing the tools
--------------------
> [!NOTE]
> The .NET Core SDK version 2.1.300 and newer includes **dotnet ef** commands that are compatible with EF Core 2.0 and later versions. Therefore if you are using recent versions of the .NET Core SDK and the EF Core runtime, no installation is required and you can ignore the rest of this section.
>
> On the other hand, the **dotnet ef** tool contained in .NET Core SDK version 2.1.300 and newer is not compatible with EF Core version 1.0 and 1.1. Before you can work with a project that uses these earlier versions of EF Core on a computer that has .NET Core SDK 2.1.300 or newer installed, you must also install version 2.1.200 or older of the SDK and configure the application to use that older version by modifying its [global.json](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json) file. This file is normally included in the solution directory (one above the project). Then you can proceed with the installlation instruction below.

For previous versions of the .NET Core SDK, you can install the EF Core .NET Command-line Tools using these steps:

1. Edit the project file and add Microsoft.EntityFrameworkCore.Tools.DotNet as a DotNetCliToolReference item (See below)
2. Run the following commands:

       dotnet add package Microsoft.EntityFrameworkCore.Design
       dotnet restore

The resulting project should look something like this:

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design"
                      Version="2.0.0"
                      PrivateAssets="All" />
  </ItemGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet"
                            Version="2.0.0" />
  </ItemGroup>
</Project>
```

> [!NOTE]
> A package reference with `PrivateAssets="All"` means it isn't exposed to projects that reference this project, which
> is especially useful for packages that are typically only used during development.

If you did everything right, you should be able to successfully run the following command in a command prompt.

``` Console
dotnet ef
```

Using the tools
---------------
Whenever you invoke a command, there are two projects involved:

The target project is where any files are added (or in some cases removed). The target project defaults to the project
in the current directory, but can be changed using the <nobr>**--project**</nobr> option.

The startup project is the one emulated by the tools when executing your project's code. It also defaults to the project
in the current directory, but can be changed using the **--startup-project** option.

> [!NOTE]
> For instance, updating the database of your web application that has EF Core installed in a different project would look like this: `dotnet ef database update --project {project-path}` (from your web app directory)

Common options:

|    |                                  |                             |
|:---|:---------------------------------|:----------------------------|
|    | --json                           | Show JSON output.           |
| -c | --context \<DBCONTEXT>           | The DbContext to use.       |
| -p | --project \<PROJECT>             | The project to use.         |
| -s | --startup-project \<PROJECT>     | The startup project to use. |
|    | --framework \<FRAMEWORK>         | The target framework.       |
|    | --configuration \<CONFIGURATION> | The configuration to use.   |
|    | --runtime \<IDENTIFIER>          | The runtime to use.         |
| -h | --help                           | Show help information.      |
| -v | --verbose                        | Show verbose output.        |
|    | --no-color                       | Don't colorize output.      |
|    | --prefix-output                  | Prefix output with level.   |


> [!TIP]
> To specify the ASP.NET Core environment, set the **ASPNETCORE_ENVIRONMENT** environment variable before running.

Commands
--------

### dotnet ef database drop

Drops the database.

Options:

|    |           |                                                          |
|:---|:----------|:---------------------------------------------------------|
| -f | --force   | Don't confirm.                                           |
|    | --dry-run | Show which database would be dropped, but don't drop it. |

### dotnet ef database update

Updates the database to a specified migration.

Arguments:

|              |                                                                                              |
|:-------------|:---------------------------------------------------------------------------------------------|
| \<MIGRATION> | The target migration. If 0, all migrations will be reverted. Defaults to the last migration. |

### dotnet ef dbcontext info

Gets information about a DbContext type.

### dotnet ef dbcontext list

Lists available DbContext types.

### dotnet ef dbcontext scaffold

Scaffolds a DbContext and entity types for a database.

Arguments:

|               |                                                                             |
|:--------------|:----------------------------------------------------------------------------|
| \<CONNECTION> | The connection string to the database.                                      |
| \<PROVIDER>   | The provider to use. (for example, Microsoft.EntityFrameworkCore.SqlServer) |

Options:

|                 |                                         |                                                                                                  |
|:----------------|:----------------------------------------|:-------------------------------------------------------------------------------------------------|
| <nobr>-d</nobr> | --data-annotations                      | Use attributes to configure the model (where possible). If omitted, only the fluent API is used. |
| -c              | --context \<NAME>                       | The name of the DbContext.                                                                       |
|                 | --context-dir \<PATH>                   | The directory to put DbContext file in. Paths are relative to the project directory.             |
| -f              | --force                                 | Overwrite existing files.                                                                        |
| -o              | --output-dir \<PATH>                    | The directory to put files in. Paths are relative to the project directory.                      |
|                 | <nobr>--schema \<SCHEMA_NAME>...</nobr> | The schemas of tables to generate entity types for.                                              |
| -t              | --table \<TABLE_NAME>...                | The tables to generate entity types for.                                                         |
|                 | --use-database-names                    | Use table and column names directly from the database.                                           |

### dotnet ef migrations add

Adds a new migration.

Arguments:

|         |                            |
|:--------|:---------------------------|
| \<NAME> | The name of the migration. |

Options:

|                 |                                   |                                                                                                                  |
|:----------------|:----------------------------------|:-----------------------------------------------------------------------------------------------------------------|
| <nobr>-o</nobr> | <nobr>--output-dir \<PATH></nobr> | The directory (and sub-namespace) to use. Paths are relative to the project directory. Defaults to "Migrations". |

### dotnet ef migrations list

Lists available migrations.

### dotnet ef migrations remove

Removes the last migration.

Options:

|    |         |                                                                       |
|:---|:--------|:----------------------------------------------------------------------|
| -f | --force | Revert the migration if it has been applied to the database. |

### dotnet ef migrations script

Generates a SQL script from migrations.

Arguments:

|         |                                                               |
|:--------|:--------------------------------------------------------------|
| \<FROM> | The starting migration. Defaults to 0 (the initial database). |
| \<TO>   | The ending migration. Defaults to the last migration.         |

Options:

|    |                  |                                                                    |
|:---|:-----------------|:-------------------------------------------------------------------|
| -o | --output \<FILE> | The file to write the result to.                                   |
| -i | --idempotent     | Generate a script that can be used on a database at any migration. |


  [1]: powershell.md
  [2]: https://www.microsoft.com/net/core
