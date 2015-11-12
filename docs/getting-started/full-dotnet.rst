Getting Started on Full .NET (Console, WinForms, WPF, etc.)
===========================================================

In this walkthrough, you will build a console application that performs basic data access against a Microsoft SQL Server database using Entity Framework.

In this article:
	- `Ensure NuGet 2.8.6 or later`_
	- `Create a new project`_
	- `Install Entity Framework`_
	- `Create your model`_
	- `Create your database`_
	- `Use your model`_

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/full-dotnet/sample

.. include:: /_shared/rc1-notice.txt

Ensure NuGet 2.8.6 or later
---------------------------

Installing EF7 requires NuGet 2.8.6 (or higher). Make sure you restart Visual Studio after installing the update.

- **Visual Studio 2015** - No updates needed, a compatible version of NuGet is included.
- **Visual Studio 2013** - `Install the latest NuGet for VS2013 <https://visualstudiogallery.msdn.microsoft.com/4ec1526c-4a8c-4a84-b702-b21a8f5293ca>`_.

.. note::
    NuGet version numbers can be confusing, while the required release is branded 2.8.6 the product version of the extension is 2.8.60610.xxx.

Create a new project
--------------------

* Open Visual Studio (this walkthrough uses 2015 but you can use any version from 2013 onwards)
* :menuselection:`File --> New --> Project...`
* From the left menu select :menuselection:`Templates --> Visual C# --> Windows`
* Select the **Console Application** project template
* Ensure you are targeting **.NET Framework 4.5.1** or later
* Give the project a name and click **OK**

Install Entity Framework
----------------------------------------
To use EF7 you install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package EntityFramework.MicrosoftSqlServer –Pre``

Later in this walkthrough we will also be using some Entity Framework commands to maintain the database. So we will install the commands package as well.

* Run ``Install-Package EntityFramework.Commands –Pre``

Create your model
-----------------

Now it's time to define a context and entity classes that make up your model.

* :menuselection:`Project --> Add Class...`
* Enter *Model.cs* as the name and click **OK**
* Replace the contents of the file with the following code

.. note::
    Notice the ``OnConfiguring`` method (new in EF7) that is used to specify the provider to use and, optionally, other configuration too.

.. literalinclude:: full-dotnet/sample/EFGetStarted.ConsoleApp/Model.cs
        :language: c#
        :linenos:

.. tip::
    In a real application you would typically put each class from your model in a separate file. For the sake of simplicity, we are putting all the classes in one file for this tutorial.

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

.. literalinclude:: full-dotnet/sample/EFGetStarted.ConsoleApp/Program.cs
        :language: c#
        :linenos:

* :menuselection:`Debug --> Start Without Debugging`

You will see that one blog is saved to the database and then the details of all blogs are printed to the console.

.. image:: full-dotnet/_static/console.png
