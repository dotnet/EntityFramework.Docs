---
title: Indexes - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 85b92003-b692-417d-ac1d-76d40dce664b
uid: core/modeling/indexes
---
# Indexes

Indexes are a common concept across many data stores. While their implementation in the data store may vary, they are used to make lookups based on a column (or set of columns) more efficient.

## Conventions

By convention, an index is created in each property (or set of properties) that are used as a foreign key.

## Data Annotations

Indexes can not be created using data annotations.

## Fluent API

You can use the Fluent API to specify an index on a single property. By default, indexes are non-unique.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Index.cs?name=Index&highlight=7,8)]

You can also specify that an index should be unique, meaning that no two entities can have the same value(s) for the given property(s).

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexUnique.cs?name=modelBuilder&highlight=3)]

You can also specify an index over more than one column.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexComposite.cs?name=Composite&highlight=7,8)]

> [!TIP]  
> There is only one index per distinct set of properties. If you use the Fluent API to configure an index on a set of properties that already has an index defined, either by convention or previous configuration, then you will be changing the definition of that index. This is useful if you want to further configure an index that was created by convention.
