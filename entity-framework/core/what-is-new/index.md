---
title: EF Core releases and planning
description: Current EF Core releases and schedule/planning details for future releases
author: ajcvickers
ms.date: 01/12/2021
uid: core/what-is-new/index
---

# EF Core releases and planning

## Stable releases

| Release | Target framework | Supported until | Links
|:--------|------------------|-----------------|------
| [EF Core 5.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) | .NET Standard 2.1 | Mid February, 2022 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/) / [Breaking changes](xref:core/what-is-new/ef-core-5.0/breaking-changes)
| [EF Core 3.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.1.10) | .NET Standard 2.0 | December 3, 2022 (LTS) | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-3-1-and-entity-framework-6-4/)
| ~~[EF Core 3.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.0.3)~~ | .NET Standard 2.1 | Expired March 3, 2020 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-ef-core-3-0-and-ef-6-3-general-availability/) / [Breaking changes](xref:core/what-is-new/ef-core-3.x/breaking-changes)
| ~~[EF Core 2.2](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.2.6)~~ | .NET Standard 2.0 | Expired December 23, 2019 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-2/)
| [EF Core 2.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.1.14) | .NET Standard 2.0 | August 21, 2021 (LTS) | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-1/)
| ~~[EF Core 2.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.0.3)~~ | .NET Standard 2.0 | Expired October 1, 2018 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-0/)
| ~~[EF Core 1.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/1.1.6)~~ | .NET Standard 1.3 | Expired June 27 2019 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-1-1/)
| ~~[EF Core 1.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/1.0.6)~~ | .NET Standard 1.3 | Expired June 27 2019 | [Announcement](https://devblogs.microsoft.com/dotnet/entity-framework-core-1-0-0-available/)

See [supported platforms](xref:core/miscellaneous/platforms) for information about the specific platforms supported by each EF Core release.

See the [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) for information on support expiration and long-term support (LTS) releases.

## Guidance on updating to new releases

* Supported releases are patched for security and other critical bugs. Always use the latest patch of a given release. For example, for EF Core 2.1, use 2.1.x for the highest 'x' available.
* Major version updates (for example, from EF Core 2 to EF Core 3) often have breaking changes. Thorough testing is advised when updating across major versions. Use the breaking changes links above for guidance on dealing with breaking changes.
* Minor version updates do not typically contain breaking changes. However, thorough testing is still advised since new features can introduce regressions.

## Release planning and schedules

EF Core releases align with the [.NET Core shipping schedule](https://github.com/dotnet/core/blob/master/roadmap.md).

Patch releases usually ship monthly, but have a long lead time.
We are working to improve this.

See the [release planning process](xref:core/what-is-new/release-planning) for more information on how we decide what to ship in each release.
We typically don't do detailed planning for further out than the next major or minor release.

## EF Core 6.0

The next planned stable release is **EF Core 6.0**, scheduled for **November 2021**.

A [high-level plan for EF Core 6.0](xref:core/what-is-new/ef-core-6.0/plan) has been created by following the documented [release planning process](xref:core/what-is-new/release-planning).

Your feedback on planning is important.
The best way to indicate the importance of an issue is to vote (thumbs-up 👍) for that issue on GitHub.
This data will then feed into the planning process for the next release.

### Get it now

EF Core 6.0 packages are **available now** as

* [Daily builds](https://github.com/dotnet/aspnetcore/blob/master/docs/DailyBuilds.md)
  * All the latest features and bug fixes. Generally very stable; 75,000+ tests run against each build.

In addition, as we progress, frequent previews will be pushed to NuGet. Note that the previews lag behind daily builds, but are tested to work with the corresponding ASP.NET Core and .NET Core previews.

Using the previews or daily builds is a great way to find issues and provide feedback as early as possible.
The sooner we get such feedback, the more likely it will be actionable before the next official release.
