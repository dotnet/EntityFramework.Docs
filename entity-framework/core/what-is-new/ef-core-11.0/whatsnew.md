---
title: What's New in EF Core 11
description: Overview of new features in EF Core 11
author: AndriySvyryd
ms.date: 07/13/2026
uid: core/what-is-new/ef-core-11.0/whatsnew
---

# What's New in EF Core 11

EF Core 11 (EF11) is the next release after EF Core 10 and is scheduled for release in November 2026.

EF11 is available as a preview. See [.NET 11 release notes](https://github.com/dotnet/core/tree/main/release-notes/11.0) to get information about the latest preview. This article will be updated as new preview releases are made available.

EF11 requires the .NET 11 SDK to build and requires the .NET 11 runtime to run. EF11 will not run on earlier .NET versions, and will not run on .NET Framework.

## Azure Cosmos DB for NoSQL

<a name="cosmos-modernized-materializer"></a>

### Modernized materializer using System.Text.Json

[Tracking Issue #5421](https://github.com/dotnet/EntityFramework.Docs/issues/5421)

EF Core 11 updates the Azure Cosmos DB provider to use `System.Text.Json` (`Utf8JsonReader`/`Utf8JsonWriter`) for document serialization and deserialization, replacing the previous `Newtonsoft.Json`-based approach.

As part of this change:

- The `__jObject` shadow property (of type `JObject`) that was previously added to every entity type has been removed.
- Unmapped properties in Cosmos documents are no longer preserved on round-trip.
- Floating-point query results are now **truncated** (rather than rounded) when materialized to fixed-point types such as `int`, matching standard .NET numeric conversion behavior.

These changes improve performance and align EF Core Cosmos with the rest of the .NET ecosystem. However, they are breaking changes for applications that relied on `__jObject` or unmapped property preservation. See the [breaking changes documentation](xref:core/what-is-new/ef-core-11.0/breaking-changes) for details and mitigations.
