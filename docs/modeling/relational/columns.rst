.. include:: _shared/relational-specific-note.txt

Column Mapping
==============

Column mapping identifies which column data should be queried from and saved to in the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, each property will be setup to map to a column with the same name as the property.

Data Annotations
----------------

You can use Data Annotations to configure the column to which a property is mapped.

.. includesamplefile:: Modeling/DataAnnotations/Samples/Relational/Column.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 3
        :linenos:

Fluent API
----------

You can use the Fluent API to configure the column to which a property is mapped.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relational/Column.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:
