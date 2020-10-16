---
title: Value Comparers - EF Core
description: Using value comparers to control how EF Core compares property values 
author: ajcvickers
ms.date: 03/20/2020
uid: core/modeling/value-comparers
---

# Value Comparers

> [!TIP]  
> The code in this document can be found on GitHub as a [runnable sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Modeling/ValueConversions/).

## Background

EF Core needs to compare property values when:

* Determining whether a property has been changed as part of [detecting changes for updates](xref:core/saving/basic)
* Determining whether two key values are the same when resolving relationships

This is handled automatically for common primitive types such as int, bool, DateTime, etc.

For more complex types, choices need to be made as to how to do the comparison.
For example, a byte array could be compared:

* By reference, such that a difference is only detected if a new byte array is used
* By deep comparison, such that mutation of the bytes in the array is detected

By default, EF Core uses the first of these approaches for non-key byte arrays.
That is, only references are compared and a change is detected only when an existing byte array is replaced with a new one.
This is a pragmatic decision that avoids deep comparison of many large byte arrays when executing SaveChanges.
But the common scenario of replacing, say, an image with a different image is handled in a performant way.

On the other hand, reference equality would not work when byte arrays are used to represent binary keys.
It's very unlikely that an FK property is set to the _same instance_ as a PK property to which it needs to be compared.
Therefore, EF Core uses deep comparisons for byte arrays acting as keys.
This is unlikely to have a big performance hit since binary keys are usually short.

### Snapshots

Deep comparisons on mutable types means that EF Core needs the ability to create a deep "snapshot" of the property value.
Just copying the reference instead would result in mutating both the current value and the snapshot, since they are _the same object_.
Therefore, when deep comparisons are used on mutable types, deep snapshotting is also required.

## Properties with value converters

In the case above, EF Core has native mapping support for byte arrays and so can automatically choose appropriate defaults.
However, if the property is mapped through a [value converter](xref:core/modeling/value-conversions), then EF Core can't always determine the appropriate comparison to use.
Instead, EF Core always uses the default equality comparison defined by the type of the property.
This is often correct, but may need to be overridden when mapping more complex types.

### Simple immutable classes

Consider a property that uses a value converter to map a simple, immutable class.

[!code-csharp[SimpleImmutableClass](../../../samples/core/Modeling/ValueConversions/MappingImmutableClassProperty.cs?name=SimpleImmutableClass)]

[!code-csharp[ConfigureImmutableClassProperty](../../../samples/core/Modeling/ValueConversions/MappingImmutableClassProperty.cs?name=ConfigureImmutableClassProperty)]

Properties of this type do not need special comparisons or snapshots because:

* Equality is overridden so that different instances will compare correctly
* The type is immutable, so there is no chance of mutating a snapshot value

So in this case the default behavior of EF Core is fine as it is.

### Simple immutable Structs

The mapping for simple structs is also simple and requires no special comparers or snapshotting.

[!code-csharp[SimpleImmutableStruct](../../../samples/core/Modeling/ValueConversions/MappingImmutableStructProperty.cs?name=SimpleImmutableStruct)]

[!code-csharp[ConfigureImmutableStructProperty](../../../samples/core/Modeling/ValueConversions/MappingImmutableStructProperty.cs?name=ConfigureImmutableStructProperty)]

EF Core has built-in support for generating compiled, memberwise comparisons of struct properties.
This means structs don't need to have equality overridden for EF, but you may still choose to do this for [other reasons](/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type).
Also, special snapshotting is not needed since structs immutable and are always memberwise copied anyway.
(This is also true for mutable structs, but [mutable structs should in general be avoided](/dotnet/csharp/write-safe-efficient-code).)

### Mutable classes

It is recommended that you use immutable types (classes or structs) with value converters when possible.
This is usually more efficient and has cleaner semantics than using a mutable type.

However, that being said, it is common to use properties of types that the application cannot change.
For example, mapping a property containing a list of numbers:

[!code-csharp[ListProperty](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ListProperty)]

The [`List<T>` class](/dotnet/api/system.collections.generic.list-1):

* Has reference equality; two lists containing the same values are treated as different.
* Is mutable; values in the list can be added and removed.

A typical value conversion on a list property might convert the list to and from JSON:

[!code-csharp[ConfigureListProperty](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ConfigureListProperty)]

This then requires setting a `ValueComparer<T>` on the property to force EF Core use correct comparisons with this conversion:

[!code-csharp[ConfigureListPropertyComparer](../../../samples/core/Modeling/ValueConversions/MappingListProperty.cs?name=ConfigureListPropertyComparer)]

> [!NOTE]  
> The model builder ("fluent") API to set a value comparer has not yet been implemented.
> Instead, the code above calls SetValueComparer on the lower-level IMutableProperty exposed by the builder as 'Metadata'.

The `ValueComparer<T>` constructor accepts three expressions:

* An expression for checking equality
* An expression for generating a hash code
* An expression to snapshot a value  

In this case the comparison is done by checking if the sequences of numbers are the same.

Likewise, the hash code is built from this same sequence.
(Note that this is a hash code over mutable values and hence can [cause problems](https://ericlippert.com/2011/02/28/guidelines-and-rules-for-gethashcode/).
Be immutable instead if you can.)

The snapshot is created by cloning the list with ToList.
Again, this is only needed if the lists are going to be mutated.
Be immutable instead if you can.

> [!NOTE]  
> Value converters and comparers are constructed using expressions rather than simple delegates.
> This is because EF inserts these expressions into a much more complex expression tree that is then compiled into an entity shaper delegate.
> Conceptually, this is similar to compiler inlining.
> For example, a simple conversion may just be a compiled in cast, rather than a call to another method to do the conversion.

### Key comparers

The background section covers why key comparisons may require special semantics.
Make sure to create a comparer that is appropriate for keys when setting it on a primary, principal, or foreign key property.

Use [SetKeyValueComparer](/dotnet/api/microsoft.entityframeworkcore.mutablepropertyextensions.setkeyvaluecomparer) in the rare cases where different semantics is required on the same property.

> [!NOTE]  
> SetStructuralComparer has been obsoleted in EF Core 5.0.
> Use SetKeyValueComparer instead.

### Overriding defaults

Sometimes the default comparison used by EF Core may not be appropriate.
For example, mutation of byte arrays is not, by default, detected in EF Core.
This can be overridden by setting a different comparer on the property:

[!code-csharp[OverrideComparer](../../../samples/core/Modeling/ValueConversions/OverridingByteArrayComparisons.cs?name=OverrideComparer)]

EF Core will now compare byte sequences and will therefore detect byte array mutations.
