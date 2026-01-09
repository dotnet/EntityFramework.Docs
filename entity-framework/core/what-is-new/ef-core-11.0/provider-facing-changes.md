---
title: Provider-facing changes in EF Core 11 (EF11) - EF Core
description: List of provider-facing changes introduced in Entity Framework Core 11 (EF11)
author: roji
ms.date: 01/16/2026
uid: core/what-is-new/ef-core-11.0/provider-facing-changes
---

# Provider-facing changes in EF Core 11 (EF11)

This page documents noteworthy changes in EF Core 11 which may affect EF providers; none of these should affect end-users of EF - this page is only meant for EF provider maintainers. The list is maintained on a best-effort basis, some changes may be missing.

## Changes

* Collation names are now quoted in SQL, like column and table names ([see #37462](https://github.com/dotnet/efcore/issues/37462)). If your database doesn't support collation name quoting, override `QuerySqlGenerator.VisitSql()` and `MigrationsSqlGenerator.ColumnDefinition()` to revert to the previous behavior, but it's recommended to implement some sort of restricted character validation.

## Test changes

* The inheritance specification tests have been reorganized into a folder of their own ([PR](https://github.com/dotnet/efcore/pull/37410)).
