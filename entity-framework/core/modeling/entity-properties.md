---
title: Entity Properties - EF Core
description: How to configure and map entity properties using Entity Framework Core
author: roji
ms.date: 10/12/2021
uid: core/modeling/entity-properties
---
# Entity Properties

Each entity type in your model has a set of properties, which EF Core will read and write from the database. If you're using a relational database, entity properties map to table columns.

## Included and excluded properties

By [convention](xref:core/modeling/index#built-in-conventions), all public properties with a getter and a setter will be included in the model.

Specific properties can be excluded as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/IgnoreProperty.cs?name=IgnoreProperty&highlight=6)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/IgnoreProperty.cs?name=IgnoreProperty&highlight=3,4)]

***

## Column names

By convention, when using a relational database, entity properties are mapped to table columns having the same name as the property.

If you prefer to configure your columns with different names, you can do so as following code snippet:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/ColumnName.cs?Name=ColumnName&highlight=3)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/ColumnName.cs?Name=ColumnName&highlight=3-5)]

***

## Column data types

When using a relational database, the database provider selects a data type based on the .NET type of the property. It also takes into account other metadata, such as the configured [maximum length](#maximum-length), whether the property is part of a primary key, etc.

For example, the SQL Server provider maps `DateTime` properties to `datetime2(7)` columns, and `string` properties to `nvarchar(max)` columns (or to `nvarchar(450)` for properties that are used as a key).

You can also configure your columns to specify an exact data type for a column. For example, the following code configures `Url` as a non-unicode string with maximum length of `200` and `Rating` as decimal with precision of `5` and scale of `2`:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/ColumnDataType.cs?name=ColumnDataType&highlight=5,8)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/ColumnDataType.cs?name=ColumnDataType&highlight=6-7)]

***

### Maximum length

Configuring a maximum length provides a hint to the database provider about the appropriate column data type to choose for a given property. Maximum length only applies to array data types, such as `string` and `byte[]`.

> [!NOTE]
> Entity Framework does not do any validation of maximum length before passing data to the provider. It is up to the provider or data store to validate if appropriate. For example, when targeting SQL Server, exceeding the maximum length will result in an exception as the data type of the underlying column will not allow excess data to be stored.

In the following example, configuring a maximum length of 500 will cause a column of type `nvarchar(500)` to be created on SQL Server:

#### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/MaxLength.cs?name=MaxLength&highlight=5)]

#### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/MaxLength.cs?name=MaxLength&highlight=3-5)]

***

### Precision and Scale

Some relational data types support the precision and scale facets; these control what values can be stored, and how much storage is needed for the column. Which data types support precision and scale is database-dependent, but in most databases `decimal` and `DateTime` types do support these facets. For `decimal` properties, precision defines the maximum number of digits needed to express any value the column will contain, and scale defines the maximum number of decimal places needed. For `DateTime` properties, precision defines the maximum number of digits needed to express fractions of seconds, and scale is not used.

> [!NOTE]
> Entity Framework does not do any validation of precision or scale before passing data to the provider. It is up to the provider or data store to validate as appropriate. For example, when targeting SQL Server, a column of data type `datetime` does not allow the precision to be set, whereas a `datetime2` one can have precision between 0 and 7 inclusive.

In the following example, configuring the `Score` property to have precision 14 and scale 2 will cause a column of type `decimal(14,2)` to be created on SQL Server, and configuring the `LastUpdated` property to have precision 3 will cause a column of type `datetime2(3)`:

#### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/PrecisionAndScale.cs?name=PrecisionAndScale&highlight=4,6)]

Scale is never defined without first defining precision, so the Data Annotation for defining the scale is `[Precision(precision, scale)]`.

#### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/PrecisionAndScale.cs?name=PrecisionAndScale&highlight=5,9)]

Scale is never defined without first defining precision, so the Fluent API for defining the scale is `HasPrecision(precision, scale)`.

***

### Unicode

