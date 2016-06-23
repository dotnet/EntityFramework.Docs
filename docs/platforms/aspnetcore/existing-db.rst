.. include:: /_shared/rc2-notice.txt

ASP.NET Core Application to Existing Database (Database First)
==============================================================

In this walkthrough, you will build an ASP.NET Core MVC application that performs basic data access using Entity Framework.  You will use reverse engineering to create an Entity Framework model based on an existing database.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/samples/Platforms/AspNetCore/AspNetCore.ExistingDb

Prerequisites
-------------

The following prerequisites are needed to complete this walkthrough:

  * Visual Studio 2015 Update 2
  * `ASP.NET Core RC2 Tools for Visual Studio <https://docs.asp.net/en/latest/getting-started/installing-on-windows.html>`_
  * `Blogging database`_

Blogging database
^^^^^^^^^^^^^^^^^

This tutorial uses a **Blogging** database on your LocalDb instance as the existing database.

.. include:: /platforms/_shared/create-blogging-database-vs.txt

Create a new project
--------------------

* Open Visual Studio 2015
* :menuselection:`File --> New --> Project...`
* From the left menu select :menuselection:`Templates --> Visual C# --> Web`
* Select the **ASP.NET Core Web Application (.NET Core)** project template
* Enter **EFGetStarted.AspNetCore.ExistingDb** as the name and click **OK**
* Wait for the **New ASP.NET Core Web Application** dialog to appear
* Ensure that **Authentication** is set to **No Authentication**
* Click **OK**


Install Entity Framework
-------------------------

To use EF Core, install the package for the database provider(s) you want to target. This walkthrough uses SQL Server. For a list of available providers see :doc:`/providers/index`.

* :menuselection:`Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package Microsoft.EntityFrameworkCore.SqlServer –Pre``

.. note::
    In ASP.NET Core projects the ``Install-Package`` will complete quickly and the package installation will occur in the background. You will see **(Restoring...)** appear next to **References** in **Solution Explorer** while the install occurs.

To enable reverse engineering from an existing database we need to install a couple of other packages too.

  * Run ``Install-Package Microsoft.EntityFrameworkCore.Tools –Pre``
  * Run ``Install-Package Microsoft.EntityFrameworkCore.SqlServer.Design –Pre``
  * Open **project.json**
  * Locate the ``tools`` section and add the highlighted lines as shown below

  .. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.NewDb/project.json
        :linenos:
        :lines: 26-33
        :emphasize-lines: 2-8

Reverse engineer your model
---------------------------

Now it's time to create the EF model based on your existing database.

* :menuselection:`Tools –> NuGet Package Manager –> Package Manager Console`
* Run the following command to create a model from the existing database

.. literalinclude:: _static/reverse-engineer-command.txt

.. caution::
  Note that the connection string is double quoted (with single quotes inside the double quotes). This is a workaround for a `known issue in RC2 <https://github.com/aspnet/EntityFramework/issues/5376>`_ for more details.

The reverse engineer process created entity classes and a derived context based on the schema of the existing database. The entity classes are simple C# objects that represent the data you will be querying and saving.


.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Models/Blog.cs
        :language: c#
        :linenos:

The context represents a session with the database and allows you to query and save instances of the entity classes.

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Models/BloggingContextUnmodified.txt
        :language: c#
        :linenos:

Register your context with dependency injection
-----------------------------------------------

The concept of dependency injection is central to ASP.NET Core. Services (such as ``BloggingContext``) are registered with dependency injection during application startup. Components that require these services (such as your MVC controllers) are then provided these services via constructor parameters or properties. For more information on dependency injection see the `Dependency Injection <http://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_ article on the ASP.NET site.

Remove inline context configuration
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

In ASP.NET Core, configuration is generally performed in **Startup.cs**. To conform to this pattern, we will move configuration of the database provider to **Startup.cs**.

  * Open **Models\\BloggingContext.cs**
  * Delete the lines of code highligted below

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Models/BloggingContextUnmodified.txt
        :language: c#
        :lines: 6-13
        :emphasize-lines: 3-7
        :linenos:

* Add the lines of code highligted below

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Models/BloggingContext.cs
        :language: c#
        :lines: 6-10
        :emphasize-lines: 3-5
        :linenos:

Register and configure your context in Startup.cs
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

In order for our MVC controllers to make use of ``BloggingContext`` we are going to register it as a service.

	* Open **Startup.cs**
	* Add the following ``using`` statements at the start of the file

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Startup.cs
        :language: c#
        :linenos:
        :lines: 1-2

Now we can use the ``AddDbContext`` method to register it as a service.

	* Locate the ``ConfigureServices`` method
	* Add the lines that are highlighted in the following code

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Startup.cs
        :language: c#
        :linenos:
        :lines: 30-34
        :emphasize-lines: 3-4

Create a controller
-------------------

Next, we'll add an MVC controller that will use EF to query and save data.

	* Right-click on the **Controllers** folder in **Solution Explorer** and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> Code`
	* Select the **Class** item template
	* Enter **BlogsController.cs** as the name and click **OK**
	* Replace the contents of the file with the following code

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Controllers/BlogsController.cs
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

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Views/Blogs/Index.cshtml
        :linenos:

We'll also add a view for the ``Create`` action, which allows the user to enter details for a new blog.

	* Right-click on the **Blogs** folder and select :menuselection:`Add --> New Item...`
	* From the left menu select :menuselection:`Installed --> ASP.NET`
	* Select the **MVC View Page** item template
	* Enter **Create.cshtml** as the name and click **Add**
	* Replace the contents of the file with the following code

.. literalinclude:: /samples/Platforms/AspNetCore/AspNetCore.ExistingDb/Views/Blogs/Create.cshtml
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

.. image:: _static/index-existing-db.png
