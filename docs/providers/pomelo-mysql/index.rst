MySQL (Provided by Pomelo)
==========================

This database provider allows Entity Framework Core to be used with MySQL. The provider is maintained as part of the `Pomelo Foundation Project <https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql>`_.

.. contents:: `In this article:`
    :depth: 2
    :local:

Install
-------

Install the Pomelo.EntityFrameworkCore.MySQL.

.. code-block:: text

    PM>  Install-Package Pomelo.EntityFrameworkCore.MySQL

Get Started
-----------

        .. includesamplefile:: Platforms/NetCore/ConsoleApp.MySQL/Model.cs
            :language: c#
            :linenos:
            :lines: 1-38
            :dedent: 4

        .. includesamplefile:: Platforms/NetCore/ConsoleApp.MySQL/Program.cs
            :language: c#
            :linenos:
            :lines: 1-25
            :dedent: 4

Supported Database Engines
--------------------------

  * MySQL

Supported Platforms
-------------------

  * Full .NET (4.5.1 onwards)
  * .NET Core
  * Mono (4.2.0 onwards)
