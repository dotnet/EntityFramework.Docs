---
title: Azure Cosmos DB Provider - Limitations - EF Core
description: Limitations of the Entity Framework Core Azure Cosmos DB provider as compared to other providers
author: AndriySvyryd
ms.date: 11/05/2019
uid: core/providers/cosmos/limitations
---
# EF Core Azure Cosmos DB Provider Limitations

The Cosmos provider has a number of limitations. Many of these limitations are a result of limitations in the underlying Cosmos database engine and are not specific to EF. But most simply [haven't been implemented yet](https://github.com/dotnet/efcore/issues?page=1&q=is%3Aissue+is%3Aopen+Cosmos+in%3Atitle+label%3Atype-enhancement+sort%3Areactions-%2B1-desc).

## Temporary limitations

- `Include` calls are not supported
- `Join` calls are not supported

## Azure Cosmos DB SDK limitations

- Only async methods are provided

> [!WARNING]
> Since there are no sync versions of the low level methods EF Core relies on, the corresponding functionality is currently implemented by calling `.Wait()` on the returned `Task`. This means that using methods like `SaveChanges`, or `ToList` instead of their async counterparts could lead to a deadlock in your application

## Azure Cosmos DB limitations

You can see the full overview of [Azure Cosmos DB supported features](/azure/cosmos-db/modeling-data), these are the most notable differences compared to a relational database:

- Client-initiated transactions are not supported
- Some cross-partition queries are either not supported or much slower depending on the operators involved
