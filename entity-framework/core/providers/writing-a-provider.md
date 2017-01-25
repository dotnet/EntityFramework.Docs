---
title: Writing a Database Provider | Microsoft Docs
author: rowanmiller, ajcvickers
ms.author: rowmil
ms.date: 10/27/2016
ms.assetid: 1165e2ec-e421-43fc-92ab-d92f9ab3c494
ms.technology: entity-framework-core
uid: core/providers/writing-a-provider
---

# Writing a Database Provider

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../../ef6/index.md).

For information about writing an Entity Framework Core database provider, see [So you want to write an EF Core provider](https://blog.oneunicorn.com/2016/11/11/so-you-want-to-write-an-ef-core-provider/) by [Arthur Vickers](https://github.com/ajcvickers).

The EF Core code base is open source and contains several database providers that can be used as a reference. You can find the source code at https://github.com/aspnet/EntityFramework.

## The providers-beware label

Once you begin work on a provider, watch for the [`providers-beware`](https://github.com/aspnet/EntityFramework/labels/providers-beware) label on our GitHub issues and pull requests. We use this label to identify changes that may impact provider writers.

## Suggested naming of third party providers

We suggest using the following naming for NuGet packages. This is consistent with the names of packages delivered by the EF Core team.

`<Optional project/company name>.EntityFrameworkCore.<Database engine name>`

For example:
* `Microsoft.EntityFrameworkCore.SqlServer`
* `Npgsql.EntityFrameworkCore.PostgreSQL`
* `EntityFrameworkCore.SqlServerCompact40`
