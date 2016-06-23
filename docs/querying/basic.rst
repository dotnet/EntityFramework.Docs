Basic Query
===========

Entity Framework Core uses Language Integrate Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries based on your derived context and entity classes.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Querying

101 LINQ samples
----------------

This page shows a few examples to achieve common tasks with Entity Framework Core. For an extensive set of samples showing what is possible with LINQ, see `101 LINQ Samples <https://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b>`_.

Loading all data
----------------

.. includesamplefile:: Querying/Querying/Basics/Sample.cs
        :language: c#
        :lines: 9-12
        :dedent: 12
        :linenos:

Loading a single entity
-----------------------

.. includesamplefile:: Querying/Querying/Basics/Sample.cs
        :language: c#
        :lines: 14-18
        :dedent: 12
        :linenos:

Filtering
---------

.. includesamplefile:: Querying/Querying/Basics/Sample.cs
        :language: c#
        :lines: 20-25
        :dedent: 12
        :linenos:
