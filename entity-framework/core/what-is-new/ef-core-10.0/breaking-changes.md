---
title: Breaking changes in EF Core 10 (EF10) - EF Core
description: List of breaking changes introduced in Entity Framework Core 10 (EF10)
author: maumar
ms.date: 01/05/2025
uid: core/what-is-new/ef-core-10.0/breaking-changes
---

# Breaking changes in EF Core 10 (EF10)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 9 to EF Core 10. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Summary

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [ExecuteUpdateAsync now accepts a regular, non-expression lambda](#ExecuteUpdateAsync-lambda)             | Low        |

## Low-impact changes

<a name="ExecuteUpdateAsync-lambda"></a>

### ExecuteUpdateAsync now accepts a regular, non-expression lambda

[Tracking Issue #32018](https://github.com/dotnet/efcore/issues/32018)

#### Old behavior

Previously, <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> accepted an expression tree argument (`Expression<Func<...>>`) for the column setters.

#### New behavior

Starting with EF Core 10.0, <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> now accepts a non-expression argument (`Func<...>`) for the column setters. If you were building expression trees to dynamically create the column setters argument, your code will no longer compile - but can be replaced with a much simpler alternative (see below).

#### Why

The fact that the column setters parameter was an expression tree made it quite difficult to do dynamic construction of the column setters, where some setters are only present based on some condition (see Mitigations below for an example).

#### Mitigations

Code that was building expression trees to dynamically create the column setters argument will need to be rewritten - but the result will be much simpler. For example, let's assume we want to update a Blog's Views, but conditionally also its Name. Since the setters argument was an expression tree, code such as the following needed to be written:

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
