---
title: Inheritance - EF Core
description: How to configure entity type inheritance using Entity Framework Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 10/27/2016
uid: core/modeling/inheritance
---
# Inheritance

EF can map a .NET type hierarchy to a database. This allows you to write your .NET entities in code as usual, using base and derived types, and have EF seamlessly create the appropriate database schema, issue queries, etc. The actual details of how a type hierarchy is mapped are provider-dependent; this page describes inheritance support in the context of a relational database.

At the moment, EF Core only supports the table-per-hierarchy (TPH) pattern. TPH uses a single table to store the data for all types in the hierarchy, and a discriminator column is used to identify which type each row represents.

> [!NOTE]
> The table-per-type (TPT) and table-per-concrete-type (TPC), which are supported by EF6, are not yet supported by EF Core. TPT is a major feature planned for EF Core 5.0.

## Entity type hierarchy mapping

By convention, EF will only set up inheritance if two or more inherited types are explicitly included in the model. EF will not automatically scan for base or derived types that are not otherwise included in the model.

You can include types in the model by exposing a DbSet for each type in the inheritance hierarchy:

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/InheritanceDbSets.cs?name=InheritanceDbSets&highlight=3-4)]

This model be mapped to the following database schema (note the implicitly-created *Discriminator* column, which identifies which type of *Blog* is stored in each row):

![image](_static/inheritance-tph-data.png)

>[!NOTE]
> Database columns are automatically made nullable as necessary when using TPH mapping. For example, the *RssUrl* column is nullable because regular *Blog* instances do not have that property.

If you don't want to expose a DbSet for one or more entities in the hierarchy, you can also use the Fluent API to ensure they are included in the model.

> [!TIP]
> If you don't rely on conventions, you can specify the base type explicitly using `HasBaseType`. You can also use `.HasBaseType((Type)null)` to remove an entity type from the hierarchy.

## Discriminator configuration

You can configure the name and type of the discriminator column and the values that are used to identify each type in the hierarchy:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DiscriminatorConfiguration.cs?name=DiscriminatorConfiguration&highlight=4-6)]

In the examples above, EF added the discriminator implicitly as a [shadow property](xref:core/modeling/shadow-properties) on the base entity of the hierarchy. This property can be configured like any other:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DiscriminatorPropertyConfiguration.cs?name=DiscriminatorPropertyConfiguration&highlight=4-5)]

Finally, the discriminator can also be mapped to a regular .NET property in your entity:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/NonShadowDiscriminator.cs?name=NonShadowDiscriminator&highlight=4)]

## Shared columns

By default, when two sibling entity types in the hierarchy have a property with the same name, they will be mapped to two separate columns. However, if their type is identical they can be mapped to the same database column:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/SharedTPHColumns.cs?name=SharedTPHColumns&highlight=9,13)]
