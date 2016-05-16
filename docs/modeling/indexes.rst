.. include:: /_shared/rc2-notice.txt

Indexes
=======

Indexes are a common concept across many data stores. While their implementation in the data store may vary, they are used to make lookups based on a column (or set of columns) more efficient.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, an index is created in each property (or set of properties) that are used as a foreign key.

Data Annotations
----------------

Indexes can not be created using data annotations.

Fluent API
----------

You can use the Fluent API specify an index on a single property. By default, indexes are non-unique.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Index.cs
        :language: c#
        :lines: 5-20
        :emphasize-lines: 7-8
        :linenos:

You can also specify that an index should be unique, meaning that no two entities can have the same value(s) for the given property(s).

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IndexUnique.cs
        :language: c#
        :lines: 11-13
        :emphasize-lines: 3
        :linenos:

You can also specify an index over more than one column.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IndexComposite.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-8
        :linenos:

.. note::
    There is only one index per distinct set of properties. If you use the Fluent API to configure an index on a set of properties that already has an index defined, either by convention or previous configuration, then you will be changing the definition of that index. This is useful if you want to further configure an index that was created by convention.
