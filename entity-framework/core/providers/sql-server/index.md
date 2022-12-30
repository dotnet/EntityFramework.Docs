---
title: Microsoft SQL Server Database Provider - EF Core
description: Documentation for the database provider that allows Entity Framework Core to be used with Microsoft SQL Server
author: AndriySvyryd
ms.date: 11/15/2021
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
> The provider references Microsoft.Data.SqlClient (not System.Data.SqlClient). If your project takes a direct dependency on SqlClient, make sure it references the Microsoft.Data.SqlClient package.

>[!TIP]
> The Microsoft.Data.SqlClient package ships more frequently than the EF Core provider. If you would like to take advantage of new features and bug fixes, you can add a direct package reference to the latest version of Microsoft.Data.SqlClient.

> [!WARNING]
> The async implementation of [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient) unfortunately has some known issues (e.g. [#593](https://github.com/dotnet/SqlClient/issues/593), [#601](https://github.com/dotnet/SqlClient/issues/601), and others). If you're seeing unexpected performance problems, try using sync command execution instead, especially when dealing with large text or binary values.

## Supported Database Engines

* Microsoft SQL Server (2012 onwards)
