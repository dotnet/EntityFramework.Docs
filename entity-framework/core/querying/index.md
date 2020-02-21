---
title: Querying Data - EF Core
author: smitpatel
ms.date: 10/03/2019
ms.assetid: 7c65ec3e-46c8-48f8-8232-9e31f96c277b
uid: core/querying/index
---
# Querying Data

Entity Framework Core uses Language Integrated Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries. It uses your derived context and entity classes to reference database objects. EF Core passes a representation of the LINQ query to the database provider. Database providers in turn translate it to database-specific query language (for example, SQL for a relational database).

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

The following snippets show a few examples of how to achieve common tasks with Entity Framework Core.

## Loading all data

[!code-csharp[Main](../../../samples/core/Querying/Basics/Sample.cs#LoadingAllData)]

## Loading a single entity

[!code-csharp[Main](../../../samples/core/Querying/Basics/Sample.cs#LoadingSingleEntity)]

## Filtering

[!code-csharp[Main](../../../samples/core/Querying/Basics/Sample.cs#Filtering)]

## Further readings

- Learn more about [LINQ query expressions](/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations)
- For more detailed information on how a query is processed in EF Core, see [How Query Works](xref:core/querying/how-query-works).