In some relational databases, different types exist to represent Unicode and non-Unicode text data. For example, in SQL Server, `nvarchar(x)` is used to represent Unicode data in UTF-16, while `varchar(x)` is used to represent non-Unicode data (but see the notes on [SQL Server UTF-8 support](xref:core/providers/sql-server/columns#unicode-and-utf-8)). For databases which don't support this concept, configuring this has no effect.

Text properties are configured as Unicode by default. You can configure a column as non-Unicode as follows:

#### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/Unicode.cs?name=Unicode&highlight=6-7)]

#### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/Unicode.cs?name=Unicode&highlight=5)]

***

## Required and optional properties

A property is considered optional if it is valid for it to contain `null`. If `null` is not a valid value to be assigned to a property then it is considered to be a required property. When mapping to a relational database schema, required properties are created as non-nullable columns, and optional properties are created as nullable columns.

### Conventions

By convention, a property whose .NET type can contain null will be configured as optional, whereas properties whose .NET type cannot contain null will be configured as required. For example, all properties with .NET value types (`int`, `decimal`, `bool`, etc.) are configured as required, and all properties with nullable .NET value types (`int?`, `decimal?`, `bool?`, etc.) are configured as optional.

C# 8 introduced a new feature called [nullable reference types (NRT)](/dotnet/csharp/tutorials/nullable-reference-types), which allows reference types to be annotated, indicating whether it is valid for them to contain null or not. This feature is enabled by default in new project templates, but remains disabled in existing projects unless explicitly opted into. Nullable reference types affect EF Core's behavior in the following way:

* If nullable reference types are disabled, all properties with .NET reference types are configured as optional by convention (for example, `string`).
* If nullable reference types are enabled, properties will be configured based on the C# nullability of their .NET type: `string?` will be configured as optional, but `string` will be configured as required.

The following example shows an entity type with required and optional properties, with the nullable reference feature disabled and enabled:

#### [Without NRT (default)](#tab/without-nrt)

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/CustomerWithoutNullableReferenceTypes.cs?name=Customer&highlight=5,8)]

#### [With NRT](#tab/with-nrt)

[!code-csharp[Main](../../../samples/core/Miscellaneous/NullableReferenceTypes/Customer.cs?name=Customer&highlight=4-6)]

***

Using nullable reference types is recommended since it flows the nullability expressed in C# code to EF Core's model and to the database, and obviates the use of the Fluent API or Data Annotations to express the same concept twice.

> [!NOTE]
> Exercise caution when enabling nullable reference types on an existing project: reference type properties which were previously configured as optional will now be configured as required, unless they are explicitly annotated to be nullable. When managing a relational database schema, this may cause migrations to be generated which alter the database column's nullability.

For more information on nullable reference types and how to use them with EF Core, [see the dedicated documentation page for this feature](xref:core/miscellaneous/nullable-reference-types).

### Explicit configuration

A property that would be optional by convention can be configured to be required as follows:

#### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/Required.cs?name=Required&highlight=5)]

#### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/Required.cs?name=Required&highlight=3-5)]

***

## Column collations

A collation can be defined on text columns, determining how they are compared and ordered. For example, the following code snippet configures a SQL Server column to be case-insensitive:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Collations/Program.cs?name=ColumnCollation)]

If all columns in a database need to use a certain collation, define the collation at the database level instead.

General information about EF Core support for collations can be found in the [collation documentation page](xref:core/miscellaneous/collations-and-case-sensitivity).

## Column comments

You can set an arbitrary text comment that gets set on the database column, allowing you to document your schema in the database:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/DataAnnotations/ColumnComment.cs?name=ColumnComment&highlight=5)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityProperties/FluentAPI/ColumnComment.cs?name=ColumnComment&highlight=5)]

***

## Column order

By default when creating a table with [Migrations](xref:core/managing-schemas/migrations/index), EF Core orders primary key columns first, followed by properties of the entity type and owned types, and finally properties from base types. You can, however, specify a different column order:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[](../../../samples/core/Modeling/EntityProperties/DataAnnotations/ColumnOrder.cs#snippet_ColumnAttribute)]

The Fluent API can be used to override ordering made with attributes, including resolving any conflicts when attributes on different properties specify the same order number.

### [Fluent API](#tab/fluent-api)

[!code-csharp[](../../../samples/core/Modeling/EntityProperties/FluentAPI/ColumnOrder.cs#snippet_HasColumnOrder)]

***

Note that, in the general case, most databases only support ordering columns when the table is created. This means that the column order attribute cannot be used to re-order columns in an existing table.
