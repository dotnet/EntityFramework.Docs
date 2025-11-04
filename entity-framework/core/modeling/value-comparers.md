---
title: Value Comparers - EF Core
description: Using value comparers to control how EF Core compares property values
author: SamMonoRT
ms.date: 11/15/2021
uid: core/modeling/value-comparers
---

# Value Comparers

> [!TIP]
> The code in this document can be found on GitHub as a [runnable sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/ValueConversions/).

## Background

[Change tracking](xref:core/change-tracking/index) means that EF Core automatically determines what changes were performed by the application on a loaded entity instance, so that those changes can be saved back to the database when <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> is called. EF Core usually performs this by taking a *snapshot* of the instance when it's loaded from the database, and *comparing* that snapshot to the instance handed out to the application.

EF Core comes with built-in logic for snapshotting and comparing most standard types used in databases, so users don't usually need to worry about this topic. However, when a property is mapped through a [value converter](xref:core/modeling/value-conversions), EF Core needs to perform comparison on arbitrary user types, which may be complex. By default, EF Core uses the default equality comparison defined by types (e.g. the `Equals` method); for snapshotting, [value types](/dotnet/csharp/language-reference/builtin-types/value-types) are copied to produce the snapshot, while for [reference types](/dotnet/csharp/language-reference/keywords/reference-types) no copying occurs, and the same instance is used as the snapshot.

In cases where the built-in comparison behavior isn't appropriate, users may provide a *value comparer*, which contains logic for snapshotting, comparing and calculating a hash code. For example, the following sets up value conversion for `List<int>` property to be value converted to a JSON string in the database, and defines an appropriate value comparer as well:

[!code-csharp[ListProperty](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ConfigureListProperty)]

See [mutable classes](#mutable-classes) below for further details.

Note that value comparers are also used when determining whether two key values are the same when resolving relationships; this is explained below.

## Shallow vs. deep comparison

For small, immutable value types such as `int`, EF Core's default logic works well: the value is copied as-is when snapshotted, and compared with the type's built-in equality comparison. When implementing your own value comparer, it's important to consider whether deep or shallow comparison (and snapshotting) logic is appropriate.

Consider byte arrays, which can be arbitrarily large. These could be compared:

* By reference, such that a difference is only detected if a new byte array is used
* By deep comparison, such that mutation of the bytes in the array is detected

By default, EF Core uses the first of these approaches for non-key byte arrays. That is, only references are compared and a change is detected only when an existing byte array is replaced with a new one. This is a pragmatic decision that avoids copying entire arrays and comparing them byte-to-byte when executing <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*>. It means that the common scenario of replacing, say, one image with another is handled in a performant way.

On the other hand, reference equality would not work when byte arrays are used to represent binary keys, since it's very unlikely that an FK property is set to the *same instance* as a PK property to which it needs to be compared. Therefore, EF Core uses deep comparisons for byte arrays acting as keys; this is unlikely to have a big performance hit since binary keys are usually short.

Note that the chosen comparison and snapshotting logic must correspond to each other: deep comparison requires deep snapshotting to function correctly.

## Simple immutable classes

Consider a property that uses a value converter to map a simple, immutable class.

[!code-csharp[SimpleImmutableClass](../../../samples/core/Modeling/ValueConversions/MappingImmutableClassProperty.cs?name=SimpleImmutableClass)]

[!code-csharp[ConfigureImmutableClassProperty](../../../samples/core/Modeling/ValueConversions/MappingImmutableClassProperty.cs?name=ConfigureImmutableClassProperty)]

Properties of this type do not need special comparisons or snapshots because:

* Equality is overridden so that different instances will compare correctly
* The type is immutable, so there is no chance of mutating a snapshot value

So in this case the default behavior of EF Core is fine as it is.

## Simple immutable structs

The mapping for simple structs is also simple and requires no special comparers or snapshotting.

[!code-csharp[SimpleImmutableStruct](../../../samples/core/Modeling/ValueConversions/MappingImmutableStructProperty.cs?name=SimpleImmutableStruct)]

[!code-csharp[ConfigureImmutableStructProperty](../../../samples/core/Modeling/ValueConversions/MappingImmutableStructProperty.cs?name=ConfigureImmutableStructProperty)]

EF Core has built-in support for generating compiled, memberwise comparisons of struct properties. This means structs don't need to have equality overridden for EF Core, but you may still choose to do this for [other reasons](/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type). Also, special snapshotting is not needed since structs are immutable and are always copied memberwise anyway. (This is also true for mutable structs, but [mutable structs should in general be avoided](/dotnet/csharp/write-safe-efficient-code).)

## Mutable classes

It is recommended that you use immutable types (classes or structs) with value converters when possible. This is usually more efficient and has cleaner semantics than using a mutable type. However, that being said, it is common to use properties of types that the application cannot change. For example, mapping a property containing a list of numbers:

[!code-csharp[ListProperty](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ListProperty)]

The <xref:System.Collections.Generic.List`1> class:

* Has reference equality; two lists containing the same values are treated as different.
* Is mutable; values in the list can be added and removed.

A typical value conversion on a list property might convert the list to and from JSON:

### [EF Core 5.0](#tab/ef5)

[!code-csharp[ListProperty](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ConfigureListProperty&highlight=7-10)]

### [Older versions](#tab/older-versions)

[!code-csharp[ListProperty](../../../samples/core/Modeling/ValueConversions/MappingListPropertyOld.cs?name=ConfigureListProperty&highlight=8-11,17)]

***

The <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer`1> constructor accepts three expressions:

* An expression for checking equality
* An expression for generating a hash code
* An expression to snapshot a value

In this case the comparison is done by checking if the sequences of numbers are the same.

Likewise, the hash code is built from this same sequence. (Note that this is a hash code over mutable values and hence can [cause problems](https://ericlippert.com/2011/02/28/guidelines-and-rules-for-gethashcode/). Be immutable instead if you can.)

The snapshot is created by cloning the list with `ToList`. Again, this is only needed if the lists are going to be mutated. Be immutable instead if you can.

> [!NOTE]
> Value converters and comparers are constructed using expressions rather than simple delegates. This is because EF Core inserts these expressions into a much more complex expression tree that is then compiled into an entity shaper delegate. Conceptually, this is similar to compiler inlining. For example, a simple conversion may just be a compiled in cast, rather than a call to another method to do the conversion.

## Key comparers

The background section covers why key comparisons may require special semantics. Make sure to create a comparer that is appropriate for keys when setting it on a primary, principal, or foreign key property.

Use <xref:Microsoft.EntityFrameworkCore.MutablePropertyExtensions.SetKeyValueComparer*> in the rare cases where different semantics is required on the same property.

> [!NOTE]
> <xref:Microsoft.EntityFrameworkCore.MutablePropertyExtensions.SetStructuralValueComparer*> has been obsoleted. Use <xref:Microsoft.EntityFrameworkCore.MutablePropertyExtensions.SetKeyValueComparer*> instead.

## Overriding the default comparer

Sometimes the default comparison used by EF Core may not be appropriate. For example, mutation of byte arrays is not, by default, detected in EF Core. This can be overridden by setting a different comparer on the property:

[!code-csharp[OverrideComparer](../../../samples/core/Modeling/ValueConversions/OverridingByteArrayComparisons.cs?name=OverrideComparer)]

EF Core will now compare byte sequences and will therefore detect byte array mutations.
