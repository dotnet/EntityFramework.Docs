---
title: Saving Related Data - EF Core
description: Information on saving graphs of related entities and managing relationships in Entity Framework Core
author: SamMonoRT
ms.date: 10/27/2016
uid: core/saving/related-data
---
# Saving Related Data

In addition to isolated entities, you can also make use of the relationships defined in your model.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Saving/RelatedData/) on GitHub.

## Adding a graph of new entities

If you create several new related entities, adding one of them to the context will cause the others to be added too.

In the following example, the blog and three related posts are all inserted into the database. The posts are found and added, because they are reachable via the `Blog.Posts` navigation property.

[!code-csharp[Main](../../../samples/core/Saving/RelatedData/Sample.cs#AddingGraphOfEntities)]

> [!TIP]
> Use the EntityEntry.State property to set the state of just a single entity. For example, `context.Entry(blog).State = EntityState.Modified`.

## Adding a related entity

If you reference a new entity from the navigation property of an entity that is already tracked by the context, the entity will be discovered and inserted into the database.

In the following example, the `post` entity is inserted because it is added to the `Posts` property of the `blog` entity which was fetched from the database.

[!code-csharp[Main](../../../samples/core/Saving/RelatedData/Sample.cs#AddingRelatedEntity)]

## Changing relationships

If you change the navigation property of an entity, the corresponding changes will be made to the foreign key column in the database.

In the following example, the `post` entity is updated to belong to the new `blog` entity because its `Blog` navigation property is set to point to `blog`. Note that `blog` will also be inserted into the database because it is a new entity that is referenced by the navigation property of an entity that is already tracked by the context (`post`).

[!code-csharp[Main](../../../samples/core/Saving/RelatedData/Sample.cs#ChangingRelationships)]

## Removing relationships

You can remove a relationship by setting a reference navigation to `null`, or removing the related entity from a collection navigation.

Removing a relationship can have side effects on the dependent entity, according to the cascade delete behavior configured in the relationship.

By default, for required relationships, a cascade delete behavior is configured and the child/dependent entity will be deleted from the database. For optional relationships, cascade delete is not configured by default, but the foreign key property will be set to null.

See [Required and Optional Relationships](xref:core/modeling/relationships#optional-and-required-relationships) to learn about how the requiredness of relationships can be configured.

See [Cascade Delete](xref:core/saving/cascade-delete) for more details on how cascade delete behaviors work, how they can be configured explicitly and  how they are selected by convention.

In the following example, a cascade delete is configured on the relationship between `Blog` and `Post`, so the `post` entity is deleted from the database.

[!code-csharp[Main](../../../samples/core/Saving/RelatedData/Sample.cs#RemovingRelationships)]
