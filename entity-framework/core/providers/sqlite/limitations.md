---
title: SQLite Database Provider - Limitations - EF Core
author: rowanmiller
ms.date: 04/09/2017
ms.assetid: 94ab4800-c460-4caa-a5e8-acdfee6e6ce2
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

The `Decimal` type provides a high level of precision. If you don't need that level of precision, however, we recommend using double instead. You can use a [value converter](../../modeling/value-conversions.md) to continue using decimal in your classes.

``` csharp
modelBuilder.Entity<MyEntity>()
    .Property(e => e.DecimalProperty)
    .HasConversion<double>();
```

## Migrations limitations

The SQLite database engine does not support a number of schema operations that are supported by the majority of other relational databases. If you attempt to apply one of the unsupported operations to a SQLite database then a `NotSupportedException` will be thrown.

| Operation            | Supported? | Requires version |
|:---------------------|:-----------|:-----------------|
| AddColumn            | ✔          | 1.0              |
| AddForeignKey        | ✗          |                  |
| AddPrimaryKey        | ✗          |                  |
| AddUniqueConstraint  | ✗          |                  |
| AlterColumn          | ✗          |                  |
| CreateIndex          | ✔          | 1.0              |
| CreateTable          | ✔          | 1.0              |
| DropColumn           | ✗          |                  |
| DropForeignKey       | ✗          |                  |
| DropIndex            | ✔          | 1.0              |
| DropPrimaryKey       | ✗          |                  |
| DropTable            | ✔          | 1.0              |
| DropUniqueConstraint | ✗          |                  |
| RenameColumn         | ✔          | 2.2.2            |
| RenameIndex          | ✔          | 2.1              |
| RenameTable          | ✔          | 1.0              |
| EnsureSchema         | ✔ (no-op)  | 2.0              |
| DropSchema           | ✔ (no-op)  | 2.0              |
| Insert               | ✔          | 2.0              |
| Update               | ✔          | 2.0              |
| Delete               | ✔          | 2.0              |

## Migrations limitations workaround

You can workaround some of these limitations by manually writing code in your migrations to perform a table rebuild. A table rebuild involves renaming the existing table, creating a new table, copying data to the new table, and dropping the old table. You will need to use the `Sql(string)` method to perform some of these steps.

See [Making Other Kinds Of Table Schema Changes](https://sqlite.org/lang_altertable.html#otheralter) in the SQLite documentation for more details.

In the future, EF may support some of these operations by using the table rebuild approach under the covers. You can [track this feature on our GitHub project](https://github.com/aspnet/EntityFrameworkCore/issues/329).
