.. include:: /_shared/rc2-notice.txt

.NET Core Application to New SQLite Database
============================================

In this walkthrough, you will build a .NET Core console application that performs basic data access using Entity Framework.
You will use migrations to create the database from your model.

.. contents:: `In this article:`
    :depth: 1
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/sample/NetCore/EFGetStarted.ConsoleApp.SQLite

Prerequisites
-------------

Minimum system requirements
 - An operating system that supports .NET Core
 - `.NET Core SDK Preview 2 <https://www.microsoft.com/net/core>`_
 - A text editor

.. caution::
    **Known Issues**

    - Migrations on SQLite do not support more complex schema changes due to
      limitations in SQLite itself. See :doc:`/providers/sqlite/limitations`

Install the .NET Core SDK
^^^^^^^^^^^^^^^^^^^^^^^^^

The .NET Core SDK provides the command-line tool ``dotnet`` which will be used to
build and run our sample application.

See the `.NET Core website <https://www.microsoft.com/net/core>`_ for instructions on installing the SDK
on your operating system.

Create a new project
--------------------

 - Create a new folder ``ConsoleApp/`` for your project. All files for the project should be in this folder.

    .. code-block:: bash

        mkdir ConsoleApp
        cd ConsoleApp/

 - Execute the following .NET Core CLI commands to create a new console application, download dependencies, and run the .NET Core app.

   .. code-block:: bash

       dotnet new
       dotnet restore
       dotnet run

Install Entity Framework
------------------------

 - To add EF to your project, modify ``project.json`` so it matches the following sample.

    .. literalinclude:: /samples/Platforms/NetCore/ConsoleApp.SQLite/project.json
            :language: json
            :linenos:

 - Run ``dotnet restore`` again to install the new packages.

    .. code-block:: bash

        dotnet restore

 - Verify that Entity Framework is installed by running ``dotnet ef --help``.

    .. code-block:: bash

        dotnet ef --help


Create your model
-----------------

With this new project, you are ready to begin using Entity Framework.
The next steps will add code to configure and access a SQLite database file.

 - Create a new file called ``Model.cs``
    All classes in the following steps will be added to this file.

        .. literalinclude:: /samples/Platforms/NetCore/ConsoleApp.SQLite/Model.cs
            :language: c#
            :linenos:
            :lines: 1-6

 - Add a new class to represent the SQLite database.
    We will call this ``BloggingContext``. The call to ``UseSqlite()`` configures EF to point to a \*.db file.

        .. literalinclude:: /samples/Platforms/NetCore/ConsoleApp.SQLite/Model.cs
            :language: c#
            :linenos:
            :lines: 7-17
            :emphasize-lines: 1, 8

 - Add classes to represent tables.
    Note that we will be using foreign keys to associate many posts to one blog.

        .. literalinclude:: /samples/Platforms/NetCore/ConsoleApp.SQLite/Model.cs
            :language: c#
            :linenos:
            :lines: 18-35

 - To make sure the files are correct, you can compile the project on the command line by running ``dotnet build``

    .. code-block:: bash

        dotnet build

Create your database
--------------------

We can now use Entity Framework command line tools to create and manage the schema of the database.

 - Create the first migration.
    Execute the command below to generate your first migration.
    This will find our context and models, and generate a migration for us in a folder named ``Migrations/``

    .. code-block:: bash

        dotnet ef migrations add MyFirstMigration

 - Apply the migrations.
    You can now begin using the existing migration to create the database file and creates the tables.

    .. code-block:: bash

        dotnet ef database update

    This should create a new file ``blog.db`` in the output path. This SQLite file should now contain two empty tables.

.. note::

  When using relative paths with SQLite, the path will be relative to the application's
  main assembly. In this sample, the main binary is ``bin/Debug/netcoreapp1.0/ConsoleApp.dll``,
  so the SQLite database will be in ``bin/Debug/netcoreapp1.0/blog.db``

Use your model
--------------

Now that we have configured our model and created the database schema, we can use BloggingContext to create, update, and delete objects.

.. literalinclude:: /samples/Platforms/NetCore/ConsoleApp.SQLite/Program.cs
    :language: c#
    :linenos:

Start your app
--------------

Run the application from the command line.

    .. code-block:: bash

        dotnet run

After adding the new post, you can verify the data has been added by inspecting the SQLite database file, ``bin/Debug/netcoreapp1.0/blog.db``.
