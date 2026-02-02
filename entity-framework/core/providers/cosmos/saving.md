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
