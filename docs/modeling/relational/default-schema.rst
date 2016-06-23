.. include:: /_shared/rc2-notice.txt

.. include:: _shared/relational-specific-note.txt

Default Schema
==============

The default schema is the database schema that objects will be created in if a schema is not explicitly configured for that object.

Conventions
-----------

By convention, the database provider will chose the most appropriate default schema. For example, Microsoft SQL Server will use the ``dbo`` schema and SQLite will not use a schema (since schemas are not supported in SQLite).

Data Annotations
----------------

You can not set the default schema using Data Annotations.

Fluent API
----------

You can use the Fluent API to specify a default schema.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relational/DefaultSchema.cs
        :language: c#
        :lines: 5-13
        :emphasize-lines: 7
        :linenos:
