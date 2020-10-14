---
title: Table Splitting - EF Core
description: How to configure table splitting using Entity Framework Core
author: AndriySvyryd
ms.date: 01/03/2020
uid: core/modeling/table-splitting
---
# Table Splitting

EF Core allows to map two or more entities to a single row. This is called _table splitting_ or _table sharing_.

## Configuration

To use table splitting the entity types need to be mapped to the same table, have the primary keys mapped to the same columns and at least one relationship configured between the primary key of one entity type and another in the same table.

A common scenario for table splitting is using only a subset of the columns in the table for greater performance or encapsulation.

In this example `Order` represents a subset of `DetailedOrder`.

[!code-csharp[Order](../../../samples/core/Modeling/TableSplitting/Order.cs?name=Order)]

[!code-csharp[DetailedOrder](../../../samples/core/Modeling/TableSplitting/DetailedOrder.cs?name=DetailedOrder)]

In addition to the required configuration we call `Property(o => o.Status).HasColumnName("Status")` to map `DetailedOrder.Status` to the same column as `Order.Status`.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=TableSplitting)]

> [!TIP]
> See the [full sample project](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Modeling/TableSplitting) for more context.

## Usage

Saving and querying entities using table splitting is done in the same way as other entities:

[!code-csharp[Usage](../../../samples/core/Modeling/TableSplitting/Program.cs?name=Usage)]

## Optional dependent entity

> [!NOTE]
> This feature was introduced in EF Core 3.0.

If all of the columns used by a dependent entity are `NULL` in the database, then no instance for it will be created when queried. This allows modeling an optional dependent entity, where the relationship property on the principal would be null. Note that this would also happen if all of the dependent's properties are optional and set to `null`, which might not be expected.

## Concurrency tokens

If any of the entity types sharing a table has a concurrency token then it must be included in all other entity types as well. This is necessary in order to avoid a stale concurrency token value when only one of the entities mapped to the same table is updated.

To avoid exposing the concurrency token to the consuming code, it's possible the create one as a [shadow property](xref:core/modeling/shadow-properties):

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=ConcurrencyToken&highlight=2)]
