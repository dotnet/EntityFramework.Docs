Client vs. Server Evaluation
============================

Entity Framework Core supports parts of the query being evaluated on the client and parts of it being pushed to the database. It is up to the database provider to determine which parts of the query will be evaluated in the database.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Querying

Client eval
-----------

In the following example a helper method is used to standardize URLs for blogs that are returned from a SQL Server database. Because the SQL Server provider has no insight into how this method is implemented, it is not possible to translate it into SQL. All other aspects of the query are evaluated in the database, but passing the returned ``URL`` through this method is performed on the client.

.. includesamplefile:: Querying/Querying/ClientEval/Sample.cs
        :language: c#
        :lines: 24-31
        :emphasize-lines: 6
        :dedent: 16
        :linenos:

.. includesamplefile:: Querying/Querying/ClientEval/Sample.cs
        :language: c#
        :lines: 8-18
        :dedent: 8
        :linenos:

Disabling client evaluation
---------------------------

While client evaluation can be very useful, in some instances it can result in poor performance. Consider the following query, where the helper method is now used in a filter. Because this can't be performed in the database, all the data is pulled into memory and then the filter is applied on the client. Depending on the amount of data, and how much of that data is filtered out, this could result in poor performance.

.. includesamplefile:: Querying/Querying/ClientEval/Sample.cs
        :language: c#
        :lines: 36-38
        :dedent: 16
        :linenos:

By default, EF Core will log a warning when client evaluation is performed. See :doc:`/miscellaneous/logging` for more information on viewing logging output. You can change the behavior when client evaluation occurs to either throw or do nothing. This is done when setting up the options for your context - typically in ``DbContext.OnConfiguring``, or in ``Startup.cs`` if you are using ASP.NET Core.

.. includesamplefile:: Querying/Querying/ClientEval/ThrowOnClientEval/BloggingContext.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 5
        :dedent: 8
        :linenos:
