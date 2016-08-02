Local SQLite on UWP
===================

In this walkthrough, you will build a Universal Windows Platform (UWP) application that performs basic data access against a local SQLite database using Entity Framework.

.. caution::
    **Avoid using anonymous types in LINQ queries on UWP**.
    Deploying a UWP application to the app store requires your application to be compiled with .NET Native. Queries with anonymous types have poor performance on .NET Native or may crash the application.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Platforms/UWP/UWP.SQLite

Prerequisites
-------------

The following items are required to complete this walkthrough:
    - Windows 10
    - `Visual Studio 2015 Update 3 <https://go.microsoft.com/fwlink/?LinkId=691129>`_
    - The latest version of `Windows 10 Developer Tools <https://dev.windows.com/en-us/downloads>`_

Create a new project
--------------------

* Open Visual Studio 2015
* :menuselection:`File --> New --> Project...`
* From the left menu select :menuselection:`Templates --> Visual C# --> Windows --> Universal`
* Select the **Blank App (Universal Windows)** project template
* Give the project a name and click **OK**

Upgrade Microsoft.NETCore.UniversalWindowsPlatform
--------------------------------------------------

Depending on your version of Visual Studio, the template may have generated your project with an old version of .NET Core for UWP.
EF Core requires ``Microsoft.NETCore.UniversalWindowsPlatform`` version **5.2.2** or greater.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Update-Package Microsoft.NETCore.UniversalWindowsPlatform``

Install Entity Framework
------------------------

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package Microsoft.EntityFrameworkCore.Sqlite``

Later in this walkthrough we will also be using some Entity Framework commands to maintain the database. So we will install the commands package as well.

* Run ``Install-Package Microsoft.EntityFrameworkCore.Tools –Pre``

Create your model
-----------------

Now it's time to define a context and entity classes that make up your model.

* :menuselection:`Project --> Add Class...`
* Enter *Model.cs* as the name and click **OK**
* Replace the contents of the file with the following code

.. includesamplefile:: Platforms/UWP/UWP.SQLite/Model.cs
    :language: csharp
    :linenos:

.. tip::
    In a real application you would put each class in a separate file and put the connection string in the ``App.Config`` file and read it out using ``ConfigurationManager``. For the sake of simplicity, we are putting everything in a single code file for this tutorial.

Create your database
--------------------

.. warning::
    **Known Issue in Preview 2**

    Using EF Tools on UWP projects does not work without manually adding binding redirects.

    * :menuselection:`File –> New –> File...`
    * From the left menu select :menuselection:`Visual C# -> General -> Text File`
    * Give the file the name "App.config"
    * Add the following contents to the file

    .. includesamplefile:: Platforms/UWP/UWP.SQLite/App.config
        :language: xml

    See `Issue #5471 <https://github.com/aspnet/EntityFramework/issues/5471>`_ for more details.

Now that you have a model, you can use migrations to create a database for you.

* :menuselection:`Tools –> NuGet Package Manager –> Package Manager Console`
* Run ``Add-Migration MyFirstMigration`` to scaffold a migration to create the initial set of tables for your model.

Since we want the database to be created on the device that the app runs on, we will add some code to apply any pending migrations to the local database on application startup. The first time that the app runs, this will take care of creating the local database for us.

* Right-click on **App.xaml** in **Solution Explorer** and select **View Code**
* Add the highlighted using to the start of the file

.. includesamplefile:: Platforms/UWP/UWP.SQLite/App.xaml.cs
        :language: c#
        :linenos:
        :lines: 1-6
        :emphasize-lines: 1

* Add the highlighted code to apply any pending migrations

.. includesamplefile:: Platforms/UWP/UWP.SQLite/App.xaml.cs
        :language: c#
        :linenos:
        :lines: 30-39
        :emphasize-lines: 6-9

.. tip::
    If you make future changes to your model, you can use the ``Add-Migration`` command to scaffold a new migration to apply the corresponding changes to the database. Any pending migrations will be applied to the local database on each device when the application starts.

    EF uses a ``__EFMigrationsHistory`` table in the database to keep track of which migrations have already been applied to the database.


Use your model
--------------

You can now use your model to perform data access.

* Open *MainPage.xaml*
* Add the page load handler and UI content highlighted below

.. includesamplefile:: Platforms/UWP/UWP.SQLite/MainPage.xaml
        :language: c#
        :linenos:
        :emphasize-lines: 9,12-22

Now we'll add code to wire up the UI with the database

* Right-click **MainPage.xaml** in **Solution Explorer** and select **View Code**
* Add the highlighted code from the following listing

.. includesamplefile:: Platforms/UWP/UWP.SQLite/MainPage.xaml.cs
        :language: c#
        :linenos:
        :lines: 23-49
        :emphasize-lines: 8-26

You can now run the application to see it in action.

* :menuselection:`Debug --> Start Without Debugging`
* The application will build and launch
* Enter a URL and click the **Add** button

.. image:: _static/create.png

.. image:: _static/list.png

Next steps
----------

Tada! You now have a simple UWP app running Entity Framework.

Check out the numerous articles in this documentation to learn more about Entity Framework's features.
