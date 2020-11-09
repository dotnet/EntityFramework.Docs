---
title: Microsoft SQL Server Database Provider - EF Core
description: Documentation for the database provider that allows Entity Framework Core to be used with Microsoft SQL Server
author: AndriySvyryd
ms.date: 11/05/2019
uid: core/providers/sql-server/index
---
# Microsoft SQL Server EF Core Database Provider

This database provider allows Entity Framework Core to be used with Microsoft SQL Server (including Azure SQL Database). The provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

## Install

Install the [Microsoft.EntityFrameworkCore.SqlServer NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/).

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### [Visual Studio](#tab/vs)

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
```

***

> [!NOTE]
> Since version 3.0.0, the provider references Microsoft.Data.SqlClient (previous versions depended on System.Data.SqlClient). If your project takes a direct dependency on SqlClient, make sure it references the Microsoft.Data.SqlClient package.

## Supported Database Engines

* Microsoft SQL Server (2012 onwards)
