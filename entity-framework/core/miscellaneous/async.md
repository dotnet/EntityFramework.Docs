---
title: Asynchronous Programming - EF Core
description: Querying and saving data asynchronously with Entity Framework Core
author: roji
ms.date: 10/1/2021
uid: core/miscellaneous/async
---
# Asynchronous Programming

Asynchronous operations avoid blocking a thread while the query is executed in the database. Async operations are important for keeping a responsive UI in rich client applications, and can also increase throughput in web applications where they free up the thread to service other requests in web applications.

Following the .NET standard, EF Core provides asynchronous counterparts to all synchronous methods which perform I/O. These have the same effects as the sync methods, and can be used with the C# `async` and `await` keywords. For example, instead of using DbContext.SaveChanges, which will block a thread while database I/O is performed, DbContext.SaveChangesAsync can be used:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Async/Program.cs#SaveChangesAsync)]

For more information, see [the general C# asynchronous programming docs](/dotnet/csharp/async).

> [!WARNING]
> EF Core doesn't support multiple parallel operations being run on the same context instance. You should always wait for an operation to complete before beginning the next operation. This is typically done by using the `await` keyword on each async operation.

> [!WARNING]
> The async implementation of [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient) unfortunately has some known issues (e.g. [#593](https://github.com/dotnet/SqlClient/issues/593), [#601](https://github.com/dotnet/SqlClient/issues/601), and others). If you're seeing unexpected performance problems, try using sync command execution instead, especially when dealing with large text or binary values.

> [!NOTE]
> EF Core passes cancellation tokens down to the underlying database provider in use (e.g. Microsoft.Data.SqlClient). These tokens may or may not be honored - consult your database provider's documentation.

## Async LINQ operators

In order to support executing LINQ queries asynchronously, EF Core provides a set of async extension methods which execute the query and return results. These counterparts to the standard, synchronous LINQ operators include <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync*>, <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync*>, <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable*>, etc.:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Async/Program.cs#ToListAsync)]

Note that there are no async versions of some LINQ operators such as <xref:System.Linq.Queryable.Where*> or <xref:System.Linq.Queryable.OrderBy*>, because these only build up the LINQ expression tree and don't cause the query to be executed in the database. Only operators which cause query execution have async counterparts.

> [!IMPORTANT]
> The EF Core async extension methods are defined in the `Microsoft.EntityFrameworkCore` namespace. This namespace must be imported for the methods to be available.

## Client-side async LINQ operators

In certain cases, you may want to apply client-side LINQ operators to results coming back from the database; this is needed especially when you need to perform an operation that cannot be translated to SQL. For such cases, use <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable*> to execute the query on the database, and continue composing client-side LINQ operators over the resulting <xref:System.Collections.Generic.IAsyncEnumerable`1>. For example, the following executes a local .NET function on the asynchronous results of the EF LINQ query:

```c#
var blogs = context.Blogs
    .Where(b => b.Rating > 3) // server-evaluated (translated to SQL)
    .AsAsyncEnumerable()
    .Where(b => SomeLocalFunction(b)); // client-evaluated (in .NET)

await foreach (var blog in blogs)
{
    // ...
}

```

> [!NOTE]
> LINQ operators over <xref:System.Collections.Generic.IAsyncEnumerable`1> were introduced in .NET 10. When using an older version of .NET, reference the [`System.Linq.Async` package](https://www.nuget.org/packages/System.Linq.Async).
