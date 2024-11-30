---
title: Generated Values - EF Core
description: How to configure value generation for properties when using Entity Framework Core
author: AndriySvyryd
ms.date: 1/10/2021
uid: core/modeling/generated-properties
---

# Generated Values

Database columns can have their values generated in various ways: primary key columns are frequently auto-incrementing integers, other columns have default or computed values, etc. This page details various patterns for configuration value generation with EF Core.

## Default values

On relational databases, a column can be configured with a default value; if a row is inserted without a value for that column, the default value will be used.

You can configure a default value on a property:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/DefaultValue.cs?name=DefaultValue&highlight=5)]

You can also specify a SQL fragment that is used to calculate the default value:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/DefaultValueSql.cs?name=DefaultValueSql&highlight=5)]

## Computed columns

On most relational databases, a column can be configured to have its value computed in the database, typically with an expression referring to other columns:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ComputedColumn.cs?name=DefaultComputedColumn&highlight=3)]

The above creates a *virtual* computed column, whose value is computed every time it is fetched from the database. You may also specify that a computed column be *stored* (sometimes called *persisted*), meaning that it is computed on every update of the row, and is stored on disk alongside regular columns:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ComputedColumn.cs?name=StoredComputedColumn&highlight=3)]

## Primary keys

By convention, non-composite primary keys of type short, int, long, or Guid are set up to have values generated for inserted entities if a value isn't provided by the application. Your database provider typically takes care of the necessary configuration; for example, a numeric primary key in SQL Server is automatically set up to be an IDENTITY column.

For more information, [see the documentation about keys](xref:core/modeling/keys) and [guidance for specific inheritance mapping strategies](xref:core/modeling/inheritance#key-generation).

## Explicitly configuring value generation

We saw above that EF Core automatically sets up value generation for primary keys - but we may want to do the same for non-key properties. You can configure any property to have its value generated for inserted entities as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/DataAnnotations/ValueGeneratedOnAdd.cs?name=ValueGeneratedOnAdd&highlight=6)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ValueGeneratedOnAdd.cs?name=ValueGeneratedOnAdd&highlight=5)]

***

Similarly, a property can be configured to have its value generated on add or update:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/DataAnnotations/ValueGeneratedOnAddOrUpdate.cs?name=ValueGeneratedOnAddOrUpdate&highlight=6)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ValueGeneratedOnAddOrUpdate.cs?name=ValueGeneratedOnAddOrUpdate&highlight=5)]

***

Unlike with default values or computed columns, we are not specifying *how* the values are to be generated; that depends on the database provider being used. Database providers may automatically set up value generation for some property types, but others may require you to manually set up how the value is generated.

For example, on SQL Server, when a GUID property is configured as a primary key, the provider automatically performs value generation client-side, using an algorithm to generate optimal sequential GUID values. However, specifying <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder.ValueGeneratedOnAdd*> on a DateTime property will have no effect ([see the section below for DateTime value generation](#datetime-value-generation)).

Similarly, byte[] properties that are configured as generated on add or update and marked as concurrency tokens are set up with the rowversion data type, so that values are automatically generated in the database. However, specifying <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder.ValueGeneratedOnAdd*> has no effect.

Consult your provider's documentation for the specific value generation techniques it supports. The SQL Server value generation documentation can be found [here](xref:core/providers/sql-server/value-generation).

## Date/time value generation

A common request is to have a database column which contains the date/time for when the row was first inserted (value generated on add), or for when it was last updated (value generated on add or update). As there are various strategies to do this, EF Core providers usually don't set up value generation automatically for date/time columns - you have to configure this yourself.

### Creation timestamp

Configuring a date/time column to have the creation timestamp of the row is usually a matter of configuring a default value with the appropriate SQL function. For example, on SQL Server you may use the following:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/DefaultValueSql.cs?name=DefaultValueSql&highlight=5)]

Be sure to select the appropriate function, as several may exist (e.g. `GETDATE()` vs. `GETUTCDATE()`).

### Update timestamp

Although stored computed columns seem like a good solution for managing last-updated timestamps, databases usually don't allow specifying functions such as `GETDATE()` in a computed column. As an alternative, you can set up a database trigger to achieve the same effect:

```sql
CREATE TRIGGER [dbo].[Blogs_UPDATE] ON [dbo].[Blogs]
    AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

    UPDATE B
    SET LastUpdated = GETDATE()
    FROM dbo.Blogs AS B
    INNER JOIN INSERTED AS I
        ON B.BlogId = I.BlogId
END
```

For information on creating triggers, [see the documentation on using raw SQL in migrations](xref:core/managing-schemas/migrations/managing#adding-raw-sql).

## Overriding value generation

Although a property is configured for value generation, in many cases you may still explicitly specify a value for it. Whether this will actually work depends on the specific value generation mechanism that has been configured; while you may specify an explicit value instead of using a column's default value, the same cannot be done with computed columns.

To override value generation with an explicit value, simply set the property to any value that is not the CLR default value for that property's type (`null` for `string`, `0` for `int`, `Guid.Empty` for `Guid`, etc.).

> [!NOTE]
> Trying to insert explicit values into SQL Server IDENTITY fails by default; [see these docs for a workaround](xref:core/providers/sql-server/value-generation#inserting-explicit-values-into-identity-columns).

To provide an explicit value for properties that have been configured as value generated on add or update, you must also configure the property as follows:

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ValueGeneratedOnAddOrUpdateWithPropertySaveBehavior.cs?name=ValueGeneratedOnAddOrUpdateWithPropertySaveBehavior&highlight=5)]

## No value generation

Apart from specific scenarios such as those described above, properties typically have no value generation configured; this means that it's up to the application to always supply a value to be saved to the database. This value must be assigned to new entities before they are added to the context.

However, in some cases you may want to disable value generation that has been set up by convention. For example, a primary key of type int is usually implicitly configured as value-generated-on-add (e.g. identity column on SQL Server). You can disable this via the following:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/DataAnnotations/ValueGeneratedNever.cs?name=ValueGeneratedNever&highlight=3)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/GeneratedProperties/FluentAPI/ValueGeneratedNever.cs?name=ValueGeneratedNever&highlight=5)]

***
