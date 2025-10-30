---
title: Tracking vs. No-Tracking Queries - EF Core
description: Information on tracking and no-tracking queries in Entity Framework Core
author: smitpatel
ms.date: 3/15/2023
uid: core/querying/tracking
---
# Tracking vs. No-Tracking Queries

Tracking behavior controls if Entity Framework Core keeps information about an entity instance in its change tracker. If an entity is tracked, any changes detected in the entity are persisted to the database during `SaveChanges`. EF Core also fixes up navigation properties between the entities in a tracking query result and the entities that are in the change tracker.

> [!NOTE]
> [Keyless entity types](xref:core/modeling/keyless-entity-types) are never tracked. Wherever this article mentions entity types, it refers to entity types which have a key defined.

> [!TIP]
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Querying/Tracking) on GitHub.

## Tracking queries

By default, queries that return entity types are tracking. A tracking query means any changes to entity instances are persisted by `SaveChanges`. In the following example, the change to the blogs rating is detected and persisted to the database during `SaveChanges`:

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#Tracking)]

When the results are returned in a tracking query, EF Core checks if the entity is already in the context. If EF Core finds an existing entity, then the same instance is returned, which can potentially use less memory and be faster than a no-tracking query. EF Core doesn't overwrite current and original values of the entity's properties in the entry with the database values. If the entity isn't found in the context, EF Core creates a new entity instance and attaches it to the context. Query results don't contain any entity which is added to the context but not yet saved to the database.

## No-tracking queries

No-tracking queries are useful when the results are used in a read-only scenario. They're generally quicker to execute because there's no need to set up the change tracking information. If the entities retrieved from the database don't need to be updated, then a no-tracking query should be used. An individual query can be set to be no-tracking. A no-tracking query also give results based on what's in the database disregarding any local changes or added entities.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#NoTracking)]

The default tracking behavior can be changed at the context instance level:

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#ContextDefaultTrackingBehavior)]

The next section explains when a no-tracking query might be less efficient than a tracking query.

## Identity resolution

Since a tracking query uses the change tracker, EF Core does identity resolution in a tracking query. When materializing an entity, EF Core returns the same entity instance from the change tracker if it's already being tracked. If the result contains the same entity multiple times, the same instance is returned for each occurrence. No-tracking queries:

* Don't use the change tracker and don't do identity resolution.
* Return a new instance of the entity even when the same entity is contained in the result multiple times.

Tracking and no-tracking can be combined in the same query. That is, you can have a no-tracking query, which does identity resolution in the results. Just like <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking*> queryable operator, we've added another operator <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution%60`1(System.Linq.IQueryable{%60%600})>. There's also associated entry added in the <xref:Microsoft.EntityFrameworkCore.QueryTrackingBehavior> enum. When the query to use identity resolution is configured with no tracking, a stand-alone change tracker is used in the background when generating query results so each instance is materialized only once. Since this change tracker is different from the one in the context, the results are not tracked by the context. After the query is enumerated fully, the change tracker goes out of scope and garbage collected as required.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#NoTrackingWithIdentityResolution)]

## Configuring the default tracking behavior

If you find yourself changing the tracking behavior for many queries, you may want to change the default instead:

[!code-csharp[Main](../../../samples/core/Querying/Tracking/NonTrackingBloggingContext.cs?name=OnConfiguring&highlight=5)]

This makes all your queries no-tracking by default. You can still add <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsTracking*> to make specific queries tracking.

## Tracking and custom projections

Even if the result type of the query isn't an entity type, EF Core will still track entity types contained in the result by default. In the following query, which returns an anonymous type, the instances of `Blog` in the result set will be tracked.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#CustomProjection1)]

If the result set contains entity types coming out from LINQ composition, EF Core will track them.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#CustomProjection2)]

If the result set doesn't contain any entity types, then no tracking is done. In the following query, we return an anonymous type with some of the values from the entity (but no instances of the actual entity type). There are no tracked entities coming out of the query.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#CustomProjection3)]

 EF Core supports doing client evaluation in the top-level projection. If EF Core materializes an entity instance for client evaluation, it will be tracked. Here, since we're passing `blog` entities to the client method `StandardizeURL`, EF Core will track the blog instances too.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#ClientProjection)]

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#ClientMethod)]

EF Core doesn't track the keyless entity instances contained in the result. But EF Core tracks all the other instances of entity types with a key according to rules above.

## Previous versions

Before version 3.0, EF Core had some differences in how tracking was done. Notable differences are as follows:

* As explained in the [Client vs Server Evaluation](xref:core/querying/client-eval) page, EF Core supported client evaluation in any part of the query before version 3.0. Client evaluation caused materialization of entities, which weren't part of the result. So EF Core analyzed the result to detect what to track. This design had certain differences as follows:
  * Client evaluation in the projection, which caused materialization but didn't return the materialized entity instance wasn't tracked. The following example didn't track `blog` entities.
    [!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#ClientProjection)]

  * EF Core didn't track the objects coming out of LINQ composition in certain cases. The following example didn't track `Post`.
    [!code-csharp[Main](../../../samples/core/Querying/Tracking/Program.cs#CustomProjection2)]

* Whenever query results contained keyless entity types, the whole query was made non-tracking. That means that entity types with keys, which are in the result weren't being tracked either.
* EF Core used to do identity resolution in no-tracking queries. It used weak references to keep track of entities that had already been returned. So if a result set contained the same entity multiples times, you would get the same instance for each occurrence. Though if a previous result with the same identity went out of scope and got garbage collected, EF Core returned a new instance.
