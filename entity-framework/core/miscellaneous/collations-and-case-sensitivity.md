---
title: Collations and case sensitivity - EF Core
description: Configuring collations and case-sensitivity in the database and on queries with Entity Framework Core
author: roji
ms.date: 04/27/2020
uid: core/miscellaneous/collations-and-case-sensitivity
---
# Collations and Case Sensitivity

Text processing in databases can be complex, and requires more user attention than one would suspect. For one thing, databases vary considerably in how they handle text; for example, while some databases are case-sensitive by default (e.g. Sqlite, PostgreSQL), others are case-insensitive (SQL Server, MySQL). In addition, because of index usage, case-sensitivity and similar aspects can have a far-reaching impact on query performance: while it may be tempting to use `string.ToLower` to force a case-insensitive comparison in a case-sensitive database, doing so may prevent your application from using indexes. This page details how to configure case sensitivity, or more generally, collations, and how to do so in an efficient way without compromising query performance.

## Introduction to collations

A fundamental concept in text processing is the *collation*, which is a set of rules determining how text values are ordered and compared for equality. For example, while a case-insensitive collation disregards differences between upper- and lower-case letters for the purposes of equality comparison, a case-sensitive collation does not. However, since case-sensitivity is culture-sensitive (e.g. `i` and `I` represent different letters in Turkish), there exist multiple case-insensitive collations, each with its own set of rules. The scope of collations also extends beyond case-sensitivity, to other aspects of character data; in German, for example, it is sometimes (but not always) desirable to treat `ä` and `ae` as identical. Finally, collations also define how text values are *ordered*: while German places `ä` after `a`, Swedish places it at the end of the alphabet.

All text operations in a database use a collation - whether explicitly or implicitly - to determine how the operation compares and orders strings. The actual list of available collations and their naming schemes is database-specific; consult [the section below](#database-specific-information) for links to relevant documentation pages of various databases. Fortunately, databases do generally allow a default collation to be defined at the database or column level, and to explicitly specify which collation should be used for specific operations in a query.

## Database collation

In most database systems, a default collation is defined at the database level; unless overridden, that collation implicitly applies to all text operations occurring within that database. The database collation is typically set at database creation time (via the `CREATE DATABASE` DDL statement), and if not specified, defaults to a some server-level value determined at setup time. For example, the default server-level collation in SQL Server for the "English (United States)" machine locale is `SQL_Latin1_General_CP1_CI_AS`, which is a case-insensitive, accent-sensitive collation. Although database systems usually do permit altering the collation of an existing database, doing so can lead to complications; it is recommended to pick a collation before database creation.

When using EF Core migrations to manage your database schema, the following in your model's `OnModelCreating` method configures a SQL Server database to use a case-sensitive collation:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Collations/Program.cs?name=DatabaseCollation)]

## Column collation

Collations can also be defined on text columns, overriding the database default. This can be useful if certain columns need to be case-insensitive, while the rest of the database needs to be case-sensitive.

When using EF Core migrations to manage your database schema, the following configures the column for the `Name` property to be case-insensitive in a database that is otherwise configured to be case-sensitive:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Collations/Program.cs?name=ColumnCollation)]

## Explicit collation in a query

In some cases, the same column needs to be queried using different collations by different queries. For example, one query may need to perform a case-sensitive comparison on a column, while another may need to perform a case-insensitive comparison on the same column. This can be accomplished by explicitly specifying a collation within the query itself:

[!code-csharp[Main](../../../samples/core/Miscellaneous/Collations/Program.cs?name=SimpleQueryCollation)]

This generates a `COLLATE` clause in the SQL query, which applies a case-sensitive collation regardless of the collation defined at the column or database level:

```sql
SELECT [c].[Id], [c].[Name]
FROM [Customers] AS [c]
WHERE [c].[Name] COLLATE SQL_Latin1_General_CP1_CS_AS = N'John'
```

### Explicit collations and indexes

Indexes are one of the most important factors in database performance - a query that runs efficiently with an index can grind to a halt without that index. Indexes implicitly inherit the collation of their column; this means that all queries on the column are automatically eligible to use indexes defined on that column - provided that the query doesn't specify a different collation. Specifying an explicit collation in a query will generally prevent that query from using an index defined on that column, since the collations would no longer match; it is therefore recommended to exercise caution when using this feature. It is always preferable to define the collation at the column (or database) level, allowing all queries to implicitly use that collation and benefit from any index.

Note that some databases allow the collation to be defined when creating an index (e.g. PostgreSQL, Sqlite). This allows multiple indexes to be defined on the same column, speeding up operations with different collations (e.g. both case-sensitive and case-insensitive comparisons). Consult your database provider's documentation for more details.

> [!WARNING]
> Always inspect the query plans of your queries, and make sure the proper indexes are being used in performance-critical queries executing over large amounts of data. Overriding case-sensitivity in a query via `EF.Functions.Collate` (or by calling `string.ToLower`) can have a very significant impact on your application's performance.

## Translation of built-in .NET string operations

In .NET, string equality is case-sensitive by default: `s1 == s2` performs an ordinal comparison that requires the strings to be identical. Because the default collation of databases varies, and because it is desirable for simple equality to use indexes, EF Core makes no attempt to translate simple equality to a database case-sensitive operation: C# equality is translated directly to SQL equality, which may or may not be case-sensitive, depending on the specific database in use and its collation configuration.

In addition, .NET provides overloads of [`string.Equals`](/dotnet/api/system.string.equals) accepting a [`StringComparison`](/dotnet/api/system.stringcomparison) enum, which allows specifying case-sensitivity and a culture for the comparison. By design, EF Core refrains from translating these overloads to SQL, and attempting to use them will result in an exception. For one thing, EF Core does not know which case-sensitive or case-insensitive collation should be used. More importantly, applying a collation would in most cases prevent index usage, significantly impacting performance for a very basic and commonly-used .NET construct. To force a query to use case-sensitive or case-insensitive comparison, specify a collation explicitly via `EF.Functions.Collate` as [detailed above](#explicit-collations-and-indexes).

## Additional resources

### Database-specific information

* [SQL Server documentation on collations](/sql/relational-databases/collations/collation-and-unicode-support).
* [Microsoft.Data.Sqlite documentation on collations](/dotnet/standard/data/sqlite/collation).
* [PostgreSQL documentation on collations](https://www.postgresql.org/docs/current/collation.html).
* [MySQL documentation on collations](https://dev.mysql.com/doc/refman/en/charset-general.html).

### Other resources

* [.NET Data Community Standup session](https://www.youtube.com/watch?v=OgMhLVa_VfA&list=PLdo4fOcmZ0oX-DBuRG4u58ZTAJgBAeQ-t&index=1), introducing collations and exploring perf and indexing aspects.
