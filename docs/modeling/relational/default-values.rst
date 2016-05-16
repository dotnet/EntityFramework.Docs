.. include:: /_shared/rc2-notice.txt

.. include:: _shared/relational-specific-note.txt

Default Values
==============

The default value of a column is the value that will be inserted if a new row is inserted but no value is specified for the column.

Conventions
-----------

By convention, a default value is not configured.

Data Annotations
----------------

You can not set a default value using Data Annotations.

Fluent API
----------

You can use the Fluent API to specify the default value for a property.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DefaultValue.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 9
        :linenos:

You can also specify a SQL fragment that is used to calculate the default value.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DefaultValueSql.cs
        :language: c#
        :lines: 6-23
        :emphasize-lines: 9
        :linenos:
