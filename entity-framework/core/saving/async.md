---
title: Asynchronous Saving - EF Core
author: rowanmiller
ms.date: 01/24/2017
ms.assetid: b64a606e-ecd9-4807-829a-b6ec05ade33f
uid: core/saving/async
---

# Asynchronous Saving

Asynchronous saving avoids blocking a thread while the changes are written to the database. This can be useful to avoid freezing the UI of a thick-client application. Asynchronous operations can also increase throughput in a web application, where the thread can be freed up to service other requests while the database operation completes. For more information, see [Asynchronous Programming in C#](https://docs.microsoft.com/dotnet/csharp/async).

> [!WARNING]  
> EF Core does not support multiple parallel operations being run on the same context instance. You should always wait for an operation to complete before beginning the next operation. This is typically done by using the `await` keyword on each asynchronous operation.

Entity Framework Core provides `DbContext.SaveChangesAsync()` as an asynchronous alternative to `DbContext.SaveChanges()`.

[!code-csharp[Main](../../../samples/core/Saving/Saving/Async/Sample.cs#Sample)]
