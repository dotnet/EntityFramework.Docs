---
title: Owned Entity Types - EF Core
author: julielerman
ms.author: divega
ms.date: 2/26/2018
ms.assetid: 2B0BADCE-E23E-4B28-B8EE-537883E16DF3
ms.technology: entity-framework-core
uid: core/modeling/owned-entities
---
# Owned Entity Types

>[!NOTE]
> This feature is new in EF Core 2.0.

EF Core allows you to model entity types that can only ever appear on navigation properties of other entity types. These are called _owned entity types_. The entity containing an owned entity type is its _owner_.

## Explicit configuration

Owned entity types are never included by EF Core in the model by convention. You can use the `OwnsOne` method in `OnModelCreating` or annotate the type with `OwnedAttrbibute` (new in EF Core 2.1) to configure the type as an owned type.

In this example, StreetAddress is a type with no identity property. It is used as a property of the Order type to specify the shipping address for a particular order. In `OnModelCreating`, we use the `OwnsOne` method to specify that the ShippingAddress property is an Owned Entity of the Order type.

``` csharp
public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public StreetAddress ShippingAddress { get; set; }
}

// OnModelCreating
modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);
```

If the ShippingAddress property is private in the Order type, you can use the string version of the `OwnsOne` method:

``` csharp
modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress");
```

In this example, we use the `OwnedAttribute` to achieve the same goal:

``` csharp
[Owned]
public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public StreetAddress ShippingAddress { get; set; }
}
```

## Implicit keys

In EF Core 2.0 and 2.1, only reference navigation properties can point to owned types. Collections of owned types are not supported. These reference owned types always have a one-to-one relationship with the owner, therefore they don't need their own key values. In the previous example, the StreetAddress type does need to define a key property.  

In order to understanding how EF Core tracks these objects, it is useful to think that a primary key is created as a [shadow property](xref:core/modeling/shadow-properties) for the owned type. The value of the key of an instance of the owned type will be the same as the value of the key of the owner instance.      

## Mapping owned types with table splitting

When using relational databases, by convention owned types are mapped to the same table as the owner. This requires splitting the table in two: some columns will be used to store the data of the owner, and some columns will be used to store data of the owned entity. This is a common feature known as table splitting.

Owned types stored with table splitting can be used very similarly to how complex types are used in EF6.

By contention, EF Core will name the database columns for the properties of the owned entity type following the pattern _EntityProperty_OwnedEntityProperty_. Therefore the StreetAddress properties will appear in the Orders table with the names ShippingAddress_Street and ShippingAddress_City.

You can append the `HasColumnName` method to rename those columns. In the case where StreetAddress is a public property, the mappings would be

``` csharp
modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress)
                            .Property(p=>p.Street).HasColumnName("ShipsToStreet");
modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress)
                            .Property(p=>p.City).HasColumnName("ShipsToCity");
```

## Sharing the same .NET type among multiple owned types

An owned entity type can be of the same .NET type as another owned entity type, therefore the .NET type may not be enough to identify an owned type.

In those cases, the property pointing from the owner to the owned entity becomes the _defining navigation_ of the owned entity type. From the perspective of EF Core, the defining navigation is part of the type's identity alongside the .NET type.   

For example, in the following class, ShippingAddress and BillingAddress are both of the same .NET type, StreetAddress:

``` csharp
public class Order
{
    public int Id { get; set; }
    public StreetAddress ShippingAddress { get; set; }
    public StreetAddress BillingAddress { get; set; }
}
```

In order to understand how EF Core will distinguish tracked instances of these objects, it may be useful to think that the defining navigation has become part of the key of the instance alongside the value of the key of the owner and the .NET type of the owned type.

## Nested owned types

In this example OrderDetails owns BillingAddress and ShippingAddress, which are both StreetAddress types. Then OrderDetails is owned by the Order type.

``` csharp
public class Order
{
    public int Id { get; set; }
    public OrderDetails OrderDetails { get; set; }
    public OrderStatus Status { get; set; }
}

public class OrderDetails
{
    public StreetAddress BillingAddress { get; set; }
    public StreetAddress ShippingAddress { get; set; }
}

public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
}
```

It is possible to chain the `OwnsOne` method in a fluent mapping to configure this model:

``` csharp
modelBuilder.Entity<Order>().OwnsOne(p => p.OrderDetails, od =>
    {
        od.OwnsOne(c => c.BillingAddress);
        od.OwnsOne(c => c.ShippingAddress);
    });
```

Of course, it would be possible to achieve the same thing using `OwnedAttribute`.

In addition to nested owned types, an owned type can reference a regular entity. In the following example, Country is a regular (i.e. non-owned) entity:

``` csharp
public class StreetAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public Country Country { get; set; }
}
```

This capability sets owned entity types apart from complex types in EF6.

## Storing owned types in separate tables

Also unlike EF6 complex types, owned types can be stored in a separate table from the owner. In order to override the convention that maps an owned type to the same table as the owner, you can simply call `ToTable` and provide a different table name. The following example will map OrderDetails and its two addresses to a separate table from Order:

``` csharp
modelBuilder.Entity<Order>().OwnsOne(p => p.OrderDetails, od =>
    {
        od.OwnsOne(c => c.BillingAddress);
        od.OwnsOne(c => c.ShippingAddress);
    }).ToTable("OrderDetails");
```

## Querying owned types

When querying the owner the owned types will be included by default. It is not necessary to use the `Include` method, even if the owned types are stored in a separate table. Based on the model described before, the following query will pull Order, OrderDetails and the two owned StreeAddresses for all pending orders:

``` csharp
var orders = context.Orders.Where(o => o.Status == OrderStatus.Pending);
```  

## Limitations

Here are some limitations of owned entity types. Some of these limitations are fundamental to how owned types work, but some others are point-in-time restrictions that we would like to remove in future releases:

### Current shortcomings
- Inheritance of owned types is not supported
- Owned types cannot be pointed at by a collection navigation property
- Since they use table splitting by default owned types also have the following restrictions unless explicitly mapped to a different table:
   - They cannot be owned by a derived type
   - The defining navigation property cannot be set to null (i.e. owned types on the same table are always required)

### By-design restrictions
- You cannot create a `DbSet<T>`
- You cannot call `Entity<T>()` with an owned type on `ModelBuilder`
