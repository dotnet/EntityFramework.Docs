---
title: Indexes - EF Core
author: roji
ms.date: 12/16/2019
ms.assetid: 85b92003-b692-417d-ac1d-76d40dce664b
uid: core/modeling/indexes
---
# Indexes

Indexes are a common concept across many data stores. While their implementation in the data store may vary, they are used to make lookups based on a column (or set of columns) more efficient.

Indexes cannot be created using data annotations. You can use the Fluent API to specify an index on a single column as follows:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Index.cs?name=Index&highlight=4)]

You can also specify an index over more than one column:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexComposite.cs?name=Composite&highlight=4)]

> [!NOTE]
> By convention, an index is created in each property (or set of properties) that are used as a foreign key.
>
> EF Core only supports one index per distinct set of properties. If you use the Fluent API to configure an index on a set of properties that already has an index defined, either by convention or previous configuration, then you will be changing the definition of that index. This is useful if you want to further configure an index that was created by convention.

## Index uniqueness

By default, indexes aren't unique: multiple rows are allowed to have the same value(s) for the index's column set. You can make an index unique as follows:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexUnique.cs?name=IndexUnique&highlight=5)]

Attempting to insert more than one entity with the same values for the index's column set will cause an exception to be thrown.

## Index name

By convention, indexes created in a relational database are named `IX_<type name>_<property name>`. For composite indexes, `<property name>` becomes an underscore separated list of property names.

You can use the Fluent API to set the name of the index created in the database:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexName.cs?name=IndexName&highlight=5)]

## Index filter

Some relational databases allow you to specify a filtered or partial index. This allows you to index only a subset of a column's values, reducing the index's size and improving both performance and disk space usage. For more information on SQL Server filtered indexes, [see the documentation](https://docs.microsoft.com/sql/relational-databases/indexes/create-filtered-indexes).

You can use the Fluent API to specify a filter on an index, provided as a SQL expression:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexFilter.cs?name=IndexFilter&highlight=5)]

When using the SQL Server provider EF adds an `'IS NOT NULL'` filter for all nullable columns that are part of a unique index. To override this convention you can supply a `null` value.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexNoFilter.cs?name=IndexNoFilter&highlight=6)]

## Included columns

Some relational databases allow you to configure a set of columns which get included in the index, but aren't part of its "key". This can significantly improve query performance when all columns in the query are included in the index either as key or nonkey columns, as the table itself doesn't need to be accessed. For more information on SQL Server included columns, [see the documentation](https://docs.microsoft.com/sql/relational-databases/indexes/create-indexes-with-included-columns).

In the following example, the `Url` column is part of the index key, so any query filtering on that column can use the index. But in addition, queries accessing only the `Title` and `PublishedOn` columns will not need to access the table and will run more efficiently:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IndexInclude.cs?name=IndexInclude&highlight=5-9)]
