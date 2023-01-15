---
title: Alternating between multiple models with the same DbContext type - EF Core
description: Alternating between multiple models with the same DbContext type using Entity Framework Core
author: AndriySvyryd
ms.date: 01/03/2020
uid: core/modeling/dynamic-model
---
# Alternating between multiple models with the same DbContext type

The model built in `OnModelCreating` can use a property on the context to change how the model is built. For example, suppose you wanted to configure an entity differently based on some property:

[!code-csharp[Main](../../../samples/core/Modeling/DynamicModel/DynamicContext.cs?name=OnModelCreating)]

Unfortunately, this code wouldn't work as-is, since EF builds the model and runs `OnModelCreating` only once, caching the result for performance reasons. However, you can hook into the model caching mechanism to make EF aware of the property producing different models.

## IModelCacheKeyFactory

EF uses the `IModelCacheKeyFactory` to generate cache keys for models; by default, EF assumes that for any given context type the model will be the same, so the default implementation of this service returns a key that just contains the context type. To produce different models from the same context type, you need to replace the `IModelCacheKeyFactory` service with the correct implementation; the generated key will be compared to other model keys using the `Equals` method, taking into account all the variables that affect the model.

The following implementation takes the `UseIntProperty` into account when producing a model cache key:

[!code-csharp[Main](../../../samples/core/Modeling/DynamicModel/DynamicModelCacheKeyFactory.cs?name=DynamicModel)]

You also have to implement the overload of the Create method that also handles design-time model caching. As in the following example:

[!code-csharp[Main](../../../samples/core/Modeling/DynamicModel/DynamicModelCacheKeyFactoryDesignTimeSupport.cs?name=DynamicModelDesignTimeSupport)]

Finally, register your new `IModelCacheKeyFactory` in your context's `OnConfiguring`:

[!code-csharp[Main](../../../samples/core/Modeling/DynamicModel/DynamicContext.cs?name=OnConfiguring)]

See the [full sample project](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/DynamicModel) for more context.
