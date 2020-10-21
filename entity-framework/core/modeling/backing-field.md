---
title: Backing Fields - EF Core
description: Configuring backing fields for properties in an Entity Framework Core model
author: ajcvickers
ms.date: 10/27/2016
uid: core/modeling/backing-field
---
# Backing Fields

Backing fields allow EF to read and/or write to a field rather than a property. This can be useful when encapsulation in the class is being used to restrict the use of and/or enhance the semantics around access to the data by application code, but the value should be read from and/or written to the database without using those restrictions/enhancements.

## Basic configuration

By convention, the following fields will be discovered as backing fields for a given property (listed in precedence order).

* `_<camel-cased property name>`
* `_<property name>`
* `m_<camel-cased property name>`
* `m_<property name>`

In the following sample, the `Url` property is configured to have `_url` as its backing field:

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/BackingField.cs#Sample)]

Note that backing fields are only discovered for properties that are included in the model. For more information on which properties are included in the model, see [Including & Excluding Properties](xref:core/modeling/entity-properties).

You can also configure backing fields by using a Data Annotation (available in EFCore 5.0) or the Fluent API, e.g. if the field name doesn't correspond to the above conventions:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/BackingField.cs?name=BackingField&highlight=7)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/BackingField.cs?name=BackingField&highlight=5)]

***

## Field and property access

By default, EF will always read and write to the backing field - assuming one has been properly configured - and will never use the property. However, EF also supports other access patterns. For example, the following sample instructs EF to write to the backing field only while materializing, and to use the property in all other cases:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/BackingFieldAccessMode.cs?name=BackingFieldAccessMode&highlight=6)]

See the [PropertyAccessMode enum](/dotnet/api/microsoft.entityframeworkcore.propertyaccessmode) for the complete set of supported options.

> [!NOTE]
> With EF Core 3.0, the default property access mode changed from `PreferFieldDuringConstruction` to `PreferField`.

## Field-only properties

You can also create a conceptual property in your model that does not have a corresponding CLR property in the entity class, but instead uses a field to store the data in the entity. This is different from [Shadow Properties](xref:core/modeling/shadow-properties), where the data is stored in the change tracker, rather than in the entity's CLR type. Field-only properties are commonly used when the entity class uses methods instead of properties to get/set values, or in cases where fields shouldn't be exposed at all in the domain model (e.g. primary keys).

You can configure a field-only property by providing a name in the `Property(...)` API:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/BackingFieldNoProperty.cs#Sample)]

EF will attempt to find a CLR property with the given name, or a field if a property isn't found. If neither a property nor a field are found, a shadow property will be set up instead.

You may need to refer to a field-only property from LINQ queries, but such fields are typically private. You can use the `EF.Property(...)` method in a LINQ query to refer to the field:

```csharp
var blogs = db.blogs.OrderBy(b => EF.Property<string>(b, "_validatedUrl"));
```
