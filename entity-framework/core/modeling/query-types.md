---
title: Query Types - EF Core
author: anpete
ms.author: anpete
ms.date: 2/26/2018
ms.assetid: 9F4450C5-1A3F-4BB6-AC19-9FAC64292AAD
ms.technology: entity-framework-core
uid: core/modeling/query-types
---
# Query Types
> [!NOTE]
> This feature is new in EF Core 2.1

In addition to entity types, an EF Core model can contain _query types_, which can be used to carry out database queries against data that isn't mapped to entity types.

Query types have many similarities with entity types:

- They can also be added to the model either in `OnModelCreating`, or via a "set" property on a derived _DbContext_.
- They support many of the same mapping capabilities, like inheritance mapping, navigation properties (see limitations below) and, on relational stores, the ability to configure the target database objects and columns via fluent API methods or data annotations.

However they are different from entity types in that they:

- Do not require a key to be defined.
- Are never tracked for changes on the _DbContext_ and therefore are never inserted, updated or deleted on the database.
- Are never discovered by convention.
- Only support a subset of navigation mapping capabilities - Specifically:
  - They may never act as the principal end of a relationship.
  - They can only contain reference navigation properties pointing to entities.
  - Entities cannot contain navigation properties to query types.
- Are addressed on the _ModelBuilder_ using the `Query` method rather than the `Entity` method.
- Are mapped on the _DbContext_ through properties of type `DbQuery<T>` rather than `DbSet<T>`
- Are mapped to database objects using the `ToView` method, rather than `ToTable`.
- May be mapped to a _defining query_ - A defining query is a secondary query declared in the model that acts a data source for a query type.

Some of the main usage scenarios for query types are:

- Serving as the return type for ad hoc `FromSql()` queries.
- Mapping to database views.
- Mapping to tables that do not have a primary key defined.
- Mapping to queries defined in the model.

> [!TIP]
> Mapping a query type to a database object is achieved using the `ToView` fluent API. From the perspective of EF Core, the database object specified in this method is a _view_, meaning that it is treated as a read-only query source and cannot be the target of update, insert or delete operations. However, this does not mean that the database object is actually required to be a database view - It can alternatively be a database table that will be treated as read-only. Conversely, for entity types, EF Core assumes that a database object specified in the `ToTable` method can be treated as a _table_, meaning that it can be used as a query source but also targeted by update, delete and insert operations. In fact, you can specify the name of a database view in `ToTable` and everything should work fine as long as the view is configured to be updatable on the database.

## Example

The following example shows how to use Query Type to query a database view.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFrameworkCore/tree/dev/samples/QueryTypes) on GitHub.

First, we define a simple Blog and Post model:

[!code-csharp[Main](../../../efcore-repo/samples/QueryTypes/Program.cs#Entities)]

Next, we define a simple database view that will allow us to query the number of posts associated with each blog:

[!code-csharp[Main](../../../efcore-repo/samples/QueryTypes/Program.cs#View)]

Next, we define a class to hold the result from the database view:

[!code-csharp[Main](../../../efcore-repo/samples/QueryTypes/Program.cs#QueryType)]

Next, we configure the query type in _OnModelCreating_ using the `modelBuilder.Query<T>` API.
We use standard fluent configuration APIs to configure the mapping for the Query Type:

[!code-csharp[Main](../../../efcore-repo/samples/QueryTypes/Program.cs#Configuration)]

Finally, we can query the database view in the standard way:

[!code-csharp[Main](../../../efcore-repo/samples/QueryTypes/Program.cs#Query)]

> [!TIP]
> Note we have also defined a context level query property (DbQuery) to act as a root for queries against this type.
