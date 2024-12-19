---
title: Advanced Performance Topics - EF Core
description: Advanced performance topics for Entity Framework Core
author: roji
ms.date: 9/26/2023
uid: core/performance/advanced-performance-topics
---
# Advanced Performance Topics

## DbContext pooling

A `DbContext` is generally a light object: creating and disposing one doesn't involve a database operation, and most applications can do so without any noticeable impact on performance. However, each context instance does set up various internal services and objects necessary for performing its duties, and the overhead of continuously doing so may be significant in high-performance scenarios. For these cases, EF Core can *pool* your context instances: when you dispose your context, EF Core resets its state and stores it in an internal pool; when a new instance is next requested, that pooled instance is returned instead of setting up a new one. Context pooling allows you to pay context setup costs only once at program startup, rather than continuously.

Note that context pooling is orthogonal to database connection pooling, which is managed at a lower level in the database driver.

### [With dependency injection](#tab/with-di)

The typical pattern in an ASP.NET Core app using EF Core involves registering a custom <xref:Microsoft.EntityFrameworkCore.DbContext> type into the [dependency injection](/aspnet/core/fundamentals/dependency-injection) container via <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*>. Then, instances of that type are obtained through constructor parameters in controllers or Razor Pages.

To enable context pooling, simply replace `AddDbContext` with <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*>:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPooling/Program.cs#AddDbContextPool)]

The `poolSize` parameter of <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> sets the maximum number of instances retained by the pool (defaults to 1024). Once `poolSize` is exceeded, new context instances are not cached and EF falls back to the non-pooling behavior of creating instances on demand.

### [Without dependency injection](#tab/without-di)

To use context pooling without dependency injection, initialize a `PooledDbContextFactory` and request context instances from it:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#DbContextPoolingWithoutDI)]

The `poolSize` parameter of the `PooledDbContextFactory` constructor sets the maximum number of instances retained by the pool (defaults to 1024). Once `poolSize` is exceeded, new context instances are not cached and EF falls back to the non-pooling behavior of creating instances on demand.

***

### Benchmarks

Following are the benchmark results for fetching a single row from a SQL Server database running locally on the same machine, with and without context pooling. As always, results will change with the number of rows, the latency to your database server and other factors. Importantly, this benchmarks single-threaded pooling performance, while a real-world contended scenario may have different results; benchmark on your platform before making any decisions. [The source code is available here](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Benchmarks/ContextPooling.cs), feel free to use it as a basis for your own measurements.

|                Method | NumBlogs |     Mean |    Error |   StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------- |--------- |---------:|---------:|---------:|--------:|------:|------:|----------:|
| WithoutContextPooling |        1 | 701.6 us | 26.62 us | 78.48 us | 11.7188 |     - |     - |  50.38 KB |
|    WithContextPooling |        1 | 350.1 us |  6.80 us | 14.64 us |  0.9766 |     - |     - |   4.63 KB |

### Managing state in pooled contexts

Context pooling works by reusing the same context instance across requests; this means that it's effectively registered as a [Singleton](/aspnet/core/fundamentals/dependency-injection#service-lifetimes), and the same instance is reused across multiple requests (or DI scopes). This means that special care must be taken when the context involves any state that may change between requests. Crucially, the context's `OnConfiguring` is only invoked once - when the instance context is first created - and so cannot be used to set state which needs to vary (e.g. a tenant ID).

A typical scenario involving context state would be a multi-tenant ASP.NET Core application, where the context instance has a *tenant ID* which is taken into account by queries (see [Global Query Filters](xref:core/querying/filters) for more details). Since the tenant ID needs to change with each web request, we need to go through some extra steps to make it all work with context pooling.

Let's assume that your application registers a scoped `ITenant` service, which wraps the tenant ID and any other tenant-related information:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPoolingWithState/Program.cs#TenantResolution)]

As written above, pay special attention to where you get the tenant ID from - this is an important aspect of your application's security.

Once we have our scoped `ITenant` service, register a pooling context factory as a Singleton service, as usual:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPoolingWithState/Program.cs#RegisterSingletonContextFactory)]

