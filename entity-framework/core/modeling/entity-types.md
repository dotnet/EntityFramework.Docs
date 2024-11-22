---
title: Entity Types - EF Core
description: How to configure and map entity types using Entity Framework Core
author: AndriySvyryd
ms.date: 10/25/2021
uid: core/modeling/entity-types
---
# Entity Types

Including a DbSet of a type on your context means that it is included in EF Core's model; we usually refer to such a type as an *entity*. EF Core can read and write entity instances from/to the database, and if you're using a relational database, EF Core can create tables for your entities via migrations.

## Including types in the model

By convention, types that are exposed in DbSet properties on your context are included in the model as entities. Entity types that are specified in the `OnModelCreating` method are also included, as are any types that are found by recursively exploring the navigation properties of other discovered entity types.

In the code sample below, all types are included:

* `Blog` is included because it's exposed in a DbSet property on the context.
* `Post` is included because it's discovered via the `Blog.Posts` navigation property.
* `AuditEntry` because it is specified in `OnModelCreating`.

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/EntityTypes.cs?name=EntityTypes&highlight=3,7,16)]

## Excluding types from the model

If you don't want a type to be included in the model, you can exclude it:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/DataAnnotations/IgnoreType.cs?name=IgnoreType&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/IgnoreType.cs?name=IgnoreType&highlight=3)]

***

### Excluding from migrations

It is sometimes useful to have the same entity type mapped in multiple `DbContext` types. This is especially true when using [bounded contexts](https://www.martinfowler.com/bliki/BoundedContext.html), for which it is common to have a different `DbContext` type for each bounded context.

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/TableExcludeFromMigrations.cs?name=TableExcludeFromMigrations&highlight=4)]

With this configuration migrations will not create the `AspNetUsers` table, but `IdentityUser` is still included in the model and can be used normally.

If you need to start managing the table using migrations again then a new migration should be created where `AspNetUsers` is not excluded. The next migration will now contain any changes made to the table.

## Table name

By convention, each entity type will be set up to map to a database table with the same name as the DbSet property that exposes the entity. If no DbSet exists for the given entity, the class name is used.

You can manually configure the table name:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/DataAnnotations/TableName.cs?Name=TableName&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/TableName.cs?Name=TableName&highlight=3-4)]

***

## Table schema

When using a relational database, tables are by convention created in your database's default schema. For example, Microsoft SQL Server will use the `dbo` schema (SQLite does not support schemas).

You can configure tables to be created in a specific schema as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/DataAnnotations/TableNameAndSchema.cs?name=TableNameAndSchema&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/TableNameAndSchema.cs?name=TableNameAndSchema&highlight=3-4)]

***

Rather than specifying the schema for each table, you can also define the default schema at the model level with the fluent API:

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/DefaultSchema.cs?name=DefaultSchema&highlight=3)]

Note that setting the default schema will also affect other database objects, such as sequences.

## View mapping

Entity types can be mapped to database views using the Fluent API.

> [!NOTE]
> EF will assume that the referenced view already exists in the database, it will not create it automatically in a migration.

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/ViewNameAndSchema.cs?name=ViewNameAndSchema&highlight=1)]

 Mapping to a view will remove the default table mapping, but the entity type can also be mapped to a table explicitly. In this case the query mapping will be used for queries and the table mapping will be used for updates.

> [!TIP]
> To test keyless entity types mapped to views using the in-memory provider, map them to a query via <xref:Microsoft.EntityFrameworkCore.InMemoryEntityTypeBuilderExtensions.ToInMemoryQuery*>. See the [in-memory provider docs](xref:core/testing/testing-without-the-database#in-memory-provider) for more information.

## Table-valued function mapping

It's possible to map an entity type to a table-valued function (TVF) instead of a table in the database. To illustrate this, let's define another entity that represents blog with multiple posts. In the example, the entity is [keyless](xref:core/modeling/keyless-entity-types), but it doesn't have to be.

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/EntityTypes.cs#BlogWithMultiplePostsEntity)]

Next, create the following table-valued function in the database, which returns only blogs with multiple posts as well as the number of posts associated with each of these blogs:

```sql
CREATE FUNCTION dbo.BlogsWithMultiplePosts()
RETURNS TABLE
AS
RETURN
(
    SELECT b.Url, COUNT(p.BlogId) AS PostCount
    FROM Blogs AS b
    JOIN Posts AS p ON b.BlogId = p.BlogId
    GROUP BY b.BlogId, b.Url
    HAVING COUNT(p.BlogId) > 1
)
```

Now, the entity `BlogWithMultiplePosts` can be mapped to this function in a following way:

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/EntityTypes.cs#QueryableFunctionConfigurationToFunction)]

> [!NOTE]
> In order to map an entity to a table-valued function the function must be parameterless.

Conventionally, the entity properties will be mapped to matching columns returned by the TVF. If the columns returned by the TVF have different names than the entity property, then the entity's columns can be configured using `HasColumnName` method, just like when mapping to a regular table.

When the entity type is mapped to a table-valued function, the query:

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/Program.cs#ToFunctionQuery)]

Produces the following SQL:

```sql
SELECT [b].[Url], [b].[PostCount]
FROM [dbo].[BlogsWithMultiplePosts]() AS [b]
WHERE [b].[PostCount] > 3
```

## Table comments

You can set an arbitrary text comment that gets set on the database table, allowing you to document your schema in the database:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/DataAnnotations/TableComment.cs?name=TableComment&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/EntityTypes/FluentAPI/TableComment.cs?name=TableComment&highlight=4)]

***

## Shared-type entity types

Entity types that use the same CLR type are known as shared-type entity types. These entity types need to be configured with a unique name, which must be supplied whenever the shared-type entity type is used, in addition to the CLR type. This means that the corresponding `DbSet` property must be implemented using a `Set` call.

[!code-csharp[Main](../../../samples/core/Modeling/ShadowAndIndexerProperties/SharedType.cs?name=SharedType&highlight=3,7)]
