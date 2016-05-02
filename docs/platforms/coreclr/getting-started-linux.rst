.. include:: /_shared/rc1-notice.txt

Getting Started on Linux
========================

This walkthrough will create a simple console application using .NET Core and
the SQLite provider.

.. contents:: `In this article:`
    :depth: 1
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/platforms/coreclr/sample

Prerequisites
-------------

Minimum system requirements
 - Ubuntu, Debian or one of their derivatives

.. caution::
    **Known Issues**

    .. include:: issues.txt

    - On Linux, Microsoft.EntityFrameworkCore.Sqlite uses the operating
      system's shared SQLite library, ``libsqlite3``. This may
      not be installed by default.

      On Ubuntu 14, install the SQLite library with apt-get.

          .. code-block:: bash

              sudo apt-get install libsqlite3-dev

      On RedHat, install the SQLite library with yum.

          .. code-block:: bash

              sudo yum install sqlite-devel


Install .NET Core
-----------------

A summary of steps to install .NET Core are included below. For a more up-to-date guide, follow the steps for `Getting Started <http://dotnet.github.io/getting-started/>`_ on .NET Core's website.

 - Add the new apt-get feed

    .. code-block:: bash

        sudo sh -c 'echo "deb [arch=amd64] http://apt-mo.trafficmanager.net/repos/dotnet/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'
        sudo apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
        sudo apt-get update


 - Install .NET Core

    .. code-block:: bash

        sudo apt-get install dotnet
        

.. include:: guide.txt
