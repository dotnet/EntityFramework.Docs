---
title: Sequences - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 94f81a92-3c72-4e14-912a-f99310374e42
uid: core/modeling/relational/sequences
---
# Sequences

> [!NOTE]  
> The configuration in this section is applicable to relational databases in general. The extension methods shown here will become available when you install a relational database provider (due to the shared *Microsoft.EntityFrameworkCore.Relational* package).

A sequence generates a sequential numeric values in the database. Sequences are not associated with a specific table.

## Conventions

By convention, sequences are not introduced in to the model.

## Data Annotations

You can not configure a sequence using Data Annotations.

## Fluent API

You can use the Fluent API to create a sequence in the model.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/Sequence.cs?name=model&highlight=7)]

You can also configure additional aspect of the sequence, such as its schema, start value, and increment.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/SequenceConfigured.cs?name=sequence&highlight=7,8,9)]

Once a sequence is introduced, you can use it to generate values for properties in your model. For example, you can use [Default Values](default-values.md) to insert the next value from the sequence.

[!code-csharp[Main](../../../../samples/core/Modeling/FluentAPI/Relational/SequenceUsed.cs?name=default&highlight=11,12,13)]
