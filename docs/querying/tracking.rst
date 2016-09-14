Tracking vs. No-Tracking
========================

Tracking behavior controls whether or not Entity Framework Core will keep information about an entity instance in its change tracker. If an entity is tracked, any changes detected in the entity will be persisted to the database during ``SaveChanges()``. Tracking is also used by Entity Framework Core to automatically fix-up navigation properties in entities that were previously loaded into the context instance.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Querying

Tracking queries
----------------

By default, queries that return entity types are tracking. This means you can make changes to those entity instances and have those changes persisted by ``SaveChanges()``.

In the following example, the change to the blogs rating will be detected and persisted to the database during ``SaveChanges()``.

.. includesamplefile:: Querying/Querying/Tracking/Sample.cs
        :language: c#
        :lines: 10-15
        :dedent: 12
        :linenos:

No-tracking queries
-------------------

No tracking queries are useful when the results are used in a read-only scenario. They are quicker to execute because there is no need to setup change tracking information.

You can swap an individual query to be no-tracking:

.. includesamplefile:: Querying/Querying/Tracking/Sample.cs
        :language: c#
        :lines: 17-22
        :emphasize-lines: 4
        :dedent: 12
        :linenos:

You can also change the default tracking behavior at the context instance level:

.. includesamplefile:: Querying/Querying/Tracking/Sample.cs
        :language: c#
        :lines: 24-29
        :emphasize-lines: 3
        :dedent: 12
        :linenos:

Tracking and projections
------------------------

Even if the result type of the query isn't an entity type, if the result contains entity types they will still be tracked by default. In the following query, which returns an anonymous type, the instances of ``Blog`` in the result set will be tracked.

.. includesamplefile:: Querying/Querying/Tracking/Sample.cs
        :language: c#
        :lines: 31-40
        :emphasize-lines: 7
        :dedent: 12
        :linenos:

If the result set does not contain any entity types, then no tracking is performed. In the following query, which returns an anonymous type with some of the values from the entity (but no instances of the actual entity type), there is no tracking performed.

.. includesamplefile:: Querying/Querying/Tracking/Sample.cs
        :language: c#
        :lines: 42-51
        :dedent: 12
        :linenos:
