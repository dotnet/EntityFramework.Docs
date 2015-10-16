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

 - Verify execution of the program works by running ``dnx run``.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx run
        Hello dnx!

 - Verify that Entity Framework is installed by running ``dnx ef``.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx ef

                             _/\__
                       ---==/    \\
                 ___  ___   |.    \|\
                | __|| __|  |  )   \\\
                | _| | _|   \_/ |  //|\\
                |___||_|       /   \\\/\\

        Entity Framework Commands 7.0.0-beta8-15964

        Usage: dnx ef [options] [command]

        Options:
          --version     Show version information
          -?|-h|--help  Show help information

        Commands:
          database    Commands to manage your database
          dbcontext   Commands to manage your DbContext types
          migrations  Commands to manage your migrations

        Use "dnx ef [command] --help" for more information about a command.


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
            :lines: 1-8

 - Add a new class to represent the SQLite database.
    We will call this ``BloggingContext``. Note that to configure this for SQLite, we must call ``UseSqlite()`` with a connection string pointing to the \*.db file.
    Note that we are making the path relative to the application path, rather than the current working directory of the invoking process.

        .. literalinclude:: x-plat/sample/src/ConsoleApp/Model.cs
            :language: c#
            :linenos:
            :lines: 9-21
            :emphasize-lines: 1, 10

 - Add classes to represent tables.
    Note that we will be using foreign keys to associate many posts to one blog.

        .. literalinclude:: x-plat/sample/src/ConsoleApp/Model.cs
            :language: c#
            :linenos:
            :lines: 22-39

 - To make sure the files are correct, you can compile the project on the command line by running ``dnu build --quiet``

    .. code-block:: console
        :emphasize-lines: 9-11

        ~/ConsoleApp/ $ dnu build --quiet
        Microsoft .NET Development Utility Mono-x64-1.0.0-beta8-15858


        Building ConsoleApp for DNX,Version=v4.5.1

        Building ConsoleApp for DNXCore,Version=v5.0

        Build succeeded.
            0 Warning(s)
            0 Error(s)

        Time elapsed 00:00:01.7288901
        Total build time elapsed: 00:00:01.7470325
        Total projects built: 1

Create your database
--------------------

We can now use Entity Framework commands to create and manage the schema of our database.

 - Create the first migration.
    Execute the command below to generate your first migration.
    This will find our context and models, and generate a migration for us in a folder named ``Migrations/``

    .. code-block:: console

        ~/ConsoleApp/ $ dnx ef migrations add MyFirstMigration

 - Apply the migrations.
    You can now begin using the existing migration to create the database file and creates the tables.

    .. code-block:: console

        ~/ConsoleApp/ $ dnx ef database update

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

        ~/ConsoleApp/ $ dnx run
        1 records saved to database

        All blogs in database:
          - http://blogs.msdn.com/adonet

After adding the new post, you can verify the data has been added by inspecting the SQLite database file, ``blog.db``.


Workarounds
-----------

This demo was written for beta 8, which has bugs in it. The following workarounds will make this sample project work for beta 8.

Add a Startup class to your project
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

When generating migrations, you may see this error message:

.. code-block:: console

    System.InvalidOperationException: A type named 'StartupDevelopment' or 'Startup' could not be found in assembly 'ConsoleApp'.

To get around this, add the following into your project.

.. literalinclude:: x-plat/sample/src/ConsoleApp/Program.cs
    :linenos:
    :language: c#
    :lines: 25-29
