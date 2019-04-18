---
title: Including & Excluding Properties - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: e9dff604-3469-4a05-8f9e-18ac281d82a9
uid: core/modeling/included-properties
---
# Including & Excluding Properties

Including a property in the model means that EF has metadata about that property and will attempt to read and write values from/to the database.

## Conventions

By convention, public properties with a getter and a setter will be included in the model.

## Data Annotations

You can use Data Annotations to exclude a property from the model.

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Samples/IgnoreProperty.cs?highlight=17)]

## Fluent API

You can use the Fluent API to exclude a property from the model.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Samples/IgnoreProperty.cs?highlight=12,13)]
