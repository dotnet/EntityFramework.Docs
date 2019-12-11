---
title: Entity Types - EF Core
description: How to configure and map entity types using Entity Framework Core
author: roji
ms.date: 12/03/2019
ms.assetid: cbe6935e-2679-4b77-8914-a8d772240cf1
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

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/EntityTypes.cs?name=EntityTypes&highlight=3,7,16)]

## Excluding types from the model

If you don't want a type to be included in the model, you can exclude it:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/IgnoreType.cs?name=IgnoreType&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/IgnoreType.cs?name=IgnoreType&highlight=3)]

***

## Table name

By convention, each entity type will be set up to map to a database table with the same name as the DbSet property that exposes the entity. If no DbSet exists for the given entity, the class name is used.

You can manually configure the table name:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/TableName.cs?Name=TableName&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/TableName.cs?Name=TableName&highlight=3-4)]

***

## Table schema

When using a relational database, tables are by convention created in your database's default schema. For example, Microsoft SQL Server will use the `dbo` schema (SQLite does not support schemas).

You can configure tables to be created in a specific schema as follows:

### [Data Annotations](#tab/data-annotations)

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/TableNameAndSchema.cs?name=TableNameAndSchema&highlight=1)]

### [Fluent API](#tab/fluent-api)

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/TableNameAndSchema.cs?name=TableNameAndSchema&highlight=3-4)]

***

Rather than specifying the schema for each table, you can also define the default schema at the model level with the fluent API:

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/DefaultSchema.cs?name=DefaultSchema&highlight=3)]

Note that setting the default schema will also affect other database objects, such as sequences.
