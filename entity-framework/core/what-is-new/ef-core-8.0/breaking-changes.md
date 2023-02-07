---
title: Breaking changes in EF Core 8.0 (EF8) - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 8.0 (EF7)
author: ajcvickers
ms.date: 12/13/2022
uid: core/what-is-new/ef-core-8.0/breaking-changes
---

# Breaking changes in EF Core 8.0 (EF8)

This page will document API and behavior changes that have the potential to break existing applications updating to EF Core 8.0.

## Summary

| **Breaking change**                                                                                                                      | **Impact** |
|:---------------------------------------------------------------------------------------------------------------------------------------- | ---------- |
| [SQL Server `date` and `time` now scaffold to .NET `DateOnly` and `TimeOnly`](#sqlserver-date-time-only)                                 | Medium     |

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
