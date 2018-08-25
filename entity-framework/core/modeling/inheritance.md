---
title: Inheritance - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 754be334-dd21-450e-9d22-2591e80012a2
uid: core/modeling/inheritance
---
# Inheritance

Inheritance in the EF model is used to control how inheritance in the entity classes is represented in the database.

## Conventions

By convention, it is up to the database provider to determine how inheritance will be represented in the database. See [Inheritance (Relational Database)](relational/inheritance.md) for how this is handled with a relational database provider.

EF will only setup inheritance if two or more inherited types are explicitly included in the model. EF will not scan for base or derived types that were not otherwise included in the model. You can include types in the model by exposing a *DbSet<TEntity>* for each type in the inheritance hierarchy.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Samples/InheritanceDbSets.cs?highlight=3-4&name=Model)]

If you don't want to expose a *DbSet<TEntity>* for one or more entities in the hierarchy, you can use the Fluent API to ensure they are included in the model.
And if you don't rely on conventions you can specify the base type explicitly using `HasBaseType`.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Samples/InheritanceModelBuilder.cs?highlight=7&name=Context)]

> [!NOTE]
> You can use `.HasBaseType((Type)null)` to remove an entity type from the hierarchy.

## Data Annotations

You cannot use Data Annotations to configure inheritance.

## Fluent API

The Fluent API for inheritance depends on the database provider you are using. See [Inheritance (Relational Database)](relational/inheritance.md) for the configuration you can perform for a relational database provider.
