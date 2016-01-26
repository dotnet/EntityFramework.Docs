Testing with the InMemory Provider
==================================

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/providers/in-memory/sample

When to use InMemory for testing
--------------------------------

The InMemory provider is useful when you want to test components using something that approximates connecting to the real database, without the overhead of actual database operations.

For example, consider the following service that allows application code to perform some operations related to blogs. Internally it uses a ``DbContext`` that connects to a SQL Server database. It would be useful to swap this context to connect to an InMemory database so that we can write efficient tests for this service without having to modify the code, or do a lot of work to create a test double of the context.

.. literalinclude:: sample\BusinessLogic\BlogService.cs
        :language: csharp
        :linenos:

Avoid configuring two database providers
----------------------------------------

In your tests you are going to externally configure the context to use the InMemory provider. If you are configuring a database provider by overriding ``OnConfiguring`` in your context, then you need to add some conditional code to ensure that you only configure the database provider if one has not already been configured.

.. note::
  If you are using ASP.NET Core, then you should not need this code since your database provider is configured outside of the context (in Startup.cs).

.. literalinclude:: sample\BusinessLogic\BloggingContext.cs
        :language: csharp
        :linenos:
        :emphasize-lines: 11

Controlling database and context scope
--------------------------------------

The key to testing with this provider is the ability to control the scope of the in-memory database. Typically you want a clean database for each test method.

.. caution::
  Currently you need to use ``ServiceCollection`` and ``IServiceProvider`` to control the scope of the database, which adds complexity to your tests. We have a `feature on our backlog <https://github.com/aspnet/EntityFramework/issues/3253>`_ to provide an easier mechanism for controlling the scope of InMemory databases.

Here is an example of a test class that uses the InMemory database. The important points are:
  * **Create a global ServiceCollection**, register the context as a service, and configure it to use the InMemory database.
  * **Create an IServiceProvider for each InMemory database you want** (using ``ServiceCollection.BuildServiceProvider``). Typically you want one database per test method.
  * **Create an IServiceScope for each context instance you want.** This allows you to use a clean context for seeding the database, running the test, verifying data, etc.

.. literalinclude:: sample\TestProject\BlogServiceTests.cs
        :language: csharp
        :linenos:

A simplification for read-only tests
------------------------------------

If a test class has read-only tests that share the same seed data, then you can share the InMemory database instance for the whole class (rather than a new one for each method).

.. literalinclude:: sample\TestProject\BlogServiceReadOnlyTests.cs
        :language: csharp
        :linenos:
