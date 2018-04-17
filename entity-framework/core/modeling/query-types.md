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

Query Types are read-only query result types that can be added to the EF Core model. Query Types enable ad-hoc querying (like anonymous types), but are more flexible because they can have mapping configuration specified.

They are conceptually similar to Entity Types in that:

- They are simple classes that are added to the model, either in `OnModelCreating` using the `ModelBuilder.Query` method, or via a DbContext "set" property (for query types such a property is typed as `DbQuery<T>` rather than `DbSet<T>`).
- They support much of the same mapping capabilities as regular entity types. For example, inheritance mapping, navigations (see limitations below) and, on relational stores, the ability to configure the target database objects via fluent API methods (or data annotations).

Query Types are different from entity types in that they:

- Do not require a key to be defined.
- Are never tracked by the Change Tracker.
- Are never discovered by convention.
- Are mapped to database objects using the `ToView` method, rather than `ToTable`.
- Only support a subset of navigation mapping capabilities - Specifically, they may never act as the principal end of a relationship.
- May be mapped to a _defining query_ - A Defining Query is a secondary query that acts a data source for a Query Type.

Some of the main usage scenarios for query types are:

- Mapping to database views.
- Mapping to tables that do not have a primary key defined.
- Serving as the return type for ad hoc `FromSql()` queries.
- Mapping to queries defined in the model.

> [!TIP]
> Mapping a query type to a database object is achieved using the `ToView` fluent API. From the perspective of EF Core, the database object specified in this method is a _view_, which simply means that it is treated as a read-only query source and cannot not be the target of update, insert or delete operations. However, this does not mean that the database object is required to be database view - It can alternatively be a database table that needs to be treated as read-only. Conversely, for entity types, EF Core assumes that a database object specified in the `ToTable` method can be treated as a _table_, meaning that it can be used as a query source but also targeted by update, delete and insert operations. In fact, you can specify the name of a database view in `ToTable` and everything should work as long as the view is configured on the database to be updatable.    

## Example

The following example shows how to use Query Type to query a database view.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFrameworkCore/tree/dev/samples/QueryTypes) on GitHub.

First, we define a simple Blog and Post model:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Entities)]

Next, we define a simple database view that will allow us to query the number of posts associated with each blog:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#View)]

Next, we define a class to hold the result from the database view:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#QueryType)]

Next, we configure the query type in _OnModelCreating_ using the `modelBuilder.Query<T>` API.
We use standard fluent configuration APIs to configure the mapping for the Query Type:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Configuration)]

Finally, we can query the database view in the standard way:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Query)]

> [!TIP]
> Note we have also defined a context level query property (DbQuery) to act as a root for queries against this type.
