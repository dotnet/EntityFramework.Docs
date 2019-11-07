---
title: Inheritance (Relational Database) - EF Core
description: How to configure entity type inheritance in a relational database using Entity Framework Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 11/06/2019
uid: core/modeling/relational/inheritance
---
# Inheritance (Relational Database)

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

Inheritance in the EF model is used to control how inheritance in the entity classes is represented in the database.

> [!NOTE]  
> Currently, only the table-per-hierarchy (TPH) pattern is implemented in EF Core. Other common patterns like table-per-type (TPT) and table-per-concrete-type (TPC) are not yet available.

## Conventions

By default, inheritance will be mapped using the table-per-hierarchy (TPH) pattern. TPH uses a single table to store the data for all types in the hierarchy. A discriminator column is used to identify which type each row represents.

EF Core will only setup inheritance if two or more inherited types are explicitly included in the model (see [Inheritance](../inheritance.md) for more details).

Below is an example showing a simple inheritance scenario and the data stored in a relational database table using the TPH pattern. The *Discriminator* column identifies which type of *Blog* is stored in each row.

[!code-csharp[Main](../../../../samples/core/Modeling/Conventions/InheritanceDbSets.cs#Model)]

![image](_static/inheritance-tph-data.png)

>[!NOTE]
> Database columns are automatically made nullable as necessary when using TPH mapping.

## Data Annotations

You cannot use Data Annotations to configure inheritance.

## Fluent API

You can use the Fluent API to configure the name and type of the discriminator column and the values that are used to identify each type in the hierarchy.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/InheritanceTPHDiscriminator.cs#Inheritance)]

## Configuring the discriminator property

In the examples above, the discriminator is created as a [shadow property](xref:core/modeling/shadow-properties) on the base entity of the hierarchy. Since it is a property in the model, it can be configured just like other properties. For example, to set the max length when the default, by-convention discriminator is being used:

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/DefaultDiscriminator.cs#DiscriminatorConfiguration)]

The discriminator can also be mapped to a .NET property in your entity and configure it. For example:

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/NonShadowDiscriminator.cs#NonShadowDiscriminator)]

## Shared columns

When two sibling entity types have a property with the same name they will be mapped to two separate columns by default. But if they are compatible they can be mapped to the same column:

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/SharedTPHColumns.cs#SharedTPHColumns)]