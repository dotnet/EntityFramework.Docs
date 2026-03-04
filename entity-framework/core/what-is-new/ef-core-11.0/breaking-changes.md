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
| [EF tools no longer support .NET Framework projects](#ef-tools-no-netfx)                                        | Low        |
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

<a name="ef-tools-no-netfx"></a>

### EF tools no longer support .NET Framework projects

[Tracking Issue #37739](https://github.com/dotnet/efcore/issues/37739)

#### Old behavior

Previously, the EF Core tools (`dotnet-ef` CLI and Package Manager Console tools) did not work with projects targeting .NET Framework, but the error message was unclear.

#### New behavior

Starting with EF Core 11.0, the EF tools produce a clear error message when the startup project targets .NET Framework:

> Startup project '&lt;project name&gt;' targets framework '.NETFramework'. The Entity Framework Core .NET Command-line Tools don't support .NET Framework projects. Consider updating the project to target .NET.

#### Why

The EF Core tools already did not work with .NET Framework projects in EF Core 10.0, but the error produced was not clear. This change provides a more informative error message to help users understand the issue.

#### Mitigations

Update your project to target .NET (e.g., .NET 10 or later). If your project currently targets .NET Framework, see the [porting guide](/dotnet/core/porting/) for information on migrating to .NET.

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
