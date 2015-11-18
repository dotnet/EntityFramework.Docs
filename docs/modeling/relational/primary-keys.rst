.. include:: /_shared/rc1-notice.txt

.. include:: _shared/relational-specific-note.txt

Primary Keys
============

A primary key constraint is introduced for the key of each entity type.

Conventions
-----------

By convention, the primary key in the database will be named ``PK_<type name>``.

Data Annotations
----------------

No relational database specific aspects of a primary key can be configured using Data Annotations.

Fluent API
----------

You can use the Fluent API to configure the name of the primary key constraint in the database.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/KeyName.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 9
        :linenos:
