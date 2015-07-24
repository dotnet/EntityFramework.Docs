.. include:: /stub-topic.txt

Getting Started on OSX
======================

This walkthrough will create a simple console application using ASP.NET 5 and
the SQLite provider.

.. note::
    This article was written for OS X Mavericks or newer. It uses Beta 6 of ASP.NET and EF7. 

    You can find nightly builds of the EF7 code base hosted on https://www.myget.org/F/aspnetvnext/api/v2/ but the code base is rapidly changing and we do not maintain up-to-date documentation for getting started.


In this article
    - `Prerequisites`_
    - `Install ASP.NET 5`_
    - `Create a new project`_
    - `Create your model`_
    - `Create your database`_
    - `Use your model`_
    - `Start your app`_

.. note:: `View this article's samples on GitHub <https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/x-plat/sample>`_.


Prerequisites
-------------

Minimum system requirements
 - Mono 4.0.2
 - OS X Mavericks

.. caution::
    **Known Issues**

     - Bugs in Mono 4.0.2 may cause Entity Framework to crash when using async methods. This is resolved with Mono >4.2.0, which has not yet been publicly released. `See this issue on GitHub <https://github.com/aspnet/EntityFramework/issues/2708>`_
     - Migrations on SQLite do not support more complex schema changes due to limitations in SQLite itself.

     .. TODO add workaround demo for SQLite rebuilds


Install ASP.NET 5
-----------------

A summary of steps to install ASP.NET 5 are included below. For a more up to date guide, follow the steps for `Installing ASP.NET 5 on Mac OS X <http://docs.asp.net/en/latest/getting-started/installing-on-mac.html>`_. This will ensure you meet the following requirements.

The following steps will install `dnvm <https://github.com/aspnet/home#running-an-application>`_, a command-line tool for installing the .NET Execution environment.

 - Install `Homebrew <http://brew.sh>`_
 - Use brew to install Mono

    .. code-block:: console

        $ brew install mono

 - Run the dnvm

    .. code-block:: console

        $ curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | bash && source ~/.dnx/dnvm/dnvm.sh

 - Verify dnvm has the latest version
    
    .. code-block:: console

        $ dnvm upgrade

If you have trouble installing dnvm, consult this `Getting Started guide <http://dotnet.github.io/core/getting-started/>`_.


Create a new project
--------------------
 - Create a new folder for your project. All files for the project should be contained in this folder. 
 - Open a new Terminal window and navigate to your project folder. All commands listed below are executed from the project root folder.
 - Create a new file ``project.json`` with the following contents

    .. literalinclude:: x-plat/sample/src/ConsoleApp/project.json
            :language: json
            :linenos:
 
 - Execute the following command to install the packages required for this project, including EntityFramework 7 and all its dependencies.

    .. code-block:: console

        $ dnu restore

 - Create a new file named ``Program.cs``. Add the following contents to

    .. code-block:: c#
        :linenos:

        // Program.cs
        using System;

        namespace ConsoleApp
        {
            public class Program
            {
                public void Main(string[] args)
                {
                    Console.WriteLine("Hello dnx!");
                }
            }
        }

To verify that this project has all dependencies and packages installed, perform the following steps:

 - Verify execution of the program works by running ``dnx . run``.

    .. code-block:: console

        $ dnx . run
        Hello dnx!
 - Verify that Entity Framework is installed by running ``dnx . ef``.

    .. code-block:: console

        $ dnx . ef

                             _/\__
                       ---==/    \\
                 ___  ___   |.    \|\
                | __|| __|  |  )   \\\
                | _| | _|   \_/ |  //|\\
                |___||_|       /   \\\/\\

        Usage: ef [options] [command]

        Options:
          -v|--version  Show version information
          -h|--help     Show help information

        Commands:
          context    Commands to manage your DbContext
          migration  Commands to manage your Code First Migrations
          revEng     Command to reverse engineer code from a database
          help       Show help information

        Use "ef [command] --help" for more information about a command.


Create your model
-----------------

With this new project, you are ready to begin using Entity Framework.
We will create a simple console application that allows us to write a 
blog post from the command line.

 - Create a new file called ``Model.cs``
    All classes in follow steps will be added to this file.

        .. literalinclude:: x-plat/sample/src/ConsoleApp/Model.cs
            :language: c#
            :linenos:
            :lines: 1-5

 - Add a new class to represent the SQLite database. 
    We will call this ``BloggingContext``. Note that to configure this for SQLite, we must call ``UseSqlite()`` with a connection string pointing to the \*.db file.

        .. literalinclude:: x-plat/sample/src/ConsoleApp/Model.cs
            :language: c#
            :linenos:
            :lines: 6-16
            :emphasize-lines: 1, 8

 - Add classes to represent tables. 
    Note that we will be using foreign keys to associate many posts to one blog.

        .. literalinclude:: x-plat/sample/src/ConsoleApp/Model.cs
            :language: c#
            :linenos:
            :lines: 17-34

 - To make sure the files are correct, you can compile the project on the command line by running ``dnu build --quiet``

    .. code-block:: console
        :emphasize-lines: 4-6

        $ dnu build --quiet   
        Microsoft .NET Development Utility Mono-x86-1.0.0-beta6

        Build succeeded.
            0 Warning(s)
            0 Error(s)

        Total build time elapsed: 00:00:01.9609581
        Total projects built: 1

Create your database
--------------------

We can now use Entity Framework commands to create and manage the schema of our database.

 - Create the first migration.
    Execute the command below to generate your first migration.
    This will find our context and models, and generate a migration for us in a folder named ``Migrations/``

    .. code-block:: console

        $ dnx . ef migration add MyFirstMigration

 - Apply the migrations.
    You can now using the existing migration to create the database file and creates the tables.

    .. code-block:: console

        $ dnx . ef migration apply

    This should create a new file, ``blog.db`` which contains two empty tables.

Use your model
--------------

Now that we have configured our model and creating the database schema, we can use BloggingContext to create, update, and delete objects. 

.. literalinclude:: x-plat/sample/src/ConsoleApp/Program.cs
    :language: c#
    :linenos:

Start your app
--------------

Run the application from the command line.

    .. code-block:: console

        $ dnx . run
          New post title >

After adding the new post, you can verify the data has been added by inspecting the SQLite database file, ``blog.db``.
