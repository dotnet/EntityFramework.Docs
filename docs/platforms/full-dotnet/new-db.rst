.. include:: /_shared/rc2-notice.txt

Console Application to New Database
===================================

In this walkthrough, you will build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework. You will use migrations to create the database from your model.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/platforms/full-dotnet/sample

Prerequisites
-------------

The following prerequisites are needed to complete this walkthrough:

* Visual Studio 2015 Update 2
* `Latest version of NuGet Package Manager <https://visualstudiogallery.msdn.microsoft.com/5d345edc-2e2d-4a9c-b73b-d53956dc458d>`_
* `Latest version of Windows PowerShell <https://www.microsoft.com/en-us/download/details.aspx?id=40855>`_

Create a new project
--------------------

* Open Visual Studio 2015
* :menuselection:`File --> New --> Project...`
* From the left menu select :menuselection:`Templates --> Visual C# --> Windows`
* Select the **Console Application** project template
* Ensure you are targeting **.NET Framework 4.5.1** or later
* Give the project a name and click **OK**

Install Entity Framework
------------------------

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package Microsoft.EntityFrameworkCore.SqlServer –Pre``

Later in this walkthrough we will also be using some Entity Framework commands to maintain the database. So we will install the commands package as well.

* Run ``Install-Package Microsoft.EntityFrameworkCore.Tools –Pre``

Create your model
-----------------

Now it's time to define a context and entity classes that make up your model.

* :menuselection:`Project --> Add Class...`
* Enter *Model.cs* as the name and click **OK**
* Replace the contents of the file with the following code

.. literalinclude:: sample/EFGetStarted.ConsoleApp.NewDb/Model.cs
        :language: c#
        :linenos:

.. tip::
    In a real application you would put each class in a separate file and put the connection string in the ``App.Config`` file and read it out using ``ConfigurationManager``. For the sake of simplicity, we are putting everything in a single code file for this tutorial.

Create your database
--------------------

Now that you have a model, you can use migrations to create a database for you.

* :menuselection:`Tools –> NuGet Package Manager –> Package Manager Console`
* Run ``Add-Migration MyFirstMigration`` to scaffold a migration to create the initial set of tables for your model.
* Run ``Update-Database`` to apply the new migration to the database. Because your database doesn't exist yet, it will be created for you before the migration is applied.

.. tip::
    If you make future changes to your model, you can use the ``Add-Migration`` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the ``Update-Database`` command to apply the changes to the database.

    EF uses a ``__EFMigrationsHistory`` table in the database to keep track of which migrations have already been applied to the database.

Use your model
--------------

You can now use your model to perform data access.

* Open *Program.cs*
* Replace the contents of the file with the following code

.. literalinclude:: sample/EFGetStarted.ConsoleApp.NewDb/Program.cs
        :language: c#
        :linenos:

* :menuselection:`Debug --> Start Without Debugging`

You will see that one blog is saved to the database and then the details of all blogs are printed to the console.

.. image:: _static/output-new-db.png
