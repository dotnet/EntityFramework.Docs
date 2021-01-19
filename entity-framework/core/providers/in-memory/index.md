---
title: InMemory Database Provider - EF Core
description: Information on the Entity Framework Core InMemory database provider
author: ajcvickers
ms.date: 10/27/2016
uid: core/providers/in-memory/index
---
# EF Core In-Memory Database Provider

This database provider allows Entity Framework Core to be used with an in-memory database. The in-memory database can be useful for testing, although the SQLite provider in in-memory mode may be a more appropriate test replacement for relational databases. The in-memory database is designed for testing only. The provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

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

## Get Started

The following resources will help you get started with this provider.

* [Testing with InMemory](xref:core/testing/in-memory)
* [UnicornStore Sample Application Tests](https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs)

## Supported Database Engines

In-process memory database, designed for testing purposes only.
