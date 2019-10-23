---
title: Table Splitting - EF Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 04/10/2019
ms.assetid: 0EC2CCE1-BD55-45D8-9EA9-20634987F094
uid: core/modeling/table-splitting
---
# Table Splitting

>[!NOTE]
> This feature is new in EF Core 2.0.

EF Core allows to map two or more entities to a single row. This is called _table splitting_ or _table sharing_.

## Configuration

To use table splitting the entity types need to be mapped to the same table, have the primary keys mapped to the same columns and at least one relationship configured between the primary key of one entity type and another in the same table.

A common scenario for table splitting is using only a subset of the columns in the table for greater performance or encapsulation.

In this example `Order` represents a subset of `DetailedOrder`.

[!code-csharp[Order](../../../samples/core/Modeling/TableSplitting/Order.cs?name=Order)]

[!code-csharp[DetailedOrder](../../../samples/core/Modeling/TableSplitting/DetailedOrder.cs?name=DetailedOrder)]

In addition to the required configuration we call `Property(o => o.Status).HasColumnName("Status")` to map `DetailedOrder.Status` to the same column as `Order.Status`.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=TableSplitting&highlight=3)]

> [!TIP]
> See the [full sample project](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Modeling/TableSplitting) for more context.

## Usage

Saving and querying entities using table splitting is done in the same way as other entities. And starting with EF Core 3.0 the dependent entity reference can be `null`. If all of the columns used by the dependent entity are `NULL` is the database then no instance for it will be created when queried. This would also happen all of the properties are optional and set to `null`, which might not be expected.

[!code-csharp[Usage](../../../samples/core/Modeling/TableSplitting/Program.cs?name=Usage)]

## Concurrency tokens

If any of the entity types sharing a table has a concurrency token then it must be included in all other entity types to avoid a stale concurrency token value when only one of the entities mapped to the same table is updated.

To avoid exposing it to the consuming code it's possible the create one in shadow-state.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=ConcurrencyToken&highlight=2)]
