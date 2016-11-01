---
title: IBM Data Servers
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: a3d91281-233c-418f-9e81-806f9e807a86
ms.prod: entity-framework
uid: core/providers/ibm/index
---
# IBM Data Servers

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

This database provider allows Entity Framework Core to be used with IBM Data Servers. Issues, questions, etc. can be posted in the [.Net Development with DB2 and IDS forum](https://www.ibm.com/developerworks/community/forums/html/forum?id=11111111-0000-0000-0000-000000000467)

> [!NOTE]
> This provider is not maintained as part of the Entity Framework Core project. When considering a third party provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements.

> [!WARNING]
> This provider currently only supports the Entity Framework Core RC1 pre-release.

## Install

Install the [EntityFramework.IBMDataServer NuGet package](https://www.nuget.org/packages/EntityFramework.IBMDataServer).

<!-- literal_block"ids  "classes  "xml:space": "preserve", "backrefs  "linenos": false, "dupnames  : "csharp",", highlight_args}, "names": [] -->
````text

   PM>  Install-Package EntityFramework.IBMDataServer -Pre
````

## Get Started

The following resources will help you get started with this provider.
* [Sample application](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/sample_ef7_application_for_ibm_data_servers)

* [Updates & Limitations](https://www.ibm.com/developerworks/community/blogs/96960515-2ea1-4391-8170-b0515d08e4da/entry/latest_updates_and_limitations_for_ibm_data_server_entityframework_7)

## Supported Database Engines

* IBM Data Servers

## Supported Platforms

* .NET Framework (4.5.1 onwards)
