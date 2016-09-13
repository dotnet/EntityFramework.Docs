Raw SQL Queries
===============

Entity Framework Core allows you to drop down to raw SQL queries when working with a relational database. This can be useful if the query you want to perform can't be expressed using LINQ, or if using a LINQ query is resulting in inefficient SQL being sent to the database.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Querying

Limitations
-----------

There are a couple of limitations to be aware of when using raw SQL queries:
 * SQL queries can only be used to return entity types that are part of your model. There is an enhancement on our backlog to `enable returning ad-hoc types from raw SQL queries <https://github.com/aspnet/EntityFramework/issues/1862>`_.
 * The SQL query must return data for all properties of the entity type.
 * The column names in the result set must match the column names that properties are mapped to. Note this is different from EF6.x where property/column mapping was ignored for raw SQL queries and result set column names had to match the property names.
 * The SQL query cannot contain related data. However, in many cases you can compose on top of the query using the ``Include`` operator to return related data (see `Including related data`_).

Basic raw SQL queries
---------------------

You can use the `FromSql` extension method to begin a LINQ query based on a raw SQL query.

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 13-15
        :dedent: 16
        :linenos:

Raw SQL queries can be used to execute a stored procedure.

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 20-22
        :dedent: 16
        :linenos:

Passing parameters
------------------

As with any API that accepts SQL, it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter placeholders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a ``DbParameter``.

The following example passes a single parameter to a stored procedure. While this may look like ``String.Format`` syntax, the supplied value is wrapped in a parameter and the generated parameter name inserted where the ``{0}`` placeholder was specified.

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 27-31
        :dedent: 16
        :linenos:

You can also construct a DbParameter and supply it as a parameter value. This allows you to use named parameters in the SQL query string

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 36-40
        :dedent: 16
        :linenos:

Composing with LINQ
-------------------

If the SQL query can be composed on in the database, then you can compose on top of the initial raw SQL query using LINQ operators. SQL queries that can be composed on being with the ``SELECT`` keyword.

The following example uses a raw SQL query that selects from a Table-Valued Function (TVF) and then composes on it using LINQ to perform filtering and sorting.

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 45-51
        :dedent: 16
        :linenos:

Including related data
^^^^^^^^^^^^^^^^^^^^^^

Composing with LINQ operators can be used to include related data in the query.

.. includesamplefile:: Querying/Querying/RawSQL/Sample.cs
        :language: c#
        :lines: 56-61
        :dedent: 16
        :linenos:
