---
title: Provider-facing changes in EF Core 11 (EF11) - EF Core
description: List of provider-facing changes introduced in Entity Framework Core 11 (EF11)
author: SamMonoRT
ms.date: 06/22/2026
uid: core/what-is-new/ef-core-11.0/provider-facing-changes
---

# Provider-facing changes in EF Core 11 (EF11)

This page documents noteworthy changes in EF Core 11 which may affect EF providers; none of these should affect end-users of EF - this page is only meant for EF provider maintainers. The list is maintained on a best-effort basis, some changes may be missing.

## Changes

* Collation names are now quoted in SQL, like column and table names ([see #37462](https://github.com/dotnet/efcore/issues/37462)). If your database doesn't support collation name quoting, override `QuerySqlGenerator.VisitSql()` and `MigrationsSqlGenerator.ColumnDefinition()` to revert to the previous behavior, but it's recommended to implement some sort of restricted character validation.
* Type mapping has been made generic to support NativeAOT ([PR #38440](https://github.com/dotnet/efcore/pull/38440)). Provider maintainers should: (1) update custom mappings to derive from the new generic mapping base types (`CoreTypeMapping<T>` and `RelationalTypeMapping<T>` where applicable) so default comparers can be created without reflection, and (2) update calls/overrides of `CoreTypeMapping.Clone(...)` and `RelationalTypeMapping.Clone(...)` to remove the old `clrType` argument.
* The `JsonPath` property on `IColumnModification` and `ColumnModificationParameters` has changed from `string?` to the new structured `JsonPath` type ([PR #38038](https://github.com/dotnet/efcore/pull/38038)). The `JsonPath` class provides `Segments`, `Ordinals`, an `IsRoot` property, and an `AppendTo(StringBuilder)` method for rendering the JSONPATH string. Providers that override `UpdateSqlGenerator.AppendUpdateColumnValue()` or otherwise handle JSON partial updates should update their code to use this new type. Where previously you checked for `null` or `"$"`, use `JsonPath is not { IsRoot: false }` instead, and call `JsonPath.AppendTo(stringBuilder)` to write the JSONPATH string representation.
* EF Core now strips no-op SQL CASTs - i.e. `SqlUnaryExpression(Convert)` nodes whose store type matches that of their operand ([see #36247](https://github.com/dotnet/efcore/issues/36247), [PR #38156](https://github.com/dotnet/efcore/pull/38156)). This can flush out imprecise translations which assigned an imprecise store type to a node and then wrapped it with a Convert; because the store types now match, the Convert is stripped, and the imprecise store type may propagate. Review your translations to ensure that the operand of a Convert node has the correct, precise store type.
* Keys and indexes can now traverse complex-type properties ([PR #38192](https://github.com/dotnet/efcore/pull/38192)). The metadata API for indexes and keys has been broadened (e.g., `FindIndex`/`AddIndex` signatures now accept `IReadOnlyProperty`, new `ModelValidator` checks, and `ICSharpHelper` additions). Providers that consume index/key metadata, generate C# for indexes (scaffolding/codegen), or implement custom validation should review and update their code to handle complex-type-spanning keys and indexes.

## Test changes

* The inheritance specification tests have been reorganized into a folder of their own ([PR](https://github.com/dotnet/efcore/pull/37410)).
* Query test classes now support non-shared-model tests via a new `QueryFixtureBase` class that all query fixtures extend ([PR #37681](https://github.com/dotnet/efcore/pull/37681)). The previous pattern of extending `SharedStoreFixtureBase` and `IQueryFixtureBase` separately has been replaced. `NonSharedPrimitiveCollectionsQueryTestBase` has been merged into `PrimitiveCollectionsQueryTestBase`, and per-type array tests have been moved to `TypeTestBase.Primitive_collection_in_query`.
* EF Core's Specification.Tests assemblies have been upgraded from xUnit v2 to xUnit v3 ([PR #38277](https://github.com/dotnet/efcore/pull/38277)). Provider test projects that reference these assemblies and inherit from the base test classes must be migrated to xUnit v3: update to the new `xunit.v3` packages, `Microsoft.DotNet.XUnitV3Extensions`, `Microsoft.Testing.Platform` runner, and `Microsoft.NET.Test.Sdk`; account for `[Collection]`/`ITestOutputHelper` API differences; and change `[ConditionalFact]` to `[Fact]` and `[ConditionalTheory]` to `[Theory]`. When running tests directly with `dotnet exec`, add `--filter-not-trait category=failing --ignore-exit-code 8`.
