Inheritance
===========

Inheritance in the EF model is used to control how inheritance in the entity classes is represented in the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, it is up to the database provider to determine how inheritance will be represented in the database. See :doc:`relational/inheritance` for how this is handled with a relational database provider.

EF will only setup inheritance if two or more inherited types are explicitly included in the model. EF will not scan for base or derived types that were not otherwise included in the model. You can include types in the model by exposing a `DbSet<TEntity>` for each type in the inheritance hierarchy.

.. includesamplefile:: Modeling/Conventions/Samples/InheritanceDbSets.cs
        :language: c#
        :lines: 5-20
        :emphasize-lines: 3-4
        :linenos:

If you don't want to expose a `DbSet<TEntity>` for one or more entities in the hierarchy, you can use the Fluent API to ensure they are included in the model.

.. includesamplefile:: Modeling/Conventions/Samples/InheritanceModelBuilder.cs
        :language: c#
        :lines: 5-13
        :emphasize-lines: 7
        :linenos:

Data Annotations
----------------

You cannot use Data Annotations to configure inheritance.

Fluent API
----------

The Fluent API for inheritance depends on the database provider you are using. See :doc:`relational/inheritance` for the configuration you can perform for a relational database provider.
