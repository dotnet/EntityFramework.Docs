---
title: Connection Resiliency - EF Core
description: Using connection resiliency to automatically retry failed commands with Entity Framework Core
author: AndriySvyryd
ms.date: 11/15/2016
uid: core/miscellaneous/connection-resiliency
---

# Connection Resiliency

Connection resiliency automatically retries failed database commands. The feature can be used with any database by supplying an "execution strategy", which encapsulates the logic necessary to detect failures and retry commands. EF Core providers can supply execution strategies tailored to their specific database failure conditions and optimal retry policies.

As an example, the SQL Server provider includes an execution strategy that is specifically tailored to SQL Server (including SQL Azure). It is aware of the exception types that can be retried and has sensible defaults for maximum retries, delay between retries, etc.

An execution strategy is specified when configuring the options for your context. This is typically in the `OnConfiguring` method of your derived context:

[!code-csharp[Main](../../../samples/core/Miscellaneous/ConnectionResiliency/Program.cs#OnConfiguring)]

or in `Startup.cs` for an ASP.NET Core application:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<PicnicContext>(
        options => options.UseSqlServer(
            "<connection string>",
            providerOptions => providerOptions.EnableRetryOnFailure()));
}
```

> [!NOTE]
> Enabling retry on failure causes EF to internally buffer the resultset, which may significantly increase memory requirements for queries returning large resultsets. See [buffering and streaming](xref:core/performance/efficient-querying#buffering-and-streaming) for more details.

## Custom execution strategy

There is a mechanism to register a custom execution strategy of your own if you wish to change any of the defaults.

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseMyProvider(
            "<connection string>",
            options => options.ExecutionStrategy(...));
}
```

## Execution strategies and transactions

An execution strategy that automatically retries on failures needs to be able to play back each operation in a retry block that fails. When retries are enabled, each operation you perform via EF Core becomes its own retriable operation. That is, each query and each call to `SaveChangesAsync()` will be retried as a unit if a transient failure occurs.

However, if your code initiates a transaction using `BeginTransactionAsync()` you are defining your own group of operations that need to be treated as a unit, and everything inside the transaction would need to be played back shall a failure occur. You will receive an exception like the following if you attempt to do this when using an execution strategy:

> InvalidOperationException: The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not support user-initiated transactions. Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' to execute all the operations in the transaction as a retriable unit.

The solution is to manually invoke the execution strategy with a delegate representing everything that needs to be executed. If a transient failure occurs, the execution strategy will invoke the delegate again.

[!code-csharp[Main](../../../samples/core/Miscellaneous/ConnectionResiliency/Program.cs#ManualTransaction)]

This approach can also be used with ambient transactions.

[!code-csharp[Main](../../../samples/core/Miscellaneous/ConnectionResiliency/Program.cs#AmbientTransaction)]

## Transaction commit failure and the idempotency issue

In general, when there is a connection failure the current transaction is rolled back. However, if the connection is dropped while the transaction is being committed the resulting state of the transaction is unknown.

By default, the execution strategy will retry the operation as if the transaction was rolled back, but if it's not the case this will result in an exception if the new database state is incompatible or could lead to **data corruption** if the operation does not rely on a particular state, for example when inserting a new row with auto-generated key values.

There are several ways to deal with this.

### Option 1 - Do (almost) nothing

The likelihood of a connection failure during transaction commit is low so it may be acceptable for your application to just fail if this condition actually occurs.

However, you need to avoid using store-generated keys in order to ensure that an exception is thrown instead of adding a duplicate row. Consider using a client-generated GUID value or a client-side value generator.

### Option 2 - Rebuild application state

1. Discard the current `DbContext`.
2. Create a new `DbContext` and restore the state of your application from the database.
3. Inform the user that the last operation might not have been completed successfully.

### Option 3 - Add state verification

For most of the operations that change the database state it is possible to add code that checks whether it succeeded. EF provides an extension method to make this easier - `IExecutionStrategy.ExecuteInTransaction`.

This method begins and commits a transaction and also accepts a function in the `verifySucceeded` parameter that is invoked when a transient error occurs during the transaction commit.

[!code-csharp[Main](../../../samples/core/Miscellaneous/ConnectionResiliency/Program.cs#Verification)]

> [!NOTE]
> Here `SaveChanges` is invoked with `acceptAllChangesOnSuccess` set to `false` to avoid changing the state of the `Blog` entity to `Unchanged` if `SaveChanges` succeeds. This allows to retry the same operation if the commit fails and the transaction is rolled back.

### Option 4 - Manually track the transaction

If you need to use store-generated keys or need a generic way of handling commit failures that doesn't depend on the operation performed each transaction could be assigned an ID that is checked when the commit fails.

1. Add a table to the database used to track the status of the transactions.
2. Insert a row into the table at the beginning of each transaction.
3. If the connection fails during the commit, check for the presence of the corresponding row in the database.
4. If the commit is successful, delete the corresponding row to avoid the growth of the table.

[!code-csharp[Main](../../../samples/core/Miscellaneous/ConnectionResiliency/Program.cs#Tracking)]

> [!NOTE]
> Make sure that the context used for the verification has an execution strategy defined as the connection is likely to fail again during verification if it failed during transaction commit.

## Additional resources

* [Troubleshoot transient connection errors in Azure SQL Database and SQL Managed Instance](/azure/azure-sql/database/troubleshoot-common-connectivity-issues)
