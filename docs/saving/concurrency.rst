Concurrency Conflicts
=====================

If a property is configured as a concurrency token then EF will check that no other user has modified that value in the database when saving changes to that record.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Saving/Saving/Concurrency/

How concurrency works in EF
---------------------------

For a detailed description of how concurrency works in Entity Framework Core, see :doc:`/modeling/concurrency`.

Resolving concurrency conflicts
-------------------------------

Resolving a concurrency conflict involves using an algorithm to merge the pending changes from the current user with the changes made in the database. The exact approach will vary based on your application, but a common approach is to display the values to the user and have them decide the correct values to be stored in the database.

There are three sets of values available to help resolve a concurrency conflict.
 * **Current values** are the values that the application was attempting to write to the database.
 * **Original values** are the values that were originally retrieved from the database, before any edits were made.
 * **Database values** are the values currently stored in the database.

To handle a concurrency conflict, catch a ``DbUpdateConcurrencyException`` during ``SaveChanges()``, use ``DbUpdateConcurrencyException.Entries`` to prepare a new set of changes for the affected entities, and then retry the ``SaveChanges()`` operation.

In the following example, ``Person.FirstName`` and ``Person.LastName`` are setup as concurrency token. There is a ``// TODO:`` comment in the location where you would include application specific logic to chose the value to be saved to the database.

.. includesamplefile:: Saving/Saving/Concurrency/Sample.cs
        :language: c#
        :emphasize-lines: 53-54
        :linenos:
