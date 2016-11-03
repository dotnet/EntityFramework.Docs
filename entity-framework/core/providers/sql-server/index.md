---
title: Microsoft SQL Server
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 2e007c82-c6e4-45bb-8129-851b79ec1a0a
ms.prod: entity-framework
uid: core/providers/sql-server/index
---
# Microsoft SQL Server

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

This database provider allows Entity Framework Core to be used with Microsoft SQL Server (including SQL Azure). The provider is maintained as part of the [EntityFramework GitHub project](https://github.com/aspnet/EntityFramework).

## Install

Install the [Microsoft.EntityFrameworkCore.SqlServer NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/).

<!-- literal_block"language": "csharp",", "xml:space": "preserve", "classes  "backrefs  "names  "dupnames  highlight_args}, "ids  "linenos": false -->
````text

   PM>  Install-Package Microsoft.EntityFrameworkCore.SqlServer
````

## Get Started

The following resources will help you get started with this provider.
* [Getting Started on Full .NET (Console, WinForms, WPF, etc.)](../../get-started/full-dotnet/index.md)

* [Getting Started on ASP.NET Core](../../get-started/aspnetcore/index.md)

* [UnicornStore Sample Application](https://github.com/rowanmiller/UnicornStore/tree/master/UnicornStore)

## Supported Database Engines

* Microsoft SQL Server (2008 onwards)

## Supported Platforms

* .NET Framework (4.5.1 onwards)

* .NET Core

* Mono (4.2.0 onwards)

      Caution: Using this provider on Mono will make use of the Mono SQL Client implementation, which has a number of known issues. For example, it does not support secure connections (SSL).
