---
title: InMemory Database Provider - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 9af0cba7-7605-4f8f-9cfa-dd616fcb880c
uid: core/providers/in-memory/index
---
# EF Core In-Memory Database Provider

This database provider allows Entity Framework Core to be used with an in-memory database. This can be useful for testing, although the SQLite provider in in-memory mode may be a more appropriate test replacement for relational databases. The provider is maintained as part of the [Entity Framework Core Project](https://github.com/aspnet/EntityFrameworkCore).

## Install

Install the [Microsoft.EntityFrameworkCore.InMemory NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/).

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### [Visual Studio](#tab/vs)

``` powershell
Install-Package Microsoft.EntityFrameworkCore.InMemory
```

***

## Get Started

The following resources will help you get started with this provider.

* [Testing with InMemory](../../miscellaneous/testing/in-memory.md)
* [UnicornStore Sample Application Tests](https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs)

## Supported Database Engines

In-process memory database (designed for testing purposes only)
