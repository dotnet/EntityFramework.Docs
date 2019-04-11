---
title: Column Mapping - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 05a47de9-1078-488e-a823-b516a4208f33
uid: core/modeling/relational/columns
---
# Column Mapping

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

Column mapping identifies which column data should be queried from and saved to in the database.

## Conventions

By convention, each property will be set up to map to a column with the same name as the property.

## Data Annotations

You can use Data Annotations to configure the column to which a property is mapped.

[!code-csharp[Main](../../../../samples/core/Modeling/DataAnnotations/Samples/Relational/Column.cs?highlight=13)]

## Fluent API

You can use the Fluent API to configure the column to which a property is mapped.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Samples/Relational/Column.cs?highlight=11-13)]
