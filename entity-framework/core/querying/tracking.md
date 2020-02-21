---
title: Tracking vs. No-Tracking Queries - EF Core
author: smitpatel
ms.date: 10/10/2019
ms.assetid: e17e060c-929f-4180-8883-40c438fbcc01
uid: core/querying/tracking
---
# Tracking vs. No-Tracking Queries

Tracking behavior controls if Entity Framework Core will keep information about an entity instance in its change tracker. If an entity is tracked, any changes detected in the entity will be persisted to the database during `SaveChanges()`. EF Core will also fix up navigation properties between the entities in a tracking query result and the entities that are in the change tracker.

> [!NOTE]
> [Keyless entity types](xref:core/modeling/keyless-entity-types) are never tracked. Wherever this article mentions entity types, it refers to entity types which have a key defined.

> [!TIP]  
> You can view this article's [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

## Tracking queries

By default, queries that return entity types are tracking. Which means you can make changes to those entity instances and have those changes persisted by `SaveChanges()`. In the following example, the change to the blogs rating will be detected and persisted to the database during `SaveChanges()`.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#Tracking)]

## No-tracking queries

No tracking queries are useful when the results are used in a read-only scenario. They're quicker to execute because there's no need to set up the change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. You can swap an individual query to be no-tracking.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#NoTracking)]

You can also change the default tracking behavior at the context instance level:

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#ContextDefaultTrackingBehavior)]

## Identity resolution

Since a tracking query uses the change tracker, EF Core will do identity resolution in a tracking query. When materializing an entity, EF Core will return the same entity instance from the change tracker if it's already being tracked. If the result contains same entity multiple times, you get back same instance for each occurrence. No-tracking queries don't use the change tracker and don't do identity resolution. So you get back new instance of entity even when the same entity is contained in the result multiple times. This behavior was different in versions before EF Core 3.0, see [previous versions](#previous-versions).

## Tracking and custom projections

Even if the result type of the query isn't an entity type, EF Core will still track entity types contained in the result by default. In the following query, which returns an anonymous type, the instances of `Blog` in the result set will be tracked.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#CustomProjection1)]

If the result set contains entity types coming out from LINQ composition, EF Core will track them.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#CustomProjection2)]

If the result set doesn't contain any entity types, then no tracking is done. In the following query, we return an anonymous type with some of the values from the entity (but no instances of the actual entity type). There are no tracked entities coming out of the query.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#CustomProjection3)]

 EF Core supports doing client evaluation in the top-level projection. If EF Core materializes an entity instance for client evaluation, it will be tracked. Here, since we're passing `blog` entities to the client method `StandardizeURL`, EF Core will track the blog instances too.

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#ClientProjection)]

[!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#ClientMethod)]

EF Core doesn't track the keyless entity instances contained in the result. But EF Core tracks all the other instances of entity types with key according to rules above.

Some of the above rules worked differently before EF Core 3.0. For more information, see [previous versions](#previous-versions).

## Previous versions

Before version 3.0, EF Core had some differences in how tracking was done. Notable differences are as follows:

- As explained in [Client vs Server Evaluation](xref:core/querying/client-eval) page, EF Core supported client evaluation in any part of the query before version 3.0. Client evaluation caused materialization of entities, which weren't part of the result. So EF Core analyzed the result to detect what to track. This design had certain differences as follows:
  - Client evaluation in the projection, which caused materialization but didn't return the materialized entity instance wasn't tracked. The following example didn't track `blog` entities.
    [!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#ClientProjection)]

  - EF Core didn't track the objects coming out of LINQ composition in certain cases. The following example didn't track `Post`.
    [!code-csharp[Main](../../../samples/core/Querying/Tracking/Sample.cs#CustomProjection2)]

- Whenever query results contained keyless entity types, the whole query was made non-tracking. That means that entity types with keys, which are in result weren't being tracked either.
- EF Core did identity resolution in no-tracking query. It used weak references to keep track of entities that had already been returned. So if a result set contained the same entity multiples times, you would get the same instance for each occurrence. Though if a previous result with the same identity went out of scope and got garbage collected, EF Core returned a new instance.
