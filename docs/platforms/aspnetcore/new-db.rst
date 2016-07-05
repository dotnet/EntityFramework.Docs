ASP.NET Core Application to New Database
========================================

In this walkthrough, you will build an ASP.NET Core MVC application that performs basic data access using Entity Framework. You will use migrations to create the database from your model.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Platforms/AspNetCore/AspNetCore.NewDb

Prerequisites
-------------

The following prerequisites are needed to complete this walkthrough:

	* `Visual Studio 2015 Update 3 <https://go.microsoft.com/fwlink/?LinkId=691129>`_
	* `.NET Core for Visual Studio <https://go.microsoft.com/fwlink/?LinkId=817245>`_

Create a new project
--------------------

  * Open Visual Studio 2015
  * :menuselection:`File --> New --> Project...`
  * From the left menu select :menuselection:`Templates --> Visual C# --> Web`
  * Select the **ASP.NET Core Web Application (.NET Core)** project template
  * Enter **EFGetStarted.AspNetCore.NewDb** as the name and click **OK**
  * Wait for the **New ASP.NET Core Web Application** dialog to appear
  * Select the **Web Application** template and ensure that **Authentication** is set to **No Authentication**
  * Click **OK**

.. caution::
    If you use **Individual User Accounts** instead of **None** for **Authentication** then an Entity Framework model will be added to your project in `Models\\IdentityModel.cs`. Using the techniques you will learn in this walkthrough, you can choose to add a second model, or extend this existing model to contain your entity classes.

Install Entity Framework
------------------------

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package Microsoft.EntityFrameworkCore.SqlServer``

.. note::
    In ASP.NET Core projects the ``Install-Package`` command will complete quickly and the package installation will occur in the background. You will see **(Restoring...)** appear next to **References** in **Solution Explorer** while the install occurs.

Later in this walkthrough we will also be using some Entity Framework commands to maintain the database. So we will install the commands package as well.

* Run ``Install-Package Microsoft.EntityFrameworkCore.Tools –Pre``
* Open **project.json**
* Locate the ``tools`` section and add the ``ef`` command as shown below

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/project.json
      :linenos:
      :lines: 29-33
      :emphasize-lines: 2

Create your model
-----------------

Now it's time to define a context and entity classes that make up your model.

	* Right-click on the project in **Solution Explorer** and select :menuselection:`Add --> New Folder`
	* Enter **Models** as the name of the folder
	* Right-click on the **Models** folder and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> Code`
	* Select the **Class** item template
	* Enter **Model.cs** as the name and click **OK**
	* Replace the contents of the file with the following code

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Models/Model.cs
        :language: c#
        :linenos:

.. note::
    In a real application you would typically put each class from your model in a separate file. For the sake of simplicity, we are putting all the classes in one file for this tutorial.

Register your context with dependency injection
-----------------------------------------------

The concept of dependency injection is central to ASP.NET Core. Services (such as ``BloggingContext``) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the `Dependency Injection <http://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_ article on the ASP.NET site.

In order for our MVC controllers to make use of ``BloggingContext`` we are going to register it as a service.

	* Open **Startup.cs**
	* Add the following ``using`` statements at the start of the file

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Startup.cs
        :language: c#
        :linenos:
        :lines: 1-2

Now we can use the ``AddDbContext`` method to register it as a service.

	* Locate the ``ConfigureServices`` method
	* Add the lines that are highlighted in the following code

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Startup.cs
        :language: c#
        :linenos:
        :lines: 30-33
        :emphasize-lines: 3-4

Create your database
--------------------

Now that you have a model, you can use migrations to create a database for you.

* :menuselection:`Tools –> NuGet Package Manager –> Package Manager Console`
* Run ``Add-Migration MyFirstMigration`` to scaffold a migration to create the initial set of tables for your model.
* Run ``Update-Database`` to apply the new migration to the database. Because your database doesn't exist yet, it will be created for you before the migration is applied.

.. tip::
    If you make future changes to your model, you can use the ``Add-Migration`` command to scaffold a new migration to make the corresponding schema changes to the database. Once you have checked the scaffolded code (and made any required changes), you can use the ``Update-Database`` command to apply the changes to the database.

    EF uses a ``__EFMigrationsHistory`` table in the database to keep track of which migrations have already been applied to the database.

Create a controller
-------------------

Next, we'll add an MVC controller that will use EF to query and save data.

	* Right-click on the **Controllers** folder in **Solution Explorer** and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> Server-side`
	* Select the **Class** item template
	* Enter **BlogsController.cs** as the name and click **OK**
	* Replace the contents of the file with the following code

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Controllers/BlogsController.cs
        :language: c#
        :linenos:

You'll notice that the controller takes a ``BloggingContext`` as a constructor parameter. ASP.NET dependency injection will take care of passing an instance of ``BloggingContext`` into your controller.

The controller contains an ``Index`` action, which displays all blogs in the database, and a ``Create`` action, which inserts a new blogs into the database.

Create views
------------

Now that we have a controller it's time to add the views that will make up the user interface.

We'll start with the view for our ``Index`` action, that displays all blogs.

	* Right-click on the **Views** folder in **Solution Explorer** and select :menuselection:`Add --> New Folder`
	* Enter **Blogs** as the name of the folder
	* Right-click on the **Blogs** folder and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> ASP.NET`
	* Select the **MVC View Page** item template
	* Enter **Index.cshtml** as the name and click **Add**
	* Replace the contents of the file with the following code

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Views/Blogs/Index.cshtml
        :linenos:

We'll also add a view for the ``Create`` action, which allows the user to enter details for a new blog.

	* Right-click on the **Blogs** folder and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> ASP.NET Core`
	* Select the **MVC View Page** item template
	* Enter **Create.cshtml** as the name and click **Add**
	* Replace the contents of the file with the following code

.. includesamplefile:: Platforms/AspNetCore/AspNetCore.NewDb/Views/Blogs/Create.cshtml
        :linenos:

Run the application
-------------------

You can now run the application to see it in action.

	* :menuselection:`Debug --> Start Without Debugging`
	* The application will build and open in a web browser
	* Navigate to **/Blogs**
	* Click **Create New**
	* Enter a **Url** for the new blog and click **Create**

.. image:: _static/create.png

.. image:: _static/index-new-db.png
