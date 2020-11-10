---
title: Client vs. Server Evaluation - EF Core
description: Client and server evaluation of queries with Entity Framework Core
author: smitpatel
ms.date: 11/09/2020
uid: core/querying/client-eval
---
# Client vs. Server Evaluation

As a general rule, Entity Framework Core attempts to evaluate a query on the server as much as possible. EF Core converts parts of the query into parameters, which it can evaluate on the client side. The rest of the query (along with the generated parameters) is given to the database provider to determine the equivalent database query to evaluate on the server. EF Core supports partial client evaluation in the top-level projection (essentially, the last call to `Select()`). If the top-level projection in the query can't be translated to the server, EF Core will fetch any required data from the server and evaluate remaining parts of the query on the client. If EF Core detects an expression, in any place other than the top-level projection, which can't be translated to the server, then it throws a runtime exception. See [How queries work](xref:core/querying/how-query-works) to understand how EF Core determines what can't be translated to server.

> [!NOTE]
> Prior to version 3.0, Entity Framework Core supported client evaluation anywhere in the query. For more information, see the [previous versions section](#previous-versions).

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying/ClientEvaluation) on GitHub.

## Client evaluation in the top-level projection

In the following example, a helper method is used to standardize URLs for blogs, which are returned from a SQL Server database. Since the SQL Server provider has no insight into how this method is implemented, it isn't possible to translate it into SQL. All other aspects of the query are evaluated in the database, but passing the returned `URL` through this method is done on the client.

[!code-csharp[Main](../../../samples/core/Querying/ClientEvaluation/Program.cs#ClientProjection)]

[!code-csharp[Main](../../../samples/core/Querying/ClientEvaluation/Program.cs#ClientMethod)]

## Unsupported client evaluation

While client evaluation is useful, it can result in poor performance sometimes. Consider the following query, in which the helper method is now used in a where filter. Because the filter can't be applied in the database, all the data needs to be pulled into memory to apply the filter on the client. Based on the filter and the amount of data on the server, client evaluation could result in poor performance. So Entity Framework Core blocks such client evaluation and throws a runtime exception.

[!code-csharp[Main](../../../samples/core/Querying/ClientEvaluation/Program.cs#ClientWhere)]

## Explicit client evaluation

You may need to force into client evaluation explicitly in certain cases like following

- The amount of data is small so that evaluating on the client doesn't incur a huge performance penalty.
- The LINQ operator being used has no server-side translation.

In such cases, you can explicitly opt into client evaluation by calling methods like `AsEnumerable` or `ToList` (`AsAsyncEnumerable` or `ToListAsync` for async). By using `AsEnumerable` you would be streaming the results, but using `ToList` would cause buffering by creating a list, which also takes additional memory. Though if you're enumerating multiple times, then storing results in a list helps more since there's only one query to the database. Depending on the particular usage, you should evaluate which method is more useful for the case.

[!code-csharp[Main](../../../samples/core/Querying/ClientEvaluation/Program.cs#ExplicitClientEvaluation)]

> [!TIP]
> If you are using `AsAsyncEnumerable` and want to compose the query further on client side then you can use [System.Interactive.Async](https://www.nuget.org/packages/System.Interactive.Async/) library which defines operators for async enumerables. For more information, see [client side linq operators](xref:core/miscellaneous/async#client-side-async-linq-operators).

## Potential memory leak in client evaluation

Since query translation and compilation are expensive, EF Core caches the compiled query plan. The cached delegate may use client code while doing client evaluation of top-level projection. EF Core generates parameters for the client-evaluated parts of the tree and reuses the query plan by replacing the parameter values. But certain constants in the expression tree can't be converted into parameters. If the cached delegate contains such constants, then those objects can't be garbage collected since they're still being referenced. If such an object contains a DbContext or other services in it, then it could cause the memory usage of the app to grow over time. This behavior is generally a sign of a memory leak. EF Core throws an exception whenever it comes across constants of a type that can't be mapped using current database provider. Common causes and their solutions are as follows:

- **Using an instance method**: When using instance methods in a client projection, the expression tree contains a constant of the instance. If your method doesn't use any data from the instance, consider making the method static. If you need instance data in the method body, then pass the specific data as an argument to the method.
- **Passing constant arguments to method**: This case arises generally by using `this` in an argument to client method. Consider splitting the argument in to multiple scalar arguments, which can be mapped by the database provider.
- **Other constants**: If a constant is come across in any other case, then you can evaluate whether the constant is needed in processing. If it's necessary to have the constant, or if you can't use a solution from the above cases, then create a local variable to store the value and use local variable in the query. EF Core will convert the local variable into a parameter.

## Previous versions

The following section applies to EF Core versions before 3.0.

Older EF Core versions supported client evaluation in any part of the query--not just the top-level projection. That's why queries similar to one posted under the [Unsupported client evaluation](#unsupported-client-evaluation) section worked correctly. Since this behavior could cause unnoticed performance issues, EF Core logged a client evaluation warning. For more information on viewing logging output, see [Logging](xref:core/logging-events-diagnostics/index).

Optionally EF Core allowed you to change the default behavior to either throw an exception or do nothing when doing client evaluation (except for in the projection). The exception throwing behavior would make it similar to the behavior in 3.0. To change the behavior, you need to configure warnings while setting up the options for your context - typically in `DbContext.OnConfiguring`, or in `Startup.cs` if you're using ASP.NET Core.

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFQuerying;Trusted_Connection=True;")
        .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
}
```
