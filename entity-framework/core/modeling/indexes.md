---
title: Indexes - EF Core
description: Configuring indexes in an Entity Framework Core model
author: roji
ms.date: 10/1/2021
uid: core/modeling/indexes
---
# Indexes

Indexes are a common concept across many data stores. While their implementation in the data store may vary, they are used to make lookups based on a column (or set of columns) more efficient. See the [indexes section](xref:core/performance/efficient-querying#use-indexes-properly) in the performance documentation for more information on good index usage.

You can specify an index over a column as follows:

## [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/Index.cs?name=Index&highlight=1)]

## [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/Index.cs?name=Index&highlight=4)]

***

> [!NOTE]
> By convention, an index is created in each property (or set of properties) that are used as a foreign key.

## Composite index

An index can also span more than one column:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/IndexComposite.cs?name=Composite&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexComposite.cs?name=Composite&highlight=4)]

***

Indexes over multiple columns, also known as *composite indexes*, speed up queries which filter on index's columns, but also queries which only filter on the *first* columns covered by the index. See the [performance docs](xref:core/performance/efficient-querying#use-indexes-properly) for more information.

## Index uniqueness

By default, indexes aren't unique: multiple rows are allowed to have the same value(s) for the index's column set. You can make an index unique as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/IndexUnique.cs?name=IndexUnique&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexUnique.cs?name=IndexUnique&highlight=5)]

***

Attempting to insert more than one entity with the same values for the index's column set will cause an exception to be thrown.

## Index sort order

> [!NOTE]
> This feature is being introduced in EF Core 7.0.

In most databases, each column covered by an index can be either ascending or descending. For indexes covering only one column, this typically does not matter: the database can traverse the index in reverse order as needed. However, for composite indexes, the ordering can be crucial for good performance, and can mean the difference between an index getting used by a query or not. In general, the index columns' sort orders should correspond to those specified in the `ORDER BY` clause of your query.

The index sort order is ascending by default. You can make all columns have descending order as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/IndexDescending.cs?name=IndexDescending&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexDescending.cs?name=IndexDescending&highlight=5)]

***

You may also specify the sort order on a column-by-column basis as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/IndexDescendingAscending.cs?name=IndexDescendingAscending&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexDescendingAscending.cs?name=IndexDescendingAscending&highlight=5)]

***

## Index naming and multiple indexes

By convention, indexes created in a relational database are named `IX_<type name>_<property name>`. For composite indexes, `<property name>` becomes an underscore separated list of property names.

You can set the name of the index created in the database:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/DataAnnotations/IndexName.cs?name=IndexName&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexName.cs?name=IndexName&highlight=5)]

***

Note that if you call `HasIndex` more than once on the same set of properties, that continues to configure a single index rather than create a new one:

```csharp
modelBuilder.Entity<Person>()
    .HasIndex(p => new { p.FirstName, p.LastName })
    .HasDatabaseName("IX_Names_Ascending");

modelBuilder.Entity<Person>()
    .HasIndex(p => new { p.FirstName, p.LastName })
    .HasDatabaseName("IX_Names_Descending")
    .IsDescending();
```

Since the second `HasIndex` call overrides the first one, this creates only a single, descending index. This can be useful for further configuring an index that was created by convention.

To create multiple indexes over the same set of properties, pass a name to the `HasIndex`, which will be used to identify the index in the EF model, and to distinguish it from other indexes over the same properties:

```c#
modelBuilder.Entity<Person>()
    .HasIndex(p => new { p.FirstName, p.LastName }, "IX_Names_Ascending");

modelBuilder.Entity<Person>()
    .HasIndex(p => new { p.FirstName, p.LastName }, "IX_Names_Descending")
    .IsDescending();
```

Note that this name is also used as a default for the database name, so explicitly calling `HasDatabaseName` isn't required.

## Index filter

Some relational databases allow you to specify a filtered or partial index. This allows you to index only a subset of a column's values, reducing the index's size and improving both performance and disk space usage. For more information on SQL Server filtered indexes, [see the documentation](/sql/relational-databases/indexes/create-filtered-indexes).

You can use the Fluent API to specify a filter on an index, provided as a SQL expression:

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexFilter.cs?name=IndexFilter&highlight=5)]

When using the SQL Server provider EF adds an `'IS NOT NULL'` filter for all nullable columns that are part of a unique index. To override this convention you can supply a `null` value.

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexNoFilter.cs?name=IndexNoFilter&highlight=6)]

## Included columns

Some relational databases allow you to configure a set of columns which get included in the index, but aren't part of its "key". This can significantly improve query performance when all columns in the query are included in the index either as key or nonkey columns, as the table itself doesn't need to be accessed. For more information on SQL Server included columns, [see the documentation](/sql/relational-databases/indexes/create-indexes-with-included-columns).

In the following example, the `Url` column is part of the index key, so any query filtering on that column can use the index. But in addition, queries accessing only the `Title` and `PublishedOn` columns will not need to access the table and will run more efficiently:

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/IndexInclude.cs?name=IndexInclude&highlight=5-9)]

## Check constraints

Check constraints are a standard relational feature that allows you to define a condition that must hold for all rows in a table; any attempt to insert or modify data that violates the constraint will fail. Check constraints are similar to non-null constraints (which prohibit nulls in a column) or to unique constraints (which prohibit duplicates), but allow arbitrary SQL expression to be defined.

You can use the Fluent API to specify a check constraint on a table, provided as a SQL expression:

[!code-csharp[Main](../../../samples/core/Modeling/IndexesAndConstraints/FluentAPI/CheckConstraint.cs?name=CheckConstraint&highlight=4)]

Multiple check constraints can be defined on the same table, each with their own name.

Note: some common check constraints can be configured via the community package [EFCore.CheckConstraints](https://github.com/efcore/EFCore.CheckConstraints).
