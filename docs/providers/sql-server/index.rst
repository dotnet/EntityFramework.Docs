Microsoft SQL Server
====================

.. contents:: `In this article:`
    :depth: 2
    :local:

Getting Started
---------------

The following tutorials use this provider:
  * :doc:`/platforms/full-dotnet/index`
  * :doc:`/platforms/aspnetcore/index`

The following sample applications use this provider:
  * `UnicornStore <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornStore>`_

Supported Database Engines
--------------------------

Microsoft SQL Server (2008 onwards)

Supported Platforms
-------------------

 * Full .NET (4.5.1 onwards)
 * .NET Core
 * Mono (4.2.0 onwards)

.. caution::
    Using this provider on Mono will make use of the Mono SQL Client implementation, which has a number of known issues. For example, it does not support secure connections (SSL).

Status
------

`Microsoft.EntityFrameworkCore.SqlServer package available on NuGet.org <https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer>`_

Project Site
------------

`EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_
