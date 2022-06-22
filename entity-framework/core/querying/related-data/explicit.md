---
title: Explicit Loading of Related Data - EF Core
description: Explicit loading of related data with Entity Framework Core
author: roji
ms.date: 9/8/2020
uid: core/querying/related-data/explicit
---
# Explicit Loading of Related Data

## Explicit loading

You can explicitly load a navigation property via the `DbContext.Entry(...)` API.

[!code-csharp[Main](../../../../samples/core/Querying/RelatedData/Program.cs#Explicit)]

You can also explicitly load a navigation property by executing a separate query that returns the related entities. If change tracking is enabled, then when a query materializes an entity, EF Core will automatically set the navigation properties of the newly-loaded entity to refer to any entities already loaded, and set the navigation properties of the already-loaded entities to refer to the newly loaded entity.

## Querying related entities

You can also get a LINQ query that represents the contents of a navigation property.

This allows you to apply other operators over the query. For example, applying an aggregate operator over the related entities without loading them into memory.

[!code-csharp[Main](../../../../samples/core/Querying/RelatedData/Program.cs#NavQueryAggregate)]

You can also filter which related entities are loaded into memory.

[!code-csharp[Main](../../../../samples/core/Querying/RelatedData/Program.cs#NavQueryFiltered)]
