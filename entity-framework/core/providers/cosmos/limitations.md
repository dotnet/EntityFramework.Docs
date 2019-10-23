---
title: Azure Cosmos DB Provider - Limitations - EF Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 09/12/2019
ms.assetid: 9d02a2cd-484e-4687-b8a8-3748ba46dbc9
uid: core/providers/cosmos/limitations
---
# EF Core Azure Cosmos DB Provider Limitations

The Cosmos provider has a number of limitations. Many of these limitations are a result of limitations in the underlying Cosmos database engine and are not specific to EF. But most simply [haven't been implemented yet](https://github.com/aspnet/EntityFrameworkCore/issues?page=1&q=is%3Aissue+is%3Aopen+Cosmos+in%3Atitle+label%3Atype-enhancement+sort%3Areactions-%2B1-desc).

## Temporary limitations

- Even if there is only one entity type without inheritance mapped to a container it still has a discriminator property.
- Entity types with partition keys don't work correctly in some scenarios
- `Include` calls are not supported
- `Join` calls are not supported

## Azure Cosmos DB SDK limitations

- Only async methods are provided

> [!WARNING]
> Since there are no sync versions of the low level methods EF Core relies on, the corresponding functionality is currently implemented by calling `.Wait()` on the returned `Task`. This means that using methods like `SaveChanges`, or `ToList` instead of their async counterparts could lead to a deadlock in your application

## Azure Cosmos DB limitations

You can see the full overview of [Azure Cosmos DB supported features](https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data), these are the most notable differences compared to a relational database:

- Client-initiated transactions are not supported
- Some cross-partition queries are either not supported or much slower depending on the operators involved
