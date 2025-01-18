---
title: In-memory Database Provider - EF Core
description: Information on the Entity Framework Core in-memory database provider
author: SamMonoRT
ms.date: 02/11/2023
uid: core/providers/in-memory/index
---
# EF Core In-Memory Database Provider

This database provider allows Entity Framework Core to be used with an in-memory database. While some users use the in-memory database for testing, this is discouraged. For more information on how to test EF Core applications, see the [_Testing EF Core Applications_](xref:core/testing/index). The provider is maintained by Microsoft as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

> [!WARNING]
> The EF Core in-memory database is not designed for performance or robustness and should not be used outside of testing environments. It is not designed for production use.

> [!IMPORTANT]
> New features are not being added to the in-memory database.

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

* [Testing with in-memory (not recommended)](xref:core/testing/testing-without-the-database#inmemory-provider)
* [UnicornStore Sample Application Tests](https://github.com/rowanmiller/UnicornStore/blob/master/UnicornStore/src/UnicornStore.Tests/Controllers/ShippingControllerTests.cs)

## Supported Database Engines

In-process naive, non-performant, and non-persisted in-memory database. Not designed for production use.
