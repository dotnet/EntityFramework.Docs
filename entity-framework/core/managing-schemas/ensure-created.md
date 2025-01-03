---
title: Create and Drop APIs - EF Core
description: APIs for creating and dropping databases with Entity Framework Core
author: SamMonoRT
ms.date: 10/21/2021
no-loc: [EnsureDeleted, EnsureCreated]
uid: core/managing-schemas/ensure-created
---
# Create and Drop APIs

The <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreatedAsync*> and <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureDeletedAsync*> methods provide a lightweight alternative to [Migrations](xref:core/managing-schemas/migrations/index) for managing the database schema. These methods are useful in scenarios when the data is transient and can be dropped when the schema changes. For example during prototyping, in tests, or for local caches.

Some providers (especially non-relational ones) don't support Migrations. For these providers, `EnsureCreatedAsync` is often the easiest way to initialize the database schema.

> [!WARNING]
> `EnsureCreatedAsync` and Migrations don't work well together. If you're using Migrations, don't use `EnsureCreatedAsync` to initialize the schema.

Transitioning from `EnsureCreatedAsync` to Migrations is not a seamless experience. The simplest way to do it is to drop the database and re-create it using Migrations. If you anticipate using migrations in the future, it's best to just start with Migrations instead of using `EnsureCreatedAsync`.

## EnsureDeletedAsync

The `EnsureDeletedAsync` method will drop the database if it exists. If you don't have the appropriate permissions, an exception is thrown.

```csharp
// Drop the database if it exists
await dbContext.Database.EnsureDeletedAsync();
```

## EnsureCreatedAsync

`EnsureCreatedAsync` will create the database if it doesn't exist and initialize the database schema. If any tables exist (including tables for another `DbContext` class), the schema won't be initialized.

```csharp
// Create the database if it doesn't exist
dbContext.Database.EnsureCreatedAsync();
```

> [!TIP]
> Async versions of these methods are also available.

## SQL Script

To get the SQL used by `EnsureCreatedAsync`, you can use the <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.GenerateCreateScript*> method.

```csharp
var sql = dbContext.Database.GenerateCreateScript();
```

## Multiple DbContext classes

EnsureCreated only works when no tables are present in the database. If needed, you can write your own check to see if the schema needs to be initialized, and use the underlying IRelationalDatabaseCreator service to initialize the schema.

```csharp
// TODO: Check whether the schema needs to be initialized

// Initialize the schema for this DbContext
var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();
databaseCreator.CreateTables();
```
