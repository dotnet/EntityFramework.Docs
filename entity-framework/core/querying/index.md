---
title: Querying Data - EF Core
description: Overview of information on querying in Entity Framework Core
author: smitpatel
ms.date: 10/03/2019
uid: core/querying/index
---
# Querying Data

Entity Framework Core uses Language-Integrated Query (LINQ) to query data from the database. LINQ allows you to use C# (or your .NET language of choice) to write strongly typed queries. It uses your derived context and entity classes to reference database objects. EF Core passes a representation of the LINQ query to the database provider. Database providers in turn translate it to database-specific query language (for example, SQL for a relational database). Queries are always executed against the database even if the entities returned in the result already exist in the context.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying/Overview) on GitHub.

The following snippets show a few examples of how to achieve common tasks with Entity Framework Core.

## Loading all data

[!code-csharp[Main](../../../samples/core/Querying/Overview/Program.cs#LoadingAllData)]

## Loading a single entity

[!code-csharp[Main](../../../samples/core/Querying/Overview/Program.cs#LoadingSingleEntity)]

## Filtering

[!code-csharp[Main](../../../samples/core/Querying/Overview/Program.cs#Filtering)]

## Further readings

- Learn more about [LINQ query expressions](/dotnet/csharp/programming-guide/concepts/linq/basic-linq-query-operations)
- For more detailed information on how a query is processed in EF Core, see [How queries Work](xref:core/querying/how-query-works).
