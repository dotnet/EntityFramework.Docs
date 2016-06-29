SQLite
======

This database provider allows Entity Framework Core to be used with SQLite. The provider is maintained as part of the `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_.

.. toctree::
    :hidden:

    limitations

.. contents:: `In this article:`
    :depth: 2
    :local:

Install
-------

Install the `Microsoft.EntityFrameworkCore.SQLite NuGet package <hhttps://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SQLite/>`_.

.. code-block:: text

    PM>  Install-Package Microsoft.EntityFrameworkCore.SQLite

Get Started
-----------

The following resources will help you get started with this provider.
  * :doc:`/platforms/uwp/getting-started`
  * :doc:`/platforms/netcore/new-db-sqlite`
  * `Unicorn Clicker Sample Application <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornClicker/UWP>`_
  * `Unicorn Packer Sample Application <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornPacker>`_

Supported Database Engines
--------------------------

  * SQLite (3.7 onwards)

Supported Platforms
-------------------

  * Full .NET (4.5.1 onwards)
  * .NET Core
  * Mono (4.2.0 onwards)
  * Universal Windows Platform

Limitations
-----------

See :doc:`limitations` for some important limitations of the SQLite provider.
