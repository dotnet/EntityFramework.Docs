.NET Core CLI
=============

EF command-line tools for .NET Core Command Line Interface (CLI).

.. contents:: `In this article:`
    :depth: 2
    :local:

.. note::
    Command-line tools for .NET Core CLI has known issues. See `Preview 2 Known Issues`_ for more details.

Installation
------------

Prerequisites
~~~~~~~~~~~~~

EF command-line tools requires .NET Core CLI Preview 2 or newer. See the `.NET Core <https://www.microsoft.com/net/core>`_ website for installation instructions.

Supported Frameworks
~~~~~~~~~~~~~~~~~~~~

EF supports .NET Core CLI commands on these frameworks:

 - .NET Framework 4.5.1 and newer. ("net451", "net452", "net46", etc.)
 - .NET Core App 1.0. ("netcoreapp1.0")

Install by editing project.json
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

EF command-line tools for .NET Core CLI are installed by manually editing ``project.json``.

1. Add ``Microsoft.EntityFrameworkCore.Tools`` as a "tool" and ``Microsoft.EntityFrameworkCore.Design`` as a build-only dependency under "dependencies". Also add "imports" if you are using "netcoreapp1.0" as a framework. See sample project.json below.
2. Execute ``dotnet restore``. If restore does not succeed, the command-line tools may not have installed correctly.

The resulting project.json should include these items (in addition to your other project dependencies).

.. code-block:: json

    {
        "dependencies": {
            "Microsoft.EntityFrameworkCore.Design": {
                "type": "build",
                "version": "1.0.0-preview2-final"
            }
        },

        "tools": {
            "Microsoft.EntityFrameworkCore.Tools": {
                "imports": "portable-net451+win8",
                "version": "1.0.0-preview2-final"
            }
        },

        "frameworks": {
            "netcoreapp1.0": {
                "imports": "portable-net451+win8"
            }
        }
    }

.. tip::
    A build-only dependency (``"type": "build"``) means this dependency is local to the current project. For example, if Project A has a build only dependency and Project B depends on A, ``dotnet restore`` will not add A's build-only dependencies into Project B.

Usage
-----
Commands can be run from the command line by navigating to the project directory and executing ``dotnet ef [subcommand]``. To see usage, add ``--help`` to any command to see more information about parameters and subcommands.

dotnet-ef
~~~~~~~~~
.. code-block:: none

    Usage: dotnet ef [options] [command]
    Options:
      -h|--help                        Show help information
      -v|--verbose                     Enable verbose output
      --version                        Show version information
      --framework <FRAMEWORK>          Target framework to load
      --configuration <CONFIGURATION>  Configuration under which to load
      --build-base-path <OUTPUT_DIR>   Directory in which to find temporary outputs
      --no-build                       Do not build before executing
    Commands:
      database    Commands to manage your database
      dbcontext   Commands to manage your DbContext types
      migrations  Commands to manage your migrations

dotnet-ef-database
~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef database [options] [command]
    Options:
      -h|--help     Show help information
      -v|--verbose  Enable verbose output
    Commands:
      drop    Drop the database for specific environment
      update  Updates the database to a specified migration

dotnet-ef-database-drop
~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: text

    Usage: dotnet ef database drop [options]
    Options:
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
      -f|--force                      Drop without confirmation
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output

dotnet-ef-database-update
~~~~~~~~~~~~~~~~~~~~~~~~~

.. code-block:: text

    Usage: dotnet ef database update [arguments] [options]
    Arguments:
      [migration]  The target migration. If '0', all migrations will be reverted. If omitted, all pending migrations will be applied
    Options:
      -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output

dotnet-ef-dbcontext
~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef dbcontext [options] [command]
    Options:
      -h|--help     Show help information
      -v|--verbose  Enable verbose output
    Commands:
      list      List your DbContext types
      scaffold  Scaffolds a DbContext and entity type classes for a specified database

dotnet-ef-dbcontext-list
~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef dbcontext list [options]
    Options:
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output

dotnet-ef-dbcontext-scaffold
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef dbcontext scaffold [arguments] [options]
    Arguments:
      [connection]  The connection string of the database
      [provider]    The provider to use. For example, EntityFramework.MicrosoftSqlServer
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

