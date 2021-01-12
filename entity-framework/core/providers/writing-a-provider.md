---
title: Writing a Database Provider - EF Core
description: Information on writing a new Entity Framework Core provider
author: ajcvickers
ms.date: 10/27/2016
uid: core/providers/writing-a-provider
---

# Writing a Database Provider

For information about writing an Entity Framework Core database provider, see [So you want to write an EF Core provider](https://blog.oneunicorn.com/2016/11/11/so-you-want-to-write-an-ef-core-provider/) by [Arthur Vickers](https://github.com/ajcvickers).

> [!NOTE]
> These posts have not been updated since EF Core 1.1 and there have been significant changes since that time.
[Issue 681](https://github.com/dotnet/EntityFramework.Docs/issues/681) is tracking updates to this documentation.

The EF Core codebase is open source and contains several database providers that can be used as a reference. You can find the source code at <https://github.com/dotnet/efcore>. It may also be helpful to look at the code for commonly used third-party providers, such as [Npgsql](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL), [Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql), and [SQL Server Compact](https://github.com/ErikEJ/EntityFramework.SqlServerCompact). In particular, these projects are set up to extend from and run functional tests that we publish on NuGet. This kind of setup is strongly recommended.

## Keeping up-to-date with provider changes

Starting with work after the 2.1 release, we have created a [log of changes](xref:core/providers/provider-log) that may need corresponding changes in provider code. This is intended to help when updating an existing provider to work with a new version of EF Core.

Prior to 2.1, we used the [`providers-beware`](https://github.com/dotnet/efcore/labels/providers-beware) and [`providers-fyi`](https://github.com/dotnet/efcore/labels/providers-fyi) labels on our GitHub issues and pull requests for a similar purpose. We will continiue to use these lables on issues to give an indication which work items in a given release may also require work to be done in providers. A `providers-beware` label typically means that the implementation of an work item may break providers, while a `providers-fyi` label typically means that providers will not be broken, but code may need to be changed anyway, for example, to enable new functionality.

## Suggested naming of third party providers

We suggest using the following naming for NuGet packages. This is consistent with the names of packages delivered by the EF Core team.

`<Optional project/company name>.EntityFrameworkCore.<Database engine name>`

For example:

* `Microsoft.EntityFrameworkCore.SqlServer`
* `Npgsql.EntityFrameworkCore.PostgreSQL`
* `EntityFrameworkCore.SqlServerCompact40`
