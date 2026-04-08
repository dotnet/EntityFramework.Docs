---
title: Provider-facing changes in EF Core 11 (EF11) - EF Core
description: List of provider-facing changes introduced in Entity Framework Core 11 (EF11)
author: roji
ms.date: 04/08/2026
uid: core/what-is-new/ef-core-11.0/provider-facing-changes
---

# Provider-facing changes in EF Core 11 (EF11)

This page documents noteworthy changes in EF Core 11 which may affect EF providers; none of these should affect end-users of EF - this page is only meant for EF provider maintainers. The list is maintained on a best-effort basis, some changes may be missing.

## Changes

* Collation names are now quoted in SQL, like column and table names ([see #37462](https://github.com/dotnet/efcore/issues/37462)). If your database doesn't support collation name quoting, override `QuerySqlGenerator.VisitSql()` and `MigrationsSqlGenerator.ColumnDefinition()` to revert to the previous behavior, but it's recommended to implement some sort of restricted character validation.
* The `JsonPath` property on `IColumnModification` and `ColumnModificationParameters` has changed from `string?` to the new structured `JsonPath` type ([PR #38038](https://github.com/dotnet/efcore/pull/38038)). The `JsonPath` class provides `Segments`, `Ordinals`, an `IsRoot` property, and an `AppendTo(StringBuilder)` method for rendering the JSONPATH string. Providers that override `UpdateSqlGenerator.AppendUpdateColumnValue()` or otherwise handle JSON partial updates should update their code to use this new type. Where previously you checked for `null` or `"$"`, use `JsonPath is not { IsRoot: false }` instead, and call `JsonPath.AppendTo(stringBuilder)` to write the JSONPATH string representation.

## Test changes

* The inheritance specification tests have been reorganized into a folder of their own ([PR](https://github.com/dotnet/efcore/pull/37410)).
* Query test classes now support non-shared-model tests via a new `QueryFixtureBase` class that all query fixtures extend ([PR #37681](https://github.com/dotnet/efcore/pull/37681)). The previous pattern of extending `SharedStoreFixtureBase` and `IQueryFixtureBase` separately has been replaced. `NonSharedPrimitiveCollectionsQueryTestBase` has been merged into `PrimitiveCollectionsQueryTestBase`, and per-type array tests have been moved to `TypeTestBase.Primitive_collection_in_query`.
