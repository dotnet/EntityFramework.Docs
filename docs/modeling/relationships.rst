.. include:: /_shared/rc2-notice.txt

Relationships
=============

A relationship defines how two entities relate to each other. In a relational database, this is represented by a foreign key constraint.

.. note::
    Most of the samples in this article use a one-to-many relationship to demonstrate concepts. For examples of one-to-one and many-to-many relationships see the `Other Relationship Patterns`_ section at the end of the article.

.. contents:: In this article:
    :depth: 3

Definition of Terms
-------------------

There are a number of terms used to describe relationships
 - **Dependent entity:** This is the entity that contains the foreign key property(s). Sometimes referred to as the 'child' of the relationship.
 - **Principal entity:** This is the entity that contains the primary/alternate key property(s). Sometimes referred to as the 'parent' of the relationship.
 - **Foreign key:** The property(s) in the dependent entity that is used to store the values of the principal key property that the entity is related to.
 - **Principal key:** The property(s) that uniquely identifies the principal entity. This may be the primary key or an alternate key.
 - **Navigation property:** A property defined on the principal and/or dependent entity that contains a reference(s) to the related entity(s).
 - **Collection navigation property:** A navigation property that contains references to many related entities.
 - **Reference navigation property:** A navigation property that holds a reference to a single related entity.
 - **Inverse navigation property:** When discussing a particular navigation property, this term refers to the navigation property on the other end of the relationship.

The following code listing shows a one-to-many relationship between ``Blog`` and ``Post``
  - ``Post`` is the dependent entity
  - ``Blog`` is the principal entity
  - ``Post.BlogId`` is the foreign key
  - ``Blog.BlogId`` is the principal key (in this case it is a primary key rather than an alternate key)
  - ``Post.Blog`` is a reference navigation property
  - ``Blog.Posts`` is a collection navigation property
  - ``Post.Blog`` is the inverse navigation property of ``Blog.Posts`` (and vice versa)

.. includesamplefile:: Modeling/Conventions/Samples/Relationships/Full.cs
        :language: c#
        :lines: 12-28
        :linenos:

Conventions
-----------

By convention, a relationship will be created when there is a navigation property discovered on a type. A property is considered a navigation property if the type it points to can not be mapped as a scalar type by the current database provider.

.. note::
    Relationships that are discovered by convention will always target the primary key of the principal entity. To target an alternate key, additional configuration must be performed using the Fluent API.

Fully Defined Relationships
^^^^^^^^^^^^^^^^^^^^^^^^^^^

The most common pattern for relationships is to have navigation properties defined on both ends of the relationship and a foreign key property defined in dependent entity class.
 - If a pair of navigation properties is found between two types, then they will be configured as inverse navigation properties of the same relationship.
 - If the dependent entity contains a property named ``<primary key property name>``, ``<navigation property name><primary key property name>``, or ``<principal entity name><primary key property name>`` then it will be configured as the foreign key.

.. includesamplefile:: Modeling/Conventions/Samples/Relationships/Full.cs
       :language: c#
       :lines: 12-28
       :emphasize-lines: 6, 15-16
       :linenos:

.. caution::
    If there are multiple navigation properties defined between two types (i.e. more than one distinct pair of navigations that point to each other), then no relationships will be created by convention and you will need to manually configure them to identify how the navigation properties pair up.

No Foreign Key Property
^^^^^^^^^^^^^^^^^^^^^^^

While it is recommended to have a foreign key property defined in the dependent entity class, it is not required. If no foreign key property is found, a shadow foreign key property will be introduced with the name ``<navigation property name><principal key property name>`` (see :doc:`shadow-properties` for more information).

.. includesamplefile:: Modeling/Conventions/Samples/Relationships/NoForeignKey.cs
        :language: c#
        :lines: 12-27
        :emphasize-lines: 6, 15
        :linenos:

Single Navigation Property
^^^^^^^^^^^^^^^^^^^^^^^^^^

Including just one navigation property (no inverse navigation, and no foreign key property) is enough to have a relationship defined by convention. You can also have a single navigation property and a foreign key property.

.. includesamplefile:: Modeling/Conventions/Samples/Relationships/OneNavigation.cs
        :language: c#
        :lines: 12-25
        :emphasize-lines: 6
        :linenos:

Cascade Delete
^^^^^^^^^^^^^^

By convention, cascade delete will be set to *Cascade* for required relationships and *SetNull* for optional relationships. *Cascade* means dependent entities are also deleted. *SetNull* means that foreign key properties in dependent entities are set to null.

.. note::
    This cascading behavior is only applied to entities that are being tracked by the context. A corresponding cascade behavior should be setup in the database to ensure data that is not being tracked by the context has the same action applied. If you use EF to create the database, this cascade behavior will be setup for you.

Data Annotations
----------------

There are two data annotations that can be used to configure relationships, ``[ForeignKey]`` and ``[InverseProperty]``.

[ForeignKey]
^^^^^^^^^^^^

You can use the Data Annotations to configure which property should be used as the foreign key property for a given relationship. This is typically done when the foreign key property is not discovered by convention.

.. includesamplefile:: Modeling/DataAnnotations/Samples/Relationships/ForeignKey.cs
        :language: c#
        :lines: 13-31
        :emphasize-lines: 17
        :linenos:

.. note::
    The ``[ForeignKey]`` annotation can be placed on either navigation property in the relationship. It does not need to go on the navigation property in the dependent entity class.

