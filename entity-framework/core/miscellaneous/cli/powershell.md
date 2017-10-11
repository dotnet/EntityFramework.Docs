---
title: Package Manager Console (Visual Studio) - EF Core
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: c02a68d0-187a-47fb-af80-031f4187ad8a
ms.technology: entity-framework-core

uid: core/miscellaneous/cli/powershell
---
# Package Manager Console (Visual Studio) Tools for EF Core

EF Core Package Manager Console Tools for Visual Studio

> [!NOTE]  
> The tools require the [latest version of Windows PowerShell](https://www.microsoft.com/en-us/download/details.aspx?id=50395)

## Installation

Package Manager Console Tools are installed with the *Microsoft.EntityFrameworkCore.Tools* package.

To open the console, follow these steps.

* Open Visual Studio

* Tools > Nuget Package Manager > Package Manager Console

* Execute `Install-Package Microsoft.EntityFrameworkCore.Tools`

## Usage

> [!TIP]  
> All commands support the common parameters: `-Verbose`, `-ErrorAction`, `-ErrorVariable`, `-WarningAction`, `-WarningVariable`, `-OutBuffer`, `-OutVariable`, etc. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

> [!TIP]
> To obtain help about the PMC commands you can simply type:
``` console
Get-Help <command-name>
```

### Add-Migration

Adds a new migration.

``` console
SYNTAX
    Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]

PARAMETERS
    -Name <String>
        The name of the migration.

    -OutputDir <String>
        The directory (and sub-namespace) to use. Paths are relative to the project directory. Defaults to "Migrations".

    -Context <String>
        The DbContext type to use.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.
```

### Remove-Migration

Removes the last migration.

``` console
SYNTAX
    Remove-Migration [-Force] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]

PARAMETERS
    -Force [<SwitchParameter>]
        Don't check to see if the migration has been applied to the database. Always implied on UWP apps.

    -Context <String>
        The DbContext to use.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.
```

### Scaffold-DbContext

Scaffolds a DbContext and entity types for a database.

``` console
SYNTAX
    Scaffold-DbContext [-Connection] <String> [-Provider] <String> [-OutputDir <String>] [-Context <String>] [-Schemas <String[]>] [-Tables <String[]>] [-DataAnnotations] [-Force] [-Project <String>] [-StartupProject
    <String>] [<CommonParameters>]

PARAMETERS
    -Connection <String>
        The connection string to the database.

    -Provider <String>
        The provider to use. (E.g. Microsoft.EntityFrameworkCore.SqlServer)

    -OutputDir <String>
        The directory to put files in. Paths are relative to the project directory.

    -Context <String>
        The name of the DbContext to generate.

    -Schemas <String[]>
        The schemas of tables to generate entity types for.

    -Tables <String[]>
        The tables to generate entity types for.

    -DataAnnotations [<SwitchParameter>]
        Use attributes to configure the model (where possible). If omitted, only the fluent API is used.

    -Force [<SwitchParameter>]
        Overwrite existing files.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.
```

### Script-Migration

Generates a SQL script from migrations.

``` console
SYNTAX
    Script-Migration [-From] <String> [-To] <String> [-Idempotent] [-Output <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]

    Script-Migration [[-From] <String>] [-Idempotent] [-Output <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]

PARAMETERS
    -From <String>
        The starting migration. Defaults to '0' (the initial database).

    -To <String>
        The ending migration. Defaults to the last migration.

    -Idempotent [<SwitchParameter>]
        Generate a script that can be used on a database at any migration.

    -Output <String>
        The file to write the result to.

    -Context <String>
        The DbContext to use.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.
```

### Update-Database

Updates the database to a specified migration.

``` console
SYNTAX
    Update-Database [[-Migration] <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]

PARAMETERS
    -Migration <String>
        The target migration. If '0', all migrations will be reverted. Defaults to the last migration.

    -Context <String>
        The DbContext to use.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.
```

### Drop-Database

Drops the database.

``` console
SYNTAX
    Drop-Database [-Context <String>] [-Project <String>] [-StartupProject <String>] [-WhatIf] [-Confirm] [<CommonParameters>]

PARAMETERS
    -Context <String>
        The DbContext to use.

    -Project <String>
        The project to use.

    -StartupProject <String>
        The startup project to use. Defaults to the solution's startup project.

    -WhatIf [<SwitchParameter>]

    -Confirm [<SwitchParameter>]
```

## Using EF Core Tools and EF6 side-by-side

EF Core Tools do not work on EF6 or earlier version of EF. However, EF Core re-uses some of the same command names from these earlier versions. These tools can be installed side-by-side, however, EF does not automatically know which version of the command to use. This is solved by prefixing the command with the module name. The EF6 PowerShell module is named "EntityFramework", and the EF Core module is named "EntityFrameworkCore". Without the prefix, PowerShell may call the wrong version of the command.

Invokes the EF Core command:

``` PowerShell
EntityFrameworkCore\Add-Migration
```

Invokes the EF6 command:

``` PowerShell
EntityFramework\Add-Migration
```

## Common Errors

### Error: "No parameterless constructor was found"

EF tools attempt to automatically find how your application creates instances of your DbContext type. If it cannot find a suitable way to initialize your DbContext, you may encounter this error.

One of the solutions is to add an implementation of `IDesignTimeDbContextFactory<TContext>` to the current project. See [Using IDesignTimeDbContextFactory<TContext>](../configuring-dbcontext.md) for an example of how to create this factory.
