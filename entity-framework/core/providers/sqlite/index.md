---
title: SQLite Database Provider - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 3e2f7698-fec2-4cec-9e2d-2e3e0074120c
uid: core/providers/sqlite/index
---
# SQLite EF Core Database Provider

This database provider allows Entity Framework Core to be used with SQLite. The provider is maintained as part of the [Entity Framework Core project](https://github.com/aspnet/EntityFrameworkCore).

## Install

Install the [Microsoft.EntityFrameworkCore.Sqlite NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/).

``` powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```

## Get Started

The following resources will help you get started with this provider.
* [Local SQLite on UWP](../../get-started/uwp/getting-started.md)

* [.NET Core Application to New SQLite Database](../../get-started/netcore/new-db-sqlite.md)

* [Unicorn Clicker Sample Application](https://github.com/rowanmiller/UnicornStore/tree/master/UnicornClicker/UWP)

* [Unicorn Packer Sample Application](https://github.com/rowanmiller/UnicornStore/tree/master/UnicornPacker)

## Supported Database Engines

* SQLite (3.7 onwards)

## Supported Platforms

* .NET Framework (4.5.1 onwards)

* .NET Core

* Mono (4.2.0 onwards)

* Universal Windows Platform

## Limitations

See [SQLite Limitations](limitations.md) for some important limitations of the SQLite provider.
