---
title: Shadow and Indexer Properties - EF Core
description: Configuring shadow and indexer properties in an Entity Framework Core model
author: AndriySvyryd
ms.date: 10/25/2021
uid: core/modeling/shadow-properties
---
# Shadow and Indexer Properties

Shadow properties are properties that aren't defined in your .NET entity class but are defined for that entity type in the EF Core model. The value and state of these properties are maintained purely in the Change Tracker. Shadow properties are useful when there's data in the database that shouldn't be exposed on the mapped entity types.

Indexer properties are entity type properties, which are backed by an [indexer](/dotnet/csharp/programming-guide/indexers/) in .NET entity class. They can be accessed using the indexer on the .NET class instances. It also allows you to add additional properties to the entity type without changing the CLR class.

## Foreign key shadow properties

Shadow properties are most often used for foreign key properties, where they are added to the model by convention when no foreign key property has been found by convention or configured explicitly. The relationship is represented by navigation properties, but in the database it is enforced by a foreign key constraint, and the value for the foreign key column is stored in the corresponding shadow property.

The property will be named `<navigation property name><principal key property name>` (the navigation on the dependent entity, which points to the principal entity, is used for the naming). If the principal key property name starts with the name of the navigation property, then the name will just be `<principal key property name>`. If there is no navigation property on the dependent entity, then the principal type name concatenated with the primary or alternate key property name is used in its place `<principal type name><principal key property name>`.

For example, the following code listing will result in a `BlogId` shadow property being introduced to the `Post` entity:

[!code-csharp[Main](../../../samples/core/Modeling/ShadowAndIndexerProperties/ShadowForeignKey.cs?name=Conventions&highlight=21-23)]

## Configuring shadow properties

You can use the [Fluent API](xref:core/modeling/index#use-fluent-api-to-configure-a-model) to configure shadow properties. Once you have called the string overload of <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder.Property``1(System.String)>, you can chain any of the configuration calls you would for other properties. In the following sample, since `Blog` has no CLR property named `LastUpdated`, a shadow property is created:

[!code-csharp[Main](../../../samples/core/Modeling/ShadowAndIndexerProperties/ShadowProperty.cs?name=ShadowProperty&highlight=8)]

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

[!code-csharp[Main](../../../samples/core/Modeling/ShadowAndIndexerProperties/IndexerProperty.cs?name=IndexerProperty&highlight=7)]

If the name supplied to the `IndexerProperty` method matches the name of an existing indexer property, then the code will configure that existing property. If the entity type has a property, which is backed by a property on the entity class, then an exception is thrown since indexer properties must only be accessed via the indexer.

Indexer properties can be referenced in LINQ queries via the `EF.Property` static method as shown above or by using the CLR indexer property.

## Property bag entity types

Entity types that contain only indexer properties are known as property bag entity types. These entity types don't have shadow properties, and EF creates indexer properties instead. Currently only `Dictionary<string, object>` is supported as a property bag entity type. It must be configured as a [shared-type entity type](xref:core/modeling/entity-types#shared-type-entity-types) with a unique name and the corresponding `DbSet` property must be implemented using a `Set` call.

[!code-csharp[Main](../../../samples/core/Modeling/ShadowAndIndexerProperties/SharedType.cs?name=SharedType&highlight=3,7)]

Property bag entity types can be used wherever a normal entity type is used, including as an owned entity type. However, they do have certain limitations:

- They can't have shadow properties.
- [Indexer navigations aren't supported](https://github.com/dotnet/efcore/issues/13729)
- [Inheritance isn't supported](https://github.com/dotnet/efcore/issues/9630)
- [Some relationship model-building API lack overloads for shared-type entity types](https://github.com/dotnet/efcore/issues/23255)
- [Other types can't be marked as property bags](https://github.com/dotnet/efcore/issues/22009)
