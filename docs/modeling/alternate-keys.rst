.. include:: /rc1-notice.txt

Alternate Keys
==============

An alternate key serves as a alternate unique identifier for each entity instance in addition to the primary key. When using a relational database this maps to the concept of a unique index/constraint. In EF, alternate keys provide greater functionality than unique :doc:`indexes` because they can be used as the target of a foreign key.

Alternate keys are typically introduced for you when needed and you do not need to manually configure them. See `Conventions`_ for more details.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, an alternate key is introduced for you when you identify a property, that is not the primary key, as the target of a relationship.

.. literalinclude:: configuring/sample/EFModeling.Conventions/Samples/AlternateKey.cs
        :language: c#
        :lines: 6-37
        :emphasize-lines: 12
        :linenos:

Data Annotations
----------------

Alternate keys can not be configured using Data Annotations.

Fluent API
----------

You can use the Fluent API to configure a single property to be an alternate key.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/AlternateKeySingle.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-8
        :linenos:

You can also use the Fluent API to configure multiple properties to be an alternate key (known as a composite alternate key).

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/AlternateKeyComposite.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 7-8
        :linenos:

Relational Databases
--------------------

By convention, the index and constraint in the database will be named ``AK_<type name>_<property name>``. For composite alternate keys ``<property name>`` becomes an underscore separated list of property names. You can change this using the Fluent API.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/AlternateKeyName.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 9
        :linenos:
