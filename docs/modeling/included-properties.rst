Including & Excluding Properties
================================

Including a property in the model means that EF has metadata about that property and will attempt to read and write values from/to the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, every property on each entity type will be included in the model. This includes private properties and those without a getter or setter.

Data Annotations
----------------

You can use Data Annotations to exclude a property from the model.

.. literalinclude:: configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/IgnoreProperty.cs
        :language: c#
        :lines: 12-19
        :emphasize-lines: 6
        :linenos:

Fluent API
----------

You can use the Fluent API to exclude a property from the model.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IgnoreProperty.cs
        :language: c#
        :lines: 6-23
        :emphasize-lines: 7-8
        :linenos:
