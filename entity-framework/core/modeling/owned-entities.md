---
title: Owned Entity Types - EF Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 02/26/2018
ms.assetid: 2B0BADCE-E23E-4B28-B8EE-537883E16DF3
uid: core/modeling/owned-entities
---
# Owned Entity Types

>[!NOTE]
> This feature is new in EF Core 2.0.

EF Core allows you to model entity types that can only ever appear on navigation properties of other entity types. These are called _owned entity types_. The entity containing an owned entity type is its _owner_.

## Explicit configuration

Owned entity types are never included by EF Core in the model by convention. You can use the `OwnsOne` method in `OnModelCreating` or annotate the type with `OwnedAttribute` (new in EF Core 2.1) to configure the type as an owned type.

In this example, `StreetAddress` is a type with no identity property. It is used as a property of the Order type to specify the shipping address for a particular order.

We can use the `OwnedAttribute` to treat it as an owned entity when referenced from another entity type:

[!code-csharp[StreetAddress](../../../samples/core/Modeling/OwnedEntities/StreetAddress.cs?name=StreetAddress)]

[!code-csharp[Order](../../../samples/core/Modeling/OwnedEntities/Order.cs?name=Order)]

It is also possible to use the `OwnsOne` method in `OnModelCreating` to specify that the `ShippingAddress` property is an Owned Entity of the `Order` entity type and to configure additional facets if needed.

[!code-csharp[OwnsOne](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=OwnsOne)]

If the `ShippingAddress` property is private in the `Order` type, you can use the string version of the `OwnsOne` method:

[!code-csharp[OwnsOneString](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=OwnsOneString)]

See the [full sample project](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Modeling/OwnedEntities) for more context. 

## Implicit keys

Owned types configured with `OwnsOne` or discovered through a reference navigation always have a one-to-one relationship with the owner, therefore they don't need their own key values as the foreign key values are unique. In the previous example, the `StreetAddress` type does not need to define a key property.  

In order to understand how EF Core tracks these objects, it is useful to think that a primary key is created as a [shadow property](xref:core/modeling/shadow-properties) for the owned type. The value of the key of an instance of the owned type will be the same as the value of the key of the owner instance.

## Collections of owned types

>[!NOTE]
> This feature is new in EF Core 2.2.

To configure a collection of owned types `OwnsMany` should be used in `OnModelCreating`. However the primary key will not be configured by convention, so it needs to be specified explicitly. It is common to use a complex key for these type of entities incorporating the foreign key to the owner and an additional unique property that can also be in shadow state:

[!code-csharp[OwnsMany](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=OwnsMany)]

## Mapping owned types with table splitting

When using relational databases, by convention reference owned types are mapped to the same table as the owner. This requires splitting the table in two: some columns will be used to store the data of the owner, and some columns will be used to store data of the owned entity. This is a common feature known as table splitting.

> [!TIP]
> Owned types stored with table splitting can be used similarly to how complex types are used in EF6.

By convention, EF Core will name the database columns for the properties of the owned entity type following the pattern _Navigation_OwnedEntityProperty_. Therefore the `StreetAddress` properties will appear in the 'Orders' table with the names 'ShippingAddress_Street' and 'ShippingAddress_City'.

You can append the `HasColumnName` method to rename those columns:

[!code-csharp[ColumnNames](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=ColumnNames)]

## Sharing the same .NET type among multiple owned types

An owned entity type can be of the same .NET type as another owned entity type, therefore the .NET type may not be enough to identify an owned type.

In those cases, the property pointing from the owner to the owned entity becomes the _defining navigation_ of the owned entity type. From the perspective of EF Core, the defining navigation is part of the type's identity alongside the .NET type.   

For example, in the following class `ShippingAddress` and `BillingAddress` are both of the same .NET type, `StreetAddress`:

[!code-csharp[OrderDetails](../../../samples/core/Modeling/OwnedEntities/OrderDetails.cs?name=OrderDetails)]

In order to understand how EF Core will distinguish tracked instances of these objects, it may be useful to think that the defining navigation has become part of the key of the instance alongside the value of the key of the owner and the .NET type of the owned type.

## Nested owned types

In this example `OrderDetails` owns `BillingAddress` and `ShippingAddress`, which are both `StreetAddress` types. Then `OrderDetails` is owned by the `DetailedOrder` type.

[!code-csharp[DetailedOrder](../../../samples/core/Modeling/OwnedEntities/DetailedOrder.cs?name=DetailedOrder)]

[!code-csharp[OrderStatus](../../../samples/core/Modeling/OwnedEntities/OrderStatus.cs?name=OrderStatus)]

In addition to nested owned types, an owned type can reference a regular entity, it can be either the owner or a different entity as long as the owned entity is on the dependent side. This capability sets owned entity types apart from complex types in EF6.

[!code-csharp[OrderDetails](../../../samples/core/Modeling/OwnedEntities/OrderDetails.cs?name=OrderDetails)]

It is possible to chain the `OwnsOne` method in a fluent call to configure this model:

[!code-csharp[OwnsOneNested](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=OwnsOneNested)]

It is also possible to achieve the same thing using `OwnedAttribute` on both `OrderDetails` and `StreetAdress`.

## Storing owned types in separate tables

Also unlike EF6 complex types, owned types can be stored in a separate table from the owner. In order to override the convention that maps an owned type to the same table as the owner, you can simply call `ToTable` and provide a different table name. The following example will map `OrderDetails` and its two addresses to a separate table from `DetailedOrder`:

[!code-csharp[OwnsOneTable](../../../samples/core/Modeling/OwnedEntities/OwnedEntityContext.cs?name=OwnsOneTable)]

## Querying owned types

When querying the owner the owned types will be included by default. It is not necessary to use the `Include` method, even if the owned types are stored in a separate table. Based on the model described before, the following query will get `Order`, `OrderDetails` and the two owned `StreetAddresses` from the database:

[!code-csharp[DetailedOrderQuery](../../../samples/core/Modeling/OwnedEntities/Program.cs?name=DetailedOrderQuery)]

## Limitations

Some of these limitations are fundamental to how owned entity types work, but some others are restrictions that we may be able to remove in future releases:

### By-design restrictions
- You cannot create a `DbSet<T>` for an owned type
- You cannot call `Entity<T>()` with an owned type on `ModelBuilder`

### Current shortcomings
- Inheritance hierarchies that include owned entity types are not supported
- Reference navigations to owned entity types cannot be null unless they are explicitly mapped to a separate table from the owner
- Instances of owned entity types cannot be shared by multiple owners (this is a well-known scenario for value objects that cannot be implemented using owned entity types)

### Shortcomings in previous versions
- In EF Core 2.0, navigations to owned entity types cannot be declared in derived entity types unless the owned entities are explicitly mapped to a separate table from the owner hierarchy. This limitation has been removed in EF Core 2.1
- In EF Core 2.0 and 2.1 only reference navigations to owned types were supported. This limitation has been removed in EF Core 2.2
