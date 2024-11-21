---
title: SQLite Database Provider - EF Core
description: Information on the Entity Framework Core SQLite database provider
author: SamMonoRT
ms.date: 10/27/2016
uid: core/providers/sqlite/index
---
# SQLite EF Core Database Provider

This database provider allows Entity Framework Core to be used with SQLite. The provider is maintained as part of the [Entity Framework Core project](https://github.com/dotnet/efcore).

## Install

Install the [Microsoft.EntityFrameworkCore.Sqlite NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/).

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### [Visual Studio](#tab/vs)

```powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```

***

## Supported Database Engines

* SQLite (3.46.1 onwards)

## Limitations

See [SQLite Limitations](xref:core/providers/sqlite/limitations) for some important limitations of the SQLite provider.
