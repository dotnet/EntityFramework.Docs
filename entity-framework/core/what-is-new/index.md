---
title: EF Core releases and planning
description: Current EF Core releases and schedule/planning details for future releases
author: SamMonoRT
ms.date: 11/13/2024
uid: core/what-is-new/index
---

# EF Core releases and planning

## Stable releases

| Release                                                                                | Target framework  | Supported until           | Links                                                                                                                                                                                  |
|----------------------------------------------------------------------------------------|-------------------|---------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [EF Core 9.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)            | .NET 8            | May 12, 2026              | [What's new](xref:core/what-is-new/ef-core-9.0/whatsnew) / [Breaking changes](xref:core/what-is-new/ef-core-9.0/breaking-changes)                                                      |
| [EF Core 8.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)            | .NET 8            | November 10, 2026         | [What's new](xref:core/what-is-new/ef-core-8.0/whatsnew) / [Breaking changes](xref:core/what-is-new/ef-core-8.0/breaking-changes)                                                      |
| ~~[EF Core 7.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/7.0.0)~~  | .NET 6            | Expired May 14, 2024      | [What's new](xref:core/what-is-new/ef-core-7.0/whatsnew) / [Breaking changes](xref:core/what-is-new/ef-core-7.0/breaking-changes)                                                      |
| ~~[EF Core 6.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/6.0.0)~~  | .NET 6            | Expired November 12, 2024 | [What's new](xref:core/what-is-new/ef-core-6.0/whatsnew) / [Breaking changes](xref:core/what-is-new/ef-core-6.0/breaking-changes)                                                      |
| ~~[EF Core 5.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/5.0.17)~~ | .NET Standard 2.1 | Expired May 10, 2022      | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-the-release-of-ef-core-5-0/) / [Breaking changes](xref:core/what-is-new/ef-core-5.0/breaking-changes)                  |
| ~~[EF Core 3.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.1.31)~~ | .NET Standard 2.0 | Expired December 13, 2022 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-3-1-and-entity-framework-6-4/)                                                                   |
| ~~[EF Core 3.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.0.3)~~  | .NET Standard 2.1 | Expired March 3, 2020     | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-ef-core-3-0-and-ef-6-3-general-availability/) / [Breaking changes](xref:core/what-is-new/ef-core-3.x/breaking-changes) |
| ~~[EF Core 2.2](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.2.6)~~  | .NET Standard 2.0 | Expired December 23, 2019 | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-2/)                                                                                            |
| ~~[EF Core 2.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.1.14)~~ | .NET Standard 2.0 | Expired August 21, 2021*  | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-1/)                                                                                            |
| ~~[EF Core 2.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/2.0.3)~~  | .NET Standard 2.0 | Expired October 1, 2018   | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-2-0/)                                                                                            |
| ~~[EF Core 1.1](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/1.1.6)~~  | .NET Standard 1.3 | Expired June 27 2019      | [Announcement](https://devblogs.microsoft.com/dotnet/announcing-entity-framework-core-1-1/)                                                                                            |
| ~~[EF Core 1.0](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/1.0.6)~~  | .NET Standard 1.3 | Expired June 27 2019      | [Announcement](https://devblogs.microsoft.com/dotnet/entity-framework-core-1-0-0-available/)                                                                                           |

See [supported platforms](xref:core/miscellaneous/platforms) for information about the specific platforms supported by each EF Core release.

Entity Framework Core releases and support are aligned with .NET releases and support. See the [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) for information on support expiration and long-term support (LTS) releases.

\* EF Core 2.1 will continue to be supported when used with ASP.NET Core 2.1 on .NET Framework only. See [ASP.NET Support Policy](https://dotnet.microsoft.com/platform/support/policy/aspnet) for details.

## Guidance on updating to new releases

* Supported releases are patched for security and other critical bugs. Always use the latest patch of a given release. For example, for EF Core 9.0, use 9.0.x for the highest 'x' available.
* Major version updates (for example, from EF Core 8 to EF Core 9) often have breaking changes. Thorough testing is advised when updating across major versions. Use the breaking changes links above for guidance on dealing with breaking changes.
* Minor version updates do not typically contain breaking changes. However, thorough testing is still advised since new features can introduce regressions.

## Release planning and schedules

EF Core releases align with the [.NET Core shipping schedule](https://github.com/dotnet/core/blob/main/roadmap.md).

Patch releases usually ship monthly, but have a long lead time.

See the [release planning process](xref:core/what-is-new/release-planning) for more information on how we decide what to ship in each release. We typically don't do detailed planning for further out than the next major or minor release.

## EF Core 10.0

The next planned stable release is **EF Core 10.0**, or just **EF10**, scheduled for **November 2025**.
