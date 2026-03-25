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
| [EF Core now throws by default when no migrations are found](#migrations-not-found)                             | Low        |
| [`EFOptimizeContext` MSBuild property has been removed](#ef-optimize-context-removed)                           | Low        |
| [EF tools packages no longer reference Microsoft.EntityFrameworkCore.Design](#ef-tools-no-design-dep)           | Low        |
| [SqlVector properties are no longer loaded by default](#sqlvector-not-auto-loaded)                              | Low        |
| [Cosmos: empty owned collections now return an empty collection instead of null](#cosmos-empty-collections)     | Low        |
| [Microsoft.Data.SqlClient has been updated to 7.0](#sqlclient-7)                                               | Low        |

## Medium-impact changes

<a name="cosmos-nosync"></a>

### Sync I/O via the Azure Cosmos DB provider has been fully removed

[Tracking Issue #37059](https://github.com/dotnet/efcore/issues/37059)

#### Old behavior

Synchronous I/O via the Azure Cosmos DB provider has been unsupported since EF 9.0 ([note](/ef/core/what-is-new/ef-core-9.0/breaking-changes#cosmos-nosync)); calling any sync I/O API - like `ToList` or `SaveChanges` threw an exception, unless a special opt-in was configured. When the opt-in was configured, sync I/O APIs worked as before, causing the provider to perform "sync-over-async" blocking against the Azure Cosmos DB SDK, which could result in deadlocks and other performance issues.

#### New behavior

Starting with EF Core 11.0, EF now always throws when a synchronous I/O API is called. There is no way to opt back into using sync I/O APIs.

#### Why

Synchronous blocking on asynchronous methods ("sync-over-async") is highly discouraged, and can lead to deadlock and other performance problems. Since the Azure Cosmos DB SDK only supports async methods, so does the EF Cosmos provider.

#### Mitigations

Convert your code to use async I/O APIs instead of sync I/O ones. For example, replace calls to `SaveChanges()` with `await SaveChangesAsync()`.

## Low-impact changes

<a name="migrations-not-found"></a>

### EF Core now throws by default when no migrations are found

[Tracking Issue #35218](https://github.com/dotnet/efcore/issues/35218)

#### Old behavior

Previously, when calling <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate*> or <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync*> on a database with no migrations in the assembly, EF Core logged an informational message and returned without applying any changes.

#### New behavior

Starting with EF Core 11.0, EF Core throws an exception by default when no migrations are found in the assembly. This is consistent with the `PendingModelChangesWarning` behavior [introduced in EF 9.0](xref:core/what-is-new/ef-core-9.0/breaking-changes#pending-model-changes).

#### Why

Calling `Migrate()` or `MigrateAsync()` when no migrations exist typically indicates a misconfiguration. Rather than silently continuing and leaving the database in a potentially incorrect state, EF Core now alerts developers to this issue immediately.

#### Mitigations

If you intentionally call `Migrate()` without having any migrations (for example, because you manage the database schema through other means), remove the `Migrate()` call or suppress the exception by configuring warnings:

```csharp
options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsNotFound))
```

Or to log the event instead of throwing:

```csharp
options.ConfigureWarnings(w => w.Log(RelationalEventId.MigrationsNotFound))
```

<a name="ef-optimize-context-removed"></a>

### `EFOptimizeContext` MSBuild property has been removed

[Tracking Issue #35079](https://github.com/dotnet/efcore/issues/35079)

#### Old behavior

Previously, the `EFOptimizeContext` MSBuild property could be set to `true` to enable compiled model and precompiled query code generation during build or publish:

```xml
<EFOptimizeContext Condition="'$(Configuration)'=='Release'">true</EFOptimizeContext>
```

#### New behavior

Starting with EF Core 11.0, the `EFOptimizeContext` MSBuild property has been removed. Code generation is now controlled exclusively through the `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties. When `PublishAOT` is set to `true`, code generation is automatically enabled during publish without needing any additional property.

#### Why

The `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties already provide fine-grained control over when code generation occurs. `EFOptimizeContext` was a redundant enablement gate.

#### Mitigations

Replace usages of `EFOptimizeContext` with the `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties. These can be set to `publish` or `build` to control at which stage code generation occurs:

```xml
<EFScaffoldModelStage>publish</EFScaffoldModelStage>
<EFPrecompileQueriesStage>publish</EFPrecompileQueriesStage>
```

Any other value (for example, `none`) disables the corresponding generation.

If you have `PublishAOT` set to `true`, code generation is automatically enabled during publish and no additional configuration is needed.

<a name="ef-tools-no-design-dep"></a>

### EF tools packages no longer reference Microsoft.EntityFrameworkCore.Design

[Tracking Issue #37739](https://github.com/dotnet/efcore/issues/37739)

#### Old behavior

Previously, the `Microsoft.EntityFrameworkCore.Tools` and `Microsoft.EntityFrameworkCore.Tasks` NuGet packages had a dependency on `Microsoft.EntityFrameworkCore.Design`.

#### New behavior

Starting with EF Core 11.0, the `Microsoft.EntityFrameworkCore.Tools` and `Microsoft.EntityFrameworkCore.Tasks` NuGet packages no longer have a dependency on `Microsoft.EntityFrameworkCore.Design`.

#### Why

There was no hard dependency on the code in `Microsoft.EntityFrameworkCore.Design`, and this dependency was causing issues when using the latest `Microsoft.EntityFrameworkCore.Tools` with projects targeting older frameworks.

#### Mitigations

If your project relies on `Microsoft.EntityFrameworkCore.Design` being brought in transitively through the tools packages, add a direct reference to it in your project:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="11.0.0" PrivateAssets="all" />
```

<a name="sqlvector-not-auto-loaded"></a>

### SqlVector properties are no longer loaded by default

[Tracking Issue #37279](https://github.com/dotnet/efcore/issues/37279)

#### Old behavior

Previously, when querying entities with `SqlVector<T>` properties, EF Core included the vector column in `SELECT` statements and populated the property on the returned entity.

#### New behavior

Starting with EF Core 11.0, `SqlVector<T>` properties are no longer included in `SELECT` statements when materializing entities. The property will be `null` on returned entities.

Vector properties can still be used in `WHERE` and `ORDER BY` clauses—including with `VectorDistance()` and `VectorSearch()`; they just won't be included in the entity projection.

#### Why

Vector columns can be very large, containing hundreds or thousands of floating-point values. In the vast majority of cases, vectors are written to the database and then used for search, without needing to be read back. Excluding them from `SELECT` by default avoids unnecessary data transfer.

#### Mitigations

> [!NOTE]
> A mechanism for opting vector properties back into automatic loading will be introduced later in the EF Core 11 release.

If you need to read back vector values, use an explicit projection:

```csharp
var embeddings = await context.Blogs
    .Select(b => new { b.Id, b.Embedding })
    .ToListAsync();
```

<a name="cosmos-empty-collections"></a>

### Cosmos: empty owned collections now return an empty collection instead of null

[Tracking Issue #36577](https://github.com/dotnet/efcore/issues/36577)

#### Old behavior

Previously, when querying entities via the Azure Cosmos DB provider where an owned collection contained no items, the collection property was `null` on the materialized entity.

#### New behavior

Starting with EF Core 11.0, the Azure Cosmos DB provider correctly initializes empty owned collections, returning an empty collection instead of `null`.

#### Why

The previous behavior of materializing empty owned collections as `null` was a bug.

#### Mitigations

If your code explicitly checks owned collection properties for `null` to detect that the collection is empty, those checks can simply be removed, since the collection is now always initialized:

```csharp
// Before
if (entity.OwnedCollection is null or { Count: 0 })
{
    // treated as empty
}

// After
if (entity.OwnedCollection is { Count: 0 })
{
    // treated as empty
}
```

<a name="sqlclient-7"></a>

### Microsoft.Data.SqlClient has been updated to 7.0

[Tracking Issue: dotnet/efcore#37949](https://github.com/dotnet/efcore/pull/37949)

#### Old behavior

EF Core 10 used [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) 6.x, which included Azure/Entra ID authentication dependencies (such as `Azure.Core`, `Azure.Identity`, and `Microsoft.Identity.Client`) in the core package.

#### New behavior

EF Core 11 now depends on [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) 7.0. This version removes Azure/Entra ID (formerly Azure Active Directory) authentication dependencies from the core package. If your application uses Entra ID authentication (for example, `ActiveDirectoryDefault`, `ActiveDirectoryInteractive`, `ActiveDirectoryManagedIdentity`, or `ActiveDirectoryServicePrincipal`), you must now install the [`Microsoft.Data.SqlClient.Extensions.Azure`](https://www.nuget.org/packages/Microsoft.Data.SqlClient.Extensions.Azure/) package separately.

In addition, `SqlAuthenticationMethod.ActiveDirectoryPassword` has been marked as obsolete.

For more details, see the [Microsoft.Data.SqlClient 7.0 release notes](https://github.com/dotnet/SqlClient/blob/main/release-notes/7.0/7.0.0.md).

#### Why

This change was made in [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) to reduce dependency bloat for applications that don't use Azure authentication, which is especially beneficial for containerized deployments and local development.

#### Mitigations

If your application uses Entra ID authentication with SQL Server, add a reference to the `Microsoft.Data.SqlClient.Extensions.Azure` package in your project:

```xml
<PackageReference Include="Microsoft.Data.SqlClient.Extensions.Azure" Version="7.0.0" />
```

No code changes are required beyond adding this package reference. If you use `SqlAuthenticationMethod.ActiveDirectoryPassword`, migrate to a modern authentication method such as `ActiveDirectoryDefault` or `ActiveDirectoryInteractive`.
