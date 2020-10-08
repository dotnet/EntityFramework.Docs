---
title: SQLite Database Provider - Limitations - EF Core
description: Limitations of the Entity Framework Core SQLite database provider as compared to other providers
author: bricelam
ms.date: 09/24/2020
uid: core/providers/sqlite/limitations
---
# SQLite EF Core Database Provider Limitations

The SQLite provider has a number of migrations limitations. Most of these limitations are a result of limitations in the underlying SQLite database engine and are not specific to EF.

## Modeling limitations

The common relational library (shared by Entity Framework relational database providers) defines APIs for modelling concepts that are common to most relational database engines. A couple of these concepts are not supported by the SQLite provider.

* Schemas
* Sequences

## Query limitations

SQLite doesn't natively support the following data types. EF Core can read and write values of these types, and querying for equality (`where e.Property == value`) is also supported. Other operations, however, like comparison and ordering will require evaluation on the client.

* DateTimeOffset
* Decimal
* TimeSpan
* UInt64

Instead of `DateTimeOffset`, we recommend using DateTime values. When handling multiple time zones, we recommend converting the values to UTC before saving and then converting back to the appropriate time zone.

The `Decimal` type provides a high level of precision. If you don't need that level of precision, however, we recommend using double instead. You can use a [value converter](xref:core/modeling/value-conversions) to continue using decimal in your classes.

```csharp
modelBuilder.Entity<MyEntity>()
    .Property(e => e.DecimalProperty)
    .HasConversion<double>();
```

## Migrations limitations

The SQLite database engine does not support a number of schema operations that are supported by the majority of other relational databases. If you attempt to apply one of the unsupported operations to a SQLite database then a `NotSupportedException` will be thrown.

A rebuild will be attempted in order to perform certain operations. Rebuilds are only possible for database artifacts that are part of your EF Core model. If a database artifact isn't part of the model--for example, if it was created manually inside a migration--then a `NotSupportedException` is still thrown.

| Operation            | Supported?  | Requires version |
|:---------------------|:------------|:-----------------|
| AddCheckConstraint   | ✔ (rebuild) | 5.0              |
| AddColumn            | ✔           |                  |
| AddForeignKey        | ✔ (rebuild) | 5.0              |
| AddPrimaryKey        | ✔ (rebuild) | 5.0              |
| AddUniqueConstraint  | ✔ (rebuild) | 5.0              |
| AlterColumn          | ✔ (rebuild) | 5.0              |
| CreateIndex          | ✔           |                  |
| CreateTable          | ✔           |                  |
| DropCheckConstraint  | ✔ (rebuild) | 5.0              |
| DropColumn           | ✔ (rebuild) | 5.0              |
| DropForeignKey       | ✔ (rebuild) | 5.0              |
| DropIndex            | ✔           |                  |
| DropPrimaryKey       | ✔ (rebuild) | 5.0              |
| DropTable            | ✔           |                  |
| DropUniqueConstraint | ✔ (rebuild) | 5.0              |
| RenameColumn         | ✔           | 2.2              |
| RenameIndex          | ✔ (rebuild) |                  |
| RenameTable          | ✔           |                  |
| EnsureSchema         | ✔ (no-op)   |                  |
| DropSchema           | ✔ (no-op)   |                  |
| Insert               | ✔           |                  |
| Update               | ✔           |                  |
| Delete               | ✔           |                  |

### Migrations limitations workaround

You can workaround some of these limitations by manually writing code in your migrations to perform a rebuild. Table rebuilds involve creating a new table, copying data to the new table, dropping the old table, renaming the new table. You will need to use the `Sql(string)` method to perform some of these steps.

See [Making Other Kinds Of Table Schema Changes](https://sqlite.org/lang_altertable.html#otheralter) in the SQLite documentation for more details.

### Idempotent script limitations

Unlike other databases, SQLite doesn't include a procedural language. Because of this, there is no way to generate the if-then logic required by the idempotent migration scripts.

If you know the last migration applied to a database, you can generate a script from that migration to the latest migration.

```dotnetcli
dotnet ef migrations script CurrentMigration
```

Otherwise, we recommend using `dotnet ef database update` to apply migrations. Starting in EF Core 5.0, you can specify the database file when running the command.

```dotnetcli
dotnet ef database update --connection "Data Source=My.db"
```

## See also

* [Microsoft.Data.Sqlite Async Limitations](/dotnet/standard/data/sqlite/async)
* [Microsoft.Data.Sqlite ADO.NET Limitations](/dotnet/standard/data/sqlite/adonet-limitations)
