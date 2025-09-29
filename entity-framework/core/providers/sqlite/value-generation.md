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

By convention, numeric primary key columns that are configured to have their values generated on add are set up with SQLite's AUTOINCREMENT feature. Starting with EF Core 10, SQLite AUTOINCREMENT is a first-class feature with full support through conventions and the Fluent API.

### Configuring AUTOINCREMENT

By convention, integer primary keys are automatically configured with AUTOINCREMENT when they are not composite and don't have a foreign key on them. However, you may need to explicitly configure a property to use SQLite AUTOINCREMENT when the property has a value conversion from a non-integer type, or when overriding conventions:

[!code-csharp[Main](../../../../samples/core/Sqlite/ValueGeneration/SqliteAutoincrementWithValueConverter.cs?name=SqliteAutoincrementWithValueConverter&highlight=6)]

## Disabling AUTOINCREMENT for default SQLite value generation

In some cases, you may want to disable AUTOINCREMENT and use SQLite's default value generation behavior instead. You can do this using the Metadata API:

[!code-csharp[Main](../../../../samples/core/Sqlite/ValueGeneration/SqliteValueGenerationStrategyNone.cs?name=SqliteValueGenerationStrategyNone&highlight=5)]

Starting with EF Core 10, you can also use the strongly-typed Metadata API:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .Property(p => p.Id)
        .Metadata.SetValueGenerationStrategy(SqliteValueGenerationStrategy.None);
}
```

Alternatively, you can disable value generation entirely:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Property(b => b.Id)
        .ValueGeneratedNever();
}
```

This means that it's up to the application to supply a value for the property before saving to the database. Note that this still won't disable the default value generation server-side, so non-EF usages could still get a generated value. To completely disable value generation the user can change the column type from `INTEGER` to `INT`.

## Migration behavior

When EF Core generates migrations for SQLite AUTOINCREMENT columns, the generated migration will include the `Sqlite:Autoincrement` annotation:

```csharp
migrationBuilder.CreateTable(
    name: "Blogs",
    columns: table => new
    {
        Id = table.Column<int>(type: "INTEGER", nullable: false)
            .Annotation("Sqlite:Autoincrement", true),
        Title = table.Column<string>(type: "TEXT", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_Blogs", x => x.Id);
    });
```

This ensures that the AUTOINCREMENT feature is properly applied when the migration is executed against the SQLite database.
