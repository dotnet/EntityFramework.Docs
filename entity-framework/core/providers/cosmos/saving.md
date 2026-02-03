---
title: Saving Data - Azure Cosmos DB Provider - EF Core
description: Saving data with the Azure Cosmos DB EF Core Provider
author: roji
ms.date: 02/02/2026
uid: core/providers/cosmos/saving
---
# Saving Data with the EF Core Azure Cosmos DB Provider

## Saving basics

Saving data with the Azure Cosmos DB provider works in a similar fashion to other providers. You add, modify or remove entities, and then call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*> to persist those changes to the database. For more information on the basics of saving, see [Saving data](xref:core/saving/index).

## Transactionality and transactional batches

> [!NOTE]
> Transactional batches support is being introduced in EF Core 11, which is currently in preview.

Azure Cosmos DB provides limited support for atomic transactions; this is a common limitation of document databases, where the focus is on scalability and availability rather than strict transactional semantics. Azure Cosmos DB does support [transactional batches](/azure/cosmos-db/transactional-batch), which allow operations to be executed together as a batch within a single partition (see additional limitations below). Atomicity is guaranteed within a single batch: if any operation fails, the entire batch is rolled back and none of its changes are applied. However, once a batch is written, it cannot be rolled back or deferred, and atomicity cannot be enforced across multiple batches. Transactional batches also provide a performance benefit, as multiple documents can be updated in a single roundtrip, rather than performing a roundtrip for each update.

Starting with EF 11, the EF Core Azure Cosmos DB provider leverages transactional batches by default whenever possible, providing a best-effort approximation of atomicity (and optimal performance) when saving changes. The batching behavior can be controlled by the <xref:Microsoft.EntityFrameworkCore.AutoTransactionBehavior> property, allowing developers to trade off between performance, consistency guarantees, and failure behavior depending on the application’s needs.

* **Auto** (default) – Operations are grouped into transactional batches by the container and partition they affect, and with a maximum of 100 changes per batch; these batches are then executed sequentially. If a batch fails, execution stops immediately and no subsequent changes are saved. Any batches that were successfully saved before the failure remain saved. This generally provides good performance for performing multiple operations within the same partition with a best-effort approximation of atomicity.
* **Never** – All operations are performed individually and sequentially, in the exact order they were tracked. This avoids batching and can be slower, especially for large numbers of changes. This was the behavior prior to version 11.
* **Always** – Requires that all operations can be executed as a single transaction batch; if any operation cannot be included in a batch (e.g. because they affect different partitions), an exception is thrown. This allows you to guarantee full atomicity (and a single roundtrip) when executing `SaveChangesAsync`, but it is then up to you to manually ensure that all operations can be performed in a transactional batch.

Here is an example of using <xref:Microsoft.EntityFrameworkCore.AutoTransactionBehavior.Always>, which causes `SaveChangesAsync` to fail because too many operations are attempted:

```csharp
using var context = new BlogsContext();
context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
context.AddRange(Enumerable.Range(0, 101).Select(i => new Post())); // 101 entries exceeds the batch item limit of 100.
await context.SaveChangesAsync(); // Throws InvalidOperationException since the changes cannot be saved in a single batch.
```

### Limitations

