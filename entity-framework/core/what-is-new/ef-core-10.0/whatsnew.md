---
title: What's New in EF Core 10
description: Overview of new features in EF Core 10
author: maumar
ms.date: 01/05/2025
uid: core/what-is-new/ef-core-10.0/whatsnew
---

# What's New in EF Core 10

EF Core 10 (EF10) is the next release after EF Core 9 and is scheduled for release in November 2025.

EF10 is available as a preview. See [.NET 10 release notes](https://github.com/dotnet/core/tree/main/release-notes/10.0) to get information about the latest preview. This article will be updated as new preview releases are made available.

> [!TIP]
> You can run and debug into the samples by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs). Each section below links to the source code specific to that section.

EF10 requires the .NET 10 SDK to build and requires the .NET 10 runtime to run. EF10 will not run on earlier .NET versions, and will not run on .NET Framework.

<a name="cosmos"></a>

## Azure Cosmos DB for NoSQL

<a name="improved-model-evolution"></a>

### Improved experience when evolving the model

In previous versions of EF Core, evolving the model when using Azure Cosmos DB was quite painful. Specifically, when adding a new required property to the entity, EF would no longer be able to materialize that entity. The reason was that EF expected a value for the new property (since it was required), but the document created before the change didn't contain those values. The workaround was to mark the property as optional first, manually add default values for the property, and only then change it to required.

In EF 10 we improved this experience - EF will now materialize a default value for a required property, if no data is present for it in the document, rather than throw.

<a name="linq-and-sql-translation"></a>

## LINQ and SQL translation

<a name="support-left-join"></a>

### Support for the .NET 10 `LeftJoin` and `RightJoin` operators

`LEFT JOIN` is a common and useful operation when working with EF Core. In previous versions, implementing `LEFT JOIN` in LINQ was quite complicated, requiring `SelectMany`, `GroupJoin` and `DefaultIfEmpty` operations [in a particular configuration](/dotnet/csharp/linq/standard-query-operators/join-operations#perform-left-outer-joins).

.NET 10 adds first-class LINQ support for `LeftJoin` method, making those queries much simpler to write. EF Core recognizes the new method, so it can be used in EF LINQ queries instead of the old construct:

```C#
var query = context.Students
    .LeftJoin(
        context.Departments,
        student => student.DepartmentID,
        department => department.ID,
        (student, department) => new 
        { 
            student.FirstName,
            student.LastName,
            Department = department.Name ?? "[NONE]"
        });
```

> [!NOTE]
> EF 10 also supports the analogous `RightJoin` operator, which keeps all the data from the second collection and only the matching data from the first collection. EF 10 translates this to `RIGHT JOIN` operation in the database.

See [#12793](https://github.com/dotnet/efcore/issues/12793) and [#35367](https://github.com/dotnet/efcore/issues/35367) for more details.

<a name="other-query-improvements"></a>

### Other query improvements

* Translation for DateOnly.ToDateTime(timeOnly) ([#35194](https://github.com/dotnet/efcore/pull/35194), contributed by [@mseada94](https://github.com/mseada94)).
* Optimization for multiple consecutive `LIMIT`s ([#35384](https://github.com/dotnet/efcore/pull/35384), contributed by [@ranma42](https://github.com/ranma42)).
* Optimization for use of `Count` operation on `ICollection<T>` ([#35381](https://github.com/dotnet/efcore/pull/35381), contributed by [@ChrisJollyAU](https://github.com/ChrisJollyAU)).
* Optimization for `MIN`/`MAX` over `DISTINCT` ([#34699](https://github.com/dotnet/efcore/pull/34699), contributed by [@ranma42](https://github.com/ranma42)).
* Translation for date/time functions using `DatePart.Microsecond` and `DatePart.Nanosecond` arguments ([#34861](https://github.com/dotnet/efcore/pull/34861)).
* Simplifying parameter names (e.g. from `@__city_0` to `city`) ([#35200](https://github.com/dotnet/efcore/pull/35200)).

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

<a name="other-improvements"></a>

## Other improvements

* Make SQL Server scaffolding compatible with Azure Data Explorer ([#34832](https://github.com/dotnet/efcore/pull/34832), contributed by [@barnuri](https://github.com/barnuri)).
* Associate the DatabaseRoot with the scoped options instance and not the singleton options ([#34477](https://github.com/dotnet/efcore/pull/34477), contributed by [@koenigst](https://github.com/koenigst)).
* Redact inlined constants from log when sensitive logging is off ([#35724](https://github.com/dotnet/efcore/pull/35724)).
* Improve LoadExtension to work correctly with dotnet run and lib* named libs ([#35617](https://github.com/dotnet/efcore/pull/35617), contributed by [@krwq](https://github.com/krwq)).
