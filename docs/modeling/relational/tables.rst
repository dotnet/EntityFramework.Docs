.. include:: /_shared/rc2-notice.txt

.. include:: _shared/relational-specific-note.txt

Table Mapping
=============

Table mapping identifies which table data should be queried from and saved to in the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, each entity will be setup to map to a table with the same name as the ``DbSet<TEntity>`` property that exposes the entity on the derived context. If no ``DbSet<TEntity>`` is included for the given entity, the class name is used.

Data Annotations
----------------

You can use Data Annotations to configure the table that a type maps to.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/Relational/Table.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 1
        :linenos:

You can also specify a schema that the table belongs to.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/Relational/TableAndSchema.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 1
        :linenos:

Fluent API
----------

You can use the Fluent API to configure the table that a type maps to.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/Table.cs
        :language: c#
        :lines: 5-20
        :emphasize-lines: 7-8
        :linenos:

You can also specify a schema that the table belongs to.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/TableAndSchema.cs
        :language: c#
        :lines: 11-12
        :emphasize-lines: 2
        :linenos:
