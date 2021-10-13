---
title: Microsoft SQL Server Database Provider - Columns - EF Core
description: Column features specific to the Entity Framework Core SQL Server provider
author: roji
ms.date: 10/12/2021
uid: core/providers/sql-server/columns
---
# Column features specific to the Entity Framework Core SQL Server provider

This page details column configuration options that are specific to the SQL Server provider.

## Unicode and UTF-8

SQL Server has two column types for storing textual data: [`nvarchar(x)`](/sql/t-sql/data-types/nchar-and-nvarchar-transact-sql) and [`varchar(x)`](/sql/t-sql/data-types/char-and-varchar-transact-sql); these have traditionally been used to hold Unicode data in the UTF-16 encoding and non-Unicode data, respectively. SQL Server 2019 [introduced](/sql/relational-databases/collations/collation-and-unicode-support#utf8) the ability to store UTF-8 Unicode data in `varchar(x)` columns.

Unfortunately, this does not currently work out-of-the-box with EF Core's SQL Server provider. To map a string property to a `varchar(x)` column, the Fluent or Data Annotation API is typically used to disable Unicode ([see these docs](xref:core/modeling/entity-properties#unicode)). While this causes the correct column type to be created, it also makes EF Core send database parameters in a way which is incompatible with UTF-8 data: `DbType.AnsiString` is used (signifying non-Unicode data), but `DbType.String` is needed to properly send Unicode data.

To store UTF-8 data in SQL Server, follow these steps:

* Configure the collation for the property with one of SQL Server's UTF-8 collations; these have a `UTF8` suffix ([see the docs on collations](xref:core/modeling/entity-properties##column-collations)).
* Do not disable Unicode on the property; this will cause EF Core to create an `nvarchar(x)` column.
* Edit the migrations and manually set the column type to `varchar(x)` instead.

## Sparse columns

> [!NOTE]
> Sparse column support was introduced in EF Core 6.0.

Sparse columns are ordinary columns that have an optimized storage for null values, reducing the space requirements for null values at the cost of more overhead to retrieve non-null values.

As an example, consider a type hierarchy mapped via [the table-per-hierarchy (TPH) strategy](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration). In TPH, a single database table is used to hold all types in a hierarchy; this means that the table must contain columns for each and every property across the entire hierarchy, and for columns belonging to rare types, most rows will contain a null value for that column. In these cases, it may make sense to configure the column as *sparse*, in order to reduce the space requirements. The decision whether to make a column sparse must be made by the user, and depends on expectations for actual data in the table.

A column can be made sparse via the Fluent API:

[!code-csharp[SparseColumn](../../../../samples/core/SqlServer/Columns/SparseColumnContext.cs?name=SparseColumn&highlight=5)]

For more information on sparse columns, [see the SQL Server docs](/sql/relational-databases/tables/use-sparse-columns).
