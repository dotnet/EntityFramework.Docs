Related Data
============

In addition to isolated entities, you can also make use of the relationships defined in your model.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Saving/Saving/RelatedData/

Adding a graph of new entities
------------------------------

If you create several new related entities, adding one of them to the context will cause the others to be added too.

In the following example, the blog and three related posts are all inserted into the database. The posts are found and added, because they are reachable via the ``Blog.Posts`` navigation property.

.. includesamplefile:: Saving/Saving/RelatedData/Sample.cs
        :language: c#
        :lines: 18-33
        :linenos:


Adding a related entity
-----------------------

If you reference a new entity from the navigation property of an entity that is already tracked by the context, the entity will be discovered and inserted into the database.

In the following example, the ``post`` entity is inserted because it is added to the ``Posts`` property of the ``blog`` entity which was fetched from the database.

.. includesamplefile:: Saving/Saving/RelatedData/Sample.cs
        :language: c#
        :lines: 36-43
        :linenos:

Changing relationships
----------------------

If you change the navigation property of an entity, the corresponding changes will be made to the foreign key column in the database.

In the following example, the ``post`` entity is updated to belong to the new ``blog`` entity because its ``Blog`` navigation property is set to point to ``blog``. Note that ``blog`` will also be inserted into the database because it is a new entity that is referenced by the navigation property of an entity that is already tracked by the context (``post``).

.. includesamplefile:: Saving/Saving/RelatedData/Sample.cs
        :language: c#
        :lines: 46-53
        :linenos:

Removing relationships
----------------------

You can remove a relationship by setting a reference navigation to ``null``, or removing the related entity from a collection navigation.

If a cascade delete is configured, the child/dependent entity will be deleted from the database, see :doc:`cascade-delete` for more information. If no cascade delete is configured, the foreign key column in the database will be set to null (if the column does not accept nulls, an exception will be thrown).

In the following example, a cascade delete is configured on the relationship between ``Blog`` and ``Post``, so the ``post`` entity is deleted from the database.

.. includesamplefile:: Saving/Saving/RelatedData/Sample.cs
        :language: c#
        :lines: 56-63
        :linenos:
