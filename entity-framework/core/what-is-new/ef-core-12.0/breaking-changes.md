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

An index that has a filter set explicitly, with `HasFilter` or by calling the metadata API, no longer counts as covering a foreign key. EF creates the foreign key's own index, in addition to the filtered one. This applies whether the filtered index is added before or after the foreign key, and it is re-evaluated when the filter is added, changed or removed.

A model like the one above now has two indexes: the filtered `(PeriodId, CompanyId)` index and a new `IX_Invoices_PeriodId`. The first migration generated after updating contains a `CreateIndex` for each such foreign key.

Some cases behave as before:

- Filters that EF sets by convention are ignored. In particular, the filter that SQL Server adds to unique indexes over nullable columns (`[A] IS NOT NULL AND [B] IS NOT NULL`) doesn't cause new indexes, so existing models that only have those filters are unchanged.
- An unnamed index with a filter on exactly the foreign key's properties is still the foreign key's index, because a model can't contain two unnamed indexes on the same properties. Giving the filtered index a name makes EF create a separate index for the foreign key.
- Table-specific filters, which come from the root of a table-sharing hierarchy rather than from the index itself, aren't taken into account.

The new `RelationalForeignKeyIndexConvention` replaces `ForeignKeyIndexConvention` for relational providers. `ForeignKeyIndexConvention` gets a protected virtual `IsPartial` method, which returns `false` by default, and a protected virtual `ProcessIndexCoverageChanged` method.

#### Why

An index with a filter contains only the rows that satisfy the filter. Queries and constraint checks on a foreign key, such as finding the dependents of a principal or checking for dependents when a principal is deleted, need every row, so the filtered index can't serve them. Without the foreign key's index, such queries scan the table. See [#24237](https://github.com/dotnet/efcore/issues/24237).

#### Mitigations

- In most cases the new index is wanted, and no action is needed other than applying the migration. On a large table, creating the index can take time and may lock the table depending on the database, so review the generated migration before applying it.
- To keep the previous behavior for your application, use a custom convention that derives from `RelationalForeignKeyIndexConvention` and overrides `IsPartial` to return `false`.
