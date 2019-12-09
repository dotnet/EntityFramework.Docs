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

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### [Visual Studio](#tab/vs)

``` powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```

***

## Supported Database Engines

* SQLite (3.7 onwards)

## Limitations

See [SQLite Limitations](limitations.md) for some important limitations of the SQLite provider.
