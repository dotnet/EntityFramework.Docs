---
title: Working with nullable reference types - EF Core
description: Working with C# nullable reference types when using Entity Framework Core
author: roji
ms.date: 09/09/2019
uid: core/miscellaneous/nullable-reference-types
---
# Working with Nullable Reference Types

C# 8 introduced a new feature called [nullable reference types (NRT)](/dotnet/csharp/tutorials/nullable-reference-types), allowing reference types to be annotated, indicating whether it is valid for them to contain null or not. If you are new to this feature, it is recommended that make yourself familiar with it by reading the C# docs.

This page introduces EF Core's support for nullable reference types, and describes best practices for working with them.

## Required and optional properties

The main documentation on required and optional properties and their interaction with nullable reference types is the [Required and Optional Properties](xref:core/modeling/entity-properties#required-and-optional-properties) page. It is recommended you start out by reading that page first.

> [!NOTE]
> Exercise caution when enabling nullable reference types on an existing project: reference type properties which were previously configured as optional will now be configured as required, unless they are explicitly annotated to be nullable. When managing a relational database schema, this may cause migrations to be generated which alter the database column's nullability.

## Non-nullable properties and initialization

When nullable reference types are enabled, the C# compiler emits warnings for any uninitialized non-nullable property, as these would contain null. As a result, the following, common way of writing entity types cannot be used:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/CustomerWithWarning.cs?name=CustomerWithWarning&highlight=5-6)]

[Constructor binding](xref:core/modeling/constructors) is a useful technique to ensure that your non-nullable properties are initialized:

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/CustomerWithConstructorBinding.cs?name=CustomerWithConstructorBinding&highlight=6-9)]

Unfortunately, in some scenarios constructor binding isn't an option; navigation properties, for example, cannot be initialized in this way.

Required navigation properties present an additional difficulty: although a dependent will always exist for a given principal, it may or may not be loaded by a particular query, depending on the needs at that point in the program ([see the different patterns for loading data](xref:core/querying/related-data)). At the same time, it is undesirable to make these properties nullable, since that would force all access to them to check for null, even if they are required.

One way to deal with these scenarios, is to have a non-nullable property with a nullable [backing field](xref:core/modeling/backing-field):

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Order.cs?range=10-17)]

Since the navigation property is non-nullable, a required navigation is configured; and as long as the navigation is properly loaded, the dependent will be accessible via the property. If, however, the property is accessed without first properly loading the related entity, an InvalidOperationException is thrown, since the API contract has been used incorrectly. Note that EF must be configured to always access the backing field and not the property, as it relies on being able to read the value even when unset; consult the documentation on [backing fields](xref:core/modeling/backing-field) on how to do this, and consider specifying `PropertyAccessMode.Field` to make sure the configuration is correct.

As a terser alternative, it is possible to simply initialize the property to null with the help of the null-forgiving operator (!):

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Order.cs?range=19)]

An actual null value will never be observed except as a result of a programming bug, e.g. accessing the navigation property without properly loading the related entity beforehand.

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

## Limitations

* Reverse engineering does not currently support [C# 8 nullable reference types (NRTs)](/dotnet/csharp/tutorials/nullable-reference-types): EF Core always generates C# code that assumes the feature is off. For example, nullable text columns will be scaffolded as a property with type `string` , not `string?`, with either the Fluent API or Data Annotations used to configure whether a property is required or not. You can edit the scaffolded code and replace these with C# nullability annotations. Scaffolding support for nullable reference types is tracked by issue [#15520](https://github.com/dotnet/efcore/issues/15520).
* EF Core's public API surface has not yet been annotated for nullability (the public API is "null-oblivious"), making it sometimes awkward to use when the NRT feature is turned on. This notably includes the async LINQ operators exposed by EF Core, such as [FirstOrDefaultAsync](/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.firstordefaultasync#Microsoft_EntityFrameworkCore_EntityFrameworkQueryableExtensions_FirstOrDefaultAsync__1_System_Linq_IQueryable___0__System_Linq_Expressions_Expression_System_Func___0_System_Boolean___System_Threading_CancellationToken_). We plan to address this for the 6.0 release.
