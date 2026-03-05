---
title: Breaking changes in EF Core 11 (EF11) - EF Core
description: List of breaking changes introduced in Entity Framework Core 11 (EF11)
author: roji
ms.date: 03/27/2026
uid: core/what-is-new/ef-core-11.0/breaking-changes
---

# Breaking changes in EF Core 11 (EF11)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 10 to EF Core 11. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

> [!NOTE]
> If you are using Microsoft.Data.Sqlite, please see the [separate section below on Microsoft.Data.Sqlite breaking changes](#MDS-breaking-changes).

| **Breaking change**                                                                                             | **Impact** |
|:--------------------------------------------------------------------------------------------------------------- | -----------|
| [Sync I/O via the Azure Cosmos DB provider has been fully removed](#cosmos-nosync)                              | Medium     |
| [EF Core now throws by default when no migrations are found](#migrations-not-found)                             | Low        |
| [`EFOptimizeContext` MSBuild property has been removed](#ef-optimize-context-removed)                            | Low        |
| [EF tools packages no longer reference Microsoft.EntityFrameworkCore.Design](#ef-tools-no-design-dep) | Low        |

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

<a name="MDS-breaking-changes"></a>

## Microsoft.Data.Sqlite breaking changes

> [!NOTE]
> [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw) is an external, community-maintained library that is not owned or maintained by Microsoft. Microsoft.Data.Sqlite depends on it for its SQLite connectivity.

### Summary

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [Encryption-enabled SQLite packages have been removed](#sqlite-encryption-removed)                        | Medium     |
| [Some SQLitePCLRaw bundle packages have been removed](#sqlite-bundles-removed)                            | Medium     |

### Medium-impact changes

<a name="sqlite-encryption-removed"></a>

#### Encryption-enabled SQLite packages have been removed

[Tracking Issue #5108](https://github.com/dotnet/EntityFramework.Docs/issues/5108)

##### Old behavior

Previously, the `SQLitePCLRaw.bundle_e_sqlcipher` NuGet package provided encryption-enabled SQLite builds at no cost.

##### New behavior

Starting with SQLitePCLRaw 3.0 (used by Microsoft.Data.Sqlite 11.0), the `SQLitePCLRaw.bundle_e_sqlcipher` package has been deprecated and removed from NuGet. No-cost encryption-enabled SQLite builds are no longer distributed.

##### Why

The previous no-cost `SQLitePCLRaw.bundle_e_sqlcipher` package was barely maintained, which is a significant concern for encryption software where security vulnerabilities may go unpatched. The SQLitePCLRaw maintainer removed these builds in version 3.0 in favor of professionally maintained, paid alternatives that provide ongoing security updates.

##### Mitigations

If you need SQLite encryption, you have the following options:

- **SQLite Encryption Extension (SEE)**: This is the official encryption implementation from the SQLite team. A paid license is required. See [https://sqlite.org/com/see.html](https://sqlite.org/com/see.html) for details. NuGet packages are available through [SourceGear's SQLite build service](https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-encryption-options-for-use-with-SQLitePCLRaw).
- **SQLCipher**: Purchase supported builds from [Zetetic](https://www.zetetic.net/sqlcipher/), or build the [open source code](https://github.com/sqlcipher/sqlcipher) yourself.
- **SQLite3 Multiple Ciphers**: NuGet packages are available from [SQLite3MultipleCiphers-NuGet](https://github.com/utelle/SQLite3MultipleCiphers-NuGet).
  - When encrypting a new database or opening an existing database that was encrypted with SQLCipher, you must configure the cipher scheme in the connection string using URI parameters—for example: `Data Source=file:example.db?cipher=sqlcipher&legacy=4;Password=<password>`. See [How to open an existing database encrypted with SQLCipher](https://github.com/utelle/SQLite3MultipleCiphers-NuGet#how-to-open-an-existing-database-encrypted-with-sqlcipher) for details.

For more details, see [SQLite encryption options for use with SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-encryption-options-for-use-with-SQLitePCLRaw) and [SQLitePCLRaw 3.0 Release Notes](https://github.com/ericsink/SQLitePCL.raw/blob/main/v3.md).

<a name="sqlite-bundles-removed"></a>

#### Some SQLitePCLRaw bundle packages have been removed

[Tracking Issue #5108](https://github.com/dotnet/EntityFramework.Docs/issues/5108)

##### Old behavior

Previously, the `SQLitePCLRaw.bundle_sqlite3`, `SQLitePCLRaw.bundle_winsqlite3`, `SQLitePCLRaw.bundle_green`, and `SQLitePCLRaw.bundle_e_sqlite3mc` packages provided a convenient way to configure SQLitePCLRaw with the corresponding SQLite provider.

##### New behavior

Starting with SQLitePCLRaw 3.0 (used by Microsoft.Data.Sqlite 11.0), these bundle packages have been removed. If your application depended on one of these bundles, you must now reference the corresponding provider package and explicitly initialize it.

##### Why

Each of these bundle packages contained only a single line of configuration code and added unnecessary packaging overhead. The corresponding provider packages are still supported.

##### Mitigations

Replace the removed bundle package with the corresponding provider package and add explicit initialization code.

**If using `bundle_sqlite3` or `bundle_winsqlite3`**, replace the package reference:

```xml
<!-- Old -->
<PackageReference Include="SQLitePCLRaw.bundle_sqlite3" Version="2.x.x" />
<!-- or -->
<PackageReference Include="SQLitePCLRaw.bundle_winsqlite3" Version="2.x.x" />

<!-- New -->
<PackageReference Include="SQLitePCLRaw.provider.sqlite3" Version="3.x.x" />
<!-- or -->
<PackageReference Include="SQLitePCLRaw.provider.winsqlite3" Version="3.x.x" />
```

Then add explicit initialization before using SQLite:

```csharp
// For sqlite3
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
}

// For winsqlite3
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
}
```

**If using `bundle_green`**, the recommended migration path is to switch to `SQLitePCLRaw.bundle_e_sqlite3`. Alternatively, use `SQLitePCLRaw.config.e_sqlite3` paired with a separate native library package like `SourceGear.sqlite3`, which allows you to update the SQLite version independently:

```xml
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="3.x.x" />
```

If you only target iOS and want to continue using the system SQLite library, reference the provider directly:

```xml
<PackageReference Include="SQLitePCLRaw.core" Version="3.x.x" />
<PackageReference Include="SQLitePCLRaw.provider.sqlite3" Version="3.x.x" />
```

And initialize it explicitly:

```csharp
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
}
```

> [!NOTE]
> If you are using `SQLitePCLRaw.bundle_e_sqlite3`, no changes are required—just update the version number. See the [SQLitePCLRaw 3.0 Release Notes](https://github.com/ericsink/SQLitePCL.raw/blob/main/v3.md) for details.
