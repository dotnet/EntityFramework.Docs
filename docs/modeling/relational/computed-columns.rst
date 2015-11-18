.. include:: /_shared/rc1-notice.txt

.. include:: _shared/relational-specific-note.txt

Computed Columns
================

A computed column is a column whose value is calculated in the database. A computed column can use other columns in the table to calculate its value.

Conventions
-----------

By convention, computed columns are not created in the model.

Data Annotations
----------------

Computed columns can not be configured with Data Annotations.

Fluent API
----------

You can use the Fluent API to specify that a property should map to a computed column.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/ComputedColumn.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 9
        :linenos:
