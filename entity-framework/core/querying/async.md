---
title: Asynchronous Queries - EF Core
author: rowanmiller
ms.date: 01/24/2017
ms.assetid: b6429b14-cba0-4af4-878f-b829777c89cb
uid: core/querying/async
---

# Asynchronous Queries

Asynchronous queries avoid blocking a thread while the query is executed in the database. This can be useful to avoid freezing the UI of a thick-client application. Asynchronous operations can also increase throughput in a web application, where the thread can be freed up to service other requests while the database operation completes. For more information, see [Asynchronous Programming in C#](https://docs.microsoft.com/dotnet/csharp/async).

> [!WARNING]  
> EF Core does not support multiple parallel operations being run on the same context instance. You should always wait for an operation to complete before beginning the next operation. This is typically done by using the `await` keyword on each asynchronous operation.

Entity Framework Core provides a set of asynchronous extension methods that can be used as an alternative to the LINQ methods that cause a query to be executed and results returned. Examples include `ToListAsync()`, `ToArrayAsync()`, `SingleAsync()`, etc. There are not async versions of LINQ operators such as `Where(...)`, `OrderBy(...)`, etc. because these methods only build up the LINQ expression tree and do not cause the query to be executed in the database.

> [!IMPORTANT]  
> The EF Core async extension methods are defined in the `Microsoft.EntityFrameworkCore` namespace. This namespace must be imported for the methods to be available.

[!code-csharp[Main](../../../samples/core/Querying/Querying/Async/Sample.cs#Sample)]
