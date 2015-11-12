Getting Started on OSX
======================

This walkthrough will create a simple console application using ASP.NET 5 and
the SQLite provider.

.. contents:: `In this article:`
    :depth: 1
    :local:

.. include:: /sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/getting-started/x-plat/sample

.. include:: /rc1-notice.txt

Prerequisites
-------------

Minimum system requirements
 - OS X Mavericks

.. caution::
    **Known Issues**

    .. include:: x-plat/issues.txt

Install ASP.NET 5
-----------------

A summary of steps to install ASP.NET 5 are included below. For a more up-to-date guide, follow the steps for `Installing ASP.NET 5 on Mac OS X <http://docs.asp.net/en/latest/getting-started/installing-on-mac.html>`_. This will ensure you meet the following requirements.

The following steps will install `dnvm <https://github.com/aspnet/home#running-an-application>`_, a command-line tool for installing the .NET Execution environment.

 - Install `Homebrew <http://brew.sh>`_
 - Use brew to install ICU

    .. code-block:: console

        ~ $ brew install icu4c

 - Run the dnvm

    .. code-block:: console

        ~ $ curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh

 - Install the latest version of DNX. The ``-r`` selects the CoreCLR runtime. OSX also supports the Mono runtime, but it is not used in this tutorial.

    .. code-block:: console

        ~ $ dnvm upgrade -r coreclr

If you have trouble installing dnvm, consult `Installing ASP.NET 5 on Mac OS X <http://docs.asp.net/en/latest/getting-started/installing-on-mac.html>`_.


.. include:: x-plat/guide.txt
