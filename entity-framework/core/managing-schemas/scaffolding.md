---
title: Reverse Engineering - EF Core
author: bricelam
ms.author: bricelam
ms.date: 11/13/2018
ms.assetid: 6263EF7D-4989-42E6-BDEE-45DA770342FB
uid: core/managing-schemas/scaffolding
---
# Reverse Engineering

Reverse engineering is the process of scaffolding entity type classes and a DbContext class based on a database schema. It can be performed using the `Scaffold-DbContext` command of the EF Core Package Manager Console (PMC) tools or the `dotnet ef dbcontext scaffold` command of the .NET Command-line Interface (CLI) tools.

## Installing

Before reverse engineering, you'll need to install either the [PMC tools](xref:core/miscellaneous/cli/powershell) (Visual Studio only) or the [CLI tools](xref:core/miscellaneous/cli/dotnet). See links for details.

You'll also need to install an appropriate [database provider](xref:core/providers/index) for the database schema you want to reverse engineer.

## Connection string

The first argument to the command is a connection string to the database. The tools will use this connection string to read the database schema.

How you quote and escape the connection string depends on which shell you are using to execute the command. Refer to your shell's documentation for specifics. For example, PowerShell requires you to escape the `$` character, but not `\`.

``` powershell
Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook' Microsoft.EntityFrameworkCore.SqlServer
```

``` Console
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook" Microsoft.EntityFrameworkCore.SqlServer
```

### Configuration and User Secrets

If you have an ASP.NET Core project, you can use the `Name=<connection-string>` syntax to read the connection string from configuration.

This works well with the [Secret Manager tool](https://docs.microsoft.com/aspnet/core/security/app-secrets#secret-manager) to keep your database password separate from your codebase.

``` Console
dotnet user-secrets set ConnectionStrings.Chinook "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook"
dotnet ef dbcontext scaffold Name=Chinook Microsoft.EntityFrameworkCore.SqlServer
```

## Provider name

The second argument is the provider name. The provider name is typically the same as the provider's NuGet package name.

## Specifying tables

All tables in the database schema are reverse engineered into entity types by default. You can limit which tables are reverse engineered by specifying schemas and tables.

The `-Schemas` parameter in PMC and the `--schema` option in the CLI can be used to include every table within a schema.

`-Tables` (PMC) and `--table` (CLI) can be used to include specific tables.

To include multiple tables in PMC, use an array.

``` powershell
Scaffold-DbContext ... -Tables Artist, Album
```

To include multiple tables in the CLI, specify the option multiple times.

``` Console
dotnet ef dbcontext scaffold ... --table Artist --table Album
```

## Preserving names

Table and column names are fixed up to better match the .NET naming conventions for types and properties by default. Specifying the `-UseDatabaseNames` switch in PMC or the `--use-database-names` option in the CLI will disable this behavior preserving the original database names as much as possible. Invalid .NET identifiers will still be fixed and synthesized names like navigation properties will still conform to .NET naming conventions.

## Fluent API or Data Annotations

Entity types are configured using the Fluent API by default. Specify `-DataAnnotations` (PMC) or `--data-annotations` (CLI) to instead use data annotations when possible.

For example, using the Fluent API will scaffold this:

``` csharp
entity.Property(e => e.Title)
    .IsRequired()
    .HasMaxLength(160);
```

While using Data Annotations will scaffold this:

``` csharp
[Required]
[StringLength(160)]
public string Title { get; set; }
```

## DbContext name

The scaffolded DbContext class name will be the name of the database suffixed with *Context* by default. To specify a different one, use `-Context` in PMC and `--context` in the CLI.

## Directories and namespaces

The entity classes and a DbContext class are scaffolded into the project's root directory and use the project's default namespace. You can specify the directory where classes are scaffolded using `-OutputDir` (PMC) or `--output-dir` (CLI). The namespace will be the root namespace plus the names of any subdirectories under the project's root directory.

You can also use `-ContextDir` (PMC) and `--context-dir` (CLI) to scaffold the DbContext class into a separate directory from the entity type classes.

``` powershell
Scaffold-DbContext ... -ContextDir Data -OutputDir Models
```

``` Console
dotnet ef dbcontext scaffold ... --context-dir Data --output-dir Models
```

## How it works

Reverse engineering starts by reading the database schema. It reads information about tables, columns, constraints, and indexes.

Next, it uses the schema information to create an EF Core model. Tables are used to create entity types; columns are used to create properties; and foreign keys are used to create relationships.

Finally, the model is used to generate code. The corresponding entity type classes, Fluent API, and data annotations are scaffolded in order to re-create the same model from your app.

## What doesn't work

Not everything about a model can be represented using a database schema. For example, information about **inheritance hierarchies**, **owned types**, and **table splitting** are not present in the database schema. Because of this, these constructs will never be reverse engineered.

In addition, **some column types** may not be supported by the EF Core provider. These columns won't be included in the model.

EF Core requires every entity type to have a key. Tables, however, aren't required to specify a primary key. **Tables without a primary key** are currently not reverse engineered.

You can define **concurrency tokens** in an EF Core model to prevent two users from updating the same entity at the same time. Some databases have a special type to represent this type of column (for example, rowversion in SQL Server) in which case we can reverse engineer this information; however, other concurrency tokens will not be reverse engineered.

## Customizing the model

The code generated by EF Core is your code. Feel free to change it. It will only be regenerated if you reverse engineer the same model again. The scaffolded code represents *one* model that can be used to access the database, but it's certainly not the *only* model that can be used.

Customize the entity type classes and DbContext class to fit your needs. For example, you may choose to rename types and properties, introduce inheritance hierarchies, or split a table into to multiple entities. You can also remove non-unique indexes, unused sequences and navigation properties, optional scalar properties, and constraint names from the model.

You can also add additional constructors, methods, properties, etc. using another partial class in a separate file. This approach works even when you intend to reverse engineer the model again.

## Updating the model

After making changes to the database, you may need to update your EF Core model to reflect those changes. If the database changes are simple, it may be easiest just to manually make the changes to your EF Core model. For example, renaming a table or column, removing a column, or updating a column's type are trivial changes to make in code.

More significant changes, however, are not as easy make manually. One common workflow is to reverse engineer the model from the database again using `-Force` (PMC) or `--force` (CLI) to overwrite the existing model with an updated one.

Another commonly requested feature is the ability to update the model from the database while preserving customization like renames, type hierarchies, etc. Use issue [#831](https://github.com/aspnet/EntityFrameworkCore/issues/831) to track the progress of this feature.

> [!WARNING]
> If you reverse engineer the model from the database again, any changes you've made to the files will be lost.
