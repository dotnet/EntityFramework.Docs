Testing with InMemory
=====================

This article covers how to use the InMemory provider to write efficient tests with minimal impact to the code being tested.

.. caution::
  Currently you need to use ``ServiceCollection`` and ``IServiceProvider`` to control the scope of the InMemory database, which adds complexity to your tests. In the next release after RC2, there will be improvements to make this easier, `see issue #3253 <https://github.com/aspnet/EntityFramework/issues/3253>`_ for more details.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Miscellaneous/Testing

When to use InMemory for testing
--------------------------------

The InMemory provider is useful when you want to test components using something that approximates connecting to the real database, without the overhead of actual database operations.

For example, consider the following service that allows application code to perform some operations related to blogs. Internally it uses a ``DbContext`` that connects to a SQL Server database. It would be useful to swap this context to connect to an InMemory database so that we can write efficient tests for this service without having to modify the code, or do a lot of work to create a test double of the context.

.. includesamplefile:: Miscellaneous/Testing/BusinessLogic/BlogService.cs
        :language: csharp
        :linenos:
        :lines: 6-29

InMemory is not a relational database
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

EF Core database providers do not have to be relational databases. InMemory is designed to be a general purpose database for testing, and is not designed to mimic a relational database.

Some examples of this include:
 * InMemory will allow you to save data that would violate referential integrity constraints in a relational database.
 * If you use DefaultValueSql(string) for a property in your model, this is a relational database API and will have no effect when running against InMemory.

.. tip::
  For many test purposes these difference will not matter. However, if you want to test against something that behaves more like a true relational database, then consider using `SQLite in-memory mode <http://www.sqlite.org/inmemorydb.html>`_.

Get your context ready
----------------------

Avoid configuring two database providers
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

In your tests you are going to externally configure the context to use the InMemory provider. If you are configuring a database provider by overriding ``OnConfiguring`` in your context, then you need to add some conditional code to ensure that you only configure the database provider if one has not already been configured.

.. note::
  If you are using ASP.NET Core, then you should not need this code since your database provider is configured outside of the context (in Startup.cs).

.. includesamplefile:: Miscellaneous/Testing/BusinessLogic/BloggingContext.cs
        :language: csharp
        :linenos:
        :lines: 17-23
        :emphasize-lines: 3

Add a constructor for testing
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The simplest way to enable testing with the InMemory provider is to modify your context to expose a constructor that accepts a ``DbContextOptions<TContext>``.

.. includesamplefile:: Miscellaneous/Testing/BusinessLogic/BloggingContext.cs
        :language: csharp
        :linenos:
        :lines: 7-14
        :emphasize-lines: 6-8

.. note::
  ``DbContextOptions<TContext>`` tells the context all of it's settings, such as which database to connect to. This is the same object that is built by running the OnConfiguring method in your context.

Writing tests
-------------

The key to testing with this provider is the ability to tell the context to use the InMemory provider, and control the scope of the in-memory database. Typically you want a clean database for each test method.

``DbContextOptions<TContext>`` exposes a ``UseInternalServiceProvider`` method that allows us to control the ``IServiceProvider`` the context will use. ``IServiceProvider`` is the container that EF will resolve all its services from (including the InMemory database instance). Typically, EF creates a single ``IServiceProvider`` for all contexts of a given type in an AppDomain - meaning all context instances share the same InMemory database instance. By allowing one to be passed in, you can control the scope of the InMemory database.

Here is an example of a test class that uses the InMemory database. Each test method creates a new ``DbContextOptions<TContext>`` with a new ``IServiceProvider``, meaning each method has its own InMemory database.

.. includesamplefile:: Miscellaneous/Testing/TestProject/BlogServiceTests.cs
        :language: csharp
        :linenos:

Sharing a database instance for read-only tests
-----------------------------------------------

If a test class has read-only tests that share the same seed data, then you can share the InMemory database instance for the whole class (rather than a new one for each method). This means you have a single  ``DbContextOptions<TContext>`` and ``IServiceProvider`` for the test class, rather than one for each test method.

.. includesamplefile:: Miscellaneous/Testing/TestProject/BlogServiceTestsReadOnly.cs
        :language: csharp
        :linenos:
