---
title: Indexes - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 4581e7ba-5e7f-452c-9937-0aaf790ba10a
uid: core/modeling/relational/indexes
---
# Indexes

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

An index in a relational database maps to the same concept as an index in the core of Entity Framework.

## Conventions

By convention, indexes are named `IX_<type name>_<property name>`. For composite indexes `<property name>` becomes an underscore separated list of property names.

## Data Annotations

Indexes can not be configured using Data Annotations.

## Fluent API

You can use the Fluent API to configure the name of an index.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Samples/Relational/IndexName.cs?name=Model&highlight=9)]

You can also specify a filter.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Samples/Relational/IndexFilter.cs?name=Model&highlight=9)]

When using the SQL Server provider EF adds a 'IS NOT NULL' filter for all nullable columns that are part of a unique index. To override this convention you can supply a `null` value.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Samples/Relational/IndexNoFilter.cs?name=Model&highlight=10)]
