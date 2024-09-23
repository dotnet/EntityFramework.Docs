---
title: Custom Migrations Operations - EF Core
description: Managing custom and raw SQL migrations for database schema management with Entity Framework Core
author: SamMonoRT
ms.date: 10/27/2020
uid: core/managing-schemas/migrations/operations
---
# Custom Migrations Operations

The MigrationBuilder API allows you to perform many different kinds of operations during a migration, but it's far from exhaustive. However, the API is also extensible allowing you to define your own operations. There are two ways to extend the API: Using the `Sql()` method, or by defining custom `MigrationOperation` objects.

To illustrate, let's look at implementing an operation that creates a database user using each approach. In our migrations, we want to enable writing the following code:

```csharp
migrationBuilder.CreateUser("SQLUser1", "Password");
```

## Using MigrationBuilder.Sql()

The easiest way to implement a custom operation is to define an extension method that calls `MigrationBuilder.Sql()`. Here is an example that generates the appropriate Transact-SQL.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperationSql.cs#snippet_CustomOperationSql)]

> [!TIP]
> Use the `EXEC` function when a statement must be the first or only one in a SQL batch. It might also be needed to work around parser errors in idempotent migration scripts that can occur when referenced columns don't currently exist on a table.

If your migrations need to support multiple database providers, you can use the `MigrationBuilder.ActiveProvider` property. Here's an example supporting both Microsoft SQL Server and PostgreSQL.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperationMultiSql.cs#snippet_CustomOperationMultiSql)]

This approach only works if you know every provider where your custom operation will be applied.

## Using a MigrationOperation

To decouple the custom operation from the SQL, you can define your own `MigrationOperation` to represent it. The operation is then passed to the provider so it can determine the appropriate SQL to generate.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperation.cs#snippet_CreateUserOperation)]

With this approach, the extension method just needs to add one of these operations to `MigrationBuilder.Operations`.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperation.cs#snippet_MigrationBuilderExtension)]

This approach requires each provider to know how to generate SQL for this operation in their `IMigrationsSqlGenerator` service. Here is an example overriding the SQL Server's generator to handle the new operation.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperation.cs#snippet_MigrationsSqlGenerator)]

Replace the default migrations sql generator service with the updated one.

[!code-csharp[](../../../../samples/core/Schemas/Migrations/CustomOperation.cs#snippet_OnConfiguring)]
