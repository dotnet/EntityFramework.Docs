---
title: Pomelo | Microsoft Docs
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: d0198c04-d30d-4419-98f8-a54690cea3c8
ms.technology: entity-framework-core
 
uid: core/providers/pomelo/index
---
# Pomelo

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

This database provider allows Entity Framework Core to be used with MySQL. The provider is maintained as part of the [Pomelo Foundation Project](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).

> [!NOTE]
> This provider is not maintained as part of the Entity Framework Core project. When considering a third party provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements.

## Install

Install the [Pomelo.EntityFrameworkCore.MySql NuGet package](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql).

<!-- literal_block"ids  "classes  "xml:space": "preserve", "backrefs  "linenos": false, "dupnames  : "csharp",", highlight_args}, "names": [] -->
````text
PM>  Install-Package Pomelo.EntityFrameworkCore.MySql
````

## Get Started

The following resources will help you get started with this provider.
* [Getting started documentation](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/README.md#getting-started)

* [Yuuko Blog sample application](https://github.com/Kagamine/YuukoBlog-NETCore-MySql)

## Supported Database Engines

* MySQL
* MariaDB

## Supported Platforms

* .NET Framework (4.5.1 onwards)

* .NET Core

* Mono (4.2.0 onwards)
