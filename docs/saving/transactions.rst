Transactions
============

Transactions allow several database operations to be processed in an atomic manner. If the transaction is committed, all of the operations are successfully applied to the database. If the transaction is rolled back, none of the operations are applied to the database.

.. contents:: `In this article:`
    :depth: 2
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Saving/Saving/Transactions/

Default transaction behavior
----------------------------

By default, if the database provider supports transactions, all changes in a single call to ``SaveChanges()`` are applied in a transaction. If any of the changes fail, then the transaction is rolled back and none of the changes are applied to the database. This means that ``SaveChanges()`` is guaranteed to either completely succeed, or leave the database unmodified if an error occurs.

For most applications, this default behavior is sufficient. You should only manually control transactions if your application requirements deem it necessary.

Controlling transactions
------------------------

You can use the ``DbContext.Database`` API to begin, commit, and rollback transactions. The following example shows two ``SaveChanges()`` operations and a LINQ query being executed in a single transaction.

Not all database providers support transactions. Some providers may throw or no-op when transaction APIs are called.

.. includesamplefile:: Saving/Saving/Transactions/ControllingTransaction/Sample.cs
        :language: c#
        :lines: 17-42
        :emphasize-lines: 3, 17-19
        :linenos:

Cross-context transaction (relational databases only)
-----------------------------------------------------

You can also share a transaction across multiple context instances. This functionality is only available when using a relational database provider because it requires the use of ``DbTransaction`` and ``DbConnection``, which are specific to relational databases.

To share a transaction, the contexts must share both a ``DbConnection`` and a ``DbTransaction``.

Allow connection to be externally provided
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Sharing a ``DbConnection`` requires the ability to pass a connection into a context when constructing it.

The easiest way to allow ``DbConnection`` to be externally provided, is to stop using the ``DbContext.OnConfiguring`` method to configure the context and externally create ``DbContextOptions`` and pass them to the context constructor.

.. tip::
  ``DbContextOptionsBuilder`` is the API you used in ``DbContext.OnConfiguring`` to configure the context, you are now going to use it externally to create ``DbContextOptions``.

.. includesamplefile:: Saving/Saving/Transactions/SharingTransaction/Sample.cs
        :language: c#
        :lines: 58-65
        :emphasize-lines: 3-5
        :linenos:

An alternative is to keep using ``DbContext.OnConfiguring``, but accept a ``DbConnection`` that is saved and then used in ``DbContext.OnConfiguring``.

.. code-block:: c#

  public class BloggingContext : DbContext
  {
      private DbConnection _connection;

      public BloggingContext(DbConnection connection)
      {
        _connection = connection;
      }

      public DbSet<Blog> Blogs { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
          optionsBuilder.UseSqlServer(_connection);
      }
  }

Share connection and transaction
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

You can now create multiple context instances that share the same connection. Then use the ``DbContext.Database.UseTransaction(DbTransaction)`` API to enlist both contexts in the same transaction.

.. includesamplefile:: Saving/Saving/Transactions/SharingTransaction/Sample.cs
        :language: c#
        :lines: 24-55
        :emphasize-lines: 1-3, 7, 16, 23-25
        :linenos:

Using external DbTransactions (relational databases only)
---------------------------------------------------------

If you are using multiple data access technologies to access a relational database, you may want to share a transaction between operations performed by these different technologies.

The following example, shows how to perform an ADO.NET SqlClient operation and an Entity Framework Core operation in the same transaction.

.. includesamplefile:: Saving/Saving/Transactions/ExternalDbTransaction/Sample.cs
        :language: c#
        :lines: 21-53
        :emphasize-lines: 4, 10, 21, 26-28
        :linenos:
