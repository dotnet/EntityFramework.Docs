---
title: Microsoft.Data.Sqlite
author: bricelam
ms.date: 12/05/2019
ms.assetid: 08f6f235-6889-44bc-afe0-a7b055a96538
uid: index
---
# Microsoft.Data.Sqlite

Microsoft.Data.Sqlite is a lightweight [ADO.NET](/dotnet/framework/data/adonet/) provider for SQLite. The [Entity Framework Core](/ef/core/) provider for SQLite is built on top of this library. However, it can also be used independently or with other data access libraries.

## Installation

The latest stable version is available on [NuGet](https://www.nuget.org/packages/Microsoft.Data.Sqlite).

### [.NET Core CLI](#tab/netcore-cli)

```dotnetcli
dotnet add package Microsoft.Data.Sqlite
```

### [Visual Studio](#tab/visual-studio)

``` PowerShell
Install-Package Microsoft.Data.Sqlite
```

---

## Usage

This library implements the common ADO.NET abstractions for connections, commands, data readers, and so on.

[!code-csharp[](../samples/msdata-sqlite/HelloWorldSample/Program.cs?name=snippet_HelloWorld)]

## See also

* [Connection Strings](connection-strings.md)
* [API Reference](/dotnet/api/?view=msdata-sqlite-3.0.0)
* [SQL Syntax <span class="docon docon-navigate-external" aria-hidden="true" />](https://www.sqlite.org/lang.html)