[InverseProperty]
^^^^^^^^^^^^^^^^^

You can use the Data Annotations to configure how navigation properties on the dependent and principal entities pair up. This is typically done when there is more than one pair of navigation properties between two entity types.

.. includesamplefile:: Modeling/DataAnnotations/Samples/Relationships/InverseProperty.cs
        :language: c#
        :lines: 13-37
        :emphasize-lines: 20,23
        :linenos:

Fluent API
----------

To configure a relationship in the Fluent API, you start by identifying the navigation properties that make up the relationship. ``HasOne`` or ``HasMany`` identifies the navigation property on the entity type you are beginning the configuration on. You then chain a call to ``WithOne`` or ``WithMany`` to identify the inverse navigation. ``HasOne``/``WithOne`` are used for reference navigation properties and ``HasMany``/``WithMany`` are used for collection navigation properties.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/NoForeignKey.cs
        :language: c#
        :lines: 6-34
        :emphasize-lines: 8-10
        :linenos:

Single Navigation Property
^^^^^^^^^^^^^^^^^^^^^^^^^^

If you only have one navigation property then there are parameterless overloads of ``WithOne`` and ``WithMany``. This indicates that there is conceptually a reference or collection on the other end of the relationship, but there is no navigation property included in the entity class.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/OneNavigation.cs
        :language: c#
        :lines: 6-32
        :emphasize-lines: 10
        :linenos:

Foreign Key
^^^^^^^^^^^

You can use the Fluent API to configure which property should be used as the foreign key property for a given relationship.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/ForeignKey.cs
        :language: c#
        :lines: 6-36
        :emphasize-lines: 11
        :linenos:

The following code listing shows how to configure a composite foreign key.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/CompositeForeignKey.cs
        :language: c#
        :lines: 7-42
        :emphasize-lines: 13
        :linenos:

Principal Key
^^^^^^^^^^^^^

If you want the foreign key to reference a property other than the primary key, you can use the Fluent API to configure the principal key property for the relationship. The property that you configure as the principal key will automatically be setup as an alternate key (see :doc:`alternate-keys` for more information).

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/PrincipalKey.cs
        :language: c#
        :lines: 7-39
        :emphasize-lines: 11
        :linenos:

The following code listing shows how to configure a composite principal key.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/CompositePrincipalKey.cs
        :language: c#
        :lines: 7-41
        :emphasize-lines: 11
        :linenos:

.. caution::
    The order that you specify principal key properties must match the order they are specified for the foreign key.

Required
^^^^^^^^

You can use the Fluent API to configure whether the relationship is required or optional. Ultimately this controls whether the foreign key property is required or optional. This is most useful when you are using a shadow state foreign key. If you have a foreign key property in your entity class then the requiredness of the relationship is determined based on whether the foreign key property is required or optional (see :doc:`required-optional` for more information).

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/Required.cs
        :language: c#
        :lines: 6-35
        :emphasize-lines: 11
        :linenos:

Cascade Delete
^^^^^^^^^^^^^^

You can use the Fluent API to configure the cascade delete behavior for a given relationship.

There are three behaviors that control how a delete operation is applied to dependent entities in a relationship when the principal is deleted or the relationship is severed.
 - **Cascade:** Dependent entities are also deleted.
 - **SetNull:** The foreign key properties in dependent entities are set to null.
 - **Restrict:** The delete operation is not applied to dependent entities. The dependent entities remain unchanged.

.. note::
    This cascading behavior is only applied to entities that are being tracked by the context. A corresponding cascade behavior should be setup in the database to ensure data that is not being tracked by the context has the same action applied. If you use EF to create the database, this cascade behavior will be setup for you.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/CascadeDelete.cs
        :language: c#
        :lines: 7-37
        :emphasize-lines: 11
        :linenos:

Other Relationship Patterns
---------------------------

One-to-one
^^^^^^^^^^

One to one relationships have a reference navigation property on both sides. They follow the same conventions as one-to-many relationships, but a unique index is introduced on the foreign key property to ensure only one dependent is related to each principal.


.. includesamplefile:: Modeling/Conventions/Samples/Relationships/OneToOne.cs
        :language: c#
        :lines: 12-28
        :emphasize-lines: 6,15-16
        :linenos:

.. note::
    EF will chose one of the entities to be the dependent based on its ability to detect a foreign key property. If the wrong entity is chosen as the dependent you can use the Fluent API to correct this.

When configuring the relationship with the Fluent API, you use the ``HasOne`` and ``WithOne`` methods.

When configuring the foreign key you need to specify the dependent entity type - notice the generic parameter provided to ``HasForeignKey`` in the listing below. In a one-to-many relationship it is clear that the entity with the reference navigation is the dependent and the one with the collection is the principal. But this is not so in a one-to-one relationship - hence the need to explicitly define it.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/OneToOne.cs
        :language: c#
        :lines: 6-36
        :emphasize-lines: 11
        :linenos:

Many-to-many
^^^^^^^^^^^^

Many-to-many relationships without an entity class to represent the join table are not yet supported. However, you can represent a many-to-many relationship by including an entity class for the join table and mapping two separate one-to-many relationships.

.. includesamplefile:: Modeling/FluentAPI/Samples/Relationships/ManyToMany.cs
        :language: c#
        :lines: 6-51
        :emphasize-lines: 11-14,16-19,39-46
        :linenos:
