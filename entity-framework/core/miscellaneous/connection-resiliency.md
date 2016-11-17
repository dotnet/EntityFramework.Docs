---
title: Connection Resiliency | Microsoft Docs
author: rowanmiller
ms.author: rowmil

ms.date: 11/15/2016

ms.assetid: e079d4af-c455-4a14-8e15-a8471516d748
ms.technology: entity-framework-core
 
uid: core/miscellaneous/connection-resiliency
---

# Connection Resiliency

> [!NOTE]
> This documentation is for EF Core. For EF6.x, see [Entity Framework 6](../../ef6/index.md).

Connection resiliency automatically retries failed database commands. It is specific to relational databases.

> [!NOTE]
> Connection resiliency was introduced in EF Core 1.1.0. If you are using an earlier release, the functionality shown in this article will not be available.

## SQL Server

The SQL Server provider includes an execution strategy that is specifically tailored to SQL Server (including SQL Azure). It is aware of the exception types that can be retried and has sensible defaults for maximum retries, delay between retries, etc.

An execution strategy is specified when configuring the options for your context. This is typically in the `OnConfiguring` method of your derived context, or in `Startup.cs` for an ASP.NET Core application.

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlServer(
            "<connection string>",
            options => options.EnableRetryOnFailure());
}
```

## Custom execution strategy

There is a mechanism to register a custom execution strategy of your own.

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseMyProvider(
            "<connection string>",
            options => options.ExecutionStrategy(...));
}
```
