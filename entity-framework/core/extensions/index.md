---
title: Tools & Extensions - EF Core
description: External tools and extensions for Entity Framework Core
author: ErikEJ
ms.date: 02/21/2021
uid: core/extensions/index
---

# EF Core Tools & Extensions

These tools and extensions provide additional functionality for Entity Framework Core 2.1 and later.

> [!IMPORTANT]
> Extensions are built by a variety of sources and aren't maintained as part of the Entity Framework Core project. When considering a third party extension, be sure to evaluate its quality, licensing, compatibility, support, etc. to ensure it meets your requirements. In particular, an extension built for an older version of EF Core may need updating before it will work with the latest versions.

## Tools

### LLBLGen Pro

LLBLGen Pro is an entity modeling solution with support for Entity Framework and Entity Framework Core. It lets you easily define your entity model and map it to your database, using database first or model first, so you can get started writing queries right away. For EF Core: 2, 3, 5.

[Website](https://www.llblgen.com/)

### Devart Entity Developer

Entity Developer is a powerful O/RM designer for ADO.NET Entity Framework, NHibernate, LinqConnect, Telerik Data Access, and LINQ to SQL. It supports designing EF Core models visually, using model first or database first approaches, and C# or Visual Basic code generation. For EF Core: 2, 3, 5.

[Website](https://www.devart.com/entitydeveloper/)

### nHydrate ORM for Entity Framework

An O/RM that creates strongly-typed, extendable classes for Entity Framework. The generated code is Entity Framework Core. There is no difference. This is not a replacement for EF or a custom O/RM. It is a visual, modeling layer that allows a team to manage complex database schemas. It works well with SCM software like Git, allowing multi-user access to your model with minimal conflicts. The installer tracks model changes and creates upgrade scripts. For EF Core: 3.

[Github repository](https://github.com/nHydrate/nHydrate)

### EF Core Power Tools

EF Core Power Tools is a Visual Studio extension that exposes various EF Core design-time tasks in a simple user interface. It includes reverse engineering of DbContext and entity classes from existing databases and [SQL Server DACPACs](/sql/relational-databases/data-tier-applications/data-tier-applications), management of database migrations, and model visualizations. For EF Core: 3, 5, 6.

[GitHub wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki)

### Entity Framework Visual Editor

Entity Framework Visual Editor is a Visual Studio extension that adds an O/RM designer for visual design of EF 6, and EF Core classes. Code is generated using T4 templates so can be customized to suit any needs. It supports inheritance, unidirectional and bidirectional associations, enumerations, and the ability to color-code your classes and add text blocks to explain potentially arcane parts of your design. For EF Core: 2, 3, 5.

[Marketplace](https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner)

### CatFactory

CatFactory is a scaffolding engine for .NET Core that can automate the generation of DbContext classes, entities, mapping configurations, and repository classes from a SQL Server database. For EF Core: 2.

[GitHub repository](https://github.com/hherzl/CatFactory.EntityFrameworkCore)

### LoreSoft's Entity Framework Core Generator

Entity Framework Core Generator (efg) is a .NET Core CLI tool that can generate EF Core models from an existing database, much like `dotnet ef dbcontext scaffold`, but it also supports safe code [regeneration](https://efg.loresoft.com/en/latest/regeneration/) via region replacement or by parsing mapping files. This tool supports generating view models, validation, and object mapper code. For EF Core: 2.

[Tutorial](https://www.loresoft.com/Generate-ASP-NET-Web-API) | [Documentation](https://efg.loresoft.com/en/latest/)

### Geco

Geco (Generator Console) is a simple code generator based on a console project, that runs on .NET Core and uses C# interpolated strings for code generation. Geco includes a reverse model generator for EF Core with support for pluralization, singularization, and editable templates. It also provides a seed data script generator, a script runner, and a database cleaner. For EF Core: 2.

[GitHub repository](https://github.com/iQuarc/Geco)

## Extensions

### Microsoft.EntityFrameworkCore.AutoHistory

A plugin library that enables automatically recording the data changes performed by EF Core into a history table. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/Arch/AutoHistory) | [NuGet](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.AutoHistory)

### EFCoreSecondLevelCacheInterceptor

Second level caching is a query cache. The results of EF commands will be stored in the cache, so that the same EF commands will retrieve their data from the cache rather than executing them against the database again. For EF Core: 3, 5.

[GitHub repository](https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor) | [NuGet](https://www.nuget.org/packages/EFCoreSecondLevelCacheInterceptor)

### EntityFrameworkCore.Scaffolding.Handlebars

Allows customization of classes reverse engineered from an existing database using the Entity Framework Core tool chain with Handlebars templates. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Scaffolding.Handlebars)

### NeinLinq.EntityFrameworkCore

NeinLinq extends LINQ providers such as Entity Framework to enable reusing functions, rewriting queries, and building dynamic queries using translatable predicates and selectors. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/axelheer/nein-linq) | [NuGet](https://www.nuget.org/packages/NeinLinq.EntityFrameworkCore)

### Microsoft.EntityFrameworkCore.UnitOfWork

A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple databases with distributed transaction supported. For EF Core: 2, 3.

[GitHub repository](https://github.com/Arch/UnitOfWork)

### EFCore.BulkExtensions

EF Core extensions for Bulk operations (Insert, Update, Delete). For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/borisdj/EFCore.BulkExtensions) | [NuGet](https://www.nuget.org/packages/EFCore.BulkExtensions)

### Bricelam.EntityFrameworkCore.Pluralizer

Adds design-time pluralization. For EF Core: 2, 3, 5, 6.

[GitHub repository](https://github.com/bricelam/EFCore.Pluralizer) | [NuGet](https://www.nuget.org/packages/Bricelam.EntityFrameworkCore.Pluralizer)

### Toolbelt.EntityFrameworkCore.IndexAttribute

Revival of [Index] attribute (with extension for model building). For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/jsakamoto/EntityFrameworkCore.IndexAttribute) | [NuGet](https://www.nuget.org/packages/Toolbelt.EntityFrameworkCore.IndexAttribute)

### Verify.EntityFramework

Extends [Verify](https://github.com/VerifyTests/Verify) to allow snapshot testing with Entity Framework. For EF Core: 3, 5.

[GitHub repository](https://github.com/VerifyTests/Verify.EntityFramework) | [NuGet](https://www.nuget.org/packages/Verify.EntityFramework)

### LocalDb

Provides a wrapper around [SQL Server Express LocalDB](/sql/database-engine/configure-windows/sql-server-express-localdb) to simplify running tests against Entity Framework. For EF Core: 3, 5.

[GitHub repository](https://github.com/SimonCropp/LocalDb) | [NuGet](https://www.nuget.org/packages/EfLocalDb)

### EFCore.TemporalSupport

An implementation of temporal support. For EF Core: 2.

[GitHub repository](https://github.com/cpoDesign/EFCore.TemporalSupport) | [NuGet](https://www.nuget.org/packages/EFCoreTemporalSupport)

### EfCoreTemporalTable

> [!NOTE]
> SQL Server temporal tables are supported directly within EF Core as of [EF Core 6](/ef/core/what-is-new/ef-core-6.0/whatsnew#sql-server-temporal-tables).

Easily perform temporal queries on your favorite database using introduced extension methods: `AsTemporalAll()`, `AsTemporalAsOf(date)`, `AsTemporalFrom(startDate, endDate)`, `AsTemporalBetween(startDate, endDate)`, `AsTemporalContained(startDate, endDate)`. For EF Core: 3, 5.

[GitHub repository](https://github.com/glautrou/EfCoreTemporalTable) | [NuGet](https://www.nuget.org/packages/EfCoreTemporalTable)

### EntityFrameworkCore.TemporalTables

> [!NOTE]
> SQL Server temporal tables are supported directly within EF Core as of [EF Core 6](/ef/core/what-is-new/ef-core-6.0/whatsnew#sql-server-temporal-tables).

Extension library for Entity Framework Core which allows developers who use SQL Server to easily use temporal tables. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/findulov/EntityFrameworkCore.TemporalTables) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.TemporalTables)

### EntityFrameworkCore.Cacheable

A high-performance second-level query cache. For EF Core: 2.

[GitHub repository](https://github.com/SteffenMangold/EntityFrameworkCore.Cacheable) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Cacheable)

### EntityFrameworkCore.NCache

NCache Entity Framework Core Provider is a distributed second level cache provider for caching query results. The distributed architecture of NCache makes it more scalable and highly available. For EF Core: 2, 3.

[Website](https://www.alachisoft.com/ncache/ef-core-cache.html) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.NCache)

### EntityFrameworkCore.Projectables

Flexible projection magic for EF Core. Use properties, methods, and extension methods in your query without client evaluation. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/koenbeuk/EntityFrameworkCore.Projectables) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Projectables)

### EntityFrameworkCore.Triggered

Triggers for EF Core. Respond to changes in your DbContext before and after they are committed to the database. Triggers are fully asynchronous and support dependency injection, inheritance, cascading and more. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/koenbeuk/EntityFrameworkCore.Triggered) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Triggered)

### Entity Framework Plus

Extends your DbContext with features such as: Include Filter, Auditing, Caching, Query Future, Batch Delete, Batch Update, and more. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/zzzprojects/EntityFramework-Plus) | [NuGet](https://www.nuget.org/packages/Z.EntityFramework.Plus.EFCore)

### Entity Framework Extensions

Extends your DbContext with high-performance bulk operations: BulkSaveChanges, BulkInsert, BulkUpdate, BulkDelete, BulkMerge, and more. For EF Core: 2, 3, 5.

[Website](https://entityframework-extensions.net/) | [NuGet](https://www.nuget.org/packages/Z.EntityFramework.Plus.EFCore)

### Expressionify

Add support for calling extension methods in LINQ lambdas. For EF Core: 3, 5.

[GitHub repository](https://github.com/ClaveConsulting/Expressionify) | [NuGet](https://www.nuget.org/packages/Clave.Expressionify)

### EntityLinq

Alternative (not MS based) Language Integrated Query (LINQ) technology for relational databases. It allows you to use C# to write strongly typed SQL queries. For EF Core: 3, 5, 6.

- Full C# support for query creation: multiple statements inside lambda, variables, functions, etc.
- No semantic gap with SQL. EntityLinq declares SQL statements (like `SELECT`, `FROM`, `WHERE`) as first class C# methods, combining familiar syntax with intellisense, type safety and refactoring.

As a result SQL becomes just "another" class library exposing its API locally, literally *"Language Integrated SQL"*.

[Website](https://entitylinq.com/) | [NuGet](https://www.nuget.org/packages/Streamx.Linq.SQL.EFCore)

### Ramses

Life cycle hooks (for SaveChanges). For EF Core: 2, 3.

[GitHub repository](https://github.com/JValck/Ramses) | [NuGet](https://www.nuget.org/packages/Ramses)

### EFCore.NamingConventions

This will automatically make all your table and column names have snake_case, all UPPER or all lower case naming. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/efcore/EFCore.NamingConventions) | [NuGet](https://www.nuget.org/packages/EFCore.NamingConventions)

### EFCore.CheckConstraints

This plugin allows you to opt into some check constraints - just activate it and they'll automatically get created for you. For EF Core: 5, 6.

[GitHub repository](https://github.com/efcore/EFCore.CheckConstraints) | [NuGet](https://www.nuget.org/packages/EFCore.CheckConstraints)

### SimplerSoftware.EntityFrameworkCore.SqlServer.NodaTime

Adds native support to EntityFrameworkCore for SQL Server for the NodaTime types. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/StevenRasmussen/EFCore.SqlServer.NodaTime) | [NuGet](https://www.nuget.org/packages/SimplerSoftware.EntityFrameworkCore.SqlServer.NodaTime)

### Dabble.EntityFrameworkCore.Temporal.Query

LINQ extensions to Entity Framework Core 3.1 to support Microsoft SQL Server Temporal Table Querying. For EF Core: 3.

[GitHub repository](https://github.com/Adam-Langley/efcore-temporal-query) | [NuGet](https://www.nuget.org/packages/Dabble.EntityFrameworkCore.Temporal.Query)

### EntityFrameworkCore.SqlServer.HierarchyId

Adds hierarchyid support to the SQL Server EF Core provider. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/efcore/EFCore.SqlServer.HierarchyId) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.SqlServer.HierarchyId)

### linq2db.EntityFrameworkCore

Alternative translator of LINQ queries to SQL expressions. For EF Core: 2, 3, 5, 6.

Includes support for advanced SQL features such as CTEs, bulk copy, table hints, windowed functions, temporary tables, and database-side create/update/delete operations.

[GitHub repository](https://github.com/linq2db/linq2db.EntityFrameworkCore) | [NuGet](https://www.nuget.org/packages/linq2db.EntityFrameworkCore)

### EFCore.SoftDelete

An implementation for soft deleting entities. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/AshkanAbd/efCoreSoftDeletes) | [NuGet](https://www.nuget.org/packages/EFCore.SoftDelete)

### EntityFrameworkCore.ConfigurationManager

Extends EF Core to resolve connection strings from App.config. For EF Core: 3, 5, 6.

[GitHub repository](https://github.com/efcore/EFCore.ConfigurationManager) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.ConfigurationManager)

### Detached Mapper

A DTO-Entity mapper with composition/aggregation handling (similar to GraphDiff). For EF Core: 3, 5.

[GitHub repository](https://github.com/leonardoporro/Detached-Mapper) | [NuGet](https://www.nuget.org/packages/Detached.Mappers.EntityFramework)

### EntityFrameworkCore.Sqlite.NodaTime

Adds support for [NodaTime](https://nodatime.org) types when using [SQLite](https://sqlite.org). For EF Core: 5, 6.

[GitHub repository](https://github.com/khellang/EFCore.Sqlite.NodaTime) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Sqlite.NodaTime)

### ErikEJ.EntityFrameworkCore.SqlServer.Dacpac

Enables reverse engineering an EF Core model from a SQL Server data-tier application package (.dacpac). For EF Core: 3, 5.

[GitHub wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki/ErikEJ.EntityFrameworkCore.SqlServer.Dacpac) | [NuGet](https://www.nuget.org/packages/ErikEJ.EntityFrameworkCore.SqlServer.Dacpac)

### ErikEJ.EntityFrameworkCore.DgmlBuilder

Generate DGML (Graph) content that visualizes your DbContext. Adds the AsDgml() extension method to the DbContext class. For EF Core: 3, 5.

[GitHub wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki/Inspect-your-DbContext-model) | [NuGet](https://www.nuget.org/packages/ErikEJ.EntityFrameworkCore.DgmlBuilder)

### ErikEJ.EntityFrameworkCore.SqlServer.SqlQuery

Provides the `SqlQueryAsync<T>` and `SqlQueryValueAsync<T>` methods to help you populate arbitrary classes or a list of primitive types from a raw SQL query. For EF Core: 5.

[GitHub repository](https://github.com/ErikEJ/EFCorePowerTools) | [NuGet](https://www.nuget.org/packages/ErikEJ.EntityFrameworkCore.SqlServer.SqlQuery)

### EntityFramework.Exceptions

When using Entity Framework Core all database exceptions are wrapped in DbUpdateException. EntityFramework.Exceptions handles all the database-specific details to find which constraint was violated and allows you to use typed exceptions such as `UniqueConstraintException`, `CannotInsertNullException`, `MaxLengthExceededException`, `NumericOverflowException`, `ReferenceConstraintException` when your query violates database constraints.

Supports SQL Server, Postgres, MySql, SQLite and Oracle. For EF Core: 3, 5.

[GitHub Repository](https://github.com/Giorgi/EntityFramework.Exceptions)

### EFCoreAuditing

A Library for Entity Framework Core to support automatically recording data changes history (audit logging), soft-delete, and snake_case naming convention functionality. For EF Core: 2.

[GitHub Repository](https://github.com/OKTAYKIR/EFCoreAuditing) | [NuGet](https://www.nuget.org/packages/EFCore.Auditing)

### EntityFrameworkCore.FSharp

Adds F# design-time support to EF Core. For EF Core: 5.

[GitHub repository](https://github.com/efcore/EFCore.FSharp) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.FSharp)

### EntityFrameworkCore.VisualBasic

Adds VB design-time support to EF Core. For EF Core: 5, 6.

[GitHub repository](https://github.com/efcore/EFCore.VisualBasic) | [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.VisualBasic)

### Krzysztofz01.EFCore.QueryFilterBuilder

Extension for Entity Framework that allows you to create and manage multiple query filters. For EF Core: 5.

[GitHub repository](https://github.com/Krzysztofz01/EFCore.QueryFilterBuilder)

### Pagination.EntityFrameworkCore.Extensions

This is a library for Pagination on EntityFrameworkCore. Works well with Entity Framework Core as an extension, and supports both asynchronous and synchronous.
It also has many useful features commonly used especially on web development. For EF Core: 2, 3, 5.

[GitHub repository](https://github.com/SitholeWB/Pagination.EntityFrameworkCore.Extensions) | [NuGet](https://www.nuget.org/packages/Pagination.EntityFrameworkCore.Extensions)

### Laraue.EfCoreTriggers

Fluent API to declare triggers in `Context.OnModelCreating` which are later built into migrations. Providers to Postgres, MySQL, SQL Server and SQLite. For EF Core: 5, 6.

[GitHub repository](https://github.com/win7user10/Laraue.EfCoreTriggers) | [NuGet](https://www.nuget.org/packages/Laraue.EfCoreTriggers.Common)

### EntityCloner.Microsoft.EntityFrameworkCore

Cloning entities using EF Core configuration. You can use the `Include` method to specify related data to be cloned. For EF Core: 5, 6.

[GitHub repository](https://github.com/HenkKin/EntityCloner.Microsoft.EntityFrameworkCore) | [NuGet](https://www.nuget.org/packages/EntityCloner.Microsoft.EntityFrameworkCore)

## API Integrations

These packages are designed to integrate directly with EF Core to expose various APIs.

### HotChocolate

Build your own GraphQL endpoint on top of any resource.

[GitHub repository](https://github.com/ChilliCream/hotchocolate) | [NuGet](https://www.nuget.org/packages/HotChocolate)

### GraphQL.EntityFramework

Add Entity Framework `IQueryable` support to GraphQL.

[GitHub repository](https://github.com/SimonCropp/GraphQL.EntityFramework) | [NuGet](https://www.nuget.org/packages/GraphQL.EntityFramework/)

### OData

A standard for implementing REST APIs with specifications for discovery, filtering, sorting, projections, navigations, bulk operations, and more.

[GitHub repository](https://github.com/OData) | [NuGet](https://www.nuget.org/packages/Microsoft.OData.Core/)
