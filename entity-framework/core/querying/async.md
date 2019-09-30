---
title: Asynchronous Queries - EF Core
author: smitpatel
ms.date: 10/03/2019
ms.assetid: b6429b14-cba0-4af4-878f-b829777c89cb
uid: core/querying/async
---

# Asynchronous Queries

Asynchronous queries avoid blocking a thread while the query is executed in the database. Async queries are important for keeping a responsive UI in thick-client applications. They can also increase throughput in web applications where they free up the thread to service other requests in web applications. For more information, see [Asynchronous Programming in C#](/dotnet/csharp/async).

> [!WARNING]  
> EF Core doesn't support multiple parallel operations being run on the same context instance. You should always wait for an operation to complete before beginning the next operation. This is typically done by using the `await` keyword on each async operation.

Entity Framework Core provides a set of async extension methods similar to the LINQ methods, which execute a query and return results. Examples include `ToListAsync()`, `ToArrayAsync()`, `SingleAsync()`. There are no async versions of some LINQ operators such as `Where(...)` or `OrderBy(...)` because these methods only build up the LINQ expression tree and don't cause the query to be executed in the database.

> [!IMPORTANT]  
> The EF Core async extension methods are defined in the `Microsoft.EntityFrameworkCore` namespace. This namespace must be imported for the methods to be available.

[!code-csharp[Main](../../../samples/core/Querying/Async/Sample.cs#ToListAsync)]
