---
title: Create and Drop APIs - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/10/2017
---
# Create and Drop APIs

The EnsureCreated and EnsureDeleted methods provide a lightweight alternative to [Migrations](migrations/index.md) for managing the database schema. This is useful in scenarios when the data is transient and can be dropped when the schema changes. For example during prototyping, in tests, or for local caches.

The EnsureDeleted method will drop the database if it exists. If you don't have the appropiate permissions, an exception is thrown.

``` csharp
// Drop the database if it exists
dbContext.Database.EnsureDeleted();
```

The EnsureCreated method will create the database if it doesn't exist and initialize the database schema. If any tables exist (including tables for another DbContext class), the schema won't be initialized.

``` csharp
// Create the database if it doesn't exist
dbContext.Database.EnsureCreated();
```

> [!TIP]
> Async versions of these methods are also available.

## SQL Script

To get the SQL used by EnsureCreated, you can also use the GenerateCreateScript method.

``` csharp
var sql = dbContext.Database.GenerateCreateScript();
```

## Multiple DbContext classes

As state above, EnsureCreated only works when no other tables are present. If needed, you can write your own check to see if the schema needs to be initialized, and use the underlying IRelationalDatabaseCreator service to do it.

``` csharp
// TODO: Check whether the schema need to be initialized

// Initialize the schema for this DbContext
var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();
databaseCreator.CreateTables();
```
