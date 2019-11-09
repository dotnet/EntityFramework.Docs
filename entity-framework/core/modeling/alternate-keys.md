---
title: Alternate Keys - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 8a5931d4-b480-4298-af36-0e29d74a37c0
uid: core/modeling/alternate-keys
---
# Alternate Keys

An alternate key serves as an alternate unique identifier for each entity instance in addition to the primary key. Alternate keys can be used as the target of a relationship. When using a relational database this maps to the concept of a unique index/constraint on the alternate key column(s) and one or more foreign key constraints that reference the column(s).

> [!TIP]  
> If you just want to enforce uniqueness of a column then you want a unique index rather than an alternate key, see [Indexes](indexes.md). In EF, alternate keys provide greater functionality than unique indexes because they can be used as the target of a foreign key.

Alternate keys are typically introduced for you when needed and you do not need to manually configure them. See [Conventions](#conventions) for more details.

## Conventions

By convention, an alternate key is introduced for you when you identify a property, that is not the primary key, as the target of a relationship.

[!code-csharp[Main](../../../samples/core/Modeling/Conventions/AlternateKey.cs?name=AlternateKey&highlight=12)]

## Data Annotations

Alternate keys can not be configured using Data Annotations.

## Fluent API

You can use the Fluent API to configure a single property to be an alternate key.

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/AlternateKeySingle.cs?name=AlternateKeySingle&highlight=7,8)]

You can also use the Fluent API to configure multiple properties to be an alternate key (known as a composite alternate key).

[!code-csharp[Main](../../../samples/core/Modeling/FluentAPI/AlternateKeyComposite.cs?name=AlternateKeyComposite&highlight=7,8)]
