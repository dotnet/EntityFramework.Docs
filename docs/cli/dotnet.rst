.NET Core CLI
=============

EF command-line tools for .NET Core Command Line Interface (CLI).

.. contents:: `In this article:`
    :depth: 2
    :local:

.. note::
    Command-line tools for .NET Core CLI has known issues. See `Preview 1 Known Issues`_ for more details.

Installation
------------

Prerequisites
~~~~~~~~~~~~~

EF command-line tools requires .NET Core CLI Preview 1 or newer. See `.NET Core's Website <http://dotnet.github.io/>`_ for installation instructions.

Supported Frameworks
~~~~~~~~~~~~~~~~~~~~

EF supports .NET Core CLI commands on these frameworks:

 - .NET Framework 4.5.1 and newer. ("net451", "net452", "net46", etc.)
 - .NET Core App 1.0. ("netcoreapp1.0")

Install by editing project.json
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

EF command-line tools for .NET Core CLI are installed by manually editing ``project.json``. 

1. Add ``Microsoft.EntityFrameworkCore.Tools`` as a build-only dependency under "dependencies" and as a "tool". Also add "imports" if you are using "netcoreapp1.0" as a framework. See sample project.json below.
2. Execute ``dotnet restore``. If restore does not succeed, the command-line tools may not have installed correctly.

The resulting project.json should include these items (in addition to your other project dependencies).

.. code-block:: json

    {
        "dependencies": {
            "Microsoft.EntityFrameworkCore.Tools": {
                "type": "build",
                "version": "1.0.0-preview1"
            }
        },

        "tools": {
            "Microsoft.EntityFrameworkCore.Tools": {
                "imports": ["portable-net451+win8"],
                "version": "1.0.0-preview1"
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

.. TODO add usage here

Common Errors
-------------

.. include:: _common_errors.txt

.. _dotnet_cli_issues:

Preview 1 Known Issues
----------------------

Targeting class library projects is not supported
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.NET Core CLI does not support running commands on class libraries as of Preview 1. Despite being able to install EF tools, executing commands may throw this error message.

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


NuGet error: ``One or more packages are incompatible with .NETCoreApp,Version=v1.0.``
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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
