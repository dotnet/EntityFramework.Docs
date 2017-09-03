---
title: EF Core | What is new in EF Core 2.0 | Microsoft Docs
author: julielerman

uid: core/modeling/owned-entities
---
# Owned types

An owned entity type allows you to map types that do not have their own identity and are used as properties in any of your entities (such as a value object). The OwnsOne fluent mapping alerts the DbContext to this mapping. For example, StreetAddress is a type with no identity property. It is used as a property of the Order type to specify the shipping address for a particular order. In the DbContext, you can use the Entity().OwnsOne method to specify that the ShippingAddress property is an Owned Entity of the Order type.

```
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

modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress);
```
If the ShippingAddress property is private in the Order type, you can use an overloaded signature of the OwnsOne method:

```
modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress");
```

EF Core convention will name name the database columns for the properties of the owned entity type as EntityProperty_OwnedEntityProperty. Therefore the StreetAddress properties will appear in the Orders table with the names ShippingAddress_Street and ShippingAddress_City. You can append the Property()HasColumnName() fluent method to rename those columns. In the case where StreetAddress is a public property, the mappings would be

```
modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress)
                            .Property(p=>p.Street).HasColumnName("ShipsToStreet");
modelBuilder.Entity<Order>().OwnsOne(p => p.ShippingAddress)
                            .Property(p=>p.City).HasColumnName("ShipsToCity");
```
In the case of the private ShippingAddress property, you need only to replace the Property lambda expressions with strings.

```
modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress")
                            .Property("Street").HasColumnName("ShipsToStreet");
modelBuilder.Entity<Order>().OwnsOne(typeof(StreetAddress), "ShippingAddress")
                            .Property("City").HasColumnName("ShipsToCity");
```

An owned entity type can share the same CLR type with another owned entity type, but since it cannot be identified just by the CLR type there must be a navigation to it from another entity type. The entity containing the defining navigation is the owner. When querying the owner the owned types will be included by default.

By convention a shadow primary key will be created for the owned type and it will be mapped to the same table as the owner by using table splitting. This allows to use owned types similarly to how complex types are used in EF6:

``` csharp
modelBuilder.Entity<Order>().OwnsOne(p => p.OrderDetails, cb =>
    {
        cb.OwnsOne(c => c.BillingAddress);
        cb.OwnsOne(c => c.ShippingAddress);
    });

public class Order
{
    public int Id { get; set; }
    public OrderDetails OrderDetails { get; set; }
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
