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

| **Breaking change**                                                                                  | **Impact** |
|:-----------------------------------------------------------------------------------------------------|------------|
| [Sync I/O via the Azure Cosmos DB provider is no longer supported](#cosmos-nosync)                   | Medium     |
| [EF.Functions.Unhex() now returns `byte[]?`](#unhex)                                                 | Low        |
| [SqlFunctionExpression's nullability arguments' arity validated](#sqlfunctionexpression-nullability) | Low        |

## Medium-impact changes

<a name="cosmos-nosync"></a>

### Sync I/O via the Azure Cosmos DB provider is no longer supported

[Tracking Issue #32563](https://github.com/dotnet/efcore/issues/32563)

#### Old behavior

Previously, calling synchronous methods like `ToList` or `SaveChanges` would cause EF Core to block synchronously using `.GetAwaiter().GetResult()` when executing async calls against the Azure Cosmos DB SDK. This can result in deadlock.

#### New behavior

Starting with EF Core 9.0, EF now throws by default when attempting to use synchronous I/O. The exception message is "Azure Cosmos DB does not support synchronous I/O. Make sure to use and correctly await only async methods when using Entity Framework Core to access Azure Cosmos DB. See [https://aka.ms/ef-cosmos-nosync](https://aka.ms/ef-cosmos-nosync) for more information."

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

## Low-impact changes

<a name="unhex"></a>

### EF.Functions.Unhex() now returns `byte[]?`

[Tracking Issue #33864](https://github.com/dotnet/efcore/issues/33864)

#### Old behavior

The EF.Functions.Unhex() function was previously annotated to return `byte[]`.

#### New behavior

Starting with EF Core 9.0, Unhex() is now annotated to return `byte[]?`.

#### Why

Unhex() is translated to the SQLite `unhex` function, which returns NULL for invalid inputs. As a result, Unhex() returned `null` for those cases, in violation of the annotation.

#### Mitigations

If you are sure that the text content passed to Unhex() represents a valid, hexadecimal string, you can simply add the null-forgiving operator as an assertion that the invocation will never return null:

```c#
var binaryData = await context.Blogs.Select(b => EF.Functions.Unhex(b.HexString)!).ToListAsync();
```

Otherwise, add runtime checks for null on the return value of Unhex().

<a name="sqlfunctionexpression-nullability"></a>

### SqlFunctionExpression's nullability arguments' arity validated

[Tracking Issue #33852](https://github.com/dotnet/efcore/issues/33852)

#### Old behavior

Previously it was possible to create a `SqlFunctionExpression` with a different number of arguments and nullability propagation arguments.

#### New behavior

Starting with EF Core 9.0, EF now throws if the number of arguments and nullability propagation arguments do not match.

#### Why

Not having matching number of arguments and nullability propagation arguments can lead to unexpected behavior.

#### Mitigations

Make sure the `argumentsPropagateNullability` has same number of elements as the `arguments`. When in doubt use `false` for nullability argument.
