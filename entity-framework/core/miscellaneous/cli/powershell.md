---
title: Package Manager Console (Visual Studio)
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: c02a68d0-187a-47fb-af80-031f4187ad8a
ms.prod: entity-framework-
uid: core/miscellaneous/cli/powershell
---
# Package Manager Console (Visual Studio)

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

EF command line tools for Visual Studio's Package Manager Console (PMC) window.

> [!WARNING]
> The commands require the [latest version of Windows PowerShell](https://www.microsoft.com/en-us/download/details.aspx?id=50395)

## Installation

Package Manager Console commands are installed with the *Microsoft.EntityFrameworkCore.Tools* package.

To open the console, follow these steps.

* Open Visual Studio 2015

* Tools ‣ Nuget Package Manager ‣ Package Manager Console

* Execute `Install-Package Microsoft.EntityFrameworkCore.Tools -Pre`

### .NET Core and ASP.NET Core Projects

.NET Core and ASP.NET Core projects also require installing .NET Core CLI. See [.NET Core CLI](dotnet.md) for more information about this installation.

> [!NOTE]
> .NET Core CLI has known issues in Preview 1. Because PMC commands call .NET Core CLI commands, these known issues also apply to PMC commands. See [Preview 2 Known Issues](dotnet.md).

> [!TIP]
> On .NET Core and ASP.NET Core projects, add `-Verbose` to any Package Manager Console command to see the equivalent .NET Core CLI command that was invoked.

## Usage

> [!NOTE]
> All commands support the common parameters: `-Verbose`, `-Debug`, `-ErrorAction`, `-ErrorVariable`, `-WarningAction`, `-WarningVariable`, `-OutBuffer`, `-PipelineVariable`, and `-OutVariable`. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

### Add-Migration

Adds a new migration.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>]
     [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

PARAMETERS
    -Name <String>
        Specifies the name of the migration.

    -OutputDir <String>
        The directory (and sub-namespace) to use. If omitted, "Migrations" is used. Relative paths are relative to project directory.

    -Context <String>
        Specifies the DbContext to use. If omitted, the default DbContext is used.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.
````

### Remove-Migration

Removes the last migration.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Remove-Migration [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>]
     [-Force] [<CommonParameters>]

PARAMETERS
    -Context <String>
        Specifies the DbContext to use. If omitted, the default DbContext is used.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.

    -Force [<SwitchParameter>]
        Removes the last migration without checking the database. If the last migration has been applied to the database, you will need to manually reverse the changes it made.
````

### Scaffold-DbContext

Scaffolds a DbContext and entity type classes for a specified database.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Scaffold-DbContext [-Connection] <String> [-Provider] <String> [-OutputDir <String>] [-Context <String>]
      [-Schemas <String>] [-Tables <String>] [-DataAnnotations] [-Force] [-Project <String>]
      [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

PARAMETERS
    -Connection <String>
        Specifies the connection string of the database.

    -Provider <String>
        Specifies the provider to use. For example, Microsoft.EntityFrameworkCore.SqlServer.

    -OutputDir <String>
        Specifies the directory to use to output the classes. If omitted, the top-level project directory is used.

    -Context <String>
        Specifies the name of the generated DbContext class.

    -Schemas <String>
        Specifies the schemas for which to generate classes.

    -Tables <String>
        Specifies the tables for which to generate classes.

    -DataAnnotations [<SwitchParameter>]
        Use DataAnnotation attributes to configure the model where possible. If omitted, the output code will use only the fluent API.

    -Force [<SwitchParameter>]
        Force scaffolding to overwrite existing files. Otherwise, the code will only proceed if no output files would be overwritten.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.
````

### Script-Migration

Generates a SQL script from migrations.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Script-Migration -From <String> -To <String> [-Idempotent] [-Context <String>] [-Project <String>]
      [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

    Script-Migration [-From <String>] [-Idempotent] [-Context <String>] [-Project <String>]
      [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

PARAMETERS
    -From <String>
        Specifies the starting migration. If omitted, '0' (the initial database) is used.

    -To <String>
        Specifies the ending migration. If omitted, the last migration is used.

    -Idempotent [<SwitchParameter>]
        Generates an idempotent script that can be used on a database at any migration.

    -Context <String>
        Specifies the DbContext to use. If omitted, the default DbContext is used.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.
````

### Update-Database

Updates the database to a specified migration.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Update-Database [[-Migration] <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>]
      [-Environment <String>] [<CommonParameters>]

PARAMETERS
    -Migration <String>
        Specifies the target migration. If '0', all migrations will be reverted. If omitted, all pending migrations will be applied.

    -Context <String>
        Specifies the DbContext to use. If omitted, the default DbContext is used.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.
````

### Use-DbContext

Sets the default DbContext to use.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text
SYNTAX
    Use-DbContext [-Context] <String> [-Project <String>] [-StartupProject <String>] [-Environment <String>]
      [<CommonParameters>]

PARAMETERS
    -Context <String>
        Specifies the DbContext to use.

    -Project <String>
        Specifies the project to use. If omitted, the default project is used.

    -StartupProject <String>
        Specifies the startup project to use. If omitted, the solution's startup project is used.

    -Environment <String>
        Specifies the environment to use. If omitted, "Development" is used.
````

## Using EF Core commands and EF 6 commands side-by-side

EF Core commands do not work on EF 6 or earlier version of EF. However, EF Core re-uses some of the same command names from these earlier versions. These commands can be installed side-by-side, however, EF does not automatically know which version of the command to use. This is solved by prefixing the command with the module name. The EF 6 commands PowerShell module is named "EntityFramework", and the EF Core module is named "EntityFrameworkCore". Without the prefix, PowerShell may call the wrong version of the command.

<!-- literal_block"language": "csharp",rShell", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````PowerShell
# Invokes the EF Core command
PS> EntityFrameworkCore\Add-Migration

# Invokes the EF 6 command
PS> EntityFramework\Add-Migration
````

## Common Errors

### Error: "No parameterless constructor was found"

Design-time tools attempt to automatically find how your application creates instances of your DbContext type. If EF cannot find a suitable way to initialize your DbContext, you may encounter this error.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
No parameterless constructor was found on 'TContext'. Either add a parameterless constructor to
'TContext' or add an implementation of 'IDbContextFactory<TContext>' in the same assembly as
'TContext'.
````

As the error message suggests, one solution is to add an implementation of `IDbContextFactory<TContext>` to the current project. See [Using IDbContextFactory<TContext>](../configuring-dbcontext.md) for an example of how to create this factory.

See also [Preview 2 Known Issues](dotnet.md) for .NET Core CLI commands.
