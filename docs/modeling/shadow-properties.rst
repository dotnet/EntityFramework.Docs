Shadow Properties
=================

Shadow properties are properties that do not exist in your entity class. The value and state of these properties is maintained purely in the Change Tracker.

Shadow property values can be obtained and changed through the ``ChangeTracker`` API.

.. code-block:: csharp

    context.Entry(myBlog).Property("LastUpdated").CurrentValue = DateTime.Now;

Shadow properties can be referenced in LINQ queries via the ``EF.Property`` static method.

.. code-block:: csharp

    var blogs = context.Blogs
        .OrderBy(b => EF.Property<DateTime>(b, "LastUpdated"));

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, shadow properties are only created when a relationship is discovered but no foreign key property is found in the dependent entity class. In this case, a shadow foreign key property will be introduced. The shadow foreign key property will be named ``<navigation property name><principal key property name>`` (the navigation on the dependent entity, which points to the principal entity, is used for the naming). If the principal key property name includes the name of the navigation property, then the name will just be ``<principal key property name>``. If there is no navigation property on the dependent entity, then the principal type name is used in its place.

For example, the following code listing will result in a ``BlogId`` shadow property being introduced to the ``Post`` entity.

.. includesamplefile:: Modeling/Conventions/Samples/ShadowForeignKey.cs
        :language: c#
        :lines: 6-27
        :linenos:

Data Annotations
----------------

Shadow properties can not be created with data annotations.

Fluent API
----------

You can use the Fluent API to configure shadow properties. Once you have called the string overload of ``Property`` you can chain any of the configuration calls you would for other properties.

If the name supplied to the ``Property`` method matches the name of an existing property (a shadow property or one defined on the entity class), then the code will configure that existing property rather than introducing a new shadow property.

.. includesamplefile:: Modeling/FluentAPI/Samples/ShadowProperty.cs
        :language: c#
        :lines: 6-21
        :emphasize-lines: 7-8
        :linenos:
