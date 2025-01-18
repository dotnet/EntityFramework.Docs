---
title: What is new in EF Core 1.1 - EF Core
description: Changes and improvements in Entity Framework Core 1.1
author: SamMonoRT
ms.date: 10/27/2016
uid: core/what-is-new/ef-core-1.1
---
# New features in EF Core 1.1

## Modeling

### Field mapping

Allows you to configure a backing field for a property. This can be useful for read-only properties, or data that has Get/Set methods rather than a property.

### Mapping to Memory-Optimized Tables in SQL Server

You can specify that the table an entity is mapped to is memory-optimized. When using EF Core to create and maintain a database based on your model (either with migrations or `Database.EnsureCreated()`), a memory-optimized table will be created for these entities.

## Change tracking

### Additional change tracking APIs from EF6

Such as `Reload`, `GetModifiedProperties`, `GetDatabaseValues` etc.

## Query

### Explicit Loading

Allows you to trigger population of a navigation property on an entity that was previously loaded from the database.

### DbSet.Find

Provides an easy way to fetch an entity based on its primary key value.

## Other

### Connection resiliency

Automatically retries failed database commands. This is especially useful when connection to SQL Azure, where transient failures are common.

### Simplified service replacement

Makes it easier to replace internal services that EF uses.
