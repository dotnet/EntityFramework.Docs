---
title: InMemory Database Provider - EF Core
description: Information on the Entity Framework Core InMemory database provider
author: ajcvickers
ms.date: 10/27/2016
uid: core/providers/in-memory/index
---
# EF Core In-Memory Database Provider

This database provider allows Entity Framework Core to be used with an in-memory database. While some users use the in-memory database for testing, this is generally discouraged; the SQLite provider in in-memory mode is a more appropriate test replacement for relational databases. For more information on how to test EF Core applications, see the [testing documentation](xref:core/testing/index). The provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

> [!WARNING]
> The In-Memory provider was not designed for use outside of testing environments and should never be used as such.

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

* [Testing with InMemory](xref:core/testing/testing-without-the-database#inmemory-provider)
* [UnicornStore Sample Application Tests](https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs)

## Supported Database Engines

In-process memory database, designed for testing purposes only.
