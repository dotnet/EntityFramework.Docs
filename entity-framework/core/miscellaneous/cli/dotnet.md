---
title: .NET Core CLI
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: c7c6824c-72be-4058-bdac-9b5b995b2f56
ms.prod: entity-framework
uid: core/miscellaneous/cli/dotnet
---
# .NET Core CLI

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

EF command-line tools for .NET Core Command Line Interface (CLI).

> [!NOTE]
> Command-line tools for .NET Core CLI has known issues. See [Preview 2 Known Issues](#preview-2-known-issues) for more details.

## Installation

### Prerequisites

EF command-line tools requires .NET Core CLI Preview 2 or newer. See the [.NET Core](https://www.microsoft.com/net/core) website for installation instructions.

### Supported Frameworks

EF supports .NET Core CLI commands on these frameworks:

* .NET Framework 4.5.1 and newer. ("net451", "net452", "net46", etc.)

* .NET Core App 1.0. ("netcoreapp1.0")

### Install by editing project.json

EF command-line tools for .NET Core CLI are installed by manually editing `project.json`.

1. Add `Microsoft.EntityFrameworkCore.Tools` as a "tool" and `Microsoft.EntityFrameworkCore.Design` as a build-only dependency under "dependencies". See sample project.json below.

2. Execute `dotnet restore`. If restore does not succeed, the command-line tools may not have installed correctly.

The resulting project.json should include these items (in addition to your other project dependencies).

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````json
{
    "dependencies": {
        "Microsoft.EntityFrameworkCore.Design": {
            "type": "build",
            "version": "1.0.0-preview2-final"
        }
    },

    "tools": {
        "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final"
    },

    "frameworks": {
        "netcoreapp1.0": { }
    }
}
````

> [!TIP]
> A build-only dependency (`"type": "build"`) means this dependency is local to the current project. For example, if Project A has a build only dependency and Project B depends on A, `dotnet restore` will not add A's build-only dependencies into Project B.

## Usage

Commands can be run from the command line by navigating to the project directory and executing `dotnet ef [subcommand]`. To see usage, add `--help` to any command to see more information about parameters and subcommands.

### dotnet-ef

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef [options] [command]

Options:
  -h|--help                           Show help information
  -p|--project <PROJECT>              The project to target (defaults to the project in the current directory). Can be a path to a project.json or a project directory.
  -s|--startup-project <PROJECT>      The path to the project containing Startup (defaults to the target project). Can be a path to a project.json or a project directory.
  -c|--configuration <CONFIGURATION>  Configuration under which to load (defaults to Debug)
  -f|--framework <FRAMEWORK>          Target framework to load from the startup project (defaults to the framework most compatible with .NETCoreApp,Version=v1.0).
  -b|--build-base-path <OUTPUT_DIR>   Directory in which to find temporary outputs.
  -o|--output <OUTPUT_DIR>            Directory in which to find outputs

Commands:
  database    Commands to manage your database
  dbcontext   Commands to manage your DbContext types
  migrations  Commands to manage your migrations
````

### dotnet-ef-database

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef database [options] [command]

Options:
  -h|--help     Show help information
  -v|--verbose  Enable verbose output

Commands:
  drop    Drop the database for specific environment
  update  Updates the database to a specified migration
````

### dotnet-ef-database-drop

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef database drop [options]

Options:
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -f|--force                      Drop without confirmation
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-database-update

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef database update [arguments] [options]

Arguments:
  [migration]  The target migration. If '0', all migrations will be reverted. If omitted, all pending migrations will be applied

Options:
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-dbcontext

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef dbcontext [options] [command]

Options:
  -h|--help     Show help information
  -v|--verbose  Enable verbose output

Commands:
  list      List your DbContext types
  scaffold  Scaffolds a DbContext and entity type classes for a specified database
````

### dotnet-ef-dbcontext-list

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef dbcontext list [options]

Options:
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-dbcontext-scaffold

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef dbcontext scaffold [arguments] [options]

Arguments:
  [connection]  The connection string of the database
  [provider]    The provider to use. For example, Microsoft.EntityFrameworkCore.SqlServer

Options:
  -a|--data-annotations           Use DataAnnotation attributes to configure the model where possible. If omitted, the output code will use only the fluent API.
  -c|--context <name>             Name of the generated DbContext class.
  -f|--force                      Force scaffolding to overwrite existing files. Otherwise, the code will only proceed if no output files would be overwritten.
  -o|--output-dir <path>          Directory of the project where the classes should be output. If omitted, the top-level project directory is used.
  --schema <schema>               Selects a schema for which to generate classes.
  -t|--table <schema.table>       Selects a table for which to generate classes.
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-migrations

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef migrations [options] [command]

Options:
  -h|--help     Show help information
  -v|--verbose  Enable verbose output

Commands:
  add     Add a new migration
  list    List the migrations
  remove  Remove the last migration
  script  Generate a SQL script from migrations
````

### dotnet-ef-migrations-add

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef migrations add [arguments] [options]

Arguments:
  [name]  The name of the migration

Options:
  -o|--output-dir <path>          The directory (and sub-namespace) to use. If omitted, "Migrations" is used. Relative paths are relative the directory in which the command is executed.
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-migrations-list

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef migrations list [options]

Options:
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-migrations-remove

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef migrations remove [options]

Options:
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  -f|--force                      Removes the last migration without checking the database. If the last migration has been applied to the database, you will need to manually reverse the changes it made.
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
````

### dotnet-ef-migrations-script

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Usage: dotnet ef migrations script [arguments] [options]

Arguments:
  [from]  The starting migration. If omitted, '0' (the initial database) is used
  [to]    The ending migration. If omitted, the last migration is used

Options:
  -o|--output <file>              The file to write the script to instead of stdout
  -i|--idempotent                 Generates an idempotent script that can used on a database at any migration
  -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
  -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
  -h|--help                       Show help information
  -v|--verbose                    Enable verbose output
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

<a name=dotnet-cli-issues></a>

## Preview 2 Known Issues

### Targeting class library projects is not supported

.NET Core CLI does not support running commands on class libraries as of Preview 2. Despite being able to install EF tools, executing commands may throw this error message.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````
Could not invoke this command on the startup project '(your project name)'. This preview of Entity Framework tools does not support commands on class library projects in ASP.NET Core and .NET Core applications.
````

See issue [https://github.com/dotnet/cli/issues/2645](https://github.com/dotnet/cli/issues/2645).

#### Explanation

If `dotnet run` does not work in the startup project, then `dotnet ef` cannot run either.

"dotnet ef" is invoked as an alternate entry point into an application. If the "startup" project is not an application, then it is not currently possible to run the project as an application with the "dotnet ef" alternate entry point.

The "startup" project defaults to the current project, unless specified differently with the parameter `--startup-project`.

#### Workaround 1 - Utilize a separate startup project

Convert the class library project into an "app" project. This can either be a .NET Core app or a desktop .NET app.

Example:

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````json
{
    "frameworks": {
        "netcoreapp1.0": {
            "dependencies": {
                "Microsoft.NETCore.App": {
                    "type": "platform",
                    "version": "1.0.0-*"
                }
            }
        }
    }
}
````

Be sure to register the EntityFramework Tools as a project dependency and in the tools section of your project.json.

Example:

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````json
{
    "dependencies": {
        "Microsoft.EntityFrameworkCore.Tools": {
            "version": "1.0.0-preview2-final",
            "type": "build"
        }
    },
    "tools": {
        "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final"
    }
}
````

Finally, specify a startup project that is a "runnable app."

Example:

<!-- literal_block"language": "csharp",ole", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````console
dotnet ef --startup-project ../MyConsoleApplication/ migrations list
````

#### Workaround 2 - Modify your class library to be a startup application

Convert the class library project into an "app" project. This can either be a .NET Core app or a desktop .NET app.

To make the project a .NET Core App, add the "netcoreapp1.0" framework to project.json along with the other settings in the sample below:

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````json
{
    "buildOptions": {
        "emitEntryPoint": true
    },
    "frameworks": {
        "netcoreapp1.0": {
            "dependencies": {
                "Microsoft.NETCore.App": {
                    "type": "platform",
                    "version": "1.0.0-*"
                }
            }
        }
    }
}
````

To make a desktop .NET app, ensure you project targets "net451" or newer (example "net461" also works) and ensure the build option `"emitEntryPoint"` is set to true.

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````json
{
    "buildOptions": {
        "emitEntryPoint": true
    },
    "frameworks": {
        "net451": { }
    }
}
````
