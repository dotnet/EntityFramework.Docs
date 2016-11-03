---
title: Basic Query
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: ab6e35f1-397f-41c0-9ef4-85aec5466377
ms.prod: entity-framework
uid: core/querying/basic
---
# Basic Query

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

Learn how to load entities from the database using Language Integrate Query (LINQ).

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

## 101 LINQ samples

This page shows a few examples to achieve common tasks with Entity Framework Core. For an extensive set of samples showing what is possible with LINQ, see [101 LINQ Samples](https://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b).

## Loading all data

<!-- [!code-csharp[Main](samples/core/Querying/Querying/Basics/Sample.cs)] -->
````csharp
using (var context = new BloggingContext())
{
    var blogs = context.Blogs.ToList();
}
````

## Loading a single entity

<!-- [!code-csharp[Main](samples/core/Querying/Querying/Basics/Sample.cs)] -->
````csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs
        .Single(b => b.BlogId == 1);
}
````

## Filtering

<!-- [!code-csharp[Main](samples/core/Querying/Querying/Basics/Sample.cs)] -->
````csharp
using (var context = new BloggingContext())
{
    var blogs = context.Blogs
        .Where(b => b.Url.Contains("dotnet"))
        .ToList();
}
````
