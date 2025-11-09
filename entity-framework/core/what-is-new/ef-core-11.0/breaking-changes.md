---
title: Breaking changes in EF Core 11 (EF11) - EF Core
description: List of breaking changes introduced in Entity Framework Core 11 (EF11)
author: roji
ms.date: 11/09/2025
uid: core/what-is-new/ef-core-11.0/breaking-changes
---

# Breaking changes in EF Core 11 (EF11)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 10 to EF Core 11. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

| **Breaking change**                                                                                             | **Impact** |
|:--------------------------------------------------------------------------------------------------------------- | -----------|
| [Sync I/O via the Azure Cosmos DB provider has been fully removed](#cosmos-nosync)                              | Medium     |

### Medium-impact changes

<a name="cosmos-nosync"></a>

#### Sync I/O via the Azure Cosmos DB provider has been fully removed

[Tracking Issue #37059](https://github.com/dotnet/efcore/issues/37059)

##### Old behavior

Synchronous I/O via the Azure Cosmos DB provider has been unsupported since EF 9.0 ([note](/ef/core/what-is-new/ef-core-9.0/breaking-changes#cosmos-nosync)); calling any sync I/O API - like `ToList` or `SaveChanges` threw an exception, unless a special opt-in was configured. When the opt-in was configured, sync I/O APIs worked as before, causing the provider to perform "sync-over-async" blocking against the Azure Cosmos DB SDK, which could result in deadlocks and other performance issues.

##### New behavior

Starting with EF Core 11.0, EF now always throws when a synchronous I/O API is called. There is no way to opt back into using sync I/O APIs.

##### Why

Synchronous blocking on asynchronous methods ("sync-over-async") is highly discouraged, and can lead to deadlock and other performance problems. Since the Azure Cosmos DB SDK only supports async methods, so does the EF Cosmos provider.

##### Mitigations

Convert your code to use async I/O APIs instead of sync I/O ones. For example, replace calls to `SaveChanges()` with `await SaveChangesAsync()`.
