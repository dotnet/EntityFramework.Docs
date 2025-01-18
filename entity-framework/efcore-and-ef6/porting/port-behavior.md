---
title: Porting from EF6 to EF Core - Behavior changes
description: A non-exhaustive list of some changes in behavior between EF6 and EF Core.
author: SamMonoRT
ms.date: 12/09/2021
uid: efcore-and-ef6/porting/port-behavior
---
# Behavior Changes between EF6 and EF Core

This is a non-exhaustive list of changes in behavior between EF6 and EF Core. It is important to keep these in mind as your port your application as they may change the way your application behaves, but will not show up as compilation errors after swapping to EF Core.

This is meant as a high level review to consider as part of the porting process. For more detailed, case-by-case instructions, read the [detailed cases](xref:efcore-and-ef6/porting/port-detailed-cases).

## DbSet.Add/Attach and graph behavior

In EF6, calling `DbSet.Add()` on an entity results in a recursive search for all entities referenced in its navigation properties. Any entities that are found, and are not already tracked by the context, are also marked as added. `DbSet.Attach()` behaves the same, except all entities are marked as unchanged.

**EF Core performs a similar recursive search, but with some slightly different rules.**

- If the root entity is configured for a generated key and the key is not set, it will be put into the `Added` state.
- **For entities that are found during the recursive search of navigation properties:**
  - **If the primary key of the entity is store generated**
    - If the primary key is not set to a value, the state is set to added. The primary key value is considered "not set" if it is assigned the CLR default value for the property type (for example, `0` for `int`, `null` for `string`, etc.).
    - If the primary key is set to a value, the state is set to unchanged.
  - If the primary key is not database generated, the entity is put in the same state as the root.
- This change of behavior only applies to the `Attach` and `Update` method groups. `Add` always puts entities in the `Added` state, even if the key is set.
- `Attach` methods put entities with keys set into the `Unchanged` state. This facilitates "insert it if new, otherwise leave it alone." `Update` methods put entities with keys set into the `Modified` state. This facilitates "insert it if new, otherwise update it."

The general philosophy here is that `Update` is a very simple way to handle inserts and updates of disconnected entities. It ensures any new entities are inserted, and any existing entities are updated.

At the same time, `Add` still provides an easy way force entities to be inserted. Add is mostly useful only when not using store-generated keys, such that EF doesn't know if the entity is new or not.

For more information on these behaviors in EF Core, read [Change Tracking in EF Core](xref:core/change-tracking/index).

## Code First database initialization

**EF6 has a significant amount of magic it performs around selecting the database connection and initializing the database. Some of these rules include:**

- If no configuration is performed, EF6 will select a database on SQL Express or LocalDb.
- If a connection string with the same name as the context is in the application's `App/Web.config` file, this connection will be used.
- If the database does not exist, it is created.
- If none of the tables from the model exist in the database, the schema for the current model is added to the database. If migrations are enabled, then they are used to create the database.
- If the database exists and EF6 had previously created the schema, then the schema is checked for compatibility with the current model. An exception is thrown if the model has changed since the schema was created.

**EF Core does not perform any of this magic.**

- The database connection must be explicitly configured in code.
- No initialization is performed. You must use `DbContext.Database.Migrate()` to apply migrations (or `DbContext.Database.EnsureCreated()` and `EnsureDeleted()` to create/delete the database without using migrations).

## Code First table naming convention

EF6 runs the entity class name through a pluralization service to calculate the default table name that the entity is mapped to.

EF Core uses the name of the `DbSet` property that the entity is exposed in on the derived context. If the entity does not have a `DbSet` property, then the class name is used.

For more information, read [Managing Database Schemas](xref:core/managing-schemas/index).
