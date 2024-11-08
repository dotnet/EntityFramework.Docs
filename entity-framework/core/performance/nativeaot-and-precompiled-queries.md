---
title: NativeAOT Support and Precompiled Queries (Experimental) - EF Core
description: Publishing NativeAOT Entity Framework Core applications and using precompiled queries
author: roji
ms.date: 11/10/2024
uid: core/performance/nativeaot-and-precompiled-queries
---
# NativeAOT Support and Precompiled Queries (Experimental)

> [!WARNING]
> NativeAOT and query precompilation are highly experimental feature, and are not yet suited for production use. The support described below should be viewed as infrastructure towards the final feature, which will likely be released with EF 10. We encourage you to experiment with the current support and report on your experiences, but recommend against deploying EF NativeAOT applications in production. See below for specific known limitations.

[.NET NativeAOT](/dotnet/core/deploying/native-aot) allows publishing self-contained .NET applications that have been compiled ahead-of-time (AOT). Doing so offers the following advantages:

* Significantly faster application startup time
* Small, self-contained binaries that have smaller memory footprints and are easier to deploy
* Running applications in environments where just-in-time compilation isn't supported

EF applications published with NativeAOT start up much faster than the same applications without it. In addition to the general .NET startup improvements that NativeAOT offers (i.e. no JIT compilation required each time), EF also precompiles LINQ queries when publishing your application, so that no processing is needed when starting up and the SQL is already available for immediate execution. The more EF LINQ queries an application has in its code, the faster the startup gains are expected to be.

## Publishing an EF NativeAOT Application

First, enable NativeAOT publishing for your project as follows:

```xml
<PropertyGroup>
    <PublishAot>true</PublishAot>
</PropertyGroup>
```

EF's support for LINQ query execution under NativeAOT relies on *query precompilation*: this mechanism statically identifies EF LINQ queries and generates C# [*interceptors*](/dotnet/csharp/whats-new/csharp-12#interceptors), which contain code to execute each specific query. This can significantly cut down on your application's startup time, as the heavy lifting of processing and compiling your LINQ queries into SQL no longer happens every time your application starts up. Instead, each query's interceptor contains the finalized SQL for that query, as well as optimized code to materialize database results as .NET objects.

C# interceptors are currently an experimental feature, and require a special opt-in in your project file:

```xml
<PropertyGroup>
  <InterceptorsNamespaces>$(InterceptorsPreviewNamespaces);Microsoft.EntityFrameworkCore.GeneratedInterceptors</InterceptorsNamespaces>
</PropertyGroup>
```

Finally, the [`Microsoft.EntityFrameworkCore.Tasks`](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tasks) package contains [MSBuild integration](xref:core/cli/msbuild) that will perform the query precompilation (and generate the required compiled model) when you publish your application:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tasks" Version="9.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

You're now ready to publish your EF NativeAOT application:

```console
dotnet publish -r linux-arm64 -c Release
```

This shows publishing a NativeAOT publishing for Linux running on ARM64; [consult this catalog](/dotnet/core/rid-catalog) to find your runtime identifier. If you'd like to generate the interceptors without publishing - for example to examine the generated sources - you can do so via the `dotnet ef dbcontext optimize --precompile-queries --nativeaot` command.

Due to the way C# interceptors work, any change in the application source invalidates them and requires repeating the above process. As a result, interceptor generation and actual publishing aren't expected to happen in the inner loop, as the developer is working on code; instead, both `dotnet ef dbcontext optimize` and `dotnet publish` can be executed in a publishing/deployment workflow, in a CI/CD system.

> [!NOTE]
> Publishing currently reports a number of trimming and NativeAOT warnings, meaning that your application isn't fully guaranteed to run properly. This is expected given the current experimental state of NativeAOT support; the final, non-experimental feature will report no warnings.

## Limitations

### Dynamic queries are not supported

Query precompilation performs static analysis of your source code, identifying EF LINQ queries and generating C# interceptors for them. LINQ allows expressing highly dynamic queries, where LINQ operators are composed based on arbitrary conditions; such queries unfortunately cannot be statically analyzed, and are currently unsupported. Consider the following example:

```c#
IAsyncEnumerable<Blog> GetBlogs(BlogContext context, bool applyFilter)
{
    IQueryable<Blog> query = context.Blogs.OrderBy(b => b.Id);

    if (applyFilter)
    {
        query = query.Where(b => b.Name != "foo");
    }

    return query.AsAsyncEnumerable();
}
```

The above query is split across several statements, and dynamically composes the `Where` operator based on an external parameter; such queries cannot be precompiled. However, it is sometimes possible to rewrite such dynamic queries as multiple non-dynamic queries:

```c#
IAsyncEnumerable<Blog> GetBlogs(BlogContext context, bool applyFilter)
    => applyFilter
        ? context.Blogs.OrderBy(b => b.Id).Where(b => b.Name != "foo").AsAsyncEnumerable()
        : context.Blogs.OrderBy(b => b.Id).AsAsyncEnumerable();
```

Since the two queries can each be statically analyzed from start to finish, precompilation can handle them.

Note that dynamic queries will likely be supported in the future when using NativeAOT; however, since they cannot be precompiled, they will continue to slow down your application startup, and will also generally perform less efficiently compared to non-NativeAOT execution; this is because EF internally relies on code generation to materialize database results, but code generation is not supported when using NativeAOT.

### Other limitations

* LINQ query expression syntax (sometimes termed "comprehension syntax") is not supported.
* The generated compiled model and query interceptors may currently be quite large in terms of code size, and take a long while to generate. We plan on improving this.
* EF providers may need to build in support for precompiled queries; check your provider's documentation to know whether it is compatible with EF's NativeAOT support.
* Value converters that use captured state are not supported.

## Precompiled queries without NativeAOT

Because of the current limitations of EF's NativeAOT support, it may not be usable for some applications. However, you may be able to take advantage of precompiled queries while publishing regular, non-NativeAOT applications; this allows you to at least benefit from the startup time reduction that precompiled queries offer, while being able to use dynamic queries and other features not currently supported with NativeAOT.

Using precompiled queries without NativeAOT is simply a matter of executing the following:

```console
dotnet ef dbcontext optimize --precompile-queries
```

As shown above, this will generate a compiled model and interceptors for queries which could be precompiled, removing their overhead from your application's startup time.
