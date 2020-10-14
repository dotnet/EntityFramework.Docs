---
title: Microsoft SQL Server Database Provider - Memory-Optimized Tables - EF Core
description: How to use Memory-Optimized Tables with the SQL Server Entity Framework Core Database Provider
author: AndriySvyryd
ms.date: 11/05/2019
uid: core/providers/sql-server/memory-optimized-tables
---
# Memory-Optimized Tables support in SQL Server EF Core Database Provider

[Memory-Optimized Tables](/sql/relational-databases/in-memory-oltp/memory-optimized-tables) are a feature of SQL Server where the entire table resides in memory. A second copy of the table data is maintained on disk, but only for durability purposes. Data in memory-optimized tables is only read from disk during database recovery. For example, after a server restart.

## Configuring a memory-optimized table

You can specify that the table an entity is mapped to is memory-optimized. When using EF Core to create and maintain a database based on your model (either with [migrations](xref:core/managing-schemas/migrations/index) or [EnsureCreated](/dotnet/api/Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreated)), a memory-optimized table will be created for these entities.

[!code-csharp[IsMemoryOptimized](../../../../samples/core/SqlServer/InMemory/InMemoryContext.cs?name=IsMemoryOptimized)]
