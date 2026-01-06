---
title: SQLite Database Provider - Value Generation - EF Core
description: Value Generation Patterns Specific to the SQLite Entity Framework Core Database Provider
author: AndriySvyryd
ms.date: 09/26/2025
uid: core/providers/sqlite/value-generation
---
# SQLite Value Generation

This page details value generation configuration and patterns that are specific to the SQLite provider. It's recommended to first read [the general page on value generation](xref:core/modeling/generated-properties).

## AUTOINCREMENT columns

By convention, numeric primary key columns that are configured to have their values generated on add are set up with [SQLite's AUTOINCREMENT feature](https://sqlite.org/autoinc.html). Starting with EF Core 10, SQLite AUTOINCREMENT can also be enabled or disabled via configuration.

### Configuring AUTOINCREMENT

By convention, integer primary keys are automatically configured with AUTOINCREMENT when they are not part of a composite key and don't have a foreign key on them. However, you may need to explicitly configure a property to use SQLite AUTOINCREMENT when the property has a value conversion from a non-integer type, or when overriding conventions:

[!code-csharp[Main](../../../../samples/core/Sqlite/ValueGeneration/SqliteAutoincrementWithValueConverter.cs?name=SqliteAutoincrementWithValueConverter&highlight=4)]

## Disabling AUTOINCREMENT for default SQLite value generation

AUTOINCREMENT imposes extra CPU, memory, disk space, and disk I/O overhead compared to the default key generation algorithm in SQLite - [ROWID](https://sqlite.org/lang_createtable.html#rowid). The downside of `ROWID` is that it reuses values from deleted rows. If your scenario wouldn't be affected by this, you may want to disable AUTOINCREMENT and use SQLite's default value generation behavior instead. You can do this using the Metadata API:

<!--
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .Metadata.SetValueGenerationStrategy(SqliteValueGenerationStrategy.None);
    }
-->
[!code-csharp[Main](../../../../samples/core/Sqlite/ValueGeneration/SqliteValueGenerationStrategyNone.cs?name=SqliteValueGenerationStrategyNone&highlight=5)]

Alternatively, you can disable value generation entirely:

<!--
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.Id)
            .ValueGeneratedNever();
    }
-->
[!code-csharp[Main](../../../../samples/core/Sqlite/ValueGeneration/SqliteValueGeneratedNever.cs?name=SqliteValueGeneratedNever&highlight=5)]

This means that it's up to the application to supply a value for the property before saving to the database. Note that this still won't disable the default value generation server-side, so non-EF usages could still get a generated value. To [completely disable value generation](https://sqlite.org/lang_createtable.html#rowids_and_the_integer_primary_key) the user can change the column type from `INTEGER` to `INT`.
