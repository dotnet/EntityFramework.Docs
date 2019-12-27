---
title: Migrations - EF Core
author: bricelam
ms.author: bricelam
ms.date: 10/05/2018
uid: core/managing-schemas/migrations/index
---
# Migrations

A data model changes during development and gets out of sync with the database. You can drop the database and let EF create a new one that matches the model, but this procedure results in the loss of data. The migrations feature in EF Core provides a way to incrementally update the database schema to keep it in sync with the application's data model while preserving existing data in the database.

Migrations includes command-line tools and APIs that help with the following tasks:

* [Create a migration](#create-a-migration). Generate code that can update the database to sync it with a set of model changes.
* [Update the database](#update-the-database). Apply pending migrations to update the database schema.
* [Customize migration code](#customize-migration-code). Sometimes the generated code needs to be modified or supplemented.
* [Remove a migration](#remove-a-migration). Delete the generated code.
* [Revert a migration](#revert-a-migration). Undo the database changes.
* [Generate SQL scripts](#generate-sql-scripts). You might need a script to update a production database or to troubleshoot migration code.
* [Apply migrations at runtime](#apply-migrations-at-runtime). When design-time updates and running scripts aren't the best options, call the `Migrate()` method.

> [!TIP]
> If the `DbContext` is in a different assembly than the startup project, you can explicitly specify the target and startup projects in either the [Package Manager Console tools](xref:core/miscellaneous/cli/powershell#target-and-startup-project) or the [.NET Core CLI tools](xref:core/miscellaneous/cli/dotnet#target-project-and-startup-project).

## Install the tools

Install the [command-line tools](xref:core/miscellaneous/cli/index):

* For Visual Studio, we recommend the [Package Manager Console tools](xref:core/miscellaneous/cli/powershell).
* For other development environments, choose the [.NET Core CLI tools](xref:core/miscellaneous/cli/dotnet).

## Create a migration

After you've [defined your initial model](xref:core/modeling/index), it's time to create the database. To add an initial migration, run the following command.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add InitialCreate
```

### [Visual Studio](#tab/vs)

``` powershell
Add-Migration InitialCreate
```

***

Three files are added to your project under the **Migrations** directory:

* **XXXXXXXXXXXXXX_InitialCreate.cs**--The main migrations file. Contains the operations necessary to apply the migration (in `Up()`) and to revert it (in `Down()`).
* **XXXXXXXXXXXXXX_InitialCreate.Designer.cs**--The migrations metadata file. Contains information used by EF.
* **MyContextModelSnapshot.cs**--A snapshot of your current model. Used to determine what changed when adding the next migration.

The timestamp in the filename helps keep them ordered chronologically so you can see the progression of changes.

> [!TIP]
> You are free to move Migrations files and change their namespace. New migrations are created as siblings of the last migration.

## Update the database

Next, apply the migration to the database to create the schema.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update
```

### [Visual Studio](#tab/vs)

``` powershell
Update-Database
```

***

## Customize migration code

After making changes to your EF Core model, the database schema might be out of sync. To bring it up to date, add another migration. The migration name can be used like a commit message in a version control system. For example, you might choose a name like *AddProductReviews* if the change is a new entity class for reviews.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations add AddProductReviews
```

### [Visual Studio](#tab/vs)

``` powershell
Add-Migration AddProductReviews
```

***

Once the migration is scaffolded (code generated for it), review the code for accuracy and add, remove or modify any operations required to apply it correctly.

For example, a migration might contain the following operations:

``` csharp
migrationBuilder.DropColumn(
    name: "FirstName",
    table: "Customer");

migrationBuilder.DropColumn(
    name: "LastName",
    table: "Customer");

migrationBuilder.AddColumn<string>(
    name: "Name",
    table: "Customer",
    nullable: true);
```

While these operations make the database schema compatible, they don't preserve the existing customer names. To make it better, rewrite it as follows.

``` csharp
migrationBuilder.AddColumn<string>(
    name: "Name",
    table: "Customer",
    nullable: true);

migrationBuilder.Sql(
@"
    UPDATE Customer
    SET Name = FirstName + ' ' + LastName;
");

migrationBuilder.DropColumn(
    name: "FirstName",
    table: "Customer");

migrationBuilder.DropColumn(
    name: "LastName",
    table: "Customer");
```

> [!TIP]
> The migration scaffolding process warns when an operation might result in data loss (like dropping a column). If you see that warning, be especially sure to review the migrations code for accuracy.

Apply the migration to the database using the appropriate command.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update
```

### [Visual Studio](#tab/vs)

``` powershell
Update-Database
```

***

### Empty migrations

Sometimes it's useful to add a migration without making any model changes. In this case, adding a new migration creates code files with empty classes. You can customize this migration to perform operations that don't directly relate to the EF Core model. Some things you might want to manage this way are:

* Full-Text Search
* Functions
* Stored procedures
* Triggers
* Views

## Remove a migration

Sometimes you add a migration and realize you need to make additional changes to your EF Core model before applying it. To remove the last migration, use this command.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations remove
```

### [Visual Studio](#tab/vs)

``` powershell
Remove-Migration
```

***

After removing the migration, you can make the additional model changes and add it again.

## Revert a migration

If you already applied a migration (or several migrations) to the database but need to revert it, you can use the same command to apply migrations, but specify the name of the migration you want to roll back to.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef database update LastGoodMigration
```

### [Visual Studio](#tab/vs)

``` powershell
Update-Database LastGoodMigration
```

***

## Generate SQL scripts

When debugging your migrations or deploying them to a production database, it's useful to generate a SQL script. The script can then be further reviewed for accuracy and tuned to fit the needs of a production database. The script can also be used in conjunction with a deployment technology. The basic command is as follows.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations script
```

### [Visual Studio](#tab/vs)

``` powershell
Script-Migration
```

***

There are several options to this command.

The **from** migration should be the last migration applied to the database before running the script. If no migrations have been applied, specify `0` (this is the default).

The **to** migration is the last migration that will be applied to the database after running the script. This defaults to the last migration in your project.

An **idempotent** script can optionally be generated. This script only applies migrations if they haven't already been applied to the database. This is useful if you don't exactly know what the last migration applied to the database was or if you are deploying to multiple databases that may each be at a different migration.

## Apply migrations at runtime

Some apps may want to apply migrations at runtime during startup or first run. Do this using the `Migrate()` method.

This method builds on top of the `IMigrator` service, which can be used for more advanced scenarios. Use `myDbContext.GetInfrastructure().GetService<IMigrator>()` to access it.

``` csharp
myDbContext.Database.Migrate();
```

> [!WARNING]
>
> * This approach isn't for everyone. While it's great for apps with a local database, most applications will require more robust deployment strategy like generating SQL scripts.
> * Don't call `EnsureCreated()` before `Migrate()`. `EnsureCreated()` bypasses Migrations to create the schema, which causes `Migrate()` to fail.

## Next steps

For more information, see <xref:core/miscellaneous/cli/index>.
