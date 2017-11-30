---
title: Disconnected Entities - EF Core
author: ajcvickers
ms.author: avickers

ms.date: 10/27/2016

ms.assetid: 2533b195-d357-4056-b0e0-8698971bc3b0
ms.technology: entity-framework-core

uid: core/saving/disconnected-entities
---
# Disconnected entities

A DbContext instance will automatically track entities returned from the database. Changes made to these entities will then be detected when SaveChanges is called and the database will be updated as needed. See [Basic Save](basic.md) and [Related Data](related-data.md) for details.

However, sometimes entities are queried using one context instance and then saved using a different instance. This often happens in "disconnected" scenarios such as a web application where the entities are queried, sent to the client, modified, sent back to the server in a request, and then saved. In this case, the second context instance needs to know whether the entities are new (should be inserted) or existing (should be updated).

> [!TIP]  
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Saving/Saving/Disconnected/) on GitHub.

## Identifying new entities

### Client identifies new entities

The simplest case to deal with is when the client informs the server whether the entity is new or existing. For example, often the request to insert a new entity is different from the request to update an existing entity.

The remainder of this section covers the cases where it necessary to determine in some other way whether to insert or update.

### With auto-generated keys

The value of an automatically generated key can often be used to determine whether an entity needs to be inserted or updated. If the key has not been set (i.e. it still has the CLR default value of null, zero, etc.), then the entity must be new and needs inserting. On the other hand, if the key value has been set, then it must have already been previously saved and now needs updating. In other words, if the key has a value, then entity was queried, sent to the client, and has now come back to be updated.

It is easy to check for an unset key when the entity type is known:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#IsItNewSimple)]

However, EF also has a built-in way to do this for any entity type and key type:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#IsItNewGeneral)]

> [!TIP]  
> Keys are set as soon as entities are tracked by the context, even if the entity is in the Added state. This helps when traversing a graph of entities and deciding what to do with each, such as when using the TrackGraph API. The key value should only be used in the way shown here _before_ any call is made to track the entity.

### With other keys

Some other mechanism is needed to identity new entities when key values are not generated automatically. There are two general approaches to this:
 * Query for the entity
 * Pass a flag from the client

To query for the entity, just use the Find method:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#IsItNewQuery)]

It is beyond the scope of this document to show the full code for passing a flag from a client. In a web app, it usually means making different requests for different actions, or passing some state in the request then extracting it in the controller.

## Saving single entities

If it is known whether or not an insert or update is needed, then either Add or Update can be used appropriately:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertAndUpdateSingleEntity)]

However, if the entity uses auto-generated key values, then the Update method can be used for both cases:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertOrUpdateSingleEntity)]

The Update method normally marks the entity for update, not insert. However, if the entity has a auto-generated key, and no key value has been set, then the entity is instead automatically marked for insert.

> [!TIP]  
> This behavior was introduced in EF Core 2.0. For earlier releases it is always necessary to explicitly choose either Add or Update.

If the entity is not using auto-generated keys, then the application must decide whether the entity should be inserted or updated: For example:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertOrUpdateSingleEntityWithFind)]

The steps here are:
* If Find returns null, then the database doesn't already contain the blog with this ID, so we call Add mark it for insertion.
* If Find returns an entity, then it exists in the database and the context is now tracking the existing entity
  * We then use SetValues to set the values for all properties on this entity to those that came from the client.
  * The SetValues call will mark the entity to be updated as needed.

> [!TIP]  
> SetValues will only mark as modified the properties that have different values to those in the tracked entity. This means that when the update is sent, only those columns that have actually changed will be updated. (And if nothing has changed, then no update will be sent at all.)

## Working with graphs

### All new/all existing entities

An example of working with graphs is inserting or updating a blog together with its collection of associated posts. If all the entities in the graph should be inserted, or all should be updated, then the process is the same as described above for single entities. For example, a graph of blogs and posts created like this:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#CreateBlogAndPosts)]

can be inserted like this:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertGraph)]

The call to Add will mark the blog and all the posts to be inserted.

Likewise, if all the entities in a graph need to be updated, then Update can be used:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#UpdateGraph)]

The blog and all its posts will be marked to be updated.

### Mix of new and existing entities

With auto-generated keys, Update can again be used for both inserts and updates, even if the graph contains a mix of entities that require inserting and those that require updating:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertOrUpdateGraph)]

Update will mark any entity in the graph, blog or post, for insertion if it does not have a key value set, while all other entities are marked for update.

As before, when not using auto-generated keys, a query and some processing can be used:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertOrUpdateGraphWithFind)]

## Handling deletes

Delete can be tricky to handle since often the absence of an entity means that it should be deleted. One way to deal with this is to use "soft deletes" such that the entity is marked as deleted rather than actually being deleted. Deletes then becomes the same as updates. Soft deletes can be implemented in using [query filters](xref:core/querying/filters).

For true deletes, a common pattern is to use an extension of the query pattern to perform what is essentially a graph diff. For example:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#InsertUpdateOrDeleteGraphWithFind)]

## TrackGraph

Internally, Add, Attach, and Update use graph-traversal with a determination made for each entity as to whether it should be marked as Added (to insert), Modified (to update), Unchanged (do nothing), or Deleted (to delete). This mechanism is exposed via the TrackGraph API. For example, let's assume that when the client sends back a graph of entities it sets some flag on each entity indicating how it should be handled. TrackGraph can then be used to process this flag:

[!code-csharp[Main](../../../samples/core/Saving/Saving/Disconnected/Sample.cs#TrackGraph)]

The flags are only shown as part of the entity for simplicity of the example. Typically the flags would be part of a DTO or some other state included in the request.
