---
title: Managing Migrations - EF Core
description: Adding, removing and otherwise managing database schema migrations with Entity Framework Core
author: SamMonoRT
ms.date: 08/05/2026
uid: core/managing-schemas/migrations/managing
ms.custom: sfi-ropc-nochange
---
# Managing Migrations

As your model changes, migrations are added and removed as part of normal development, and the migration files are checked into your project's source control. To manage migrations, you must first install the [EF Core command-line tools](xref:core/cli/index).

> [!TIP]
> If the `DbContext` is in a different assembly than the startup project, you can explicitly specify the target and startup projects in either the [Package Manager Console tools](xref:core/cli/powershell#target-and-startup-project) or the [.NET CLI tools](xref:core/cli/dotnet#target-project-and-startup-project).

## Add a migration

After your model has been changed, you can add a migration for that change:

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add AddBlogCreatedTimestamp
```

### [Visual Studio](#tab/vs)

```powershell
Add-Migration AddBlogCreatedTimestamp
```

***

The migration name can be used like a commit message in a version control system. For example, you might choose a name like *AddBlogCreatedTimestamp* if the change is a new `CreatedTimestamp` property on your `Blog` entity.

Three files are added to your project under the **Migrations** directory:

* **XXXXXXXXXXXXXX_AddBlogCreatedTimestamp.cs**--The main migrations file. Contains the operations necessary to apply the migration (in `Up`) and to revert it (in `Down`).
* **XXXXXXXXXXXXXX_AddBlogCreatedTimestamp.Designer.cs**--The migrations metadata file. Contains information used by EF.
* **MyContextModelSnapshot.cs**--A snapshot of your current model. Used to determine what changed when adding the next migration.

The timestamp in the filename helps keep them ordered chronologically so you can see the progression of changes.

### Namespaces

You are free to move Migrations files and change their namespace manually. New migrations are created as siblings of the last migration. Alternatively, you can specify the directory at generation time as follows:

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add InitialCreate --output-dir Your/Directory
```

> [!NOTE]
> You can also change the namespace independently of the directory using `--namespace`.

#### [Visual Studio](#tab/vs)

```powershell
Add-Migration InitialCreate -OutputDir Your\Directory
```

> [!NOTE]
> You can also change the namespace independently of the directory using `-Namespace`.

***

## Create and apply a migration in one step

> [!NOTE]
> This feature was added in EF Core 11.

The `dotnet ef database update` command supports creating and applying a migration in a single step using the `--add` option. This uses Roslyn to compile the migration at runtime, enabling scenarios like .NET Aspire and containerized applications where the application cannot be stopped and rebuilt:

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update InitialCreate --add
```

The same options available for `dotnet ef migrations add` can be used:

```dotnetcli
dotnet ef database update AddProducts --add --output-dir Migrations/Products --namespace MyApp.Migrations
```

### [Visual Studio](#tab/vs)

```powershell
Update-Database -Migration InitialCreate -Add
```

***

This command scaffolds a new migration with the specified name, compiles it using Roslyn, and immediately applies it to the database. The migration files are still saved to disk for source control and future recompilation.

If no pending model changes are detected, the command applies any existing pending migrations without creating a new one.

## Customize migration code

While EF Core generally creates accurate migrations, you should always review the code and make sure it corresponds to the desired change; in some cases, it is even necessary to do so.

### Column renames

One notable example where customizing migrations is required is when renaming a property. For example, if you rename a property from `Name` to `FullName`, EF Core will generate the following migration:

```csharp
migrationBuilder.DropColumn(
    name: "Name",
    table: "Customers");

migrationBuilder.AddColumn<string>(
    name: "FullName",
    table: "Customers",
    nullable: true);
```

EF Core is generally unable to know when the intention is to drop a column and create a new one (two separate changes), and when a column should be renamed. If the above migration is applied as-is, all your customer names will be lost. To rename a column, replace the above generated migration with the following:

```csharp
migrationBuilder.RenameColumn(
    name: "Name",
    table: "Customers",
    newName: "FullName");
```

> [!TIP]
> The migration scaffolding process warns when an operation might result in data loss (like dropping a column). If you see that warning, be especially sure to review the migrations code for accuracy.

### Data operations

Migrations can move data as well as change the schema. Choose the operation based on whether the values are known when the migration is written:

* Use `InsertData`, `UpdateData`, and `DeleteData` for fixed values and rows identified by explicit keys. EF Core translates these operations into provider-specific SQL, so they also work when generating scripts and bundles.
* Use `Sql` when the new values must be calculated from existing database data. SQL syntax can differ by provider; branch on `MigrationBuilder.ActiveProvider` when necessary.
* Define a [custom migration operation](xref:core/managing-schemas/migrations/operations) when a reusable operation needs provider-specific SQL generation.

Don't use the current `DbContext` or entity CLR types to move data in a migration. Historical migrations must continue to compile and behave the same after those types are changed or removed.

#### Transform existing data

When replacing columns, preserve the source data until the destination has been populated:

1. Add the destination column as nullable.
2. Populate it from the existing columns.
3. Make the destination column required, if appropriate.
4. Drop the source columns.

The following migration implements that sequence for SQL Server and SQLite:

[!code-csharp[](../../../../samples/core/Schemas/Migrations/DataOperations.cs#snippet_RawSqlDataMigration)]

Add a branch for every provider the application supports. Throwing for an unknown provider is safer than silently applying an incomplete migration. Don't build SQL from untrusted values; migration SQL is executed with schema-changing privileges.

Some transformations cannot be reversed without losing information. Implement `Down` only when the original values can be reconstructed safely. Otherwise, fail explicitly and require restoring the data from a backup as part of the rollback procedure.

#### Insert fixed data

Use `InsertData` when the keys and values are known when the migration is written:

[!code-csharp[](../../../../samples/core/Schemas/Migrations/DataOperations.cs#snippet_InsertData)]

The corresponding `Down` method should call `DeleteData` with the same keys.

#### Update fixed data

`UpdateData` identifies a row by its key and sets one or more columns to fixed values:

[!code-csharp[](../../../../samples/core/Schemas/Migrations/DataOperations.cs#snippet_UpdateData)]

The `Down` method should restore the previous values.

#### Delete fixed data

`DeleteData` also identifies rows by key:

[!code-csharp[](../../../../samples/core/Schemas/Migrations/DataOperations.cs#snippet_DeleteData)]

If the delete must be reversible, the `Down` method should use `InsertData` to restore every deleted value. These operations don't query the current database state; use `Sql` or initialization-time seeding when behavior depends on existing data.

### Arbitrary changes via raw SQL

Raw SQL can also be used to manage database objects that EF Core isn't aware of. To do this, add a migration without making any model change; an empty migration will be generated, which you can then populate with raw SQL operations.

For example, the following migration creates a SQL Server stored procedure:

```csharp
migrationBuilder.Sql(
@"
    EXEC ('CREATE PROCEDURE getFullName
        @LastName nvarchar(50),
        @FirstName nvarchar(50)
    AS
        SELECT @LastName + @FirstName;')");
```

> [!TIP]
> `EXEC` is used when a statement must be the first or only one in a SQL batch. It can also be used to work around parser errors in idempotent migration scripts that can occur when referenced columns don't currently exist on a table.

This can be used to manage any aspect of your database, including:

* Stored procedures
* Full-Text Search
* Functions
* Triggers
* Views

In most cases, EF Core will automatically wrap each migration in its own transaction when applying migrations. Unfortunately, some migration operations cannot be performed within a transaction in some databases; for these cases, you may opt out of the transaction by passing `suppressTransaction: true` to `migrationBuilder.Sql`.

> [!NOTE]
> In EF Core 9, EF Core spans all pending migrations with a single transaction by default (this was reverted in EF Core 10). See [the breaking change note](xref:core/what-is-new/ef-core-9.0/breaking-changes#migrations-single-transaction) for details.

## Remove a migration

Sometimes you add a migration and realize you need to make additional changes to your EF Core model before applying it. To remove the last migration, use this command.

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations remove
```

### [Visual Studio](#tab/vs)

```powershell
Remove-Migration
```

***

After removing the migration, you can make the additional model changes and add it again.

> [!WARNING]
> Avoid removing any migrations which have already been applied to production databases. Doing so means you won't be able to revert those migrations from the databases, and may break the assumptions made by subsequent migrations.

### If the migration was applied locally

For a disposable development database, first update the database to the previous migration, and then remove the migration from the project. Use `0` as the target when removing the first migration.

#### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update PreviousMigration
dotnet ef migrations remove
```

Alternatively, `--force` performs both steps:

```dotnetcli
dotnet ef migrations remove --force
```

#### [Visual Studio](#tab/vs)

```powershell
Update-Database PreviousMigration
Remove-Migration
```

Alternatively, `-Force` performs both steps:

```powershell
Remove-Migration -Force
```

***

### If the migration was applied to a shared database

Don't delete a migration that has been applied to a shared, test, or production database. Usually, keep the migration in the project and add a new corrective migration. If a planned rollback is required, execute the rollback while the original migration code is still available, and coordinate the application and database deployment.

### Remove an older unapplied migration

The tools remove only the latest migration. Don't delete a migration from the middle of the sequence and hand-edit the model snapshot. If the migration and every migration after it are unpublished and unapplied, remove the later migrations in reverse order, remove the unwanted migration, and then scaffold the retained model changes again.

If the migrations were created on different branches, follow the [diverged migration tree](xref:core/managing-schemas/migrations/teams#resolving-diverged-migration-trees) workflow instead.

## Listing migrations

You can list all existing migrations as follows:

### [.NET CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations list
```

### [Visual Studio](#tab/vs)

```powershell
Get-Migration
```

***

You can also inspect migration state programmatically:

```csharp
var allMigrations = context.Database.GetMigrations();
var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
```

`GetPendingMigrationsAsync` compares migrations in the configured migrations assembly with the migrations recorded in the target database. It doesn't detect model changes that haven't been captured in a migration; use the pending model changes check below for that.

## Checking for pending model changes

> [!NOTE]
> This feature was added in EF Core 8.0.

Sometimes you may want to check if there have been any model changes made since the last migration. This can help you know when you or a teammate forgot to add a migration. One way to do that is using this command.

```dotnetcli
dotnet ef migrations has-pending-model-changes
```

You can also perform this check programmatically using `context.Database.HasPendingModelChanges()`. This can be used to write a unit test that fails when you forget to add a migration.

> [!NOTE]
> Starting with EF Core 9, calling <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate*> or <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync*> with pending model changes throws an exception (event ID <xref:Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning>). See the [applying migrations documentation](xref:core/managing-schemas/migrations/applying#apply-migrations-at-runtime) and the [breaking change note](xref:core/what-is-new/ef-core-9.0/breaking-changes#pending-model-changes) for more information.

## Resetting all migrations

In some extreme cases, it may be necessary to remove all migrations and start over. This can be easily done by deleting your **Migrations** folder and dropping your database; at that point you can create a new initial migration, which will contain your entire current schema.

It's also possible to reset all migrations and create a single one without losing your data. This is called **squashing migrations**, and involves some manual work. EF Core doesn't currently provide an automated squashing command; see [dotnet/efcore#2174](https://github.com/dotnet/efcore/issues/2174).

1. Back up your database, in case something goes wrong.
2. In your database, delete all rows from the migrations history table (e.g. `DELETE FROM [__EFMigrationsHistory]` on SQL Server).
3. Delete your **Migrations** folder.
4. Create a new migration and generate a SQL script for it (`dotnet ef migrations script`).
5. Insert a single row into the migrations history, to record that the first migration has already been applied, since your tables are already there. The insert SQL is the last operation in the SQL script generated above, and resembles the following (don't forget to update the values):

```sql
INSERT INTO [__EFMigrationsHistory] ([MIGRATIONID], [PRODUCTVERSION])
VALUES (N'<full_migration_timestamp_and_name>', N'<EF_version>');
```

> [!WARNING]
> Any [custom migration code](#customize-migration-code) will be lost when the **Migrations** folder is deleted.  Any customizations must be applied to the new initial migration manually in order to be preserved.

Before squashing, verify that every deployed database is at a known migration and back it up. New databases must be created from the new initial migration, while existing databases must have the replacement migration recorded without executing schema operations that have already been applied. Test both paths before deployment.

## Additional resources

* [Entity Framework Core tools reference - .NET CLI](xref:core/cli/dotnet) : Includes commands to update, drop, add, remove, and  more.
* [Entity Framework Core tools reference - Package Manager Console in Visual Studio](xref:core/cli/powershell) : Includes commands to update, drop, add, remove, and  more.
