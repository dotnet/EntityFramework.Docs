---
title: Microsoft SQL Server Database Provider - Memory-Optimized Tables - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 2e007c82-c6e4-45bb-8129-851b79ec1a0a
uid: core/providers/sql-server/memory-optimized-tables
---

# Memory-Optimized Tables support in SQL Server EF Core Database Provider

> [!NOTE]  
>
> This feature was introduced in EF Core 1.1.

[Memory-Optimized Tables](https://docs.microsoft.com/sql/relational-databases/in-memory-oltp/memory-optimized-tables) are a feature of SQL Server where the entire table resides in memory. A second copy of the table data is maintained on disk, but only for durability purposes. Data in memory-optimized tables is only read from disk during database recovery. For example, after a server restart.

## Configuring a memory-optimized table

You can specify that the table an entity is mapped to is memory-optimized. When using EF Core to create and maintain a database based on your model (either with migrations or `Database.EnsureCreated()`), a memory-optimized table will be created for these entities.

``` csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .ForSqlServerIsMemoryOptimized();
}
```
