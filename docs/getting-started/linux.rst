Getting Started on Linux
========================

This walkthrough will create a simple console application using ASP.NET 5 and
the SQLite provider.

.. note::
    This article was written for beta 6 of ASP.NET and EF7. 

    You can find nightly builds of the EF7 code base hosted on https://www.myget.org/F/aspnetvnext/api/v2/ but the code base is rapidly changing and we do not maintain up-to-date documentation for getting started.


.. contents:: `In this article:`
    :depth: 1
    :local:

.. note:: `View this article's samples on GitHub <https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/x-plat/sample>`_.


Prerequisites
-------------

Minimum system requirements
 - Mono 4.0.2
 - Ubuntu, Debian or one of their derivatives

.. caution::
    **Known Issues**

    .. include:: x-plat/issues.rst


Install ASP.NET 5
-----------------

A summary of steps to install ASP.NET 5 are included below. For a more up-to-date guide, follow the steps for `Installing ASP.NET 5 on Linux <http://docs.asp.net/en/latest/getting-started/installing-on-linux.html>`_. This will ensure you meet the following requirements.

The following steps will install `dnvm <https://github.com/aspnet/home#running-an-application>`_, a command-line tool for installing the .NET Execution environment.

 - Install the required libraries

    .. code-block:: console

        ~ $ sudo apt-get install libunwind8 libssl-dev unzip

 - Install mono.

    .. code-block:: console

        ~ $ sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
        ~ $ echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
        ~ $ sudo apt-get update
        ~ $ sudo apt-get install mono-complete

 - Import required certificates for Nuget

    .. code-block:: console

        ~ $ mozroots --import --sync

 - Run the dnvm

    .. code-block:: console

        ~ $ curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh

 - Verify dnvm has the latest version
    
    .. code-block:: console

        ~ $ dnvm upgrade

If you have trouble installing dnvm, consult this `Getting Started guide <http://dotnet.github.io/core/getting-started/>`_.


.. include:: x-plat/guide.rst