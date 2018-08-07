---
title: Data Seeding - EF Core
author: AndriySvyryd
ms.author: divega
ms.date: 02/23/2018
ms.assetid: 3154BF3C-1749-4C60-8D51-AE86773AA116
uid: core/modeling/data-seeding
---
# Data Seeding

> [!NOTE]  
> This feature is new in EF Core 2.1.

Data seeding allows to provide initial data to populate a database. Unlike in EF6, in EF Core, seeding data is associated with an entity type as part of the model configuration. Then EF Core [migrations](xref:core/managing-schemas/migrations/index) can automatically compute what insert, update or delete operations need to be applied when upgrading the database to a new version of the model.

As an example, you can use this to configure seed data for a `Blog` in `OnModelCreating`:

[!code-csharp[Main](../../../samples/core/DataSeeding/DataSeedingContext.cs?name=BlogSeed)]

To add entities that have a relationship the foreign key values need to be specified. Frequently the foreign key properties are in shadow state, so to be able to set the values an anonymous class should be used:

[!code-csharp[Main](../../../samples/core/DataSeeding/DataSeedingContext.cs?name=PostSeed)]

Once entities have been added, it is recommended to use [migrations](xref:core/managing-schemas/migrations/index) to apply changes. 

Alternatively, you can use `context.Database.EnsureCreated()` to create a new database containing the seed data, for example for a test database or when using the in-memory provider. Note that if the database already exists, `EnsureCreated()` will neither update the schema nor the seed data in the database.
