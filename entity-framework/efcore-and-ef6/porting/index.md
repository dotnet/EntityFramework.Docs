---
title: Porting from EF6 to EF Core - EF
description: General information on porting an application from Entity Framework 6 to Entity Framework Core
author: ajcvickers
ms.date: 10/27/2016
uid: efcore-and-ef6/porting/index
---
# Porting from EF6 to EF Core

Because of the fundamental changes in EF Core we do not recommend attempting to move an EF6 application to EF Core unless you have a compelling reason to make the change.
You should view the move from EF6 to EF Core as a port rather than an upgrade.

> [!IMPORTANT]
> Before you start the porting process it is important to validate that EF Core meets the data access requirements for your application.

## Missing features

Make sure that EF Core has all the features you need to use in your application. See [Feature Comparison](xref:efcore-and-ef6/index) for a detailed comparison of how the feature set in EF Core compares to EF6. If any required features are missing, ensure that you can compensate for the lack of these features before porting to EF Core.

## Behavior changes

This is a non-exhaustive list of some changes in behavior between EF6 and EF Core. It is important to keep these in mind as your port your application as they may change the way your application behaves, but will not show up as compilation errors after swapping to EF Core.

### DbSet.Add/Attach and graph behavior

In EF6, calling `DbSet.Add()` on an entity results in a recursive search for all entities referenced in its navigation properties. Any entities that are found, and are not already tracked by the context, are also marked as added. `DbSet.Attach()` behaves the same, except all entities are marked as unchanged.

**EF Core performs a similar recursive search, but with some slightly different rules.**

* The root entity is always in the requested state (added for `DbSet.Add` and unchanged for `DbSet.Attach`).
* **For entities that are found during the recursive search of navigation properties:**
  * **If the primary key of the entity is store generated**
    * If the primary key is not set to a value, the state is set to added. The primary key value is considered "not set" if it is assigned the CLR default value for the property type (for example, `0` for `int`, `null` for `string`, etc.).
    * If the primary key is set to a value, the state is set to unchanged.
  * If the primary key is not database generated, the entity is put in the same state as the root.

### Code First database initialization

**EF6 has a significant amount of magic it performs around selecting the database connection and initializing the database. Some of these rules include:**

* If no configuration is performed, EF6 will select a database on SQL Express or LocalDb.

* If a connection string with the same name as the context is in the applications `App/Web.config` file, this connection will be used.

* If the database does not exist, it is created.

* If none of the tables from the model exist in the database, the schema for the current model is added to the database. If migrations are enabled, then they are used to create the database.

* If the database exists and EF6 had previously created the schema, then the schema is checked for compatibility with the current model. An exception is thrown if the model has changed since the schema was created.

**EF Core does not perform any of this magic.**

* The database connection must be explicitly configured in code.

* No initialization is performed. You must use `DbContext.Database.Migrate()` to apply migrations (or `DbContext.Database.EnsureCreated()` and `EnsureDeleted()` to create/delete the database without using migrations).

### Code First table naming convention

EF6 runs the entity class name through a pluralization service to calculate the default table name that the entity is mapped to.

EF Core uses the name of the `DbSet` property that the entity is exposed in on the derived context. If the entity does not have a `DbSet` property, then the class name is used.
