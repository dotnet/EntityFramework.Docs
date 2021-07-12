---
title: Reverse Engineering - EF Core
description: Reverse engineering a model from an existing database using Entity Framework Core
author: bricelam
ms.date: 11/13/2018
uid: core/managing-schemas/scaffolding
---
# Reverse Engineering

Reverse engineering is the process of scaffolding entity type classes and a DbContext class based on a database schema. It can be performed using the `Scaffold-DbContext` command of the EF Core Package Manager Console (PMC) tools or the `dotnet ef dbcontext scaffold` command of the .NET Command-line Interface (CLI) tools.

## Installing

Before reverse engineering, you'll need to install either the [PMC tools](xref:core/cli/powershell) (Visual Studio only) or the [CLI tools](xref:core/cli/dotnet). See links for details.

You'll also need to install an appropriate [database provider](xref:core/providers/index) for the database schema you want to reverse engineer.

## Connection string

The first argument to the command is a connection string to the database. The tools will use this connection string to read the database schema.

How you quote and escape the connection string depends on which shell you are using to execute the command. Refer to your shell's documentation for specifics. For example, PowerShell requires you to escape the `$` character, but not `\`.

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

### [Visual Studio](#tab/vs)

```powershell
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer
```

***

### Configuration and User Secrets

If you have an ASP.NET Core project, you can use the `Name=<connection-string>` syntax to read the connection string from configuration.

