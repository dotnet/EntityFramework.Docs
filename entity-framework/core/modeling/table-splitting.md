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

In addition to the required configuration we call `HasBaseType((string)null)` to avoid mapping `DetailedOrder` in the same hierarchy as `Order`.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=TableSplitting&highlight=3)]

> [!TIP]  
> See the [full sample project](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Modeling/TableSplitting) for more context.

## Usage

Saving and querying entities using table splitting is done in the same way as other entities, the only difference is that all entities sharing a row must be tracked for the insert.

[!code-csharp[Usage](../../../samples/core/Modeling/TableSplitting/Program.cs?name=Usage)]

## Concurrency tokens

If any of the entity types sharing a table has a concurrency token then it must be included in all other entity types to avoid a stale concurrency token value when only one of the entities mapped to the same table is updated.

To avoid exposing it to the consuming code it's possible the create one in shadow-state.

[!code-csharp[TableSplittingConfiguration](../../../samples/core/Modeling/TableSplitting/TableSplittingContext.cs?name=ConcurrencyToken&highlight=2)]