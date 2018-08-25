---
title: Backing Fields - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: a628795e-64df-4f24-a5e8-76bc261e7ed8
uid: core/modeling/backing-field
---
# Backing Fields

> [!NOTE]  
> This feature is new in EF Core 1.1.

Backing fields allow EF to read and/or write to a field rather than a property. This can be useful when encapsulation in the class is being used to restrict the use of and/or enhance the semantics around access to the data by application code, but the value should be read from and/or written to the database without using those restrictions/enhancements.

## Conventions

By convention, the following fields will be discovered as backing fields for a given property (listed in precedence order). Fields are only discovered for properties that are included in the model. For more information on which properties are included in the model, see [Including & Excluding Properties](included-properties.md).

* `_<camel-cased property name>`
* `_<property name>`
* `m_<camel-cased property name>`
* `m_<property name>`

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/Samples/BackingField.cs#Sample)]

When a backing field is configured, EF will write directly to that field when materializing entity instances from the database (rather than using the property setter). If EF needs to read or write the value at other times, it will use the property if possible. For example, if EF needs to update the value for a property, it will use the property setter if one is defined. If the property is read-only, then it will write to the field.

## Data Annotations

Backing fields cannot be configured with data annotations.

## Fluent API

You can use the Fluent API to configure a backing field for a property.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/BackingField.cs#Sample)]

### Controlling when the field is used

You can configure when EF uses the field or property. See the [PropertyAccessMode enum](https://docs.microsoft.com/dotnet/api/microsoft.entityframeworkcore.propertyaccessmode) for the supported options.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/BackingFieldAccessMode.cs#Sample)]

### Fields without a property

You can also create a conceptual property in your model that does not have a corresponding CLR property in the entity class, but instead uses a field to store the data in the entity. This is different from [Shadow Properties](shadow-properties.md), where the data is stored in the change tracker. This would typically be used if the entity class uses methods to get/set values.

You can give EF the name of the field in the `Property(...)` API. If there is no property with the given name, then EF will look for a field.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/BackingFieldNoProperty.cs#Sample)]

You can also choose to give the property a name, other than the field name. This name is then used when creating the model, most notably it will be used for the column name that is mapped to in the database.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/BackingFieldConceptualProperty.cs#Sample)]

When there is no property in the entity class, you can use the `EF.Property(...)` method in a LINQ query to refer to the property that is conceptually part of the model.

``` csharp
var blogs = db.blogs.OrderBy(b => EF.Property<string>(b, "Url"));
```
