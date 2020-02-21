---
title: Creating and configuring a model - EF Core
author: rowanmiller
ms.date: 11/05/2019
ms.assetid: 88253ff3-174e-485c-b3f8-768243d01ee1
uid: core/modeling/index
---
# Creating and configuring a model

Entity Framework uses a set of conventions to build a model based on the shape of your entity classes. You can specify additional configuration to supplement and/or override what was discovered by convention.

This article covers configuration that can be applied to a model targeting any data store and that which can be applied when targeting any relational database. Providers may also enable configuration that is specific to a particular data store. For documentation on provider specific configuration see the [Database Providers](../providers/index.md) section.

> [!TIP]  
> You can view this article’s [sample](https://github.com/dotnet/EntityFramework.Docs/tree/master/samples) on GitHub.

## Use fluent API to configure a model

You can override the `OnModelCreating` method in your derived context and use the `ModelBuilder API` to configure your model. This is the most powerful method of configuration and allows configuration to be specified without modifying your entity classes. Fluent API configuration has the highest precedence and will override conventions and data annotations.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/Required.cs?highlight=11-13)]

## Use data annotations to configure a model

You can also apply attributes (known as Data Annotations) to your classes and properties. Data annotations will override conventions, but will be overridden by Fluent API configuration.

[!code-csharp[Main](../../../samples/core/Modeling/DataAnnotations/Required.cs?highlight=14)]