* Transactional batches can only be performed within a single partition.
* Transactional batches can contain only up to 100 operations, and cannot surpass 2MB of data in total.
* Azure Cosmos DB does not allow document writes with [pre- or post-triggers](/azure/cosmos-db/stored-procedures-triggers-udfs#triggers) to be part of a transactional batch. Because of this, any entities configured with triggers are executed separately and before any transactional batches. This can affect ordering and consistency in mixed scenarios.

## Bulk execution

> [!NOTE]
> Bulk execution is being introduced in EF Core 11, which is currently in preview.

Prior to version 11, the Azure Cosmos DB provider executed document operations sequentially when calling `SaveChangesAsync`; when saving a large number of entities, this was slow as each operation waits for the previous one to complete before starting. Version 11 enables [transactional batches](#transactionality-and-transactional-batches), which allow operations to be batched together for better performance. However, transactional batches can only be used against a single partition, and have various limitations (see the documentation above.

An alternative approach is to use Azure Cosmos DB supports _bulk execution_, which allows multiple document operations to be executed in parallel and across DbContext instances, significantly improving throughput when saving many entities at once. This is especially useful for data loading scenarios, batch operations, or any situation where you need to save many entities.

To enable bulk execution, configure your context using the `BulkExecutionEnabled()` option:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "https://localhost:8081",
        "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        databaseName: "OrdersDB",
        options => options.BulkExecutionEnabled());
```

With bulk execution enabled, document write operations are executed in parallel rather than sequentially. This can provide significant performance improvements when saving multiple entities, especially in scenarios involving hundreds or thousands of documents. However, make sure to understand [the caveats](https://devblogs.microsoft.com/cosmosdb/introducing-bulk-support-in-the-net-sdk/#what-are-the-caveats) before enabling this. Note that the behavior of `SaveChangesAsync` remains the same from an API perspective; it's only the internal execution that changes to take advantage of Azure Cosmos DB's bulk capabilities.

For more information on bulk execution in Azure Cosmos DB, see the following Azure documentation:

* [Introducing bulk support in the .NET SDK](https://devblogs.microsoft.com/cosmosdb/introducing-bulk-support-in-the-net-sdk/)
* [Bulk updates with optimistic concurrency control](https://devblogs.microsoft.com/cosmosdb/bulk-updates-with-optimistic-concurrency-control/)

## Session token management

> [!NOTE]
> Session token management is being introduced in EF Core 11, which is currently in preview.

Azure Cosmos DB uses [session tokens](/azure/cosmos-db/consistency-levels#session-consistency) to track read-your-writes consistency within a session. When using session consistency (the default), each read or write operation returns a session token that can be used in subsequent requests to ensure that the requesting client reads its own writes.

By default, the Azure Cosmos DB .NET SDK manages session tokens automatically within a single `CosmosClient` instance. However, in some scenarios you may need to manage session tokens manually:

* When your application uses a round-robin load balancer that doesn't maintain session affinity between HTTP requests
* When session tokens need to be persisted and restored across application restarts
* When session tokens need to be shared between different `DbContext` instances or application processes

### Configuring session token management mode

To enable manual session token management, configure `SessionTokenManagementMode()` when setting up your context:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseCosmos(
        "<connection string>",
        databaseName: "OrdersDB",
        options => options.SessionTokenManagementMode(SessionTokenManagementMode.SemiAutomatic));
```

The following modes are available:

Mode               | Description
-------------------|------------
`FullyAutomatic`   | The default mode. Uses the underlying Cosmos DB SDK automatic session token management. `GetSessionTokens` and `UseSessionTokens` methods will throw when invoked.
`SemiAutomatic`    | Allows overwriting the SDK's automatic session token management via `UseSessionTokens`. If `UseSessionTokens` has not been called for a container, the SDK's automatic management is used. EF tracks session tokens which can be retrieved via `GetSessionTokens`.
`Manual`           | Fully replaces SDK automatic session token management. Only session tokens specified via `UseSessionTokens` or tracked by EF operations are used.
`EnforcedManual`   | Same as `Manual`, but throws an exception if `UseSessionTokens` wasn't called for the container before executing an operation on it.

### Retrieving session tokens

After performing read or write operations, you can retrieve the session tokens tracked by EF:

```csharp
// Get the session token for the default container
string? sessionToken = context.Database.GetSessionToken();

// Get session tokens for all containers
IReadOnlyDictionary<string, string?> allTokens = context.Database.GetSessionTokens();
```

### Setting session tokens

Before performing read operations, you can set session tokens to ensure you read your own writes:

```csharp
// Set the session token for the default container
context.Database.UseSessionToken(sessionToken);

// Set session tokens for all containers
context.Database.UseSessionTokens(sessionTokens);
```

### Appending session tokens

You can also append session tokens to the existing ones. This is useful when you have session tokens from multiple sources and want to combine them:

```csharp
// Append a session token for the default container
context.Database.AppendSessionToken(sessionToken);

// Append session tokens for all containers
context.Database.AppendSessionTokens(sessionTokens);
```

### Example: Session token management in a load-balanced web application

The following example demonstrates how to manage session tokens in a web application with multiple instances behind a round-robin load balancer:

```csharp
// After writing data, retrieve and store the session token (e.g., in a cookie or header)
public async Task<IActionResult> CreateDocument(DocumentDto dto)
{
    await using var context = new MyCosmosContext();

    var document = new Document { Name = dto.Name };
    context.Documents.Add(document);
    await context.SaveChangesAsync();

    // Retrieve the session token to return to the client
    var sessionToken = context.Database.GetSessionToken();

    // Store in response header for the client to send back in subsequent requests
    Response.Headers["x-cosmos-session-token"] = sessionToken;

    return Ok(document.Id);
}

// Before reading data, apply the session token from the client
public async Task<IActionResult> GetDocument(int id)
{
    await using var context = new MyCosmosContext();

    // Get the session token from the request header
    if (Request.Headers.TryGetValue("x-cosmos-session-token", out var sessionToken))
    {
        context.Database.UseSessionToken(sessionToken!);
    }

    var document = await context.Documents.FindAsync(id);
    return document is not null ? Ok(document) : NotFound();
}
```

> [!WARNING]
> Manual session token management can break session consistency when not handled properly. Only use manual mode when you have a specific need, such as running behind a load balancer without session affinity. For more information, see [Utilize session tokens](/azure/cosmos-db/nosql/how-to-manage-consistency?tabs=portal%2Cdotnetv2%2Capi-async#utilize-session-tokens) in the Azure Cosmos DB documentation.
