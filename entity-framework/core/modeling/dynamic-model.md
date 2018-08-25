---
title: Alternating between multiple models with the same DbContext type - EF Core
author: AndriySvyryd
ms.date: 12/10/2017
ms.assetid: 3154BF3C-1749-4C60-8D51-AE86773AA116
uid: core/modeling/dynamic-model
---
# Alternating between multiple models with the same DbContext type

The model built in `OnModelCreating` could use a property on the context to change how the model is built. For example it could be used to exclude a certain property:

[!code-csharp[Main](../../../samples/core/DynamicModel/DynamicContext.cs?name=Class)]

## IModelCacheKeyFactory
However if you tried doing the above without additional changes you would get the same model every time a new context is created for any value of `IgnoreIntProperty`. This is caused by the model caching mechanism EF uses to improve the performance by only invoking `OnModelCreating` once and caching the model.

By default EF assumes that for any given context type the model will be the same. To accomplish this the default implementation of `IModelCacheKeyFactory` returns a key that just contains the context type. To change this you need to replace the `IModelCacheKeyFactory` service. The new implementation needs to return an object that can be compared to other model keys using the `Equals` method that takes into account all the variables that affect the model:

[!code-csharp[Main](../../../samples/core/DynamicModel/DynamicModelCacheKeyFactory.cs?name=Class)]
