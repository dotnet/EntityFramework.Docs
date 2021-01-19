---
title: Microsoft SQL Server Database Provider - Value Generation - EF Core
description: Value Generation Patterns Specific to the SQL Server Entity Framework Core Database Provider
author: roji
ms.date: 1/10/2020
uid: core/providers/sql-server/value-generation
---
# SQL Server Value Generation

This page details value generation configuration  and patterns that are specific to the SQL Server provider. It's recommended to first read [the general page on value generation](xref:core/modeling/generated-properties).

## IDENTITY columns

By convention, numeric columns that are configured to have their values generated on add are set up as [SQL Server IDENTITY columns](https://docs.microsoft.com/sql/t-sql/statements/create-table-transact-sql-identity-property).

### Seed and increment

By default, IDENTITY columns start off at 1 (the seed), and increment by 1 each time a row is added (the increment). You can configure a different seed and increment as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/ValueGeneration/IdentityOptionsContext.cs?name=IdentityOptions&highlight=5)]

> [!NOTE]
> The ability to configure IDENTITY seed and increment was introduced in EF Core 3.0.

### Inserting explicit values into IDENTITY columns

By default, SQL Server doesn't allow inserting explicit values into IDENTITY columns. To do so, you must manually enable `IDENTITY_INSERT` before calling `SaveChanges()`, as follows:

[!code-csharp[Main](../../../../samples/core/SqlServer/ValueGeneration/ExplicitIdentityValues.cs?name=ExplicitIdentityValues)]

> [!NOTE]
> We have a [feature request](https://github.com/aspnet/EntityFramework/issues/703) on our backlog to do this automatically within the SQL Server provider.

## Sequences

As an alternative to IDENTITY columns, you can use standard sequences. This can be useful in various scenarios; for example, you may want to have multiple columns drawing their default values from a single sequence.

SQL Server allows you to create sequences and use them as detailed in [the general page on sequences](xref:core/modeling/sequences). It's up to you to configure your properties to use sequences via `HasDefaultValueSql()`.
