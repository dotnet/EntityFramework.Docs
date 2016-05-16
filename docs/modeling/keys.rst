.. include:: /_shared/rc2-notice.txt

Keys (primary)
==============

A key serves as the primary unique identifier for each entity instance. When using a relational database this maps to the concept of a *primary key*. You can also configure a unique identifier that is not the primary key (see :doc:`alternate-keys` for more information).

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, a property named ``Id`` or ``<type name>Id`` will be configured as the key of an entity.

.. literalinclude:: configuring/sample/EFModeling.Conventions/Samples/KeyId.cs
        :language: c#
        :lines: 11-17
        :emphasize-lines: 3
        :linenos:

.. literalinclude:: configuring/sample/EFModeling.Conventions/Samples/KeyTypeNameId.cs
        :language: c#
        :lines: 11-17
        :emphasize-lines: 3
        :linenos:

Data Annotations
----------------

You can use Data Annotations to configure a single property to be the key of an entity.

.. literalinclude:: configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/KeySingle.cs
        :language: c#
        :lines: 11-18
        :emphasize-lines: 3-4
        :linenos:

Fluent API
----------

You can use the Fluent API to configure a single property to be the key of an entity.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/KeySingle.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-8
        :linenos:

You can also use the Fluent API to configure multiple properties to be the key of an entity (known as a composite key). Composite keys can only be configured using the Fluent API - conventions will never setup a composite key and you can not use Data Annotations to configure one.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/KeyComposite.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 7-8
        :linenos:
