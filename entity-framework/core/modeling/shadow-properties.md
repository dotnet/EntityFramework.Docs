---
title: Shadow and Indexer Properties - EF Core
description: Configuring shadow and indexer properties in an Entity Framework Core model
author: AndriySvyryd
ms.date: 10/09/2020
uid: core/modeling/shadow-properties
---
# Shadow and Indexer Properties

Shadow properties are properties that aren't defined in your .NET entity class but are defined for that entity type in the EF Core model. The value and state of these properties is maintained purely in the Change Tracker. Shadow properties are useful when there's data in the database that shouldn't be exposed on the mapped entity types.

Indexer properties are entity type properties, which are backed by an [indexer](/dotnet/csharp/programming-guide/indexers/) in .NET entity class. They can be accessed using the indexer on the .NET class instances. It also allows you to add additional properties to the entity type without changing the CLR class.

## Foreign key shadow properties

Shadow properties are most often used for foreign key properties, where the relationship between two entities is represented by a foreign key value in the database, but the relationship is managed on the entity types using navigation properties between the entity types. By convention, EF will introduce a shadow property when a relationship is discovered but no foreign key property is found in the dependent entity class.

The property will be named `<navigation property name><principal key property name>` (the navigation on the dependent entity, which points to the principal entity, is used for the naming). If the principal key property name includes the name of the navigation property, then the name will just be `<principal key property name>`. If there is no navigation property on the dependent entity, then the principal type name is used in its place.

For example, the following code listing will result in a `BlogId` shadow property being introduced to the `Post` entity:

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/ShadowForeignKey.cs?name=Conventions&highlight=21-23)]

## Configuring shadow properties

You can use the Fluent API to configure shadow properties. Once you have called the string overload of `Property`, you can chain any of the configuration calls you would for other properties. In the following sample, since `Blog` has no CLR property named `LastUpdated`, a shadow property is created:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/ShadowProperty.cs?name=ShadowProperty&highlight=8)]

If the name supplied to the `Property` method matches the name of an existing property (a shadow property or one defined on the entity class), then the code will configure that existing property rather than introducing a new shadow property.

## Accessing shadow properties

Shadow property values can be obtained and changed through the `ChangeTracker` API:

```csharp
context.Entry(myBlog).Property("LastUpdated").CurrentValue = DateTime.Now;
```

Shadow properties can be referenced in LINQ queries via the `EF.Property` static method:

```csharp
var blogs = context.Blogs
    .OrderBy(b => EF.Property<DateTime>(b, "LastUpdated"));
```

Shadow properties cannot be accessed after a no-tracking query since the entities returned are not tracked by the change tracker.

## Configuring indexer properties

You can use the Fluent API to configure indexer properties. Once you've called the method `IndexerProperty`, you can chain any of the configuration calls you would for other properties. In the following sample, `Blog` has an indexer defined and it will be used to create an indexer property.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexerProperty.cs?name=ShadowProperty&highlight=3)]

If the name supplied to the `IndexerProperty` method matches the name of an existing indexer property, then the code will configure that existing property. If the entity type has a property, which is backed by a property on the entity class, then an exception is thrown since indexer properties must only be accessed via the indexer.

## Property bag entity types

> [!NOTE]
> Support for Property bag entity types was introduced in EF Core 5.0.

Entity types that contain only indexer properties are known as property bag entity types. These entity types don't have shadow properties, instead EF will create indexer properties. Currently only `Dictionary<string, object>` is supported as a property bag entity type. It must be configured as a shared entity type with a unique name and the corresponding `DbSet` property must be implemented using a `Set` call.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/SharedType.cs?name=SharedType&highlight=3,7)]