dotnet-ef-migrations
~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef migrations [options] [command]
    Options:
      -h|--help     Show help information
      -v|--verbose  Enable verbose output
    Commands:
      add     Add a new migration
      list    List the migrations
      remove  Remove the last migration
      script  Generate a SQL script from migrations

dotnet-ef-migrations-add
~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef migrations add [arguments] [options]
    Arguments:
      [name]  The name of the migration
    Options:
      -o|--output-dir <path>          The directory (and sub-namespace) to use. If omitted, "Migrations" is used.
      -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output


dotnet-ef-migrations-list
~~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef migrations list [options]
    Options:
      -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      --json                          Use json output. JSON is wrapped by '//BEGIN' and '//END'
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output

dotnet-ef-migrations-remove
~~~~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

    Usage: dotnet ef migrations remove [options]
    Options:
      -c|--context <context>          The DbContext to use. If omitted, the default DbContext is used
      -e|--environment <environment>  The environment to use. If omitted, "Development" is used.
      -f|--force                      Removes the last migration without checking the database. If the last migration has been applied to the database, you will need to manually reverse the changes it made.
      -h|--help                       Show help information
      -v|--verbose                    Enable verbose output


dotnet-ef-migrations-script
~~~~~~~~~~~~~~~~~~~~~~~~~~~
.. code-block:: text

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


Common Errors
-------------

.. include:: _common_errors.txt

.. _dotnet_cli_issues:

Preview 2 Known Issues
----------------------

Targeting class library projects is not supported
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.NET Core CLI does not support running commands on class libraries as of Preview 2. Despite being able to install EF tools, executing commands may throw this error message.

.. code-block:: text

    This preview of Entity Framework tools does not support targeting class library projects in ASP.NET Core
    and .NET Core applications.

See issue https://github.com/dotnet/cli/issues/2645.

Workaround
^^^^^^^^^^
Convert your class library project into an "app" project. This can either be a .NET Core app or a desktop .NET app.

To make the project a .NET Core App, add the "netcoreapp1.0" framework to project.json along with the other settings in the sample below:

.. code-block:: json

    {
        "frameworks": {
            "netcoreapp1.0": {
                "imports": ["portable-net451+win8"],
                "buildOptions": {
                    "emitEntryPoint": true
                },
                "dependencies": {
                    "Microsoft.NETCore.App": {
                        "type": "platform",
                        "version": "1.0.0-*"
                    }
                }
            }
        }
    }

To make a desktop .NET app, ensure you project targets "net451" or newer (example "net461" also works) and ensure the build option ``"emitEntryPoint"`` is set to true.

.. code-block:: json

    {
        "frameworks": {
            "net451": {
                "buildOptions": {
                    "emitEntryPoint": true
                }
            }
        }
    }


NuGet error: "One or more packages are incompatible with .NETCoreApp,Version=v1.0."
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

When attempting to add Entity Framework Core with in to a .NET Core app, ``dotnet restore`` may issue the following NuGet error:

.. code-block:: text

    Package Ix-Async 1.2.5 is not compatible with netcoreapp1.0 (.NETCoreApp,Version=v1.0).
    Package Ix-Async 1.2.5 supports:
      - net40 (.NETFramework,Version=v4.0)
      - net45 (.NETFramework,Version=v4.5)
      - portable-net45+win8+wp8 (.NETPortable,Version=v0.0,Profile=Profile78)
    Package Remotion.Linq 2.0.2 is not compatible with netcoreapp1.0 (.NETCoreApp,Version=v1.0).
    Package Remotion.Linq 2.0.2 supports:
      - net35 (.NETFramework,Version=v3.5)
      - net40 (.NETFramework,Version=v4.0)
      - net45 (.NETFramework,Version=v4.5)
      - portable-net45+win8+wp8+wpa81 (.NETPortable,Version=v0.0,Profile=Profile259)

This happens because EF Core has two dependencies, "Ix-Async" and "Remotion.Linq", that have not upgraded to support .NET Standard yet.

See issue https://github.com/aspnet/EntityFramework/issues/5176.

Workaround
^^^^^^^^^^
As a tempoarary workaround, projects can manually import other frameworks. To import Ix-Async and Remotion.Linq, add the following to your "imports" section in project.json.

.. code-block:: json

    {
        "frameworks": {
            "netcoreapp1.0": {
                "imports": [
                    "portable-net451+win8"
                ]
            }
        }
    }
