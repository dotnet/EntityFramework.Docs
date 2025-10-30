---
title: Keyless Entity Types - EF Core
description: How to configure keyless entity types using Entity Framework Core
author: AndriySvyryd
ms.date: 10/10/2022
uid: core/modeling/keyless-entity-types
---
# Keyless Entity Types

> [!NOTE]
> This feature was added under the name of query types. It was later renamed to keyless entity types.

In addition to regular entity types, an EF Core model can contain _keyless entity types_, which can be used to carry out database queries against data that doesn't contain key values.

## Defining Keyless entity types

Keyless entity types can be defined as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/DataAnnotations/Keyless.cs?Name=Keyless&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/FluentAPI/Keyless.cs?Name=Keyless&highlight=4)]

***

## Keyless entity types characteristics

Keyless entity types support many of the same mapping capabilities as regular entity types, like inheritance mapping and navigation properties. On relational stores, they can configure the target database objects and columns via fluent API methods or data annotations.

However, they are different from regular entity types in that they:

- Cannot have a key defined.
- Are never tracked for changes in the _DbContext_ and therefore are never inserted, updated or deleted on the database.
- Are never discovered by convention.
- Only support a subset of navigation mapping capabilities, specifically:
  - They may never act as the principal end of a relationship.
  - They may not have navigations to owned entities
  - They can only contain reference navigation properties pointing to regular entities.
  - Entities cannot contain navigation properties to keyless entity types.
- Need to be configured with a `[Keyless]` data annotation or a `.HasNoKey()` method call.
- May be mapped to a _defining query_. A defining query is a query declared in the model that acts as a data source for a keyless entity type.
- Can have a hierarchy, but it must be mapped as TPH.
- Cannot use table splitting or entity splitting.

## Usage scenarios

Some of the main usage scenarios for keyless entity types are:

- Serving as the return type for [SQL queries](xref:core/querying/sql-queries).
- Mapping to database views that do not contain a primary key.
- Mapping to tables that do not have a primary key defined.
- Mapping to queries defined in the model.

## Mapping to database objects

Mapping a keyless entity type to a database object is achieved using the `ToTable` or `ToView` fluent API. From the perspective of EF Core, the database object specified in this method is a _view_, meaning that it is treated as a read-only query source and cannot be the target of update, insert or delete operations. However, this does not mean that the database object is actually required to be a database view. It can alternatively be a database table that will be treated as read-only. Conversely, for regular entity types, EF Core assumes that a database object specified in the `ToTable` method can be treated as a _table_, meaning that it can be used as a query source but also targeted by update, delete and insert operations. In fact, you can specify the name of a database view in `ToTable` and everything should work fine as long as the view is configured to be updatable on the database.

## Example

The following example shows how to use keyless entity types to query a database view.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/KeylessEntityTypes) on GitHub.

First, we define a simple Blog and Post model:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#Entities)]

Next, we define a simple database view that will allow us to query the number of posts associated with each blog:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#View)]

Next, we define a class to hold the result from the database view:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#KeylessEntityType)]

Next, we configure the keyless entity type in _OnModelCreating_ using the `HasNoKey` API.
We use fluent configuration API to configure the mapping for the keyless entity type:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#Configuration)]

Next, we configure the `DbContext` to include the `DbSet<T>`:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#DbSet)]

Finally, we can query the database view in the standard way:

[!code-csharp[Main](../../../samples/core/Modeling/KeylessEntityTypes/Program.cs#Query)]

> [!TIP]
> Note we have also defined a context level query property (DbSet) to act as a root for queries against this type.

> [!TIP]
> To test keyless entity types mapped to views using the in-memory provider, map them to a query via <xref:Microsoft.EntityFrameworkCore.InMemoryEntityTypeBuilderExtensions.ToInMemoryQuery*>. See the [in-memory provider docs](xref:core/testing/testing-without-the-database#in-memory-provider) for more information.
