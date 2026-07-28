---
title: What's New in EF Core 11
description: Overview of new features in EF Core 11
author: AndriySvyryd
ms.date: 07/28/2026
uid: core/what-is-new/ef-core-11.0/whatsnew
---

# What's New in EF Core 11

EF Core 11 (EF11) is the next release after EF Core 10 and is scheduled for release in November 2026.

EF11 is available as a preview. See [.NET 11 release notes](https://github.com/dotnet/core/tree/main/release-notes/11.0) to get information about the latest preview. This article will be updated as new preview releases are made available.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF11 requires the .NET 11 SDK to build and requires the .NET 11 runtime to run. EF11 will not run on earlier .NET versions, and will not run on .NET Framework.

## Azure Cosmos DB for NoSQL

### Improved projection materialization

EF Core 11 modernizes the Cosmos DB query materializer to use `Utf8JsonReader` directly, improving performance and correctness. As part of this work, projections of embedded/owned types are now bound more precisely, client-side projection distinct limitations have been removed, and the behavior of undefined values in projections is now consistent.

Previously, projecting properties via optional navigations where part of the path was absent in a document (i.e., the value was `undefined` in Cosmos DB) gave inconsistent results: single-property anonymous type projections would silently filter out such documents, while multi-property projections would throw a cryptic exception. Starting with EF Core 11, a clear `InvalidOperationException` is thrown whenever a projection evaluates to `undefined`, making the problem immediately visible.

Use <xref:Microsoft.EntityFrameworkCore.CosmosDbFunctionsExtensions.IsDefined*> to filter out undefined values, or <xref:Microsoft.EntityFrameworkCore.CosmosDbFunctionsExtensions.CoalesceUndefined*> to provide fallback values:

```csharp
// Filter out documents where the nested value is absent
var results = await context.Entities
    .Where(x => EF.Functions.IsDefined(x.Associate!.NestedAssociate!.Id))
    .Select(x => new { x.Associate!.NestedAssociate!.Id })
    .ToListAsync();
```

For more information, see the [breaking changes documentation](xref:core/what-is-new/ef-core-11.0/breaking-changes#cosmos-undefined-projection).
