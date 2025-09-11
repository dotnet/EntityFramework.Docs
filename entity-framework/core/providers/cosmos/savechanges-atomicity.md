---
title: Azure Cosmos DB Provider - Atomicity of SaveChanges - EF Core
description: Explains the atomicity of SaveChanges within the Entity Framework Core Azure Cosmos DB provider as compared to other providers
author: JoasE
ms.date: 09/10/2025
uid: core/providers/cosmos/savechanges-atomicity
---
# EF Core Azure Cosmos DB Provider Atomicity of SaveChanges

Azure Cosmos DB does not support transactions in the relational database sense. That is, there is no concept of a single atomic transaction spanning arbitrary operations across containers or partitions. This is a common limitation of document databases, where the focus is on scalability and availability rather than strict transactional semantics.

Instead of transactions, Azure Cosmos DB provides support for [transactional batches](/azure/cosmos-db/nosql/transactional-batch). A transactional batch allows up to 100 operations to be executed together as a batch within a single partition. Atomicity is guaranteed within a single batch: if any operation fails, the entire batch is rolled back and none of its changes are applied. However, once a batch is written, it cannot be rolled back or deferred, and atomicity cannot be enforced across multiple batches.

The EF Core Azure Cosmos DB provider leverages transactional batches whenever possible, providing a best-effort approximation of atomicity when saving changes.

## How EF Core saves changes in batches

When SaveChanges or SaveChangesAsync is called, the provider groups pending changes into transactional batches. Each batch contains up to 100 entries, grouped by container and partition key. These batches are then executed sequentially. If a batch fails, execution stops immediately and no subsequent batches are attempted. Any batches that were successfully committed before the failure remain saved.

> [!WARNING]
> Azure Cosmos DB does not allow document writes with [pre- or post-triggers](/azure/cosmos-db/nosql/stored-procedures-triggers-udfs#triggers) to be part of a transactional batch. Because of this, any entities configured with triggers are executed separately and before any transactional batches. This can affect ordering and consistency in mixed scenarios.

## Controlling batching behavior

The batching behavior of the EF Core Azure Cosmos DB provider is controlled by the <xref:Microsoft.EntityFrameworkCore.Storage.IDatabase.AutoTransactionBehavior> property. This setting allows developers to trade off between performance, consistency guarantees, and failure behavior depending on the application’s needs.

* **Auto** (default) – Entries are grouped into transactional batches as described above. This generally provides good performance for writing to multiple entities within the same partition with a best-effort approximation of atomicity.
* **Never** – All entries are written individually, in the exact order they were tracked. This avoids batching and can be slower, especially for large numbers of entries.
* **Always** – Requires that all changes can be executed as a single atomic operation. If any entries cannot be included in a batch (for example, due to partitioning, exceeding 100 items, or there are multiple changed entries including one with triggers), the provider will throw an exception.

```csharp
using var context = new BlogsContext();
context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
context.AddRange(Enumerable.Range(0, 101).Select(i => new Post()));
await context.SaveChangesAsync(); // Throws InvalidOperationException
```
