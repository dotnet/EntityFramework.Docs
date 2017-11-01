---
title: Managing Database Schemas - EF Core
author: bricelam
ms.author: divega
ms.date: 10/30/2017
ms.technology: entity-framework-core
---
# Managing Database Schemas
EF Core provides two primary ways of keeping your EF Core model and database schema in sync. To choose between the two,
you need to decide whether your EF Core model or the database schema is the source of truth.

If you want your EF Core model to be the source of truth, use [Migrations][1]. As you make changes to your EF Core
model, this approach will incrementally apply the corresponding schema changes to your database so that it remains
compatible with your EF Core model.


Use [Reverse Engineering][2] if you want your database schema to be the source of truth. This approach allows you to
scaffold a DbContext and the entity type classes by reverse engineering your database schema into an EF Core model.


  [1]: migrations.md
  [2]: scaffolding.md
