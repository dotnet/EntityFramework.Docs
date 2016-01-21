Microsoft SQL Server
====================

.. contents:: `In this article:`
    :depth: 2
    :local:

Getting Started
---------------

The following tutorials use this provider:
  * :doc:`/getting-started/full-dotnet/index`
  * :doc:`/getting-started/aspnet5/index`

The following sample applications use this provider:
  * `UnicornStore <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornStore>`_

Supported Database Engines
--------------------------

Microsoft SQL Server (2008 onwards)

Supported Platforms
-------------------

 * Full .NET (4.5.1 onwards)
 * DNX/ASP.NET 5 (dnx451 and dnxcore50)
 * Mono (4.2.0 onwards)

.. caution::
    Using this provider on Mono will make use of the Mono SQL Client implementation, which has a number of known issues. For example, it does not support secure connections (SSL).

Status
------

`Pre-release EntityFramework.MicrosoftSqlServer package on NuGet.org <https://www.nuget.org/packages/EntityFramework.MicrosoftSqlServer>`_

Project Site
------------

`EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_
