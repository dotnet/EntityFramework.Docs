.. include:: _shared/relational-specific-note.txt

Foreign Key Constraints
=======================

A foreign key constraint is introduced for each relationship in the model.

Conventions
-----------

By convention, foreign key constraints are named ``FK_<dependent type name>_<principal type name>_<foreign key property name>``. For composite foreign keys ``<foreign key property name>`` becomes an underscore separated list of foreign key property names.

Data Annotations
----------------

Foreign key constraint names cannot be configured using data annotations.

Fluent API
----------

You can use the Fluent API to configure the foreign key constraint name for a relationship.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/RelationshipConstraintName.cs
        :language: c#
        :lines: 6-36
        :emphasize-lines: 12
        :linenos:
