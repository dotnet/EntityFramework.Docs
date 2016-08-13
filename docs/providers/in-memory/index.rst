InMemory (for Testing)
======================

This database provider allows Entity Framework Core to be used with an in-memory database. This is useful when testing code that uses Entity Framework Core. The provider is maintained as part of the `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_.

.. contents:: `In this article:`
    :depth: 2
    :local:

Install
-------

Install the `Microsoft.EntityFrameworkCore.InMemory NuGet package <https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/>`_.

.. code-block:: text

    PM>  Install-Package Microsoft.EntityFrameworkCore.InMemory

Get Started
-----------

The following resources will help you get started with this provider.
  * :doc:`/miscellaneous/testing`
  * `UnicornStore Sample Application Tests <https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs>`_

Supported Database Engines
--------------------------

  * Built-in in-memory database (designed for testing purposes only)

Supported Platforms
-------------------

  * Full .NET (4.5.1 onwards)
  * .NET Core
  * Mono (4.2.0 onwards)
  * Universal Windows Platform
