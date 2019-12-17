---
title: Foreign Key Constraints - EF Core
author: AndriySvyryd
ms.date: 11/21/2019
uid: core/modeling/relational/fk-constraints
---
# Foreign Key Constraints

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

A foreign key constraint is introduced for each relationship in the model.

## Conventions

By convention, foreign key constraints are named `FK_<dependent type name>_<principal type name>_<foreign key property name>`. For composite foreign keys `<foreign key property name>` becomes an underscore separated list of foreign key property names.

## Data Annotations

Foreign key constraint names cannot be configured using data annotations.

## Fluent API

You can use the Fluent API to configure the foreign key constraint name for a relationship.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/RelationshipConstraintName.cs?name=Constraint&highlight=12)]
