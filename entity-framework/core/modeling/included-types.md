---
title: Including & Excluding Types - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: cbe6935e-2679-4b77-8914-a8d772240cf1
uid: core/modeling/included-types
---
# Including & Excluding Types

Including a type in the model means that EF has metadata about that type and will attempt to read and write instances from/to the database.

## Conventions

By convention, types that are exposed in `DbSet` properties on your context are included in your model. In addition, types that are mentioned in the `OnModelCreating` method are also included. Finally, any types that are found by recursively exploring the navigation properties of discovered types are also included in the model.

**For example, in the following code listing all three types are discovered:**

* `Blog` because it is exposed in a `DbSet` property on the context

* `Post` because it is discovered via the `Blog.Posts` navigation property

* `AuditEntry` because it is mentioned in `OnModelCreating`

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/IncludedTypes.cs?name=IncludedTypes&highlight=3,7,16)]

## Data Annotations

You can use Data Annotations to exclude a type from the model.

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/IgnoreType.cs?highlight=20)]

## Fluent API

You can use the Fluent API to exclude a type from the model.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IgnoreType.cs?highlight=12)]
