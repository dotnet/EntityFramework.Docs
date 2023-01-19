---
title: Sequences - EF Core
description: Configuring sequences in an Entity Framework Core model
author: roji
ms.date: 12/18/2019
uid: core/modeling/sequences
---
# Sequences

> [!NOTE]
> Sequences are a feature typically supported only by relational databases. If you're using a non-relational database such as Azure Cosmos DB, check your database documentation on generating unique values.

A sequence generates unique, sequential numeric values in the database. Sequences are not associated with a specific table, and multiple tables can be set up to draw values from the same sequence.

## Basic usage

You can set up a sequence in the model, and then use it to generate values for properties:

[!code-csharp[Main](../../../samples/core/Modeling/Sequences/Sequence.cs?name=Sequence&highlight=3,7)]

Note that the specific SQL used to generate a value from a sequence is database-specific; the above example works on SQL Server but will fail on other databases. Consult your specific database's documentation for more information.

## Configuring sequence settings

You can also configure additional aspects of the sequence, such as its schema, start value, increment, etc.:

[!code-csharp[Main](../../../samples/core/Modeling/Sequences/SequenceConfiguration.cs?name=SequenceConfiguration&highlight=3-5)]
