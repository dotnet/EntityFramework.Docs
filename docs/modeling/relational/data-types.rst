.. include:: /_shared/rc2-notice.txt

.. include:: _shared/relational-specific-note.txt

Data Types
==========

Data type refers to the database specific type of the column to which a property is mapped.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, the database provider selects a data type based on the CLR type of the property. It also takes into account other metadata, such as the configured :doc:`/modeling/max-length`, whether the property is part of a primary key, etc.

For example, SQL Server uses ``datetime2(7)`` for ``DateTime`` properties, and ``nvarchar(max)`` for ``string`` properties (or ``nvarchar(450)`` for ``string`` properties that are used as a key).

Data Annotations
----------------

You can use Data Annotations to specify an exact data type for the column.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/Relational/DataType.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 4
        :linenos:

Fluent API
----------

You can use the Fluent API to specify an exact data type for the column.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DataType.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:

If you are targeting more than one relational provider with the same model then you probably want to specify a data type for each provider rather than a global one to be used for all relational providers.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DataTypeForProvider.cs
        :language: c#
        :lines: 11-13
        :emphasize-lines: 3
        :linenos:
