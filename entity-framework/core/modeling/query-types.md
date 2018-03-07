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

- They are POCO C# types that are added to the model, either in ```OnModelCreating``` using the ```ModelBuilder.Query``` method, or via a DbContext "set" property (for query types such a property is typed as ```DbQuery<T>``` rather that ```DbSet<T>```).
- They support much of the same mapping capabilities as regular entity types. For example, inheritance mapping, navigations (see limitiations below) and, on relational stores, the ability to configure the target database schema objects via ```ToTable```, ```HasColumn``` fluent-api methods (or data annotations).

Query Types are different from entity types in that they:

- Do not require a key to be defined.
- Are never tracked by the Change Tracker.
- Are never discovered by convention.
- Only support a subset of navigation mapping capabilities - Specifically, they may never act as the principal end of a relationship.
- May be mapped to a _defining query_ - A Defining Query is a secondary query that acts a data source for a Query Type.

Some of the main usage scenarios for query types are:

- Mapping to database views.
- Mapping to tables that do not have a primary key defined.
- Serving as the return type for ad hoc ```FromSql()``` queries.
- Mapping to queries defined in the model.

> [!TIP]
> Mapping a query type to a database view is achieved using the ```ToTable``` fluent API.

## Example

The following example shows how to use Query Type to query a database view.

> [!TIP]
> You can view this article's [sample](https://github.com/aspnet/EntityFrameworkCore/tree/dev/samples/QueryTypes) on GitHub.

First, we define a simple Blog and Post model:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Entities)]

Next, we define a simple database view that will allow us to query the number of posts associated with each blog:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#View)]

Next, we define a class to hold the result from the database view:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Query Type)]

Next, we configure the query type in _OnModelCreating_ using the ```modelBuilder.Query<T>``` API.
We use standard fluent configuration APIs to configure the mapping for the Query Type:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Configuration)]

Finally, we can query the database view in the standard way:

[!code-csharp[Main](../../../efcore-dev/samples/QueryTypes/Program.cs#Query)]

-> [!TIP] 
-> Note we have also defined a context level query property (DbQuery) to act as a root for queries against this type. 