This works well with the [Secret Manager tool](/aspnet/core/security/app-secrets#secret-manager) to keep your database password separate from your codebase.

```dotnetcli
dotnet user-secrets set ConnectionStrings:Chinook "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook"
dotnet ef dbcontext scaffold Name=ConnectionStrings:Chinook Microsoft.EntityFrameworkCore.SqlServer
```

## Provider name

The second argument is the provider name. The provider name is typically the same as the provider's NuGet package name.

## Specifying tables

All tables in the database schema are reverse engineered into entity types by default. You can limit which tables are reverse engineered by specifying schemas and tables.

### [.NET Core CLI](#tab/dotnet-core-cli)

The `--schema` option can be used to include every table within a schema, while `--table` can be used to include specific tables.

To include multiple tables, specify the option multiple times:

```dotnetcli
dotnet ef dbcontext scaffold ... --table Artist --table Album
```

### [Visual Studio](#tab/vs)

The `-Schemas` option can be used to include every table within a schema, while `-Tables` can be used to include specific tables.

To include multiple tables, use an array:

```powershell
Scaffold-DbContext ... -Tables Artist, Album
```

***

## Preserving names

Table and column names are fixed up to better match the .NET naming conventions for types and properties by default. Specifying the `-UseDatabaseNames` switch in PMC or the `--use-database-names` option in the .NET Core CLI will disable this behavior preserving the original database names as much as possible. Invalid .NET identifiers will still be fixed and synthesized names like navigation properties will still conform to .NET naming conventions.

## Fluent API or Data Annotations

Entity types are configured using the Fluent API by default. Specify `-DataAnnotations` (PMC) or `--data-annotations` (.NET Core CLI) to instead use data annotations when possible.

For example, using the Fluent API will scaffold this:

```csharp
entity.Property(e => e.Title)
    .IsRequired()
    .HasMaxLength(160);
```

While using Data Annotations will scaffold this:

```csharp
[Required]
[StringLength(160)]
public string Title { get; set; }
```

## DbContext name

The scaffolded DbContext class name will be the name of the database suffixed with *Context* by default. To specify a different one, use `-Context` in PMC and `--context` in the .NET Core CLI.

## Directories and namespaces

The entity classes and a DbContext class are scaffolded into the project's root directory and use the project's default namespace.

### [.NET Core CLI](#tab/dotnet-core-cli)

You can specify the directory where classes are scaffolded using `--output-dir`, and `--context-dir` can be used to scaffold the DbContext class into a separate directory from the entity type classes:

```dotnetcli
dotnet ef dbcontext scaffold ... --context-dir Data --output-dir Models
```

By default, the namespace will be the root namespace plus the names of any subdirectories under the project's root directory. However, from EFCore 5.0 onwards, you can override the namespace for all output classes by using `--namespace`. You can also override the namespace for just the DbContext class using `--context-namespace`:

```dotnetcli
dotnet ef dbcontext scaffold ... --namespace Your.Namespace --context-namespace Your.DbContext.Namespace
```

### [Visual Studio](#tab/vs)

You can specify the directory where classes are scaffolded using `-OutputDir`, and `-ContextDir` can be used to scaffold the DbContext class into a separate directory from the entity type classes:

```powershell
Scaffold-DbContext ... -ContextDir Data -OutputDir Models
```

By default, the namespace will be the root namespace plus the names of any subdirectories under the project's root directory. However, from EFCore 5.0 onwards, you can override the namespace for all output classes by using `-Namespace`. You can also override the namespace for just the DbContext class using `-ContextNamespace`.

```powershell
Scaffold-DbContext ... -Namespace Your.Namespace -ContextNamespace Your.DbContext.Namespace
```

***

## How it works

Reverse engineering starts by reading the database schema. It reads information about tables, columns, constraints, and indexes.

Next, it uses the schema information to create an EF Core model. Tables are used to create entity types; columns are used to create properties; and foreign keys are used to create relationships.

Finally, the model is used to generate code. The corresponding entity type classes, Fluent API, and data annotations are scaffolded in order to re-create the same model from your app.

## Limitations

* Not everything about a model can be represented using a database schema. For example, information about [**inheritance hierarchies**](xref:core/modeling/inheritance), [**owned types**](xref:core/modeling/owned-entities), and [**table splitting**](xref:core/modeling/table-splitting) are not present in the database schema. Because of this, these constructs will never be reverse engineered.
* In addition, **some column types** may not be supported by the EF Core provider. These columns won't be included in the model.
* You can define [**concurrency tokens**](xref:core/modeling/concurrency), in an EF Core model to prevent two users from updating the same entity at the same time. Some databases have a special type to represent this type of column (for example, rowversion in SQL Server) in which case we can reverse engineer this information; however, other concurrency tokens will not be reverse engineered.
* [The C# 8 nullable reference type feature](/dotnet/csharp/tutorials/nullable-reference-types) is currently unsupported in reverse engineering: EF Core always generates C# code that assumes the feature is disabled. For example, nullable text columns will be scaffolded as a property with type `string` , not `string?`, with either the Fluent API or Data Annotations used to configure whether a property is required or not. You can edit the scaffolded code and replace these with C# nullability annotations. Scaffolding support for nullable reference types is tracked by issue [#15520](https://github.com/dotnet/efcore/issues/15520).

## Customizing the model

The code generated by EF Core is your code. Feel free to change it. It will only be regenerated if you reverse engineer the same model again. The scaffolded code represents *one* model that can be used to access the database, but it's certainly not the *only* model that can be used.

Customize the entity type classes and DbContext class to fit your needs. For example, you may choose to rename types and properties, introduce inheritance hierarchies, or split a table into multiple entities. You can also remove non-unique indexes, unused sequences and navigation properties, optional scalar properties, and constraint names from the model.

You can also add additional constructors, methods, properties, etc. using another partial class in a separate file. This approach works even when you intend to reverse engineer the model again.

## Updating the model

After making changes to the database, you may need to update your EF Core model to reflect those changes. If the database changes are simple, it may be easiest just to manually make the changes to your EF Core model. For example, renaming a table or column, removing a column, or updating a column's type are trivial changes to make in code.

More significant changes, however, are not as easy to make manually. One common workflow is to reverse engineer the model from the database again using `-Force` (PMC) or `--force` (CLI) to overwrite the existing model with an updated one.

Another commonly requested feature is the ability to update the model from the database while preserving customization like renames, type hierarchies, etc. Use issue [#831](https://github.com/dotnet/efcore/issues/831) to track the progress of this feature.

> [!WARNING]
> If you reverse engineer the model from the database again, any changes you've made to the files will be lost.

> [!TIP]
> If you use Visual Studio, the [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools/) community extension, a graphical tool which builds on top of the EF Core command line tools, offers additional workflow and customization options.
