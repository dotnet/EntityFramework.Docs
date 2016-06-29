Microsoft SQL Server
====================

This database provider allows Entity Framework Core to be used with Microsoft SQL Server (including SQL Azure). The provider is maintained as part of the `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_.

.. contents:: `In this article:`
    :depth: 2
    :local:

Install
-------

Install the `Microsoft.EntityFrameworkCore.SqlServer NuGet package <hhttps://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/>`_.

.. code-block:: text

    PM>  Install-Package Microsoft.EntityFrameworkCore.SqlServer

Get Started
-----------

The following resources will help you get started with this provider.
  * :doc:`/platforms/full-dotnet/index`
  * :doc:`/platforms/aspnetcore/index`
  * `UnicornStore Sample Application <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornStore>`_

Supported Database Engines
--------------------------

  * Microsoft SQL Server (2008 onwards)

Supported Platforms
-------------------

  * Full .NET (4.5.1 onwards)
  * .NET Core
  * Mono (4.2.0 onwards)

   .. caution::
      Using this provider on Mono will make use of the Mono SQL Client implementation, which has a number of known issues. For example, it does not support secure connections (SSL).
