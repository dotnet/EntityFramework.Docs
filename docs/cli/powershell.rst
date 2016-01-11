Package Manager Console (Visual Studio)
=======================================

Commands for PowerShell in Visual Studio's Package Manager Console window.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. caution::
  The commands require the `latest version of Windows PowerShell <https://www.microsoft.com/en-us/download/details.aspx?id=40855>`_

Installation
--------------

Package Manager Console commands are installed with the *EntityFramework.Commands* package.

To open the console, follow these steps.

* Open Visual Studio 2015
* :menuselection:`Tools --> Nuget Package Manager --> Package Manager Console`
* Execute ``Install-Package EntityFramework.Commands -Pre``

Commands
--------

.. note::

  All commands support the common parameters: ``Verbose``, ``Debug``,
  ``ErrorAction``, ``ErrorVariable``, ``WarningAction``, ``WarningVariable``,
  ``OutBuffer``, ``PipelineVariable``, and ``OutVariable``. For more information, see
  `about_CommonParameters <http://go.microsoft.com/fwlink/?LinkID=113216)>`_.


Add-Migration
~~~~~~~~~~~~~~~~~
Adds a new migration.

::


  SYNTAX
      Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

  PARAMETERS
      -Name <String>
          Specifies the name of the migration.

      -OutputDir <String>
          The directory (and sub-namespace) to use. If ``omitted``, "Migrations" is used.

      -Context <String>
          Specifies the DbContext to use. If ``omitted``, the default DbContext is used.

      -Project <String>
          Specifies the project to use. If omitted, the default project is used.

      -StartupProject <String>
          Specifies the startup project to use. If omitted, the solution's startup project is used.

      -Environment <String>
          Specifies the environment to use. If omitted, "Development" is used.


Remove-Migration
~~~~~~~~~~~~~~~~~
Removes the last migration.

::

  SYNTAX
      Remove-Migration [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

  PARAMETERS
      -Context <String>
          Specifies the DbContext to use. If omitted, the default DbContext is used.

      -Project <String>
          Specifies the project to use. If omitted, the default project is used.

      -StartupProject <String>
          Specifies the startup project to use. If omitted, the solution's startup project is used.

      -Environment <String>
          Specifies the environment to use. If omitted, "Development" is used.


Scaffold-DbContext
~~~~~~~~~~~~~~~~~~
Scaffolds a DbContext and entity type classes for a specified database.

::

  SYNTAX
      Scaffold-DbContext [-Connection] <String> [-Provider] <String> [-OutputDirectory <String>] [-ContextClassName <String>] [-Schemas <String[]>] [-Tables <String[]>] [-DataAnnotations] [-Force] [-Project
      <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

  PARAMETERS
      -Connection <String>
          Specifies the connection string of the database.

      -Provider <String>
          Specifies the provider to use. For example, EntityFramework.MicrosoftSqlServer.

      -OutputDirectory <String>
          Specifies the directory to use to output the classes. If omitted, the top-level project directory is used.

      -ContextClassName <String>

      -Schemas <String[]>
          Specifies the schemas for which to generate classes.

      -Tables <String[]>
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

.. _scaffold_directives:

Scaffold-Directives
~~~~~~~~~~~~~~~~~
Scaffolds a runtime directive file for .NET Native.
These directives instruct the compiler to include metadata for types needed at runtime.

.. note::

    This command currently only applies to Universal Windows Platform applications.

Applications built for Universal Windows Platform are compiled using
the .NET Native tool chain. The default settings for the tool chain are
currently incompatible with Entity Framework.
This provides generates the tool chain settings *Runtime Directives* to improve compatibility.

Runtime Directives are documented at http://go.microsoft.com/fwlink/?LinkID=391919

::

  SYNTAX
      Scaffold-Directives [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

  PARAMETERS
      -Project <String>
          Specifies the project to use. If omitted, the default project is used.

      -StartupProject <String>
          Specifies the startup project to use. If omitted, the solution's startup project is used.

      -Environment <String>
          Specifies the environment to use. If omitted, "Development" is used.


Script-Migration
~~~~~~~~~~~~~~~~~
Generates a SQL script from migrations.

::

  SYNTAX
      Script-Migration -From <String> -To <String> [-Idempotent] [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

      Script-Migration [-From <String>] [-Idempotent] [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]
  
  PARAMETERS
      -From <String>
          Specifies the starting migration. If omitted, '0' (the initial database) is used.

      -To <String>
          Specifies the ending migration. If omitted, the last migration is used.

      -Idempotent [<SwitchParameter>]
          Generates an idempotent script that can used on a database at any migration.

      -Context <String>
          Specifies the DbContext to use. If omitted, the default DbContext is used.

      -Project <String>
          Specifies the project to use. If omitted, the default project is used.

      -StartupProject <String>
          Specifies the startup project to use. If omitted, the solution's startup project is used.

      -Environment <String>
          Specifies the environment to use. If omitted, "Development" is used.


Update-Database
~~~~~~~~~~~~~~~~~
Updates the database to a specified migration.

::

  SYNTAX
      Update-Database [[-Migration] <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

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


Use-DbContext
~~~~~~~~~~~~~~~~~
Sets the default DbContext to use.

::

  SYNTAX
      Use-DbContext [-Context] <String> [-Project <String>] [-StartupProject <String>] [-Environment <String>] [<CommonParameters>]

  PARAMETERS
      -Context <String>
          Specifies the DbContext to use.

      -Project <String>
          Specifies the project to use. If omitted, the default project is used.

      -StartupProject <String>
          Specifies the startup project to use. If omitted, the solution's startup project is used.

      -Environment <String>
          Specifies the environment to use. If omitted, "Development" is used.
