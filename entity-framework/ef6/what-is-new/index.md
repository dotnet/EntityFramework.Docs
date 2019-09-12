---
title: "What's New - EF6"
author: divega
ms.date: "09/12/2019"
ms.assetid: 41d1f86b-ce66-4bf2-8963-48514406fb4c
---
# What's New in EF6

We highly recommend that you use the latest released version of Entity Framework to ensure you get the latest features and the highest stability.
However, we realize that you may need to use a previous version, or that you may want to experiment with new improvements in the latest pre-release.
To install specific versions of EF, see [Get Entity Framework](~/ef6/fundamentals/install.md).

## EF 6.3.0

The EF 6.3.0 runtime was released to NuGet in September 2019. The main goal of this release was to facilitate migrating existing applications that use EF 6 to .NET Core 3.0. The community has also contributed several bug fixes and enhancements. See the issues closed in each 6.3.0 [milestone](https://github.com/aspnet/EntityFramework6/milestones?state=closed) for details. Here are some of the more notable ones:

- Support for .NET Core 3.0
  - The EntityFramework package now targets .NET Standard 2.1 in addition to .NET Framework 4.x
  - The migrations commands have been rewritten to execute out of process and work with SDK-style projects
- Support for SQL Server HierarchyId
- Improved compatibility with Roslyn and NuGet PackageReference
- Added ef6.exe for enabling, adding, scripting, and applying migrations from assemblies. This replaces migrate.exe

## Past Releases

The [Past Releases](past-releases.md) page contains an archive of all previous versions of EF and the major features that were introduced on each release.
