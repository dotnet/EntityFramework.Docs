---
title: Transactions - EF Core
description: Managing transactions for atomicity when saving data with Entity Framework Core
author: roji
ms.date: 9/26/2020
uid: core/saving/transactions
---
# Using Transactions

Transactions allow several database operations to be processed in an atomic manner. If the transaction is committed, all of the operations are successfully applied to the database. If the transaction is rolled back, none of the operations are applied to the database.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Saving/Transactions/) on GitHub.

## Default transaction behavior

By default, if the database provider supports transactions, all changes in a single call to `SaveChanges` are applied in a transaction. If any of the changes fail, then the transaction is rolled back and none of the changes are applied to the database. This means that `SaveChanges` is guaranteed to either completely succeed, or leave the database unmodified if an error occurs.

For most applications, this default behavior is sufficient. You should only manually control transactions if your application requirements deem it necessary.

## Controlling transactions

You can use the `DbContext.Database` API to begin, commit, and rollback transactions. The following example shows two `SaveChanges` operations and a LINQ query being executed in a single transaction:

[!code-csharp[Main](../../../samples/core/Saving/Transactions/ControllingTransaction.cs?name=Transaction&highlight=2,16-18)]

While all relational database providers support transactions, other providers types may throw or no-op when transaction APIs are called.

> [!NOTE]
> Manually controlling transactions in this way is incompatible with implicitly invoked retrying execution strategies. See [Connection Resiliency](xref:core/miscellaneous/connection-resiliency#execution-strategies-and-transactions) for more information.

## Savepoints

When `SaveChanges` is invoked and a transaction is already in progress on the context, EF automatically creates a *savepoint* before saving any data. Savepoints are points within a database transaction which may later be rolled back to, if an error occurs or for any other reason. If `SaveChanges` encounters any error, it automatically rolls the transaction back to the savepoint, leaving the transaction in the same state as if it had never started. This allows you to possibly correct issues and retry saving, in particular when [optimistic concurrency](xref:core/saving/concurrency) issues occur.

> [!WARNING]
> Savepoints are incompatible with SQL Server's Multiple Active Result Sets (MARS). Savepoints will not be created by EF when MARS is enabled on the connection, even if MARS is not actively in use. If an error occurs during SaveChanges, the transaction may be left in an unknown state.

It's also possible to manually manage savepoints, just as it is with transactions. The following example creates a savepoint within a transaction, and rolls back to it on failure:

[!code-csharp[Main](../../../samples/core/Saving/Transactions/ManagingSavepoints.cs?name=Savepoints&highlight=9,19-20)]

## Cross-context transaction

You can also share a transaction across multiple context instances. This functionality is only available when using a relational database provider because it requires the use of `DbTransaction` and `DbConnection`, which are specific to relational databases.

To share a transaction, the contexts must share both a `DbConnection` and a `DbTransaction`.

### Allow connection to be externally provided

Sharing a `DbConnection` requires the ability to pass a connection into a context when constructing it.

The easiest way to allow `DbConnection` to be externally provided, is to stop using the `DbContext.OnConfiguring` method to configure the context and externally create `DbContextOptions` and pass them to the context constructor.

> [!TIP]
> `DbContextOptionsBuilder` is the API you used in `DbContext.OnConfiguring` to configure the context, you are now going to use it externally to create `DbContextOptions`.

[!code-csharp[Main](../../../samples/core/Saving/Transactions/SharingTransaction.cs?name=Context&highlight=3,4,5)]

An alternative is to keep using `DbContext.OnConfiguring`, but accept a `DbConnection` that is saved and then used in `DbContext.OnConfiguring`.

```csharp
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
```

### Share connection and transaction

You can now create multiple context instances that share the same connection. Then use the `DbContext.Database.UseTransaction(DbTransaction)` API to enlist both contexts in the same transaction.

[!code-csharp[Main](../../../samples/core/Saving/Transactions/SharingTransaction.cs?name=Transaction&highlight=1-4,7,15,22-24)]

## Using external DbTransactions (relational databases only)

If you are using multiple data access technologies to access a relational database, you may want to share a transaction between operations performed by these different technologies.

The following example, shows how to perform an ADO.NET SqlClient operation and an Entity Framework Core operation in the same transaction.

[!code-csharp[Main](../../../samples/core/Saving/Transactions/ExternalDbTransaction.cs?name=Transaction&highlight=4,9,20,25-27)]

## Using System.Transactions

It is possible to use ambient transactions if you need to coordinate across a larger scope.

[!code-csharp[Main](../../../samples/core/Saving/Transactions/AmbientTransaction.cs?name=Transaction&highlight=1,2,3,26-28)]

It is also possible to enlist in an explicit transaction.

[!code-csharp[Main](../../../samples/core/Saving/Transactions/CommitableTransaction.cs?name=Transaction&highlight=1-2,15,28-30)]

> [!NOTE]
> If you're using async APIs, be sure to specify [TransactionScopeAsyncFlowOption.Enabled](/dotnet/api/system.transactions.transactionscopeasyncflowoption) in the `TransactionScope` constructor to ensure that the ambient transaction flows across async calls.

For more information on `TransactionScope` and ambient transactions, [see this documentation](/dotnet/framework/data/transactions/implementing-an-implicit-transaction-using-transaction-scope).

### Limitations of System.Transactions

1. EF Core relies on database providers to implement support for System.Transactions. If a provider does not implement support for System.Transactions, it is possible that calls to these APIs will be completely ignored. SqlClient supports it.

   > [!IMPORTANT]
   > It is recommended that you test that the API behaves correctly with your provider before you rely on it for managing transactions. You are encouraged to contact the maintainer of the database provider if it does not.

2. Distributed transaction support in System.Transactions was added to .NET 7.0 for Windows only. Any attempt to use distributed transactions on older .NET versions or on non-Windows platforms will fail.

3. TransactionScope does not support async commit/rollback; that means that disposing it synchronously blocks the executing thread until the operation is complete.
