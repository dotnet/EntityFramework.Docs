---
title: Default Values - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: e541366a-130f-47dd-9997-1b110a11febe
uid: core/modeling/relational/default-values
---
# Default Values

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

The default value of a column is the value that will be inserted if a new row is inserted but no value is specified for the column.

## Conventions

By convention, a default value is not configured.

## Data Annotations

You can not set a default value using Data Annotations.

## Fluent API

You can use the Fluent API to specify the default value for a property.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/DefaultValue.cs?name=DefaultValue&highlight=9)]

You can also specify a SQL fragment that is used to calculate the default value.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/DefaultValueSql.cs?name=DefaultValueSql&highlight=9)]
