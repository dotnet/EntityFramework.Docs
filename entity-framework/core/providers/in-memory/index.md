---
title: InMemory Database Provider - EF Core
description: Information on the Entity Framework Core InMemory database provider
author: ajcvickers
ms.date: 1/31/2022
uid: core/providers/in-memory/index
---
# EF Core In-Memory Database Provider

This database provider allows Entity Framework Core to be used with an in-memory database. Although the in-memory database can be used for basic and limited testing, the SQLite provider in in-memory mode is the recommended test replacement for relational databases. For more information on how to test EF Core applications, see the [testing documentation](xref:core/testing/index). The EF Core in-memory provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

> [!WARNING]
> The in-memory provider should not be used in production applications.

## Install

Install the [Microsoft.EntityFrameworkCore.InMemory NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/).

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### [Visual Studio](#tab/vs)

```powershell
Install-Package Microsoft.EntityFrameworkCore.InMemory
```

***

## Additional resources

* [UnicornStore Sample Application Tests](https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs)

## Supported Database Engines

In-process memory database, designed for limited testing, samples, and prototyping.
