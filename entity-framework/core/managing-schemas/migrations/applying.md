---
title: Applying Migrations - EF Core
description: Strategies for applying schema migrations to production and development databases using Entity Framework Core
author: bricelam
ms.date: 11/02/2021
uid: core/managing-schemas/migrations/applying
---
# Applying Migrations

Once your migrations have been added, they need to be deployed and applied to your databases. There are various strategies for doing this, with some being more appropriate for production environments, and others for the development lifecycle.

> [!NOTE]
> Whatever your deployment strategy, always inspect the generated migrations and test them before applying to a production database. A migration may drop a column when the intent was to rename it, or may fail for various reasons when applied to a database.

## SQL scripts

The recommended way to deploy migrations to a production database is by generating SQL scripts. The advantages of this strategy include the following:

* SQL scripts can be reviewed for accuracy; this is important since applying schema changes to production databases is a potentially dangerous operation that could involve data loss.
* In some cases, the scripts can be tuned to fit the specific needs of a production database.
* SQL scripts can be used in conjunction with a deployment technology, and can even be generated as part of your CI process.
* SQL scripts can be provided to a DBA, and can be managed and archived separately.

### [.NET Core CLI](#tab/dotnet-core-cli)

#### Basic Usage

The following generates a SQL script from a blank database to the latest migration:

```dotnetcli
dotnet ef migrations script
```

#### With From (to implied)

The following generates a SQL script from the given migration to the latest migration.

```dotnetcli
dotnet ef migrations script AddNewTables
```

#### With From and To

The following generates a SQL script from the specified `from` migration to the specified `to` migration.

```dotnetcli
dotnet ef migrations script AddNewTables AddAuditTable
```

You can use a `from` that is newer than the `to` in order to generate a rollback script.

> [!WARNING]
> Please take note of potential data loss scenarios.

### [Visual Studio](#tab/vs)

#### Basic Usage

The following generates a SQL script from a blank database to the latest migration:

```powershell
Script-Migration
```

#### With From (to implied)

The following generates a SQL script from the given migration to the latest migration.

```powershell
Script-Migration AddNewTables
```

#### With From and To

The following generates a SQL script from the specified `from` migration to the specified `to` migration.

```powershell
Script-Migration AddNewTables AddAuditTable
```

You can use a `from` that is newer than the `to` in order to generate a rollback script.

> [!WARNING]
> Please take note of potential data loss scenarios.

***

Script generation accepts the following two arguments to indicate which range of migrations should be generated:

* The **from** migration should be the last migration applied to the database before running the script. If no migrations have been applied, specify `0` (this is the default).
* The **to** migration is the last migration that will be applied to the database after running the script. This defaults to the last migration in your project.

## Idempotent SQL scripts

The SQL scripts generated above can only be applied to change your schema from one migration to another; it is your responsibility to apply the script appropriately, and only to databases in the correct migration state. EF Core also supports generating **idempotent** scripts, which internally check which migrations have already been applied (via the migrations history table), and only apply missing ones. This is useful if you don't exactly know what the last migration applied to the database was, or if you are deploying to multiple databases that may each be at a different migration.