Next, write a custom context factory which gets a pooled context from the Singleton factory we registered, and injects the tenant ID into context instances it hands out:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPoolingWithState/WeatherForecastScopedFactory.cs#WeatherForecastScopedFactory)]

Once we have our custom context factory, register it as a Scoped service:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPoolingWithState/Program.cs#RegisterScopedContextFactory)]

Finally, arrange for a context to get injected from our Scoped factory:

[!code-csharp[Main](../../../samples/core/Performance/AspNetContextPoolingWithState/Program.cs#RegisterDbContext)]

As this point, your controllers automatically get injected with a context instance that has the right tenant ID, without having to know anything about it.

The full source code for this sample is available [here](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Performance/AspNetContextPoolingWithState).

> [!NOTE]
> Although EF Core takes care of resetting internal state for `DbContext` and its related services, it generally does not reset state in the underlying database driver, which is outside of EF. For example, if you manually open and use a `DbConnection` or otherwise manipulate ADO.NET state, it's up to you to restore that state before returning the context instance to the pool, e.g. by closing the connection. Failure to do so may cause state to get leaked across unrelated requests.

### Connection Pooling Considerations

With most databases, a long-lived connection is required for performing database operations, and such connections can be expensive to open and close. EF does not implement connection pooling itself, but relies on the underlying database driver (e.g. ADO.NET driver) for managing database connections. Connection pooling is a client-side mechanism that reuses existing database connections to reduce the overhead of opening and closing connections repeatedly. This mechanism is generally consistent across databases supported by EF, such as Azure SQL Database, PostgreSQL, and others., although factors specific to the database or environment, such as resource limits or service configurations, may affect pooling efficiency. Connection pooling is usually enabled by default, and any pooling configuration must be performed at the low-level driver level as documented by that driver; for example, when using ADO.NET, parameters such as minimum or maximum pool sizes are usually configured via the connection string.

Connection pooling is completely orthogonal to EF's `DbContext` pooling, which is described above: while the low-level database driver pools database connections (to avoid the overhead of opening/closing connections), EF can pool context instances (to avoid context memory allocation and initialization overheads). Regardless of whether a context instance is pooled or not, EF generally opens connections just before each operation (e.g. query), and closes it right afterwards, causing it to be returned to the pool; this is done to avoid keeping connections out of the pool any longer than is necessary.

## Compiled queries

When EF receives a LINQ query tree for execution, it must first "compile" that tree, e.g. produce SQL from it. Because this task is a heavy process, EF caches queries by the query tree shape, so that queries with the same structure reuse internally-cached compilation outputs. This caching ensures that executing the same LINQ query multiple times is very fast, even if parameter values differ.

However, EF must still perform certain tasks before it can make use of the internal query cache. For example, your query's expression tree must be recursively compared with the expression trees of cached queries, to find the correct cached query. The overhead for this initial processing is negligible in the majority of EF applications, especially when compared to other costs associated with query execution (network I/O, actual query processing and disk I/O at the database...). However, in certain high-performance scenarios it may be desirable to eliminate it.

EF supports *compiled queries*, which allow the explicit compilation of a LINQ query into a .NET delegate. Once this delegate is acquired, it can be invoked directly to execute the query, without providing the LINQ expression tree. This technique bypasses the cache lookup, and provides the most optimized way to execute a query in EF Core. Following are some benchmark results comparing compiled and non-compiled query performance; benchmark on your platform before making any decisions. [The source code is available here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Benchmarks/CompiledQueries.cs), feel free to use it as a basis for your own measurements.

|               Method | NumBlogs |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|--------------------- |--------- |---------:|---------:|---------:|-------:|----------:|
|    WithCompiledQuery |        1 | 564.2 us |  6.75 us |  5.99 us | 1.9531 |      9 KB |
| WithoutCompiledQuery |        1 | 671.6 us | 12.72 us | 16.54 us | 2.9297 |     13 KB |
|    WithCompiledQuery |       10 | 645.3 us | 10.00 us |  9.35 us | 2.9297 |     13 KB |
| WithoutCompiledQuery |       10 | 709.8 us | 25.20 us | 73.10 us | 3.9063 |     18 KB |

To use compiled queries, first compile a query with <xref:Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery*?displayProperty=nameWithType> as follows (use <xref:Microsoft.EntityFrameworkCore.EF.CompileQuery*?displayProperty=nameWithType> for synchronous queries):

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#CompiledQueryCompile)]

In this code sample, we provide EF with a lambda accepting a `DbContext` instance, and an arbitrary parameter to be passed to the query. You can now invoke that delegate whenever you wish to execute the query:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#CompiledQueryExecute)]

Note that the delegate is thread-safe, and can be invoked concurrently on different context instances.

### Limitations

* Compiled queries may only be used against a single EF Core model. Different context instances of the same type can sometimes be configured to use different models; running compiled queries in this scenario is not supported.
* When using parameters in compiled queries, use simple, scalar parameters. More complex parameter expressions - such as member/method accesses on instances - are not supported.

## Query caching and parameterization

When EF receives a LINQ query tree for execution, it must first "compile" that tree, e.g. produce SQL from it. Because this task is a heavy process, EF caches queries by the query tree shape, so that queries with the same structure reuse internally-cached compilation outputs. This caching ensures that executing the same LINQ query multiple times is very fast, even if parameter values differ.

Consider the following two queries:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#QueriesWithConstants)]

Since the expression trees contains different constants, the expression tree differs and each of these queries will be compiled separately by EF Core. In addition, each query produces a slightly different SQL command:

```sql
SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Posts] AS [b]
WHERE [b].[Name] = N'post1'

SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Posts] AS [b]
WHERE [b].[Name] = N'post2'
```

Because the SQL differs, your database server will likely also need to produce a query plan for both queries, rather than reusing the same plan.

A small modification to your queries can change things considerably:

[!code-csharp[Main](../../../samples/core/Performance/Other/Program.cs#QueriesWithParameterization)]

Since the blog name is now *parameterized*, both queries have the same tree shape, and EF only needs to be compiled once. The SQL produced is also parameterized, allowing the database to reuse the same query plan:

```sql
SELECT TOP(1) [b].[Id], [b].[Name]
FROM [Posts] AS [b]
WHERE [b].[Name] = @__postTitle_0
```

Note that there is no need to parameterize each and every query: it's perfectly fine to have some queries with constants, and indeed, databases (and EF) can sometimes perform certain optimization around constants which aren't possible when the query is parameterized. See the section on [dynamically-constructed queries](#dynamically-constructed-queries) for an example where proper parameterization is crucial.

> [!NOTE]
> EF Core's [metrics](xref:core/logging-events-diagnostics/metrics) report the Query Cache Hit Rate. In a normal application, this metric reaches 100% soon after program startup, once most queries have executed at least once. If this metric remains stable below 100%, that is an indication that your application may be doing something which defeats the query cache - it's a good idea to investigate that.

> [!NOTE]
> How the database manages caches query plans is database-dependent. For example, SQL Server implicitly maintains an LRU query plan cache, whereas PostgreSQL does not (but prepared statements can produce a very similar end effect). Consult your database documentation for more details.

## Dynamically-constructed queries

In some situations, it is necessary to dynamically construct LINQ queries rather than specifying them outright in source code. This can happen, for example, in a website which receives arbitrary query details from a client, with open-ended query operators (sorting, filtering, paging...). In principle, if done correctly, dynamically-constructed queries can be just as efficient as regular ones (although it's not possible to use the compiled query optimization with dynamic queries). In practice, however, they are frequently the source of performance issues, since it's easy to accidentally produce expression trees with shapes that differ every time.

The following example uses three techniques to construct a query's `Where` lambda expression:

1. **Expression API with constant**: Dynamically build the expression with the Expression API, using a constant node. This is a frequent mistake when dynamically building expression trees, and causes EF to recompile the query each time it's invoked with a different constant value (it also usually causes plan cache pollution at the database server).
2. **Expression API with parameter**: A better version, which substitutes the constant with a parameter. This ensures that the query is only compiled once regardless of the value provided, and the same (parameterized) SQL is generated.
3. **Simple with parameter**: A version which doesn't use the Expression API, for comparison, which creates the same tree as the method above but is much simpler. In many cases, it's possible to dynamically build your expression tree without resorting to the Expression API, which is easy to get wrong.

We add a `Where` operator to the query only if the given parameter is not null. Note that this isn't a good use case for dynamically constructing a query - but we're using it for simplicity:

### [Expression API with constant](#tab/expression-api-with-constant)

[!code-csharp[Main](../../../samples/core/Benchmarks/DynamicallyConstructedQueries.cs?name=ExpressionApiWithConstant&highlight=11-18)]

### [Expression API with parameter](#tab/expression-api-with-parameter)

[!code-csharp[Main](../../../samples/core/Benchmarks/DynamicallyConstructedQueries.cs?name=ExpressionApiWithParameter&highlight=11-26)]

### [Simple with parameter](#tab/simple-with-parameter)

[!code-csharp[Main](../../../samples/core/Benchmarks/DynamicallyConstructedQueries.cs?name=SimpleWithParameter&highlight=12)]

***

Benchmarking these two techniques gives the following results:

|                     Method |       Mean |    Error |   StdDev |    Gen0 |   Gen1 | Allocated |
|--------------------------- |-----------:|---------:|---------:|--------:|-------:|----------:|
|  ExpressionApiWithConstant | 1,665.8 us | 56.99 us | 163.5 us | 15.6250 |      - | 109.92 KB |
| ExpressionApiWithParameter |   757.1 us | 35.14 us | 103.6 us | 12.6953 | 0.9766 |  54.95 KB |
|        SimpleWithParameter |   760.3 us | 37.99 us | 112.0 us | 12.6953 |      - |  55.03 KB |

Even if the sub-millisecond difference seems small, keep in mind that the constant version continuously pollutes the cache and causes other queries to be re-compiled, slowing them down as well and having a general negative impact on your overall performance. It's highly recommended to avoid constant query recompilation.

> [!NOTE]
> Avoid constructing queries with the expression tree API unless you really need to. Aside from the API's complexity, it's very easy to inadvertently cause significant performance issues when using them.

## Compiled models

Compiled models can improve EF Core startup time for applications with large models. A large model typically means hundreds to thousands of entity types and relationships. Startup time here is the time to perform the first operation on a `DbContext` when that `DbContext` type is used for the first time in the application. Note that just creating a `DbContext` instance does not cause the EF model to be initialized. Instead, typical first operations that cause the model to be initialized include calling `DbContext.Add` or executing the first query.

Compiled models are created using the `dotnet ef` command-line tool. Ensure that you have [installed the latest version of the tool](xref:core/cli/dotnet#installing-the-tools) before continuing.

A new `dbcontext optimize` command is used to generate the compiled model. For example:

```dotnetcli
dotnet ef dbcontext optimize
```

The `--output-dir` and `--namespace` options can be used to specify the directory and namespace into which the compiled model will be generated. For example:

```dotnetcli
PS C:\dotnet\efdocs\samples\core\Miscellaneous\CompiledModels> dotnet ef dbcontext optimize --output-dir MyCompiledModels --namespace MyCompiledModels
Build started...
Build succeeded.
Successfully generated a compiled model, to use it call 'options.UseModel(MyCompiledModels.BlogsContextModel.Instance)'. Run this command again when the model is modified.
PS C:\dotnet\efdocs\samples\core\Miscellaneous\CompiledModels>
```

* For more information see [`dotnet ef dbcontext optimize`](xref:core/cli/dotnet#dotnet-ef-dbcontext-optimize).
* If you're more comfortable working inside Visual Studio, you can also use [Optimize-DbContext](xref:core/cli/powershell#optimize-dbcontext)

The output from running this command includes a piece of code to copy-and-paste into your `DbContext` configuration to cause EF Core to use the compiled model. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseModel(MyCompiledModels.BlogsContextModel.Instance)
        .UseSqlite(@"Data Source=test.db");
```

### Compiled model bootstrapping

It is typically not necessary to look at the generated bootstrapping code. However, sometimes it can be useful to customize the model or its loading. The bootstrapping code looks something like this:

<!--
[DbContext(typeof(BlogsContext))]
partial class BlogsContextModel : RuntimeModel
{
    private static BlogsContextModel _instance;
    public static IModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlogsContextModel();
                _instance.Initialize();
                _instance.Customize();
            }

            return _instance;
        }
    }

    partial void Initialize();

    partial void Customize();
}
-->
[!code-csharp[RuntimeModel](../../../samples/core/Miscellaneous/CompiledModels/SingleRuntimeModel.cs?name=RuntimeModel)]

This is a partial class with partial methods that can be implemented to customize the model as needed.

In addition, multiple compiled models can be generated for `DbContext` types that may use different models depending on some runtime configuration. These should be placed into different folders and namespaces, as shown above. Runtime information, such as the connection string, can then be examined and the correct model returned as needed. For example:

<!--
public static class RuntimeModelCache
{
    private static readonly ConcurrentDictionary<string, IModel> _runtimeModels
        = new();

    public static IModel GetOrCreateModel(string connectionString)
        => _runtimeModels.GetOrAdd(
            connectionString, cs =>
                {
                    if (cs.Contains("X"))
                    {
                        return BlogsContextModel1.Instance;
                    }

                    if (cs.Contains("Y"))
                    {
                        return BlogsContextModel2.Instance;
                    }

                    throw new InvalidOperationException("No appropriate compiled model found.");
                });
}
-->
[!code-csharp[RuntimeModelCache](../../../samples/core/Miscellaneous/CompiledModels/MultipleRuntimeModels.cs?name=RuntimeModelCache)]

### Limitations

Compiled models have some limitations:

* [Global query filters are not supported](https://github.com/dotnet/efcore/issues/24897).
* [Lazy loading and change-tracking proxies are not supported](https://github.com/dotnet/efcore/issues/24902).
* [The model must be manually synchronized by regenerating it any time the model definition or configuration change](https://github.com/dotnet/efcore/issues/24894).
* Custom IModelCacheKeyFactory implementations are not supported. However, you can compile multiple models and load the appropriate one as needed.

Because of these limitations, you should only use compiled models if your EF Core startup time is too slow. Compiling small models is typically not worth it.

If supporting any of these features is critical to your success, then please vote for the appropriate issues linked above.

## Reducing runtime overhead

As with any layer, EF Core adds a bit of runtime overhead compared to coding directly against lower-level database APIs. This runtime overhead is unlikely to impact most real-world applications in a significant way; the other topics in this performance guide, such as query efficiency, index usage and minimizing roundtrips, are far more important. In addition, even for highly-optimized applications, network latency and database I/O will usually dominate any time spent inside EF Core itself. However, for high-performance, low-latency applications where every bit of perf is important, the following recommendations can be used to reduce EF Core overhead to a minimum:

* Turn on [DbContext pooling](#dbcontext-pooling); our benchmarks show that this feature can have a decisive impact on high-perf, low-latency applications.
  * Make sure that the `maxPoolSize` corresponds to your usage scenario; if it is too low, `DbContext` instances will be constantly created and disposed, degrading performance. Setting it too high may needlessly consume memory as unused `DbContext` instances are maintained in the pool.
  * For an extra tiny perf boost, consider using `PooledDbContextFactory` instead of having DI inject context instances directly. DI management of `DbContext` pooling incurs a slight overhead.
* Use precompiled queries for hot queries.
  * The more complex the LINQ query - the more operators it contains and the bigger the resulting expression tree - the more gains can be expected from using compiled queries.
* Consider disabling thread safety checks by setting `EnableThreadSafetyChecks` to false in your context configuration.
  * Using the same `DbContext` instance concurrently from different threads isn't supported. EF Core has a safety feature which detects this programming bug in many cases (but not all), and immediately throws an informative exception. However, this safety feature adds some runtime overhead.
  * **WARNING:** Only disable thread safety checks after thoroughly testing that your application doesn't contain such concurrency bugs.
