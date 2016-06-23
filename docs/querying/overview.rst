How Query Works
===============

Entity Framework Core uses Language Integrate Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries based on your derived context and entity classes.

.. contents:: `In this article:`
    :depth: 2
    :local:

The life of a query
-------------------

The following is a high level overview of the process each query goes through.

1. The LINQ query is processed by Entity Framework Core to build a representation that is ready to be processed by the database provider

  a. The result is cached so that this processing does not need to be done every time the query is executed

2. The result is passed to the database provider

  a. The database provider identifies which parts of the query can be evaluated in the database
  b. These parts of the query are translated to database specific query language (e.g. SQL for a relational database)
  c. One or more queries are sent to the database and the result set returned (results are values from the database, not entity instances)

3. For each item in the result set

  a. If this is a tracking query, EF checks if the data represents an entity already in the change tracker for the context instance

    * If so, the existing entity is returned
    * If not, a new entity is created, change tracking is setup, and the new entity is returned

  b. If this is a no-tracking query, EF checks if the data represents an entity already in the result set for this query

    * If so, the existing entity is returned
    * If not, a new entity is created and returned

When queries are executed
-------------------------

When you call LINQ operators, you are simply building up an in-memory representation of the query. The query is only sent to the database when the results are consumed.

The most common operations that result in the query being sent to the database are:
 * Iterating the results in a ``for`` loop
 * Using an operator such as ``ToList``, ``ToArray``, ``Single``, ``Count``
 * Databinding the results of a query to a UI
