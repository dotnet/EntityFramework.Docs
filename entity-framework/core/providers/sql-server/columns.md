---
title: Microsoft SQL Server Database Provider - Columns - EF Core
description: Column features specific to the Entity Framework Core SQL Server provider
author: roji
ms.date: 10/12/2021
uid: core/providers/sql-server/columns
---
# Column features specific to the Entity Framework Core SQL Server provider

This page details column configuration options that are specific to the SQL Server provider.

## Unicode and UTF-8

SQL Server 2019 introduced [introduced UTF-8](/sql/relational-databases/collations/collation-and-unicode-support#utf8) support, which allows storing UTF-8 data in `char` and `varchar` columns by configuring them with special UTF-8 collations. You can use UTF-8 columns with EF simply by configuring the column's type to `char` or `varchar`, specify a UTF-8 collation (ending with `_UTF8`), and specifying that the column should be Unicode:

```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Property(b => b.Name)
        .HasColumnType("varchar(max)")
        .UseCollation("LATIN1_GENERAL_100_CI_AS_SC_UTF8")
        .IsUnicode();
}
```

## Sparse columns

Sparse columns are ordinary columns that have an optimized storage for null values, reducing the space requirements for null values at the cost of more overhead to retrieve non-null values.

As an example, consider a type hierarchy mapped via [the table-per-hierarchy (TPH) strategy](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration). In TPH, a single database table is used to hold all types in a hierarchy; this means that the table must contain columns for each and every property across the entire hierarchy, and for columns belonging to rare types, most rows will contain a null value for that column. In these cases, it may make sense to configure the column as *sparse*, in order to reduce the space requirements. The decision whether to make a column sparse must be made by the user, and depends on expectations for actual data in the table.

A column can be made sparse via the Fluent API:

[!code-csharp[SparseColumn](../../../../samples/core/SqlServer/Columns/SparseColumnContext.cs?name=SparseColumn&highlight=5)]

For more information on sparse columns, [see the SQL Server docs](/sql/relational-databases/tables/use-sparse-columns).
