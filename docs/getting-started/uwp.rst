Getting Started on Universal Windows Platform
=============================================

In this walkthrough, you will build a Universal Windows Platform (UWP) application that performs basic data access against a local SQLite database using Entity Framework.

.. caution::
    **Pre-releases of EF7 can be used on UWP but there are a number of known issues that you need to workaround. Look out for caution boxes (such as this one) that provide details of required workarounds.**

    We'll be working to address these issues in upcoming releases. In fact, some of them are already fixed in our working code base.

In this article:
	- `Prerequisites`_
	- `Create a new project`_
	- `Install Entity Framework`_
	- `Create your model`_
	- `Create your database`_
	- `Use your model`_

`View this article's samples on GitHub <https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/uwp/sample>`_.

.. caution::
    **This walkthrough uses EF 7.0.0-beta6. Version 7.0.0-beta7 is available on NuGet.org, but some issues prevent it from being installed in a UWP application.**

    You can find nightly builds of the EF7 code base hosted on https://www.myget.org/F/aspnetvnext/api/v2/ but the code base is rapidly changing and we do not maintain up-to-date documentation for getting started.

Prerequisites
-------------

The following items are required to complete this walkthrough:
    - Windows 10 RTM
    - Visual Studio 2015 RTM with Window 10 Developer Tools

.. tip::
    If you already have Visual Studio 2015 RTM installed without the Windows 10 Developer Tools, you can add these tools to your existing Visual Studio installation. You can `run the installer <http://go.microsoft.com/fwlink/?LinkID=619615>`_, or open **Programs and Features** from **Control Panel**, select **Visual Studio** and click **Change**. Then in setup, click **Modify** and select the **Tools for Universal Windows Apps**.


Create a new project
--------------------

* Open Visual Studio 2015
* :menuselection:`File --> New --> Project...`
* From the left menu select :menuselection:`Templates --> Visual C# --> Windows --> Universal`
* Select the **Blank App (Universal Windows)** project template
* Give the project a name and click **OK**

Install Entity Framework
----------------------------------------
To use EF7 you install the package for the database provider(s) you want to target. This walkthrough uses SQLite. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package EntityFramework.SQLite –Version 7.0.0-beta6``

Later in this walkthrough we will also be using some Entity Framework commands to maintain the database. So we will install the commands package as well.

* Run ``Install-Package EntityFramework.Commands –Version 7.0.0-beta6``
* Run ``Install-Package Microsoft.CodeAnalysis.CSharp –Version 1.0.0``

.. caution::
    **Note that the commands explicitly install EF 7.0.0-beta6.** Version 7.0.0-beta7 is available on NuGet.org, but some issues prevent it from being installed in a UWP application.

    Needing to install the **Microsoft.CodeAnalysis.CSharp** package is a workaround for an issue in Beta 6. This will not be required in later releases.

Create your model
-----------------

Now it's time to define a context and entity classes that make up your model.

* :menuselection:`Project --> Add Class...`
* Enter *Model.cs* as the name and click **OK**
* Replace the contents of the file with the following code

.. caution::
    The ``try``/``catch`` code to set ``databaseFilePath`` is a temporary workaround to enable migrations to be added at design time. When the application runs, ``databaseFilePath`` will always be under ``ApplicationData.Current.LocalFolder.Path``. However, that API can not be called when migrations creates the context at design time in Visual Studio. The database is never accessed when adding migrations, so we just return a relative file path that will never be used.

.. note::
    Notice the ``OnConfiguring`` method (new in EF7) that is used to specify the provider to use and, optionally, other configuration too.

.. literalinclude:: uwp/sample/EFGetStarted.UWP/Model.cs
    :language: c#
    :linenos:

Create your database
--------------------

Now that you have a model, you can use migrations to create a database for you.

* :menuselection:`Build -> Build Solution`
* :menuselection:`Tools –> NuGet Package Manager –> Package Manager Console`
* Run ``Add-Migration MyFirstMigration`` to scaffold a migration to create the initial set of tables for your model.

.. caution::
    Notice that you need to manually build the solution before running the ``Add-Migration`` command. The command does invoke the build operation on the project, but we are currently investigating why this does not result in the correct assemblies being outputted.

.. caution::
    Due to a bug in the migration scaffolder in Beta6 you will need to manually edit the generated migration.

    Remove (or comment out) the two calls to ``.Annotation("Sqlite:Autoincrement", true)`` as highlighted in the following code listing

    .. literalinclude:: uwp/sample/EFGetStarted.UWP/Migrations/20150729201928_MyFirstMigration.cs
        :language: c#
        :linenos:
        :lines: 10-31
        :emphasize-lines: 7-8,19-20

Since we want the database to be created on the device that the app runs on, we will add some code to apply any pending migrations to the local database on application startup. The first time that the app runs, this will take care of creating the local database for us.

* Right-click on **App.xaml** in **Solution Explorer** and select **View Code**
* Add the highlighted lines of code from the following listing

.. literalinclude:: uwp/sample/EFGetStarted.UWP/App.xaml.cs
        :language: c#
        :linenos:
        :lines: 1-25
        :emphasize-lines: 2,21-24

.. tip::
    If you make future changes to your model, you can use the ``Add-Migration`` command to scaffold a new migration to apply the corresponding changes to the database. Any pending migrations will be applied to the local database on each device when the application starts.

Use your model
--------------

You can now use your model to perform data access.

* Open *MainPage.xaml*
* Add the page load handler and UI content highlighted below

.. literalinclude:: uwp/sample/EFGetStarted.UWP/MainPage.xaml
        :language: c#
        :linenos:
        :emphasize-lines: 9,12-22

Now we'll add code to wire up the UI with the database

* Right-click **MainPage.xaml** in **Solution Explorer** and select **View Code**
* Add the highlighted code from the following listing

.. literalinclude:: uwp/sample/EFGetStarted.UWP/MainPage.xaml.cs
        :language: c#
        :linenos:
        :lines: 23-49
        :emphasize-lines: 8-26

You can now run the application to see it in action.

* :menuselection:`Debug --> Start Without Debugging`
* The application will build and launch
* Enter a URL and click the **Add** button

.. image:: uwp/_static/create.png

.. image:: uwp/_static/list.png
