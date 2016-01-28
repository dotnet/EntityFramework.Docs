Basic Save
==========

Learn how to add, modify, and remove data using your context and entity classes.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/saving/sample/EFSaving/Basics/

ChangeTracker & SaveChanges
---------------------------

Each context instance has a `ChangeTracker` that is responsible for keeping track of changes that need to be written to the database. As you make changes to instances of your entity classes, these changes are recorded in the `ChangeTracker` and then written to the database when you call `SaveChanges`.

Adding Data
-----------

Use the `DbSet.Add` method to add new instances of your entity classes. The data will be inserted in the database when you call `SaveChanges`.

.. literalinclude:: sample/EFSaving/Basics/Sample.cs
        :language: c#
        :lines: 17-24
        :linenos:

Updating Data
-------------

EF will automatically detect changes made to an existing entity that is tracked by the context. This includes entities that you load/query from the database, and entities that were previously added and saved to the database.

Simply modify the values assigned to properties and then call `SaveChanges`.

.. literalinclude:: sample/EFSaving/Basics/Sample.cs
        :language: c#
        :lines: 26-31
        :linenos:

Deleting Data
-------------

Use the `DbSet.Remove` method to delete instances of you entity classes.

If the entity already exists in the database, it will be deleted during `SaveChanges`. If the entity has not yet been saved to the database (i.e. it is tracked as added) then it will be removed from the context and will no longer be inserted when `SaveChanges` is called.

.. literalinclude:: sample/EFSaving/Basics/Sample.cs
        :language: c#
        :lines: 33-38
        :linenos:

Multiple Operations in a single SaveChanges
-------------------------------------------

You can combine multiple Add/Update/Remove operations into a single call to `SaveChanges`.

.. note::
  For most database providers, `SaveChanges` is transactional. This means  all the operations will either succeed or fail and the operations will never be left partially applied.

.. literalinclude:: sample/EFSaving/Basics/Sample.cs
        :language: c#
        :lines: 48-60
        :linenos:
