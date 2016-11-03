---
title: Npgsql
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: 5ecd1b22-c68e-4d87-ba39-b0761f4d5b90
ms.prod: entity-framework
uid: core/providers/npgsql/index
---
# Npgsql

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

This database provider allows Entity Framework Core to be used with PostgreSQL. The provider is maintained as part of the [Npgsql project](http://www.npgsql.org).

> [!NOTE]
> This provider is not maintained as part of the Entity Framework Core project. When considering a third party provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements.

## Install

Install the [Npgsql.EntityFrameworkCore.PostgreSQL NuGet package](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL).

<!-- literal_block"ids  "classes  "xml:space": "preserve", "backrefs  "linenos": false, "dupnames  : "csharp",", highlight_args}, "names": [] -->
````text

   PM>  Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
````

## Get Started

See the [Npgsql documentation](http://www.npgsql.org/doc/efcore.html) to get started.

## Supported Database Engines

* PostgreSQL

## Supported Platforms

* .NET Framework (4.5.1 onwards)

* .NET Core

* Mono (4.2.0 onwards)
