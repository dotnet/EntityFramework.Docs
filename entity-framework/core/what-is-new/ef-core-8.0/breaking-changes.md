---
title: Breaking changes in EF Core 8.0 (EF8) - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 8.0 (EF8)
author: ajcvickers
ms.date: 8/10/2023
uid: core/what-is-new/ef-core-8.0/breaking-changes
---

# Breaking changes in EF Core 8.0 (EF8)

This page documents API and behavior changes that have the potential to break existing applications updating to EF Core 8.0.

## Summary

| **Breaking change**                                                                                                                      | **Impact** |
|:---------------------------------------------------------------------------------------------------------------------------------------- | ---------- |
| [SQL Server `date` and `time` now scaffold to .NET `DateOnly` and `TimeOnly`](#sqlserver-date-time-only)                                 | Medium     |
| [SQLite `Math` methods now translate to SQL](#sqlite-math)                                                                               | Low        |

## Medium-impact changes

<a name="sqlserver-date-time-only"></a>

### SQL Server `date` and `time` now scaffold to .NET `DateOnly` and `TimeOnly`

[Tracking Issue #24507](https://github.com/dotnet/efcore/issues/24507)

#### Old behavior

Previously, when scaffolding a SQL Server database with `date` or `time` columns, EF would generate entity properties with types <xref:System.DateTime> and <xref:System.TimeSpan>.

#### New behavior

Starting with EF Core 8.0, `date` and `time` are scaffolded as <xref:System.DateOnly> and <xref:System.TimeOnly>.

#### Why

<xref:System.DateOnly> and <xref:System.TimeOnly> were introduced in .NET 6.0, and are a perfect match for mapping the database date and time types. <xref:System.DateTime> notably contains a time component that goes unused and can cause confusion when mapping it to `date`, and <xref:System.TimeSpan> represents a time interval - possibly including days - rather than a time of day at which an event occurs. Using the new types prevents bugs and confusion, and provides clarity of intent.

#### Mitigations

This change only affects users which regularly re-scaffold their database into an EF code model ("database-first" flow).

It is recommended to react to this change by modifying your code to use the newly scaffolded <xref:System.DateOnly> and <xref:System.TimeOnly> types. However, if that isn't possible, you can edit the scaffolding templates to revert to the previous mapping. To do this, set up the templates as described on [this page](xref:core/managing-schemas/scaffolding/templates). Then, edit the `EntityType.t4` file, find where the entity properties get generated (search for `property.ClrType`), and change the code to the following:

```c#
        var clrType = property.GetColumnType() switch
        {
            "date" when property.ClrType == typeof(DateOnly) => typeof(DateTime),
            "date" when property.ClrType == typeof(DateOnly?) => typeof(DateTime?),
            "time" when property.ClrType == typeof(TimeOnly) => typeof(TimeSpan),
            "time" when property.ClrType == typeof(TimeOnly?) => typeof(TimeSpan?),
            _ => property.ClrType
        };

        usings.AddRange(code.GetRequiredUsings(clrType));

        var needsNullable = Options.UseNullableReferenceTypes && property.IsNullable && !clrType.IsValueType;
        var needsInitializer = Options.UseNullableReferenceTypes && !property.IsNullable && !clrType.IsValueType;
#>
    public <#= code.Reference(clrType) #><#= needsNullable ? "?" : "" #> <#= property.Name #> { get; set; }<#= needsInitializer ? " = null!;" : "" #>
<#
```

## Low-impact changes

<a name="sqlite-math"></a>

### SQLite `Math` methods now translate to SQL

[Tracking Issue #18843](https://github.com/dotnet/efcore/issues/18843)

#### Old Behavior

Previously only the Abs, Max, Min, and Round methods on `Math` were translated to SQL. All other members would be evaluated on the client if they appeared in the final Select expression of a query.

#### New behavior

In EF Core 8.0, all `Math` methods with corresponding [SQLite math functions](https://sqlite.org/lang_mathfunc.html) are translated to SQL.

These math functions have been enabled in the native SQLite library that we provide by default (through our dependency on the SQLitePCLRaw.bundle_e_sqlite3 NuGet package). They have also been enabled in the library provided by SQLitePCLRaw.bundle_e_sqlcipher. If you're using one of these libraries, your application should not be affected by this change.

There is a chance, however, that applications including the native SQLite library by other means may not enable the math functions. In these cases, the `Math` methods will be translated to SQL and encounter *no such function* errors when executed.

#### Why

SQLite added built-in math functions in version 3.35.0. Even though they're disabled by default, they've become pervasive enough that we decided to provide default translations for them in our EF Core SQLite provider.

We also collaborated with Eric Sink on the SQLitePCLRaw project to enable math functions in all of the native SQLite libraries provided as part of that project.

#### Mitigations

The simplest way to fix breaks is, when possible, to enable the math function is the native SQLite library by specifying the [SQLITE_ENABLE_MATH_FUNCTIONS](https://sqlite.org/compile.html#enable_math_functions) compile-time option.

If you don't control compilation of the native library, you can also fix breaks by create the functions yourself at runtime using the [Microsoft.Data.Sqlite](/dotnet/standard/data/sqlite/user-defined-functions) APIs.

```csharp
sqliteConnection
    .CreateFunction<double, double, double>(
        "pow",
        Math.Pow,
        isDeterministic: true);
```

Alternatively, you can force client-evaluation by splitting the Select expression into two parts separated by `AsEnumerable`.

```csharp
// Before
var query = dbContext.Cylinders
    .Select(
        c => new
        {
            Id = c.Id
            // May throw "no such function: pow"
            Volume = Math.PI * Math.Pow(c.Radius, 2) * c.Height
        });

// After
var query = dbContext.Cylinders
    // Select the properties you'll need from the database
    .Select(
        c => new
        {
            c.Id,
            c.Radius,
            c.Height
        })
    // Switch to client-eval
    .AsEnumerable()
    // Select the final results
    .Select(
        c => new
        {
            Id = c.Id,
            Volume = Math.PI * Math.Pow(c.Radius, 2) * c.Height
        });
```
