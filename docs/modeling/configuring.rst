Configuring Your Model
======================

Entity Framework uses a set of :doc:`default-conventions` to build a model based on the shape of your entity classes. You can specify additional configuration to supplement and/or override what was discovered by convention.

.. note::
    This article covers configuration that can be applied to a model targeting any data store and that which can be applied when targeting any relational database.

    Providers may also enable configuration that is specific to a particular data store. For documentation on provider specific configuration see the the :doc:`/providers/index` section.

.. contents:: In this article:
    :depth: 3

.. include:: /sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/modeling/configuring/sample

.. include:: /rc1-notice.txt

Methods of configuration
------------------------

Fluent API
^^^^^^^^^^

You can override the ``OnModelCreating`` method in your derived context and use the ``ModelBuilder`` API to configure your model. This is the most powerful method of configuration and allows configuration to be specified without modifying your entity classes.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Required.cs
        :language: c#
        :lines: 5-15
        :emphasize-lines: 5-10
        :linenos:

Data Annotations
^^^^^^^^^^^^^^^^

.. caution::
    Data Annotations have been recently implemented in EF7 and are not yet included in this article. They will be added in the near future.

Keys
----

A key serves as the primary unique identifier for each entity instance. When using a relational database this maps to the concept of a *primary key*.

The following fragment shows how to configure a single property to be the key of an entity. In this case ``LicensePlate`` is the primary key of ``Car`` but will not have been detected by the default conventions.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/KeySingle.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-8
        :linenos:

Building on the previous example, let's say that the primary key of ``Car`` is made up of two properties (``State`` and ``LicensePlate``).

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/KeyComposite.cs
        :language: c#
        :lines: 5-23
        :emphasize-lines: 7-8
        :linenos:

Indexes
-------

Indexes are a common concept across many data stores. While their implementation in the data store may vary, they are used to make lookups based on a column (or set of columns) more efficient.

The following code shows how to specify an index on a single property. By default, indexes are non-unique.

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

Required
--------

By default, a property whose type can contain null will be configured as optional (i.e. CLR *reference types* such as ``string``, ``int?``, ``byte[]``, etc.). However, these properties can be configured to be required.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Required.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:

.. note::
    A property whose type can not contain null can not be configured as optional (i.e. CLR *value types* such as ``int``, ``bool``, ``decimal``, etc.).

Generated Properties (Identity, Computed, Store Defaults, etc.)
---------------------------------------------------------------

There are three value generation patterns that can be used for properties.

No value generation
^^^^^^^^^^^^^^^^^^^

No value generation means that you will always supply a valid value to be saved to the database. This valid value must be assigned to new entities before they are added to the context.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/ValueGeneratedNever.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-9
        :linenos:

Value generated on add
^^^^^^^^^^^^^^^^^^^^^^

Value generated on add means that if you don't specify a value, one will be generated for you.

If you add an entity to the context that has a value assigned to the primary key property, then EF will attempt to insert that value rather than generating a new one. A property is considered to have a value assigned if it is not assigned the CLR default value (``null`` for ``string``, ``0`` for ``int``, ``Guid.Empty`` for ``Guid``, etc.).

Depending on the database provider being used, values may be generated client side by EF or in the database. If the value is generated by the database, then EF may assign a temporary value when you add the entity to the context. This temporary value will then be replaced by the database generated value during ``SaveChanges``.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/ValueGeneratedOnAdd.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-9
        :linenos:

Value generated on add or update
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Value generated on add or update means that a new value is generated every time the record is saved (insert or update).

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/ValueGeneratedOnAddOrUpdate.cs
        :language: c#
        :lines: 6-24
        :emphasize-lines: 7-9
        :linenos:

Maximum Length
--------------

Configuring a maximum length provides a hint to the data store about the appropriate data type to use for a given property.

The following code allows the store to choose an appropraite data type for ``Blog.Url`` based on the fact it should only ever contain 500 characters. When targetting SQL Server this would result in the ``nvarchar(500)`` data type being used.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/MaxLength.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:

.. note::
    Entity Framework does not do any validation of maximum length before passing data to the provider. It is up to the provider or data store to validate if appropriate.

    For example, when targeting SQL Server, exceeding the maximum length will result in an exception as the data type of the underlying column will not allow excess data to be stored.

Concurrency Tokens
------------------

If a property is configured as a concurrency token then EF will check that no other user has modified that value in the database when saving changes to that record. EF uses an optimistic concurrency pattern, meaning it will assume the value has not changed and try to save the data, but throw if it finds the value has been changed.

For example we may want to configure ``SocialSecurityNumber`` on ``Person`` to be a concurrency token. This means that if one user tries to save some changes to a Person, but another user has changed the ``SocialSecurityNumber`` then an exception will be thrown. This may be desirable so that your application can prompt the user to ensure this record still represents the same actual person before saving their changes.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Concurrency.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-9
        :linenos:

How concurrency tokens work in EF
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Data stores can enforce concurrency tokens by checking that any record being updated or deleted still has the same value for the concurrency token that was assigned when the context originally loaded the data from the database.

For example, relational database achieve this by including the concurrency token in the ``WHERE`` clause of any ``UPDATE`` or ``DELETE`` commands and checking the number of rows that were affected. If the concurrency token still matches then one row will be updated. If the value in the database has changed, then no rows are updated.

.. code-block:: sql

    UPDATE [Person] SET [Name] = @p1
    WHERE [PersonId] = @p0 AND [SocialSecurityNumber] = @p2;

Ignoring Types and Properties
-----------------------------

EF uses conventions to discover the types and properties that should be included in your model. Including a type or property in your model means that it will be saved to and queried from the database.

Ignore a type
^^^^^^^^^^^^^

The following code shows how to ignore an entire type from your model.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IgnoreType.cs
        :language: c#
        :lines: 6-27
        :emphasize-lines: 7
        :linenos:

Ignore a property
^^^^^^^^^^^^^^^^^

The following code shows how to ignore a property from a type.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/IgnoreProperty.cs
        :language: c#
        :lines: 6-23
        :emphasize-lines: 7-8
        :linenos:

Relational database configuration
---------------------------------

The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *EntityFramework.Relational* package).

.. note::
    Not all relational database will support all configuration shown here. For example, SQLite does not support the concept of schemas.

Relational table
^^^^^^^^^^^^^^^^

The following code maps the ``Blog`` class to a ``blogs`` table in the database.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/Table.cs
        :language: c#
        :lines: 5-20
        :emphasize-lines: 7-8
        :linenos:

You can also specify a schema that the table belongs to.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/TableAndSchema.cs
        :language: c#
        :lines: 11-12
        :emphasize-lines: 2
        :linenos:

Relational column
^^^^^^^^^^^^^^^^^

The following code maps the ``Blog.BlogId`` property to a ``blog_id`` column in the database.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/Column.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:

Relational data type
^^^^^^^^^^^^^^^^^^^^

The following code specifies that the column for ``Blog.Url`` should use the ``varchar(200)`` datatype.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DataType.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:

If you are targeting more than one relational provider with the same model then you probably want to specify a data type for each provider rather than a global one to be used for all relational providers.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/DataTypeForProvider.cs
        :language: c#
        :lines: 11-13
        :emphasize-lines: 3
        :linenos:
