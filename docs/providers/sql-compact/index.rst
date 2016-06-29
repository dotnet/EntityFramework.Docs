Microsoft SQL Server Compact Edition
====================================

This database provider allows Entity Framework Core to be used with SQL Server Compact Edition. The provider is maintained as part of the `ErikEJ/EntityFramework.SqlServerCompact GitHub Project <https://github.com/ErikEJ/EntityFramework.SqlServerCompact>`_.


.. contents:: `In this article:`
    :depth: 2
    :local:

Install
-------

To work with SQL Server Compact Edition 4.0, install the `EntityFrameworkCore.SqlServerCompact40 NuGet package <https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact40>`_.

.. code-block:: text

    PM>  Install-Package EntityFrameworkCore.SqlServerCompact40

To work with SQL Server Compact Edition 3.5, install the `EntityFrameworkCore.SqlServerCompact35 <https://www.nuget.org/packages/EntityFrameworkCore.SqlServerCompact35>`_.

.. code-block:: text

    PM>  Install-Package EntityFrameworkCore.SqlServerCompact35

Get Started
-----------

See the `getting started documentation on the project site <https://github.com/ErikEJ/EntityFramework.SqlServerCompact/wiki/Using-EF-Core-with-SQL-Server-Compact-in-Traditional-.NET-Applications>`_

Supported Database Engines
--------------------------

  * SQL Server Compact Edition 3.5
  * SQL Server Compact Edition 4.0

Supported Platforms
-------------------

  * Full .NET (4.5.1 onwards)
