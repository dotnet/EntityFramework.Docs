---
title: What's New in EF Core 10
description: Overview of new features in EF Core 10
author: maumar
ms.date: 01/05/2025
uid: core/what-is-new/ef-core-10.0/whatsnew
---

# What's New in EF Core 10

EF Core 10 (EF10) is the next release after EF Core 9 and is scheduled for release in November 2025.

EF10 is available as [daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md) which contain all the latest EF10 features and API tweaks. The samples here make use of these daily builds.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF10 requires the .NET 10 SDK to build and requires the .NET 10 runtime to run. EF10 will not run on earlier .NET versions, and will not run on .NET Framework.

> [!TIP]
> The _What's New_ docs are updated for each preview. All the samples are set up to use the [EF10 daily builds](https://github.com/dotnet/efcore/blob/main/docs/DailyBuilds.md), which usually have several additional weeks of completed work compared to the latest preview. We strongly encourage use of the daily builds when testing new features so that you're not doing your testing against stale bits.

<a name="linq-and-sql-translation"></a>

## LINQ and SQL translation

<a name="other-query-improvements"></a>

### Other query improvements

* Translation for DateOnly.ToDateTime(timeOnly) ([#35194](https://github.com/dotnet/efcore/pull/35194), contributed by [@mseada94](https://github.com/mseada94)).
* Optimization for multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384)), contributed by [@ranma42](https://github.com/ranma42)).
* Optimization for use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381)), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).

## ExecuteUpdateAsync now accepts a regular, non-expression lambda

The <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdateAsync*> can be used to express arbitrary update operations in the database. In previous versions, the changes to be performed on the database rows were provided via an expression tree parameter; this made it quite difficult to build those changes dynamically. For example, let's assume we want to update a Blog's Views, but conditionally also its Name. Since the setters argument was an expression tree, code such as the following needed to be written:

```c#
// Base setters - update the Views only
Expression<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>> setters =
    s => s.SetProperty(b => b.Views, 8);

// Conditionally add SetProperty(b => b.Name, "foo") to setters, based on the value of nameChanged
if (nameChanged)
{
    var blogParameter = Expression.Parameter(typeof(Blog), "b");

    setters = Expression.Lambda<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>>(
        Expression.Call(
            instance: setters.Body,
            methodName: nameof(SetPropertyCalls<Blog>.SetProperty),
            typeArguments: [typeof(string)],
            arguments:
            [
                Expression.Lambda<Func<Blog, string>>(Expression.Property(blogParameter, nameof(Blog.Name)), blogParameter),
                Expression.Constant("foo")
            ]),
        setters.Parameters);
}

await context.Blogs.ExecuteUpdateAsync(setters);
```

Manually creating expression trees is complicated and error-prone, and made this common scenario much more difficult than it should have been. Starting with EF 10, you can now write the following instead:

```c#
await context.Blogs.ExecuteUpdateAsync(s =>
{
    s.SetProperty(b => b.Views, 8);
    if (nameChanged)
    {
        s.SetProperty(b => b.Name, "foo");
    }
});
```

Thanks to [@aradalvand](https://github.com/aradalvand) for proposing and pushing for this change (in [#32018](https://github.com/dotnet/efcore/issues/32018)).
