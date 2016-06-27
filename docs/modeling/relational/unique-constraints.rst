.. include:: _shared/relational-specific-note.txt

Alternate Keys (Unique Constraints)
===================================

A unique constraint is introduced for each alternate key in the model.

Conventions
-----------

By convention, the index and constraint that are introduced for an alternate key will be named ``AK_<type name>_<property name>``. For composite alternate keys ``<property name>`` becomes an underscore separated list of property names.

Data Annotations
----------------

Unique constraints can not be configured using Data Annotations.

Fluent API
----------

You can use the Fluent API to configure the index and constraint name for an alternate key.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relational/AlternateKeyName.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 9
        :linenos:
