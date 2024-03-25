---
title: Breaking changes in EF Core 9 (EF9) - EF Core
description: List of breaking changes introduced in Entity Framework Core 9 (EF9)
author: ajcvickers
ms.date: 03/25/2024
uid: core/what-is-new/ef-core-9.0/breaking-changes
---

# Breaking changes in EF Core 9 (EF9)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 8 to EF Core 9. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Target Framework

EF Core 9 targets .NET 8. This means that existing applications that target .NET 8 can continue to do so. Applications targeting older .NET, .NET Core, and .NET Framework versions will need to target .NET 8 or .NET 9 to use EF Core 9.

## Summary

| **Breaking change**                                                                | **Impact** |
|:-----------------------------------------------------------------------------------|------------|
| [Sync I/O via the Azure Cosmos DB provider is no longer supported](#cosmos-nosync) | Medium     |

## Medium-impact changes

<a name="cosmos-nosync"></a>

### Sync I/O via the Azure Cosmos DB provider is no longer supported

[Tracking Issue #32563](https://github.com/dotnet/efcore/issues/32563)

#### Old behavior

Previously, calling synchronous methods like `ToList` or `SaveChanges` would cause EF Core to block synchronously using `.GetAwaiter().GetResult()` when executing async calls against the Azure Cosmos DB SDK. This can result in deadlock.

#### New behavior

Starting with EF Core 9.0, EF now throws by default when attempting to use synchronous I/O. The exception message is, "Azure Cosmos DB does not support synchronous I/O. Make sure to use and correctly await only async methods when using Entity Framework Core to access Azure Cosmos DB. See [https://aka.ms/ef-cosmos-nosync](https://aka.ms/ef-cosmos-nosync) for more information."

#### Why

Synchronous blocking on asynchronous methods can result in deadlock, and the Azure Cosmos DB SDK only supports async methods.

#### Mitigations

In EF Core 9.0, the error can be suppressed with:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.ConfigureWarnings(w => w.Ignore(CosmosEventId.SyncNotSupported));
}
```

That being said, applications should stop using sync APIs with Azure Cosmos DB since this is not supported by the Azure Cosmos DB SDK. The ability to suppress the exception will be removed in a future release of EF Core, after which the only option will be to use async APIs.
