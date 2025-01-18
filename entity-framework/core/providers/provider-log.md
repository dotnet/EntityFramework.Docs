---
title: Log of provider-impacting changes - EF Core
description: A log of changes in Entity Framework Core which impact providers
author: SamMonoRT
ms.date: 08/08/2018
uid: core/providers/provider-log
---

# Provider-impacting changes

> [!IMPORTANT]
> This page is no longer being updated. Changes that provider authors need to be aware of are marked with [`providers-beware`](https://github.com/dotnet/efcore/labels/providers-beware).

This page contains links to pull requests made on the EF Core repo that may require authors of other database providers to react. The intention is to provide a starting point for authors of existing third-party database providers when updating their provider to a new version.

We are starting this log with changes from 2.1 to 2.2. Prior to 2.1 we used the [`providers-beware`](https://github.com/dotnet/efcore/labels/providers-beware) and [`providers-fyi`](https://github.com/dotnet/efcore/labels/providers-fyi) labels on our issues and pull requests.

## 2.2 ---> 3.x

Note that many of the [application-level breaking changes](xref:core/what-is-new/ef-core-3.x/breaking-changes) will also impact providers.

* <https://github.com/dotnet/efcore/pull/14022>
  * Removed obsolete APIs and collapsed optional parameter overloads
  * Removed DatabaseColumn.GetUnderlyingStoreType()
* <https://github.com/dotnet/efcore/pull/14589>
  * Removed obsolete APIs
* <https://github.com/dotnet/efcore/pull/15044>
  * Subclasses of CharTypeMapping may have been broken due to behavior changes required to fixing a couple bugs in the base implementation.
* <https://github.com/dotnet/efcore/pull/15090>
  * Added a base class for IDatabaseModelFactory and updated it to use a paramater object to mitigate future breaks.
* <https://github.com/dotnet/efcore/pull/15123>
  * Used parameter objects in MigrationsSqlGenerator to mitigate future breaks.
* <https://github.com/dotnet/efcore/pull/14972>
  * Explicit configuration of log levels required some changes to APIs that providers may be using. Specifically, if providers are using the logging infrastructure directly, then this change may break that use. Also, Providers that use the infrastructure (which will be public) going forward will need to derive from `LoggingDefinitions` or `RelationalLoggingDefinitions`. See the SQL Server and in-memory providers for examples.
* <https://github.com/dotnet/efcore/pull/15091>
  * Core, Relational, and Abstractions resource strings are now public.
  * `CoreLoggerExtensions` and `RelationalLoggerExtensions` are now public. Providers should use these APIs when logging events that are defined at the core or relational level. Do not access logging resources directly; these are still internal.
  * `IRawSqlCommandBuilder` has changed from a singleton service to a scoped service
  * `IMigrationsSqlGenerator` has changed from a singleton service to a scoped service
* <https://github.com/dotnet/efcore/pull/14706>
  * The infrastructure for building relational commands has been made public so it can be safely used by providers and refactored slightly.
* <https://github.com/dotnet/efcore/pull/14733>
  * `ILazyLoader` has changed from a scoped service to a transient service
* <https://github.com/dotnet/efcore/pull/14610>
  * `IUpdateSqlGenerator` has changed from a scoped service to a singleton service
  * Also, `ISingletonUpdateSqlGenerator` has been removed
* <https://github.com/dotnet/efcore/pull/15067>
  * A lot of internal code that was being used by providers has now been made public
  * It should no longer be necssary to reference `IndentedStringBuilder` since it has been factored out of the places that exposed it
  * Usages of `NonCapturingLazyInitializer` should be replaced with `LazyInitializer` from the BCL
* <https://github.com/dotnet/efcore/pull/14608>
  * This change is fully covered in the application breaking changes document. For providers, this may be more impacting because testing EF core can often result in hitting this issue, so test infrastructure has changed to make that less likely.
* <https://github.com/dotnet/efcore/issues/13961>
  * `EntityMaterializerSource` has been simplified
* <https://github.com/dotnet/efcore/pull/14895>
  * StartsWith translation has changed in a way that providers may want/need to react
* <https://github.com/dotnet/efcore/pull/15168>
  * Convention set services have changed. Providers should now inherit from either "ProviderConventionSet" or "RelationalConventionSet".
  * Customizations can be added through `IConventionSetCustomizer` services, but this is intended to be used by other extensions, not providers.
  * Conventions used at runtime should be resolved from `IConventionSetBuilder`.
* <https://github.com/dotnet/efcore/pull/15288>
  * Data seeding has been refactored into a public API to avoid the need to use internal types. This should only impact non-relational providers, since seeding is handled by the base relational class for all relational providers.

## 2.1 ---> 2.2

### Test-only changes

* <https://github.com/dotnet/efcore/pull/12057> - Allow customizable SQL delimeters in tests
  * Test changes that allow non-strict floating point comparisons in BuiltInDataTypesTestBase
  * Test changes that allow query tests to be re-used with different SQL delimeters
* <https://github.com/dotnet/efcore/pull/12072> - Add DbFunction tests to the relational specification tests
  * Such that these tests can be run against all database providers
* <https://github.com/dotnet/efcore/pull/12362> - Async test cleanup
  * Remove `Wait` calls, unneeded async, and renamed some test methods
* <https://github.com/dotnet/efcore/pull/12666> - Unify logging test infrastructure
  * Added `CreateListLoggerFactory` and removed some previous logging infrastructure, which will require providers using these tests to react
* <https://github.com/dotnet/efcore/pull/12500> - Run more query tests both synchronously and asynchronously
  * Test names and factoring has changed, which will require providers using these tests to react
* <https://github.com/dotnet/efcore/pull/12766> - Renaming navigations in the ComplexNavigations model
  * Providers using these tests may need to react
* <https://github.com/dotnet/efcore/pull/12141> - Return the context to the pool instead of disposing in functional tests
  * This change includes some test refactoring which may require providers to react

### Test and product code changes

* <https://github.com/dotnet/efcore/pull/12109> - Consolidate RelationalTypeMapping.Clone methods
  * Changes in 2.1 to the RelationalTypeMapping allowed for a simplification in derived classes. We don't believe this was breaking to providers, but providers can take advantage of this change in their derived type mapping classes.
* <https://github.com/dotnet/efcore/pull/12069> - Tagged or named queries
  * Adds infrastructure for tagging LINQ queries and having those tags show up as comments in the SQL. This may require providers to react in SQL generation.
* <https://github.com/dotnet/efcore/pull/13115> - Support spatial data via NTS
  * Allows type mappings and member translators to be registered outside of the provider
    * Providers must call base.FindMapping() in their ITypeMappingSource implementation for it to work
  * Follow this pattern to add spatial support to your provider that is consistent across providers.
* <https://github.com/dotnet/efcore/pull/13199> - Add enhanced debugging for service provider creation
  * Allows DbContextOptionsExtensions to implement a new interface that can help people understand why the internal service provider is being re-built
* <https://github.com/dotnet/efcore/pull/13289> - Adds CanConnect API for use by health checks
  * This PR adds the concept of `CanConnect` which will be used by ASP.NET Core health checks to determine if the database is available. By default, the relational implementation just calls `Exist`, but providers can implement something different if necessary. Non-relational providers will need to implement the new API in order for the health check to be usable.
* <https://github.com/dotnet/efcore/pull/13306> - Update base RelationalTypeMapping to not set DbParameter Size
  * Stop setting Size by default since it can cause truncation. Providers may need to add their own logic if Size needs to be set.
* <https://github.com/dotnet/efcore/pull/13372> - RevEng: Always specify column type for decimal columns
  * Always configure column type for decimal columns in scaffolded code rather than configuring by convention.
  * Providers should not require any changes on their end.
* <https://github.com/dotnet/efcore/pull/13469> - Adds CaseExpression for generating SQL CASE expressions
* <https://github.com/dotnet/efcore/pull/13648> - Adds the ability to specify type mappings on SqlFunctionExpression to improve store type inference of arguments and results.
