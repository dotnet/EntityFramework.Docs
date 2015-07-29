Getting Started on OSX
======================

This walkthrough will create a simple console application using ASP.NET 5 and
the SQLite provider.

.. note::
    This article was written for OS X Mavericks or newer. It uses beta 6 of ASP.NET and EF7. 

    You can find nightly builds of the EF7 code base hosted on https://www.myget.org/F/aspnetvnext/api/v2/ but the code base is rapidly changing and we do not maintain up-to-date documentation for getting started.


In this article
    - `Prerequisites`_
    - `Install ASP.NET 5`_
    - `Create a new project`_
    - `Create your model`_
    - `Create your database`_
    - `Use your model`_
    - `Start your app`_

.. note:: `View this article's samples on GitHub <https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/x-plat/sample>`_.


Prerequisites
-------------

Minimum system requirements
 - Mono 4.0.2
 - OS X Mavericks

.. caution::
    **Known Issues**

    .. include:: x-plat/issues.rst

Install ASP.NET 5
-----------------

A summary of steps to install ASP.NET 5 are included below. For a more up-to-date guide, follow the steps for `Installing ASP.NET 5 on Mac OS X <http://docs.asp.net/en/latest/getting-started/installing-on-mac.html>`_. This will ensure you meet the following requirements.

The following steps will install `dnvm <https://github.com/aspnet/home#running-an-application>`_, a command-line tool for installing the .NET Execution environment.

 - Install `Homebrew <http://brew.sh>`_
 - Use brew to install Mono

    .. code-block:: console

        ~ $ brew install mono

 - Run the dnvm

    .. code-block:: console

        ~ $ curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh

 - Verify dnvm has the latest version
    
    .. code-block:: console

        ~ $ dnvm upgrade

If you have trouble installing dnvm, consult this `Getting Started guide <http://dotnet.github.io/core/getting-started/>`_.


.. include:: x-plat/guide.rst
