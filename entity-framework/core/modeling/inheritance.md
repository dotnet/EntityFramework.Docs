---
title: Inheritance - EF Core
description: How to configure entity type inheritance using Entity Framework Core
author: AndriySvyryd
ms.date: 10/01/2020
uid: core/modeling/inheritance
---
# Inheritance

EF can map a .NET type hierarchy to a database. This allows you to write your .NET entities in code as usual, using base and derived types, and have EF seamlessly create the appropriate database schema, issue queries, etc. The actual details of how a type hierarchy is mapped are provider-dependent; this page describes inheritance support in the context of a relational database.

## Entity type hierarchy mapping

By convention, EF will not automatically scan for base or derived types; this means that if you want a CLR type in your hierarchy to be mapped, you must explicitly specify that type on your model. For example, specifying only the base type of a hierarchy will not cause EF Core to implicitly include all of its sub-types.

The following sample exposes a DbSet for `Blog` and its subclass `RssBlog`. If `Blog` has any other subclass, it will not be included in the model.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/InheritanceDbSets.cs?name=InheritanceDbSets&highlight=3-4)]

> [!NOTE]
> Database columns are automatically made nullable as necessary when using TPH mapping. For example, the `RssUrl` column is nullable because regular `Blog` instances do not have that property.

If you don't want to expose a `DbSet` for one or more entities in the hierarchy, you can also use the Fluent API to ensure they are included in the model.

> [!TIP]
> If you don't rely on conventions, you can specify the base type explicitly using `HasBaseType`. You can also use `.HasBaseType((Type)null)` to remove an entity type from the hierarchy.

## Table-per-hierarchy and discriminator configuration

By default, EF maps the inheritance using the *table-per-hierarchy* (TPH) pattern. TPH uses a single table to store the data for all types in the hierarchy, and a discriminator column is used to identify which type each row represents.

The model above is mapped to the following database schema (note the implicitly created `Discriminator` column, which identifies which type of `Blog` is stored in each row).

![Screenshot of the results of querying the Blog entity hierarchy using table-per-hierarchy pattern](_static/inheritance-tph-data.png)

You can configure the name and type of the discriminator column and the values that are used to identify each type in the hierarchy:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DiscriminatorConfiguration.cs?name=DiscriminatorConfiguration&highlight=4-6)]

In the examples above, EF added the discriminator implicitly as a [shadow property](xref:core/modeling/shadow-properties) on the base entity of the hierarchy. This property can be configured like any other:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DiscriminatorPropertyConfiguration.cs?name=DiscriminatorPropertyConfiguration&highlight=4-5)]

Finally, the discriminator can also be mapped to a regular .NET property in your entity:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/NonShadowDiscriminator.cs?name=NonShadowDiscriminator&highlight=4)]

When querying for derived entities, which use the TPH pattern, EF Core adds a predicate over discriminator column in the query. This filter makes sure that we don't get any additional rows for base types or sibling types not in the result. This filter predicate is skipped for the base entity type since querying for the base entity will get results for all the entities in the hierarchy. When materializing results from a query, if we come across a discriminator value, which isn't mapped to any entity type in the model, we throw an exception since we don't know how to materialize the results. This error only occurs if your database contains rows with discriminator values, which aren't mapped in the EF model. If you have such data, then you can mark the discriminator mapping in EF Core model as incomplete to indicate that we should always add filter predicate for querying any type in the hierarchy. `IsComplete(false)` call on the discriminator configuration marks the mapping to be incomplete.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DiscriminatorMappingIncomplete.cs?name=DiscriminatorMappingIncomplete&highlight=5)]

### Shared columns

By default, when two sibling entity types in the hierarchy have a property with the same name, they will be mapped to two separate columns. However, if their type is identical they can be mapped to the same database column:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/SharedTPHColumns.cs?name=SharedTPHColumns&highlight=9,13)]

> [!NOTE]
> Relational database providers, such as SQL Server, will not automatically use the discriminator predicate when querying shared columns when using a cast. The query `Url = (blob as RssBlog).Url` would also return the `Url` value for the sibling `Blog` rows. To restrict the query to `RssBlog` entities you need to manually add a filter on the discriminator, such as `Url = blob is RssBlog ? (blob as RssBlog).Url : null`.

## Table-per-type configuration

> [!NOTE]
> The table-per-type (TPT) feature was introduced in EF Core 5.0. Table-per-concrete-type (TPC) is supported by EF6, but is not yet supported by EF Core.

In the TPT mapping pattern, all the types are mapped to individual tables. Properties that belong solely to a base type or derived type are stored in a table that maps to that type. Tables that map to derived types also store a foreign key that joins the derived table with the base table.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/TPTConfiguration.cs?name=TPTConfiguration)]

EF will create the following database schema for the model above.

```sql
CREATE TABLE [Blogs] (
    [BlogId] int NOT NULL IDENTITY,
    [Url] nvarchar(max) NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([BlogId])
);

CREATE TABLE [RssBlogs] (
    [BlogId] int NOT NULL,
    [RssUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_RssBlogs] PRIMARY KEY ([BlogId]),
    CONSTRAINT [FK_RssBlogs_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]) ON DELETE NO ACTION
);
```

> [!NOTE]
> If the primary key constraint is renamed the new name will be applied to all the tables mapped to the hierarchy, future EF versions will allow renaming the constraint only for a particular table when [issue 19970](https://github.com/dotnet/efcore/issues/19970) is fixed.

If you are employing bulk configuration you can retrieve the column name for a specific table by calling <xref:Microsoft.EntityFrameworkCore.RelationalPropertyExtensions.GetColumnName(Microsoft.EntityFrameworkCore.Metadata.IProperty,Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier@)>.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/TPTConfiguration.cs?name=Metadata&highlight=10)]

> [!WARNING]
> In many cases, TPT shows inferior performance when compared to TPH. [See the performance docs for more information](xref:core/performance/modeling-for-performance#inheritance-mapping).
