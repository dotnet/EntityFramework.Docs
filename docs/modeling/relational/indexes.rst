.. include:: _shared/relational-specific-note.txt

Indexes
=======

An index in a relational database maps to the same concept as an index in the core of Entity Framework.

Conventions
-----------

By convention, indexes are named ``IX_<type name>_<property name>``. For composite indexes ``<property name>`` becomes an underscore separated list of property names.

Data Annotations
----------------

Indexes can not be configured using Data Annotations.

Fluent API
----------

You can use the Fluent API to configure the name of an index.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/IndexName.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 9
        :linenos:
