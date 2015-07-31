:orphan:

Create a new project
--------------------
 - Create a new folder ``ConsoleApp/`` for your project. All files for the project should be contained in this folder. 

    .. code-block:: console

        ~ $ mkdir ConsoleApp
        ~ $ cd ConsoleApp/

 - Create a new file ``project.json`` with the following contents

    .. literalinclude:: x-plat/sample/src/ConsoleApp/project.json
            :language: json
            :linenos:
 
 - Execute the following command to install the packages required for this project, including EntityFramework 7 and all its dependencies.

    .. code-block:: console

        ~/ConsoleApp/ $ dnu restore

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

        ~/ConsoleApp/ $ dnx . run
        Hello dnx!

 - Verify that Entity Framework is installed by running ``dnx . ef``.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx . ef

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

        ~/ConsoleApp/ $ dnu build --quiet   
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

        ~/ConsoleApp/ $ dnx . ef migration add MyFirstMigration

 - Apply the migrations.
    You can now begin using the existing migration to create the database file and creates the tables.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx . ef migration apply

    This should create a new file, ``blog.db`` which contains two empty tables.

Use your model
--------------

Now that we have configured our model and created the database schema, we can use BloggingContext to create, update, and delete objects. 

.. literalinclude:: x-plat/sample/src/ConsoleApp/Program.cs
    :language: c#
    :linenos:

Start your app
--------------

Run the application from the command line.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx . run
        1 records saved to database

        All blogs in database:
          - https://weblogs.asp.net/
          - http://blogs.msdn.com/adonet

After adding the new post, you can verify the data has been added by inspecting the SQLite database file, ``blog.db``.


Workarounds
-----------

This demo was written for beta 6, which has bugs in it. The following workarounds will make this sample project work for beta 6.

Add a Startup class to your project
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

When generating migrations, you may see this error message:

.. code-block:: console

    System.InvalidOperationException: A type named 'StartupProduction' or 'Startup' could not be found in assembly 'ConsoleApp'.

To get around this, add the following into your project.
    
.. literalinclude:: x-plat/sample/src/ConsoleApp/Program.cs
    :linenos:
    :language: c#
    :lines: 25-30


Migrations generates invalid annotation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

In this sample, the first step of `Create your database`_ will generate invalid
C#. You may encounter this error when building your project.

.. code-block:: console

    Microsoft.Framework.Runtime.Roslyn.RoslynCompilationException: ./src/ConsoleApp/Migrations/20150729221913_MyFirstMigration.cs(17,61): DNX,Version=v4.5.1 error CS1503: Argument 2: cannot convert from 'bool' to 'string' 

To get around this, remove the offending lines of the code in ``Migrations/xxxxx_MyFirstMigration.cs```.

.. literalinclude:: x-plat/sample/src/ConsoleApp/Migrations/20150729221913_MyFirstMigration.cs
    :linenos:
    :language: c#
    :lines: 8-32
    :emphasize-lines: 10, 23
