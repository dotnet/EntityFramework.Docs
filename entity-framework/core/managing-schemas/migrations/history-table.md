---
title: Custom Migrations History Table - EF Core
description: Customizing a history table to use for migrations with Entity Framework Core
author: SamMonoRT
ms.date: 11/07/2017
uid: core/managing-schemas/migrations/history-table
---
# Custom Migrations History Table

By default, EF Core keeps track of which migrations have been applied to the database by recording them in a table named
`__EFMigrationsHistory`. For various reasons, you may want to customize this table to better suit your needs.

> [!IMPORTANT]
> If you customize the Migrations history table *after* applying migrations, you are responsible for updating the
> existing table in the database.

## Schema and table name

You can change the schema and table name using the `MigrationsHistoryTable()` method in `OnConfiguring()` (or
`ConfigureServices()` on ASP.NET Core). Here is an example using the SQL Server EF Core provider.

[!code-csharp[Main](../../../../samples/core/Schemas/Migrations/MigrationTableNameContext.cs#TableNameContext)]

## Other changes

To configure additional aspects of the table, override and replace the provider-specific
`IHistoryRepository` service. Here is an example of changing the MigrationId column name to *Id* on SQL Server.

[!code-csharp[Main](../../../../samples/core/Schemas/Migrations/MyHistoryRepository.cs#HistoryRepositoryContext)]

> [!WARNING]
> `SqlServerHistoryRepository` is inside an internal namespace and may change in future releases.

[!code-csharp[Main](../../../../samples/core/Schemas/Migrations/MyHistoryRepository.cs#HistoryRepository)]