The following generates idempotent migrations:

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef migrations script --idempotent
```

### [Visual Studio](#tab/vs)

```powershell
Script-Migration -Idempotent
```

***

## Command-line tools

The EF command-line tools can be used to apply migrations to a database. While productive for local development and testing of migrations, this approach isn't ideal for managing production databases:

* The SQL commands are applied directly by the tool, without giving the developer a chance to inspect or modify them. This can be dangerous in a production environment.
* The .NET SDK and the EF tool must be installed on production servers and requires the project's source code.

### [.NET Core CLI](#tab/dotnet-core-cli)

The following updates your database to the latest migration:

```dotnetcli
dotnet ef database update
```

The following updates your database to a given migration:

```dotnetcli
dotnet ef database update AddNewTables
```

Note that this can be used to roll back to an earlier migration as well.

> [!WARNING]
> Please take note of potential data loss scenarios.

### [Visual Studio](#tab/vs)

The following updates your database to the latest migration:

```powershell
Update-Database
```

The following updates your database to a given migration:

```powershell
Update-Database AddNewTables
```

Note that this can be used to roll back to an earlier migration as well.

> [!WARNING]
> Please take note of potential data loss scenarios.

***

For more information on applying migrations via the command-line tools, see the [EF Core tools reference](xref:core/cli/index).

## Bundles

> [!NOTE]
> This feature was introduced in EF Core 6.0.

Migration bundles are single-file executables than can be used to apply migrations to a database. They address some of the shortcomings of the SQL script and command-line tools:

* Executing SQL scripts requires additional tools.
* The transaction handling and continue-on-error behavior of these tools are inconsistent and sometimes unexpected. This can leave your database in an undefined state if a failure occurs when applying migrations.
* Bundles can be generated as part of your CI process and easily executed later as part of your deployment process.
* Bundles can be executed without installing the .NET SDK or EF Tool (or even the .NET Runtime, when self-contained), and they don't require the project's source code.

### [.NET Core CLI](#tab/dotnet-core-cli)

The following generates a bundle:

```dotnetcli
dotnet ef migrations bundle
```

The following generates a self-contained bundle for Linux:

```dotnetcli
dotnet ef migrations bundle --self-contained -r linux-x64
```

### [Visual Studio](#tab/vs)

The following generates a bundle:

```powershell
Bundle-Migration
```

The following generates a self-contained bundle for Linux:

```dotnetcli
Bundle-Migration -SelfContained -TargetRuntime linux-x64
```

***

For more information on creating bundles see the [EF Core tools reference](xref:core/cli/index).

### `efbundle`

The resulting executable is named `efbundle` by default. It can be used to update the database to the latest migration. It's equivalent to running `dotnet ef database update` or `Update-Database`.

Arguments:

Argument                   | Description
-------------------------- | -----------
<nobr>`<MIGRATION>`</nobr> | The target migration. If '0', all migrations will be reverted. Defaults to the last migration.

Options:

Option                                   | Short             | Description
---------------------------------------- | ----------------- | -----------
<nobr>`--connection <CONNECTION>`</nobr> |                   | The connection string to the database. Defaults to the one specified in AddDbContext or OnConfiguring.
`--verbose`                              | <nobr>`-v`</nobr> | Show verbose output.
`--no-color`                             |                   | Don't colorize output.
`--prefix-output`                        |                   | Prefix output with level.

The following example applies migrations to a local SQL Server instance using the specified username and password.

```powershell
.\efbundle.exe --connection 'Data Source=(local)\MSSQLSERVER;Initial Catalog=Blogging;User ID=myUsername;Password=myPassword'
```

***

## Apply migrations at runtime

It's possible for the application itself to apply migrations programmatically, typically during startup. While productive for local development and testing of migrations, this approach is inappropriate for managing production databases, for the following reasons:

* If multiple instances of your application are running, both applications could attempt to apply the migration concurrently and fail (or worse, cause data corruption).
* Similarly, if an application is accessing the database while another application migrates it, this can cause severe issues.
* The application must have elevated access to modify the database schema. It's generally good practice to limit the application's database permissions in production.
* It's important to be able to roll back an applied migration in case of an issue. The other strategies provide this easily and out of the box.
* The SQL commands are applied directly by the program, without giving the developer a chance to inspect or modify them. This can be dangerous in a production environment.

To apply migrations programmatically, call `context.Database.Migrate()`. For example, a typical ASP.NET application can do the following:

```csharp
public static void Main(string[] args)
{
    var host = CreateHostBuilder(args).Build();

    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    host.Run();
}
```

Note that `Migrate()` builds on top of the `IMigrator` service, which can be used for more advanced scenarios. Use `myDbContext.GetInfrastructure().GetService<IMigrator>()` to access it.

> [!WARNING]
>
> * Carefully consider before using this approach in production. Experience has shown that the simplicity of this deployment strategy is outweighed by the issues it creates. Consider generating SQL scripts from migrations instead.
> * Don't call `EnsureCreated()` before `Migrate()`. `EnsureCreated()` bypasses Migrations to create the schema, which causes `Migrate()` to fail.
