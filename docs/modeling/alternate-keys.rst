Alternate Keys
==============

An alternate key serves as an alternate unique identifier for each entity instance in addition to the primary key. Alternate keys can be used as the target of a relationship. When using a relational database this maps to the concept of a unique index/constraint on the alternate key column(s) and one or more foreign key constraints that reference the column(s). 

.. note::
    If you just want to enforce uniqeness of a column then you want a unique index rather than an alternate key, see :doc:`indexes`. In EF, alternate keys provide greater functionality than unique indexes because they can be used as the target of a foreign key.

Alternate keys are typically introduced for you when needed and you do not need to manually configure them. See `Conventions`_ for more details.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, an alternate key is introduced for you when you identify a property, that is not the primary key, as the target of a relationship.

.. includesamplefile:: Modeling/Conventions/Samples/AlternateKey.cs
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

.. includesamplefile:: Modeling/FluentAPI/Samples/AlternateKeySingle.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-8
        :linenos:

You can also use the Fluent API to configure multiple properties to be an alternate key (known as a composite alternate key).

.. includesamplefile:: Modeling/FluentAPI/Samples/AlternateKeyComposite.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 7-8
        :linenos:
