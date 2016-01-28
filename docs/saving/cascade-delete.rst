Cascade Delete
==============

Cascade delete allows deletion of a principal/parent entity to have a side effect on dependent/child entities it is related to.

There are three cascade delete behaviors:
  * **Cascade:** Dependent entities are also deleted.
  * **SetNull:** The foreign key properties in dependent entities are set to null.
  * **Restrict:** The delete operation is not applied to dependent entities. The dependent entities remain unchanged.

See :doc:`/modeling/relationships` for more information about conventions and configuration for cascade delete.

.. contents:: `In this article:`
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/saving/sample/EFSaving/CascadeDelete/

Cascading to tracked entities
-----------------------------

When you call `SaveChanges`, the cascade delete rules will be applied to any entities that are being tracked by the context.

Consider a simple `Blog` and `Post` model where the relationship between the two entities is required. By convention. the cascade behavior for this relationship is set to `Cascade`.

The following code loads a Blog and all its related Posts from the database (using the `Include` method). The code then deletes the Blog.

.. literalinclude:: sample/EFSaving/CascadeDelete/Sample.cs
        :language: c#
        :lines: 30-35
        :linenos:

Because all the Posts are tracked by the context, the cascade behavior is applied to them before saving to the database. EF therefore issues a  `DELETE` statement for each entity.

::

  DELETE FROM [Post]
  WHERE [PostId] = @p0;
  DELETE FROM [Post]
  WHERE [PostId] = @p1;
  DELETE FROM [Blog]
  WHERE [BlogId] = @p2;

Cascading to untracked entities
-------------------------------

The following code is almost the same as our previous example, except it does not load the related Posts from the database.

.. literalinclude:: sample/EFSaving/CascadeDelete/Sample.cs
  :language: c#
  :lines: 52-57
  :linenos:

Because the Posts are not tracked by the context, a `DELETE` statement is only issued for the `Blog`. This relies on a corresponding cascade behavior being present in the database to ensure data that is not tracked by the context is also deleted. If you use EF to create the database, this cascade behavior will be setup for you.

::

  DELETE FROM [Blog]
  WHERE [BlogId] = @p0;
