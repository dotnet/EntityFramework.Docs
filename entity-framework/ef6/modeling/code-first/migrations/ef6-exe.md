---
title: Using ef6.exe - EF6
description: Using ef6.exe in Entity Framework 6
author: SamMonoRT
ms.date: 08/19/2026
uid: ef6/modeling/code-first/migrations/ef6-exe
---
# Using ef6.exe

> [!NOTE]
> This article assumes you know how to use Code First Migrations in basic scenarios. If you don't, then read [Code First Migrations](xref:ef6/modeling/code-first/migrations/index) before continuing.

Code First Migrations can update a database from Visual Studio, but you can also run migrations from the command line with `ef6.exe`. This utility replaces `migrate.exe`.

## Locate ef6.exe

When you install Entity Framework using NuGet, `ef6.exe` is in the tools folder of the NuGet package. With PackageReference, the package is stored in the global packages folder. For example:

```text
%USERPROFILE%\.nuget\packages\entityframework\6.4.4\tools\net45\win-x86\ef6.exe
```

Replace `6.4.4` with the version of Entity Framework installed in your project.

## View the options

```console
ef6.exe database update --help
```

## Migrate to the latest migration

```console
ef6.exe database update --assembly MyMvcApplication.dll --config ..\web.config
```

The assembly containing the migrations is required. Other settings use conventions when they aren't specified.

## Migrate to a specific migration

```console
ef6.exe database update --assembly MyApp.exe --config MyApp.exe.config --target AddTitle
```

Use `--target` to apply all migrations through a specific migration.

## Specify project directory

```console
ef6.exe database update --assembly MyApp.exe --config MyApp.exe.config --project-dir C:\MyApp
```

Use `--project-dir` to set the project directory used to resolve dependencies and relative file paths.

## Specify migration configuration to use

```console
ef6.exe database update --assembly MyAssembly.dll --migrations-config CustomConfig --config ..\web.config
```

If the assembly contains multiple migration configuration classes, use `--migrations-config` to specify which one to use.

## Provide connection string

```console
ef6.exe database update --assembly BlogDemo.dll --connection-string "Data Source=localhost;Initial Catalog=BlogDemo;Integrated Security=SSPI" --connection-provider System.Data.SqlClient
```

When you specify a connection string, you must also specify its provider name.

## Generate a SQL script

Use `--script` to write the SQL migration script to standard output. Redirect the output to save the script to a file:

```console
ef6.exe database update --assembly MyApp.dll --script > migration.sql
```

Use `--source` and `--target` to control the range of migrations included in the script.

## migrate.exe option equivalents

The following table maps `migrate.exe` arguments to their `ef6.exe database update` equivalents:

| migrate.exe | ef6.exe database update |
| --- | --- |
| `<assembly>` | `--assembly` |
| `<configurationType>` | `--migrations-config` |
| `<contextAssembly>` | No equivalent. Put it in the same directory as the assembly passed to `--assembly`. |
| `/targetMigration` | `--target` |
| `/startupDirectory` | `--project-dir` |
| `/scriptFile` | `--script` |
| `/sourceMigration` | `--source` |
| `/startupConfigurationFile` | `--config` |
| `/startupDataDirectory` | `--data-dir` |
| `/connectionStringName` | `--connection-string-name` |
| `/connectionString` | `--connection-string` |
| `/connectionProviderName` | `--connection-provider` |
| `/force` | `--force` |
| `/verbose` | `--verbose` |
| `/?` | `--help` |
