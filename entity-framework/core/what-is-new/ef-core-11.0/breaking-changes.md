---
title: Breaking changes in EF Core 11 (EF11) - EF Core
description: List of breaking changes introduced in Entity Framework Core 11 (EF11)
author: AndriySvyryd
ms.date: 07/28/2026
uid: core/what-is-new/ef-core-11.0/breaking-changes
---

# Breaking changes in EF Core 11 (EF11)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 10 to EF Core 11. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Summary

> [!NOTE]
> If you are using Azure Cosmos DB, please see the [separate section below on Azure Cosmos DB breaking changes](#azure-cosmos-db-breaking-changes).

## Azure Cosmos DB breaking changes

### Summary

| **Breaking change**                                                                                                                           | **Impact** |
|:----------------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Exception thrown when a non-entity projection evaluates to undefined](#cosmos-undefined-projection)                                          | Medium     |

### Medium-impact changes

<a name="cosmos-undefined-projection"></a>

### Exception thrown when a non-entity projection evaluates to undefined

[Tracking Issue #38550](https://github.com/dotnet/efcore/pull/38550)

#### Old behavior

Previously, when projecting scalar properties or anonymous/DTO types via navigation over optional relationships where a segment of the path was absent in the Cosmos DB document (causing the value to be `undefined`), the behavior was inconsistent:

- With single-property anonymous type or DTO projections, EF translated the query using `SELECT VALUE`, which silently filtered out any documents where the projected value was `undefined`. This meant fewer results were returned than expected, with no indication of the missing data.
- With multi-property anonymous type or DTO projections, an `InvalidOperationException` with the message "Nullable object must have a value" was thrown.

For example, given an entity `Entity` with an optional owned `Associate` which in turn has an optional owned `NestedAssociate`, the following queries behaved differently:

```csharp
// Silently returned fewer results (undefined results were filtered out)
var singlePropResults = await context.Entities
    .Select(x => new { x.Associate!.NestedAssociate!.Id })
    .ToListAsync();

// Threw InvalidOperationException: Nullable object must have a value
var multiPropResults = await context.Entities
    .Select(x => new { x.Associate!.NestedAssociate!.Id, x.Associate.NestedAssociate.String })
    .ToListAsync();
```

#### New behavior

Starting with EF Core 11.0, an `InvalidOperationException` is thrown whenever any part of a non-entity projection evaluates to `undefined` in Azure Cosmos DB. The exception message is:

> A part of the projection was undefined, use the coalesce operator to handle possible undefined values.

This applies to all anonymous type and DTO projections, regardless of how many properties are projected.

#### Why

The previous behavior was inconsistent and could silently discard data, making it difficult to diagnose unexpected missing results. The new behavior ensures consistent, predictable error reporting.

#### Mitigations

Use <xref:Microsoft.EntityFrameworkCore.CosmosDbFunctionsExtensions.IsDefined*> to filter out documents with missing values before projecting:

```csharp
var results = await context.Entities
    .Where(x => EF.Functions.IsDefined(x.Associate!.NestedAssociate!.Id))
    .Select(x => new { x.Associate!.NestedAssociate!.Id })
    .ToListAsync();
```

Alternatively, use <xref:Microsoft.EntityFrameworkCore.CosmosDbFunctionsExtensions.CoalesceUndefined*> to substitute a default value for any property that could be `undefined`:

```csharp
var results = await context.Entities
    .Select(x => new { Id = EF.Functions.CoalesceUndefined(x.Associate!.NestedAssociate!.Id, Guid.Empty) })
    .ToListAsync();
```
