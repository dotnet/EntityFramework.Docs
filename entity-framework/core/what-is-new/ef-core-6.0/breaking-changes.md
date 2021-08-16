---
title: Breaking changes in EF Core 6.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 6.0
author: ajcvickers
ms.date: 08/10/2021
uid: core/what-is-new/ef-core-6.0/breaking-changes
---

# Breaking changes in EF Core 6.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 6.0.0.

## Summary

| **Breaking change**                                                                                                                   | **Impact** |
|:--------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Cleaned up mapping between DeleteBehavior and ON DELETE values](#on-delete)                                                          | Low        |
| [Removed last ORDER BY when joining for collections](#last-order-by)                                                                  | Low        |

## Low-impact changes

<a name="on-delete"></a>

### Cleaned up mapping between DeleteBehavior and ON DELETE values

[Tracking Issue #21252](https://github.com/dotnet/efcore/issues/21252)

#### Old behavior

Some of the mappings between a relationship's `OnDelete()` behavior and the foreign keys' `ON DELETE` behavior in the database were inconsistent in both Migrations and Scaffolding.

#### New behavior

The following table illustrates the changes for **Migrations**.

OnDelete()     | ON DELETE
-------------- | ---------
NoAction       | NO ACTION
ClientNoAction | NO ACTION
Restrict       | RESTRICT
Cascasde       | CASCADE
ClientCascade  | ~~RESTRICT~~ **NO ACTION**
SetNull        | SET NULL
ClientSetNull  | ~~RESTRICT~~ **NO ACTION**

The changes for **Scaffolding** are as follows.

ON DELETE | OnDelete()
--------- | ----------
NO ACTION | ClientSetNull
RESTRICT  | ~~ClientSetNull~~ **Restrict**
CASCADE   | Cascade
SET NULL  | SetNull

#### Why

The new mappings are more consistent. The default database behavior of NO ACTION is now preferred over the more restrictive and less performant RESTRICT behavior.

#### Mitigations

The default OnDelete() behavior of optional relationships is ClientSetNull. Its mapping has changed from RESTRICT to NO ACTION. This may cause a lot of operations to be generated in your first migration added after upgrading to EF Core 6.0.

You can choose to either apply these operations or manually remove them from the migration since they have no functional impact on EF Core.

SQL Server doesn't support RESTRICT, so these foreign keys were already created using NO ACTION. The migration operations will have no affect on SQL Server and are safe to remove.

<a name="last-order-by"></a>

### Removed last ORDER BY when joining for collections

[Tracking Issue #19828](https://github.com/dotnet/efcore/issues/19828)

#### Old behavior

When performing SQL JOINs on collections (one-to-many relationships), EF Core used to add an ORDER BY for each key column of the joined table. For example, loading all Blogs with their related Posts was done via the following SQL:

```sql
SELECT [b].[BlogId], [b].[Name], [p].[PostId], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId], [p].[PostId]
```

These orderings are necessary for proper materialization of the entities.

#### New behavior

The very last ORDER BY for a collection join is now omitted:

```sql
SELECT [b].[BlogId], [b].[Name], [p].[PostId], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId]
```

An ORDER BY for the Post's ID column is no longer generated.

#### Why

Every ORDER BY imposes additional work at the database side, and the last ordering isn't necessary for EF Core's materialization needs. Data shows that removing this last ordering can produce a significant performance improvement in some scenarios.

#### Mitigations

If your application expects joined entities to be returned in a particular order, make that explicit by adding a LINQ `OrderBy` operator to your query.
