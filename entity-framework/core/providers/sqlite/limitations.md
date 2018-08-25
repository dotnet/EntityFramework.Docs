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
| RenameColumn         | ✗          |                  |
| RenameIndex          | ✔          | 2.1              |
| RenameTable          | ✔          | 1.0              |
| EnsureSchema         | ✔ (no-op)  | 2.0              |
| DropSchema           | ✔ (no-op)  | 2.0              |
| Insert               | ✔          | 2.0              |
| Update               | ✔          | 2.0              |
| Delete               | ✔          | 2.0              |

## Migrations limitations workaround

You can workaround some of these limitations by manually writing code in your migrations to perform a table rebuild. A table rebuild involves renaming the existing table, creating a new table, copying data to the new table, and dropping the old table. You will need to use the `Sql(string)` method to perform some of these steps.

See [Making Other Kinds Of Table Schema Changes](http://sqlite.org/lang_altertable.html#otheralter) in the SQLite documentation for more details.

In the future, EF may support some of these operations by using the table rebuild approach under the covers. You can [track this feature on our GitHub project](https://github.com/aspnet/EntityFrameworkCore/issues/329).
