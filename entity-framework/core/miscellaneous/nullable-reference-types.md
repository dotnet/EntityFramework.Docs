---
title: Working with nullable reference types - EF Core
description: Working with C# nullable reference types when using Entity Framework Core
author: roji
ms.date: 09/09/2019
uid: core/miscellaneous/nullable-reference-types
---
# Working with Nullable Reference Types

C# 8 introduced a new feature called [nullable reference types (NRT)](/dotnet/csharp/tutorials/nullable-reference-types), allowing reference types to be annotated, indicating whether it is valid for them to contain null or not. If you are new to this feature, it is recommended that make yourself familiar with it by reading the C# docs. Nullable reference types are enabled by default in new project templates, but remain disabled in existing projects unless explicitly opted into.

This page introduces EF Core's support for nullable reference types, and describes best practices for working with them.

## Required and optional properties

The main documentation on required and optional properties and their interaction with nullable reference types is the [Required and Optional Properties](xref:core/modeling/entity-properties#required-and-optional-properties) page. It is recommended you start out by reading that page first.

> [!NOTE]
> Exercise caution when enabling nullable reference types on an existing project: reference type properties which were previously configured as optional will now be configured as required, unless they are explicitly annotated to be nullable. When managing a relational database schema, this may cause migrations to be generated which alter the database column's nullability.

## Non-nullable properties and initialization

When nullable reference types are enabled, the C# compiler emits warnings for any uninitialized non-nullable property, as these would contain null. As a result, the following, common way of writing entity types cannot be used:

```csharp
public class Customer
{
    public int Id { get; set; }

    // Generates CS8618, uninitialized non-nullable property:
    public string Name { get; set; }
}
```

If you're using C# 11 or above, [required members](/dotnet/csharp/whats-new/csharp-11#required-members) provide the perfect solution to this problem:

```csharp
public required string Name { get; set; }
```

The compiler now guarantees that when your code instantiates a Customer, it always initializs its Name property. And since the database column mapped to the property is non-nullable, any instances loaded by EF always contain a non-null Name as well.

If you're using an older version of C#, [Constructor binding](xref:core/modeling/constructors) is an alternative technique to ensure that your non-nullable properties are initialized:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/CustomerWithConstructorBinding.cs?name=CustomerWithConstructorBinding&highlight=6-9)]

Unfortunately, in some scenarios constructor binding isn't an option; navigation properties, for example, cannot be initialized in this way. In those cases, you can simply initialize the property to null with the help of the null-forgiving operator (but see below for more details):

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Order.cs?range=19)]

### Required navigation properties

Required navigation properties present an additional difficulty: although a dependent will always exist for a given principal, it may or may not be loaded by a particular query, depending on the needs at that point in the program ([see the different patterns for loading data](xref:core/querying/related-data)). At the same time, it is undesirable to make these properties nullable, since that would force all access to them to check for null, even if they are required.

This isn't necessarily a problem! As long as a required dependent is properly loaded (e.g. via `Include`), accessing its navigation property is guaranteed to always return non-null. On the other hand, accessing a navigation property without first loading the dependent is a programmer error; as such, it may be acceptable for the navigation property to return null, and when for the (buggy) code to throw a `NullReferenceException`. After all, the code is using EF incorrectly.

If you'd like a stricter approach, you can have a non-nullable property with a nullable [backing field](xref:core/modeling/backing-field):

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Order.cs?range=10-17)]

As long as the navigation is properly loaded, the dependent will be accessible via the property. If, however, the property is accessed without first properly loading the related entity, an `InvalidOperationException` is thrown, since the API contract has been used incorrectly.

> [!NOTE]
> Collection navigations, which contain references to multiple related entities, should always be non-nullable. An empty collection means that no related entities exist, but the list itself should never be null.

## DbContext and DbSet

The common practice of having uninitialized DbSet properties on context types is also problematic, as the compiler will now emit warnings for them. This can be fixed as follows:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/NullableReferenceTypesContext.cs?name=Context&highlight=3-4)]

Another strategy is to use non-nullable auto-properties, but to initialize them to null, using the null-forgiving operator (!) to silence the compiler warning. The DbContext base constructor ensures that all DbSet properties will get initialized, and null will never be observed on them.

## Navigating and including nullable relationships

When dealing with optional relationships, it's possible to encounter compiler warnings where an actual null reference exception would be impossible. When translating and executing your LINQ queries, EF Core guarantees that if an optional related entity does not exist, any navigation to it will simply be ignored, rather than throwing. However, the compiler is unaware of this EF Core guarantee, and produces warnings as if the LINQ query were executed in memory, with LINQ to Objects. As a result, it is necessary to use the null-forgiving operator (!) to inform the compiler that an actual null value isn't possible:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Program.cs?name=Navigating)]

A similar issue occurs when including multiple levels of relationships across optional navigations:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Program.cs?name=Including&highlight=2)]

If you find yourself doing this a lot, and the entity types in question are predominantly (or exclusively) used in EF Core queries, consider making the navigation properties non-nullable, and to configure them as optional via the Fluent API or Data Annotations. This will remove all compiler warnings while keeping the relationship optional; however, if your entities are traversed outside of EF Core, you may observe null values although the properties are annotated as non-nullable.

## Limitations in older versions

Prior to EF Core 6.0, the following limitations applied:

* The public API surface wasn't annotated for nullability (the public API was "null-oblivious"), making it sometimes awkward to use when the NRT feature is turned on. This notably includes the async LINQ operators exposed by EF Core, such as [FirstOrDefaultAsync](/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.firstordefaultasync). The public API is fully annotated for nullability starting with EF Core 6.0.
* Reverse engineering did not support [C# 8 nullable reference types (NRTs)](/dotnet/csharp/tutorials/nullable-reference-types): EF Core always generated C# code that assumed the feature is off. For example, nullable text columns were scaffolded as a property with type `string` , not `string?`, with either the Fluent API or Data Annotations used to configure whether a property is required or not. If using an older version of EF Core, you can still edit the scaffolded code and replace these with C# nullability annotations.
