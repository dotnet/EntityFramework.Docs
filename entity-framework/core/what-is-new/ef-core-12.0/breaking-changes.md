---
title: Breaking changes in EF Core 12 (EF12) - EF Core
description: List of breaking changes introduced in Entity Framework Core 12 (EF12)
author: ajcvickers
ms.date: 11/14/2026
uid: core/what-is-new/ef-core-12.0/breaking-changes
---

# Breaking changes in EF Core 12 (EF12)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 11 to EF Core 12. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 11](xref:core/what-is-new/ef-core-11.0/breaking-changes)
- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

| **Breaking change**                                                                                              | **Impact** |
|:---------------------------------------------------------------------------------------------------------------- | -----------|
| [A filtered index no longer replaces the index on a foreign key](#filtered-index-foreign-key-index)             | Low        |

## Low-impact changes

<a name="filtered-index-foreign-key-index"></a>

### A filtered index no longer replaces the index on a foreign key

[Tracking Issue #24237](https://github.com/dotnet/efcore/issues/24237)

#### Old behavior

By convention, EF creates an index on the properties of each foreign key. It doesn't create one when another index already starts with the same properties, and it removes the foreign key's index if such an index is added later. For example, an index on `(PeriodId, CompanyId)` made a separate index on the foreign key property `PeriodId` unnecessary.

This applied even when the other index had a filter:

```csharp
modelBuilder.Entity<Invoice>()
    .HasIndex(i => new { i.PeriodId, i.CompanyId })
    .HasFilter("[CompanyId] > 0");
```

Here, with `PeriodId` as a foreign key, no index on `PeriodId` alone was created. Rows that don't satisfy the filter weren't in any index that a lookup on `PeriodId` could use.

#### New behavior

An index that has a filter, and starts with a foreign key's properties but has more properties after them, no longer counts as covering the foreign key. EF creates the foreign key's own index, in addition to the filtered one. This applies whether the filtered index is added before or after the foreign key, and it is re-evaluated when the filter is added, changed or removed.

A model like the one above now has two indexes: the filtered `(PeriodId, CompanyId)` index and a new `IX_Invoices_PeriodId`. The first migration generated after updating contains a `CreateIndex` for each such foreign key.

This also applies to a filter that EF sets by convention, when it excludes rows that a lookup on the foreign key needs. The filter that SQL Server adds to unique indexes over nullable columns (`[A] IS NOT NULL AND [B] IS NOT NULL`) excludes the rows where any of those columns is null. If one of the nullable columns isn't a foreign key column, those rows are excluded even though a lookup on the foreign key needs them. For example, with SQL Server:

```csharp
modelBuilder.Entity<Invoice>()
    .HasIndex(i => new { i.PeriodId, i.CompanyId })   // both int?
    .IsUnique();
```

Here SQL Server filters the index with `[PeriodId] IS NOT NULL AND [CompanyId] IS NOT NULL`, so a new index on `PeriodId` is created if it is a foreign key.

Some cases behave as before:

- An index on exactly the foreign key's properties is still the foreign key's index, whatever its filter and whether or not it has a name. A filter can't be interpreted, and a second index on the same properties would duplicate it. This includes the index that `dotnet ef dbcontext scaffold` generates for an optional one-to-one relationship, such as `HasIndex(i => i.PeriodId, "IX_Invoices_PeriodId").IsUnique().HasFilter("([PeriodId] IS NOT NULL)")`.
- A filter that EF sets by convention still lets the index cover the foreign key if every nullable column it excludes is one of the foreign key's columns, since a lookup on the foreign key never needs rows with nulls in them. Existing models whose only filters are the ones SQL Server adds, on indexes that start with the foreign key and don't have other nullable columns, are unchanged.
- Table-specific filters, which come from the root of a table-sharing hierarchy rather than from the index itself, aren't taken into account.

The new `RelationalForeignKeyIndexConvention` replaces `ForeignKeyIndexConvention` for relational providers. `ForeignKeyIndexConvention` gets a protected virtual `IsPartial(IConventionIndex, IReadOnlyList<IConventionPropertyBase>)` method, which returns `false` by default, and a protected virtual `OnIndexCoverageChanged` method.

#### Why

An index with a filter contains only the rows that satisfy the filter. Queries and constraint checks on a foreign key, such as finding the dependents of a principal or checking for dependents when a principal is deleted, need every row, so the filtered index can't serve them. Without the foreign key's index, such queries scan the table. See [#24237](https://github.com/dotnet/efcore/issues/24237).

#### Mitigations

- In most cases the new index is wanted, and no action is needed other than applying the migration. On a large table, creating the index can take time and may lock the table depending on the database, so review the generated migration before applying it.
- If a SQL Server unique index over nullable columns gets a foreign key index you don't want, make its other columns non-nullable if the data allows it, since the filter then only excludes rows with nulls in the foreign key's columns.
- To keep the previous behavior for your application, use a custom convention that derives from `RelationalForeignKeyIndexConvention` and overrides `IsPartial` to return `false`.
