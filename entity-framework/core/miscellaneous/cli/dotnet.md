---
title: EF Core | .NET Core CLI | Microsoft Docs
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: c7c6824c-72be-4058-bdac-9b5b995b2f56
ms.technology: entity-framework-core

uid: core/miscellaneous/cli/dotnet
---
# .NET Command Line Tools for EF Core

> [!IMPORTANT]  
> The [.NET Core SDK](https://www.microsoft.com/net/download/core) no longer supports `project.json` or Visual Studio 2015. Everyone doing .NET Core development is encouraged to [migrate from project.json to csproj](https://docs.microsoft.com/dotnet/articles/core/migration/) and [Visual Studio 2017](https://www.visualstudio.com/downloads/).

Entity Framework Core .NET Command Line Tools

## Installation

### Prerequisites

.NET Command Line Tools require the .NET Core SDK. See the [.NET Core](https://www.microsoft.com/net/core) website for installation instructions.

### Supported Frameworks

The .NET Command Line Tools will work on projects targeting these frameworks:

* .NET Framework 4.5.1 and newer. ("net451", "net452", "net46", etc.)

* .NET Core App 1.0 and newer. ("netcoreapp1.0", "netcoreapp1.1", etc.)

### Install by editing project

The EF Core .NET Command Line Tools are installed by manually editing the `*.csproj` file.

1. Add `Microsoft.EntityFrameworkCore.Tools.DotNet` as a DotNetCliToolReference. See sample project below.

2. Execute `dotnet add package Microsoft.EntityFrameworkCore.Design`

3. Execute `dotnet restore`. If restore does not succeed, the tools may not have installed correctly.

The resulting project should include these items (in addition to your other project dependencies).

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="2.0.0" PrivateAssets="All" />
  </ItemGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
  </ItemGroup>
</Project>
```

> [!TIP]  
> A private package reference (`PrivateAssets="All"`) means this dependency is local to the current project. For example, if Project A has a build only dependency and Project B depends on A, `dotnet restore` will not add A's build-only dependencies into Project B.

## Usage

Commands can be run from the command line by navigating to the project directory and executing `dotnet ef [subcommand]`. To see usage, add `--help` to any command to see more information about parameters and subcommands.

### dotnet-ef

``` console
Usage: dotnet ef [options] [command]

Options:
  --version        Show version information
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  database    Commands to manage the database.
  dbcontext   Commands to manage DbContext types.
  migrations  Commands to manage migrations.
```

### dotnet-ef-database

``` console
Usage: dotnet ef database [options] [command]

Options:
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  drop    Drops the database.
  update  Updates the database to a specified migration.
```

### dotnet-ef-database-drop

``` console
Usage: dotnet ef database drop [options]

Options:
  -f|--force                             Don't confirm.
  --dry-run                              Show which database would be dropped, but don't drop it.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-database-update

``` console
Usage: dotnet ef database update [arguments] [options]

Arguments:
  <MIGRATION>  The target migration. If '0', all migrations will be reverted. Defaults to the last migration.

Options:
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-dbcontext

``` console
Usage: dotnet ef dbcontext [options] [command]

Options:
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  info      Gets information about a DbContext type.
  list      Lists available DbContext types.
  scaffold  Scaffolds a DbContext and entity types for a database.
```

### dotnet-ef-dbcontext-info

``` console
Usage: dotnet ef dbcontext info [options]

Options:
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-dbcontext-list

``` console
Usage: dotnet ef dbcontext list [options]

Options:
  --json                                 Show JSON output.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-dbcontext-scaffold

``` console
Usage: dotnet ef dbcontext scaffold [arguments] [options]

Arguments:
  <CONNECTION>  The connection string to the database.
  <PROVIDER>    The provider to use. (E.g. Microsoft.EntityFrameworkCore.SqlServer)

Options:
  -d|--data-annotations                  Use attributes to configure the model (where possible). If omitted, only the fluent API is used.
  -c|--context <NAME>                    The name of the DbContext.
  -f|--force                             Overwrite existing files.
  -o|--output-dir <PATH>                 The directory to put files in. Paths are relative to the project directory.
  --schema <SCHEMA_NAME>...              The schemas of tables to generate entity types for.
  -t|--table <TABLE_NAME>...             The tables to generate entity types for.
  --json                                 Show JSON output.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-migrations

``` console
Usage: dotnet ef migrations [options] [command]

Options:
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  add     Adds a new migration.
  list    Lists available migrations.
  remove  Removes the last migration.
  script  Generates a SQL script from migrations.
```

### dotnet-ef-migrations-add

``` console
Usage: dotnet ef migrations add [arguments] [options]

Arguments:
  <NAME>  The name of the migration.

Options:
  -o|--output-dir <PATH>                 The directory (and sub-namespace) to use. Paths are relative to the project directory. Defaults to "Migrations".
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-migrations-list

``` console
Usage: dotnet ef migrations list [options]

Options:
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-migrations-remove

``` console
Usage: dotnet ef migrations remove [options]

Options:
  -f|--force                             Don't check to see if the migration has been applied to the database.
  --json                                 Show JSON output.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

### dotnet-ef-migrations-script

``` console
Usage: dotnet ef migrations script [arguments] [options]

Arguments:
  <FROM>  The starting migration. Defaults to '0' (the initial database).
  <TO>    The ending migration. Defaults to the last migration.

Options:
  -o|--output <FILE>                     The file to write the result to.
  -i|--idempotent                        Generate a script that can be used on a database at any migration.
  -c|--context <DBCONTEXT>               The DbContext to use.
  -p|--project <PROJECT>                 The project to use.
  -s|--startup-project <PROJECT>         The startup project to use.
  --framework <FRAMEWORK>                The target framework.
  --configuration <CONFIGURATION>        The configuration to use.
  --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
  -e|--environment <NAME>                The environment to use. Defaults to "Development".
  -h|--help                              Show help information
  -v|--verbose                           Show verbose output.
  --no-color                             Don't colorize output.
  --prefix-output                        Prefix output with level.
```

## Common Errors

### Error: "No parameterless constructor was found"

Design-time tools attempt to automatically find how your application creates instances of your DbContext type. If EF cannot find a suitable way to initialize your DbContext, you may encounter this error.

``` console
No parameterless constructor was found on 'TContext'. Either add a parameterless constructor to
'TContext' or add an implementation of 'IDbContextFactory<TContext>' in the same assembly as
'TContext'.
```

As the error message suggests, one solution is to add an implementation of `IDbContextFactory<TContext>` to the current project. See [Using IDbContextFactory<TContext>](../configuring-dbcontext.md) for an example of how to create this factory.

<a name=dotnet-cli-issues></a>

## .NET Standard Limitation

.NET Core CLI does not fully support projects targeting .NET Standard class libraries. Despite being able to install EF tools, executing commands will show this warning message and may ultimately terminate in error.

``` console
Startup project '(your project name)' targets framework '.NETStandard'. This framework is not intended for execution and may fail to resolve runtime dependencies. If so, specify a different project using the --startup-project option and try again.
```

See issue [https://github.com/dotnet/cli/issues/2645](https://github.com/dotnet/cli/issues/2645).

#### Explanation

.NET Standard is an abstract contract for frameworks and isn't intended for execution. As such, all the assets required to execute may not be present.

The startup project defaults to the current project, unless specified differently with the parameter `--startup-project`.

#### Workaround 1 - Use an app as the startup project

If you have an existing .NET Core App or .NET Framework App (including an ASP.NET Core Web Application), you can use it as the startup project. If not, you can create a new one just for use with the .NET Command Line Tools.

Specify a startup project that is a "runnable app."

Example:

``` console
dotnet ef migrations list --startup-project ../MyConsoleApp/
```

#### Workaround 2 - Cross-target a runnable framework

Add an additional target framework to the class library project. This can a version of either .NET Core App or .NET Framework.

To make the project a .NET Core App, add the "netcoreapp1.0" framework to project like in the sample below:

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>netcoreapp1.0;netstandard1.4</TargetFrameworks>
  </PropertyGroup>
</Project>
```

When targeting .NET Framework, ensure you project targets version 4.5.1 or newer.

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net46;netstandard1.4</TargetFrameworks>
  </PropertyGroup>
</Project>
```
