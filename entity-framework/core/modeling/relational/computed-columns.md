---
title: Computed Columns - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: e9d81f06-805d-45c9-97c2-3546df654829
uid: core/modeling/relational/computed-columns
---
# Computed Columns

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

A computed column is a column whose value is calculated in the database. A computed column can use other columns in the table to calculate its value.

## Conventions

By convention, computed columns are not created in the model.

## Data Annotations

Computed columns can not be configured with Data Annotations.

## Fluent API

You can use the Fluent API to specify that a property should map to a computed column.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/ComputedColumn.cs?name=ComputedColumn&highlight=9)]
