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

Note that unlike with relational databases, Azure Cosmos DB doesn't provide transaction guarantees by default across multiple documents.

## Bulk execution

> [!NOTE]
> Bulk execution is being introduced in EF Core 11, which is currently in preview.

By default, EF Core executes document operations sequentially when calling `SaveChangesAsync`. When saving a large number of entities, this can be slow as each operation waits for the previous one to complete before starting.

Azure Cosmos DB supports _bulk execution_, which allows multiple document operations to be executed in parallel, significantly improving throughput when saving many entities at once. This is especially useful for data loading scenarios, batch operations, or any situation where you need to save many entities.

To enable bulk execution, configure your context using the <xref:Microsoft.EntityFrameworkCore.Infrastructure.CosmosDbContextOptionsBuilder.BulkExecutionEnabled*> option:

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

To enable manual session token management, configure the <xref:Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.SessionTokenManagementMode> when setting up your context:

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
`EnforcedManual`   | Same as `Manual`, but throws an exception if `UseSessionTokens` was not called before executing a read operation.

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
