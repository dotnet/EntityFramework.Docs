---
title: Microsoft SQL Server Database Provider - Columns - EF Core
description: Column features specific to the Entity Framework Core SQL Server provider
author: roji
ms.date: 10/1/2021
uid: core/providers/sql-server/columns
---
# Column features specific to the Entity Framework Core SQL Server provider

This page details column configuration options that are specific to the SQL Server provider.

## Sparse columns

Sparse columns are ordinary columns that have an optimized storage for null values, reducing the space requirements for null values at the cost of more overhead to retrieve non-null values.

As an example, consider a type hierarchy mapped via [the table-per-hierarchy (TPH) strategy](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration). In TPH, a single database table is used to hold all types in a hierarchy; this means that the table must contain columns for each and every property across the entire hierarchy, and for columns belonging to rare types, most rows will contain a null value for that column. In these cases, it may make sense to configure the column as *sparse*, in order to reduce the space requirements. The decision whether to make a column sparse must be made by the user, and depends on expectations for actual data in the table.

A column can be made sparse via the Fluent API:

[!code-csharp[SparseColumn](../../../../samples/core/SqlServer/Columns/SparseColumnContext.cs?name=SparseColumn&highlight=5)]

For more information on sparse columns, [see the SQL Server docs](/sql/relational-databases/tables/use-sparse-columns).
