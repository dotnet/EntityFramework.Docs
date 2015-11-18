.. include:: /_shared/rc1-notice.txt

Including & Excluding Types
===========================

Including a type in the model means that EF has metadata about that type and will attempt to read and write instances from/to the database.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, types that are exposed in ``DbSet`` properties on your context are included in your model. In addition, types that are mentioned in the ``OnModelCreating`` method are also included. Finally, any types that are found by recursively exploring the navigation properties of discovered types are also included in the model.

For example, in the following code listing all three types are discovered:
 - ``Blog`` because it is exposed in a ``DbSet`` property on the context
 - ``Post`` because it is discovered via the ``Blog.Posts`` navigation property
 - ``AuditEntry`` because it is mentioned in ``OnModelCreating``

.. literalinclude:: configuring/sample/EFModeling.Conventions/Samples/IncludedTypes.cs
        :language: c#
        :lines: 6-40
        :emphasize-lines: 3,7,18
        :linenos:

Data Annotations
----------------

You can use Data Annotations to exclude a type from the model.

.. literalinclude:: configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/IgnoreType.cs
        :language: c#
        :lines: 12-24
        :emphasize-lines: 9
        :linenos:

Fluent API
----------

You can use the Fluent API to exclude a type from the model.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IgnoreType.cs
        :language: c#
        :lines: 6-27
        :emphasize-lines: 7
        :linenos:
