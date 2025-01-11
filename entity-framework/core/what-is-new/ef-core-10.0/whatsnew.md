---
title: What's New in EF Core 10
description: Overview of new features in EF Core 10
author: maumar
ms.date: 01/05/2025
uid: core/what-is-new/ef-core-10.0/whatsnew
---

# What's New in EF Core 10

EF Core 10 (EF10) is the next release after EF Core 9 and is scheduled for release in November 2025.

EF10 is available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF10 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF10 requires the .NET 10 SDK to build and requires the .NET 10 runtime to run. EF10 will not run on earlier .NET versions, and will not run on .NET Framework.

> [!TIP]
> The _What's New_ docs are updated for each preview. All the samples are set up to use the [EF10 daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md), which usually have several additional weeks of completed work compared to the latest preview. We strongly encourage use of the daily builds when testing new features so that you're not doing your testing against stale bits.

<a name="linq-and-sql-translation"></a>

## LINQ and SQL translation

<a name="other-query-improvements"></a>

### Other query improvements

* Translation for DateOnly.ToDateTime(timeOnly) ([#35194](https://github.com/dotnet/efcore/pull/35194), contributed by [@mseada94](https://github.com/mseada94)).
* Optimization for multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384)), contributed by [@ranma42](https://github.com/ranma42)).
* Optimization for use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381)), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
