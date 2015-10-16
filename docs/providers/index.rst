Database Providers
==================

The following providers are either available or being developed:
 - `EntityFramework.SqlServer`_
 - `EntityFramework.SQLite`_
 - `EntityFramework.InMemory`_
 - `EntityFramework.SqlServerCompact40`_
 - `EntityFramework.SqlServerCompact35`_
 - `EntityFramework.Npgsql`_

EntityFramework.SqlServer
-------------------------

**Database Engine:** Microsoft SQL Server (2008 onwards)

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 and dnxcore50), Mono (4.2.0 onwards)

.. caution::
    Using this provider on Mono will make use of the Mono SQL Client implementation, which has a number of known issues. For example, it does not support secure connections (SSL).

**Status:** `Pre-release EntityFramework.SqlServer package on NuGet.org <https://www.nuget.org/packages/EntityFramework.SqlServer>`_ that supports the latest EF7 pre-release

**Project Site:** `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_

**Getting Started:** See :doc:`/getting-started/full-dotnet` or :doc:`/getting-started/aspnet5` for a walkthrough that uses this provider. The `UnicornStore <https://github.com/rowanmiller/UnicornStore/tree/master/UnicornStore>`_ sample application also uses this provider.


EntityFramework.SQLite
----------------------

**Database Engine:** SQLite (3.7 onwards)

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 and dnxcore50), Mono (4.2.0 onwards), Universal Windows Platform (local development only)

**Status:** `Pre-release EntityFramework.SQLite package on NuGet.org <https://www.nuget.org/packages/EntityFramework.SQLite>`_ that supports the latest EF7 pre-release

**Project Site:** `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_

**Getting Started:** See :doc:`/getting-started/uwp`, :doc:`/getting-started/linux` or :doc:`/getting-started/osx` for walkthroughs that use this provider


EntityFramework.InMemory
------------------------

**Database Engine:** Built-in in-memory database (designed for testing purposes only)

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 and dnxcore50), Mono (4.2.0 onwards), Universal Windows Platform (local development only)

**Status:** `Pre-release EntityFramework.InMemory package on NuGet.org <https://www.nuget.org/packages/EntityFramework.InMemory>`_ that supports the latest EF7 pre-release

**Project Site:** `EntityFramework GitHub project <https://github.com/aspnet/EntityFramework>`_

**Getting Started:** See the `tests for the UnicornStore sample application <https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs>`_ for an example of using this provider.


EntityFramework.SqlServerCompact40
----------------------------------

**Database Engine:** SQL Server Compact Edition 4.0

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 only)

**Status:** `Pre-release EntityFramework.SqlServerCompact40 package on NuGet.org <https://www.nuget.org/packages/EntityFramework.SqlServerCompact40>`_ that supports the latest EF7 pre-release

**Project Site:** `ErikEJ/EntityFramework7.SqlServerCompact GitHub Project <https://github.com/ErikEJ/EntityFramework7.SqlServerCompact>`_

**Getting Started:** See the `documentation for this project <https://github.com/ErikEJ/EntityFramework7.SqlServerCompact/wiki/Using-EF7-with-SQL-Server-Compact-in-Traditional-.NET-Applications>`_

EntityFramework.SqlServerCompact35
----------------------------------

**Database Engine:** SQL Server Compact Edition 3.5

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 only)

**Status:** `Pre-release EntityFramework.SqlServerCompact35 package on NuGet.org <https://www.nuget.org/packages/EntityFramework.SqlServerCompact35>`_ that supports the latest EF7 pre-release

**Project Site:** `ErikEJ/EntityFramework7.SqlServerCompact GitHub Project <https://github.com/ErikEJ/EntityFramework7.SqlServerCompact>`_

**Getting Started:** See the `documentation for this project <https://github.com/ErikEJ/EntityFramework7.SqlServerCompact/wiki/Using-EF7-with-SQL-Server-Compact-in-Traditional-.NET-Applications>`_


EntityFramework.Npgsql
----------------------

**Database Engine:** PostgreSQL

**Platforms:** Full .NET (4.5 onwards), DNX/ASP.NET 5 (dnx451 and dnxcore50), Mono (4.2.0 onwards)

**Status:** `Pre-release EntityFramework7.Npgsql package on NuGet.org <https://www.nuget.org/packages/EntityFramework7.Npgsql/>`_ that supports the latest EF7 pre-release

**Project Site:** `Npgsql.org <http://www.npgsql.org>`_

**Getting Started:** See the `getting started documentation at the Npgsql site <http://www.npgsql.org/doc/ef7.html>`_
