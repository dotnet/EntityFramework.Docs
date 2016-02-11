.. include:: /_shared/rc1-notice.txt

Backing Fields
==============

When a backing field is configured, EF will write directly to that field when materializing entity instances from the database (rather than using the property setter). This is useful when there is no property setter, or the setter contains logic that should not be executed when setting initial property values for existing entities being loaded from the database.

.. caution::
  The ``ChangeTracker`` has not yet been enabled to use backing fields when it needs to set the value of a property. This is only an issue for foreign key properties and generated properties - as the change tracker needs to propagate values into these properties. For these properties, a property setter must still be exposed.

  `Issue #4461 <https://github.com/aspnet/EntityFramework/issues/4461>`_ is tracking enabling the ``ChangeTracker`` to write to backing fields for properties with no setter.

.. contents:: In this article:
  :depth: 2
  :local:

Conventions
-----------

By convention, the following fields will be discovered as backing fields for a given property (listed in precedence order):
  * <propertyName> differing only by case
  * _<propertyName>
  * m_<propertyName>

.. literalinclude:: configuring/sample/EFModeling.Conventions/Samples/BackingField.cs
        :language: c#
        :lines: 18-29
        :emphasize-lines: 3,7-11
        :linenos:

Data Annotations
----------------

Backing fields cannot be configured with data annotations.

Fluent API
----------

There is no top level API for configuring backing fields, but you can use the Fluent API to set annotations that are used to store backing field information.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/BackingField.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 7-9,15,18
        :linenos:
