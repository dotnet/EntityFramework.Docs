---
title: Plan for Entity Framework Core 7.0
description: The themes and features planned for EF Core 7.0
author: SamMonoRT
ms.date: 12/15/2021
uid: core/what-is-new/ef-core-7.0/plan
---

# Plan for Entity Framework Core 7.0

As described in the [planning process](xref:core/what-is-new/release-planning), we have gathered input from stakeholders into a plan for Entity Framework Core 7.0 (EF Core 7.0.) For brevity, EF Core 7.0 is also referred to as just EF7.

> [!IMPORTANT]
> This plan is not a commitment; it will evolve as we continue to learn throughout the release. Some things not currently planned for EF7 may get pulled in. Some things currently planned for EF7 may get punted out.

## General information

EF Core 7.0 is the next release after EF Core 6.0 and is currently scheduled for release in November 2022 at the same time as .NET 7. There are no plans for an EF Core 6.1 release.

### Supported platforms

EF7 currently targets .NET 6. This may be updated to .NET 7 as we near the release. EF7 does not target any .NET Standard version; for more information see [the future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/). EF7 will not run on .NET Framework.

EF7 will align with the [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core) and will therefore **not** be a long-term support (LTS) release.

### Breaking changes

EF7 will contain a small number of [breaking changes](xref:core/what-is-new/ef-core-7.0/breaking-changes) as we continue to evolve both EF Core and the .NET platform. Our goal is to minimize breaking changes as much as possible.

## Themes

The large investments in EF7 will fall mainly under the following themes:

- Highly requested features
- NET platforms and ecosystem
- Clear path forward from EF6
- Performance

Each of these themes is described in detail below. The high-level status of each theme can be tracked in the [EF Core biweekly updates](https://github.com/dotnet/efcore/issues/23884). Please comment on [GitHub Issue #26994](https://github.com/dotnet/efcore/issues/26994) with any feedback or suggestions.

## Theme: Highly requested features

As always, a major input into the [planning process](xref:core/what-is-new/release-planning) comes from [votes (👍) for features on GitHub](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc). Based on these votes in addition to other factors, we plan to work on the following highly requested features for EF7.

### JSON columns

Tracked by [Issue #4021: Map JSON values stored in database to EF properties](https://github.com/dotnet/efcore/issues/4021)

Value proposition: Save and query into JSON-based documents stored in relational database columns.

This feature will introduce a common mechanism and patterns for JSON support that can be implemented by any database provider. We will work with the community to align existing implementations for [Npgsql](https://github.com/npgsql/efcore.pg) and [Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql), and also add support for SQL Server and SQLite.

### Bulk updates

Tracked by [Issue #795: Bulk (i.e. set-based) CUD operations (without loading data into memory)](https://github.com/dotnet/efcore/issues/795)

Value proposition: Efficient, predicate-based updates for many database rows without loading data into memory.

[Change tracking](xref:core/change-tracking/index) followed by [SaveChanges](xref:core/saving/index) is the primary mechanism in EF Core for inserting, updating, and deleting entities. This mechanism ensures that database operations are ordered to satisfy constraints, and that tracked entities are kept in sync with the changes made to the database. However, it requires a database roundtrip to load entities into memory in order to create the appropriate database commands, followed by a database roundtrip to execute these commands.

In contrast, bulk, or set-based, updates involve defining the changes that should be made to the database and then executing those changes without first loading entities into memory. This can be significantly faster than tracked updates, especially when the same modification must be applied to many different entities/rows.

For EF7, we plan to implement bulk updates and deletes (but not inserts). Note that bulk updates is not the same as batching updates. EF Core already combines changes to many tracked entities into batches when sending updates to the database via `SaveChanges`.

### Lifecycle hooks

Tracked by [Issue #626: Lifecycle hooks](https://github.com/dotnet/efcore/issues/626)

Value proposition: Allow applications to react when interesting things happen in EF code.

Lifecycle hooks enable notification of an application or library whenever certain interesting conditions or actions occur for entities, properties, relationships, queries, context instances, and other EF constructs. We have implemented many lifecycle hooks over the previous versions of EF Core, including various [interceptors](xref:core/logging-events-diagnostics/interceptors) and [events](xref:core/logging-events-diagnostics/events). For EF7, we plan to add important missing hooks. For example, a hook for manipulation of entity instances after they are created, commonly known as [ObjectMaterialized](https://github.com/dotnet/efcore/issues/15911).

### Table-per-concrete-type (TPC) mapping

Tracked by [Issue #3170: TPC inheritance mapping pattern](https://github.com/dotnet/efcore/issues/3170)

Value proposition: Map entities in a hierarchy to separate tables without taking the performance hit of TPT mapping.

EF Core supports [table-per-hierarchy (TPH)](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration) and [table-per-type (TPT)](xref:core/modeling/inheritance#table-per-type-configuration) mapping for .NET inheritance hierarchies. Table-per-concrete-type (TPC) mapping is similar to TPT mapping in that each entity type in the hierarchy is mapped to a different database table. However, while TPT maps properties from a base type to columns in a table for the base type, TPC maps properties of the base type to the same table as the actual concrete type being mapped. This can result in considerably faster performance since multiple tables do not need to be joined when querying for a specific type. This comes at the expense of data denormalization, since columns are duplicated across in the tables mapped to each concrete type in the hierarchy.

The work for TPC mapping also covers more general [entity splitting](https://github.com/dotnet/efcore/issues/620), and support for [specifying different facets per table in TPT, TPC or entity splitting](https://github.com/dotnet/efcore/issues/19811).

### Map CUD operations to stored procedures

Tracked by [Issue #245: Map inserts, updates, and deletes (CUD operations) to stored procedures](https://github.com/dotnet/efcore/issues/245)

Value proposition: Use stored procedures to manage data modifications.

EF Core already supports querying data from stored procedures. This feature will allow mapping the inserts, updates, and deletes generated by `SaveChanges` to stored procedures in the database.

### Value objects

Tracked by [Issue #9906: Use C# structs or classes as value objects](https://github.com/dotnet/efcore/issues/9906)

Value proposition: Applications can use DDD-style value objects in EF models.

It was previously the team view that owned entities, intended for [aggregate support](https://www.martinfowler.com/bliki/DDD_Aggregate.html), would also be a reasonable approximation to [value objects](https://www.martinfowler.com/bliki/ValueObject.html). Experience has shown this not to be the case. Therefore, in EF7, we plan to introduce a better experience focused on the needs of value objects in domain-driven design. This approach will be based on value converters rather than owned entities.

This work is initially scoped to allow [value converters which map to multiple columns](https://github.com/dotnet/efcore/issues/13947). We may pull in additional support based on feedback during the release.

### Support value generation when using value converters

Tracked by [Issue #11597: Support more types of value generation with converters](https://github.com/dotnet/efcore/issues/11597)

Value proposition: DDD-style encapsulated key types can make full use of automatically generated key values.

EF Core 6.0 allowed more types of value generation to be used with keys mapped through [value converters](xref:core/modeling/value-conversions). We plan to generalize and expand this support in EF7.

### Raw SQL queries for unmapped types

Tracked by [Issue #10753: Support raw SQL queries without defining an entity type for the result](https://github.com/dotnet/efcore/issues/10753)

Value proposition: Applications can execute more types of raw SQL query without dropping down to ADO.NET or using third-party libraries.

Currently raw SQL queries must return a type in the model, either with or without a key defined. In EF7, we plan to allow raw SQL queries that directly return types that are not contained in the EF model.

The work here will also cover [raw SQL queries that return simple/scalar types](https://github.com/dotnet/efcore/issues/11624), such as `Guid`, `DateTime`, `int`, and `string`.

### Database scaffolding templates

Tracked by [Issue #4038: Code templates for scaffolding entity types and DbContext from an existing database](https://github.com/dotnet/efcore/issues/4038)

Value proposition: The code generated by `dotnet ef database scaffold` can be fully customized.

We frequently receive requests to adjust the code generated when scaffolding (reverse engineering) from an existing database. We plan to address these requests in EF7 by supporting [T4 templates](/visualstudio/modeling/code-generation-and-t4-text-templates) for the generated entity types and `DbContext`. Developers will be able to customize the standard templates, or create new templates from scratch.

## Theme: .NET platforms and ecosystem

Much of the work planned for EF7 involves improving the data access experience for .NET across different platforms and domains. This involves work in EF Core where needed, but also work in other areas to ensure a great experience across .NET technologies. We will focus on the following platforms/technologies for the EF7 release:

- [.NET MAUI](/dotnet/maui/what-is-maui)
- [ASP.NET Core](https://dotnet.microsoft.com/learn/aspnet/what-is-aspnet-core)
- [Azure Synapse](https://azure.microsoft.com/services/synapse-analytics/#overview)
- [Blazor Server](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [Windows Forms](/dotnet/desktop/winforms/overview)
- [Windows Presentation Foundation (WPF)](https://visualstudio.microsoft.com/vs/features/wpf/)

This list is based on many factors, including customer data, strategic direction, and available resources. The general areas we will be working on for these platforms are outlined below.

### Distributed transactions

Tracked by [Issue #715 in dotnet/runtime: Implement distributed/promoted transactions in System.Transactions](https://github.com/dotnet/runtime/issues/715)

Value proposition: .NET Framework applications using distributed transactions can be ported to .NET 7.

The `System.Transactions` library in .NET Framework contains native code that makes use of the Windows [Distributed Transactions Coordinator (DTC)](/previous-versions/windows/desktop/ms684146(v=vs.85)) to support distributed transactions. This code was never ported to .NET Core. In the .NET 7 timeframe, we plan to investigate and begin the process of bringing this functionality to modern .NET. This will be initially for Windows only, and will support only database scenarios where the ADO.NET provider also supports distributed transactions. Other uses of distributed transactions, such as in WCF, will not be supported in .NET 7. Based on feedback and cost, we may implement support for other scenarios and/or non-Windows platforms in a future release.

### EF Core tooling

Tracked by [Issue #26798: Modernize EF Core tooling](https://github.com/dotnet/efcore/issues/26798)

Value proposition: `dotnet ef` commands are easy to use and work with modern platforms and technologies.

.NET platforms have evolved since we first introduced tooling for migrations, database scaffolding, etc. in EF Core 1.0. In EF7, we plan to update the tooling architecture to better support new platforms, like .NET MAUI, and to streamline the process in areas such as the use of multiple projects. This includes providing better feedback when things go wrong, better integration with logging, performance, and new [sugar](https://en.wikipedia.org/wiki/Syntactic_sugar).

### EF Core and graphical user interfaces

Tracked by [Issue #26799: Improve experience for data binding and graphical interfaces](https://github.com/dotnet/efcore/issues/26799)

Value proposition: It is easy to build data-bound graphical applications with EF Core.

EF Core is designed to work well with data binding scenarios, such as those in Windows Forms and .NET MAUI. However, connecting the dots between these technologies is not always easy. For EF7, we plan to improve the experience with both work in EF Core and in Visual Studio to make it easier to build data-bound applications with EF Core.

### SqlServer.Core (Woodstar)

Tracked in the [.NET Data Lab repo](https://github.com/dotnet/datalab/)

Value proposition: Fast, fully managed access to SQL Server and Azure SQL for modern .NET applications.

[Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) is a fully-featured ADO.NET database provider for SQL Server. It supports a broad range of SQL Server features on both .NET Core and .NET Framework. However, it is also a large and old codebase with many complex interactions between its behaviors. This makes it difficult to investigate the potential gains that could be made using newer .NET Core features.

We began a project last year, colloquially known as "Woodstar", to investigate the potential for a highly performing SQL Server driver for .NET. We plan to make significant further investment into this project in the EF7 timeframe.

> [!IMPORTANT]
> Investment in Microsoft.Data.SqlClient is not changing. It will continue to be the recommended way to connect to SQL Server and Azure SQL, both with and without EF Core. It will continue to support new SQL Server features as they are introduced.

### Azure Cosmos DB provider

Tracked by [issues labeled with 'area-cosmos' and in the 7.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A7.0.0+label%3Aarea-cosmos)

Value proposition: Continue to make EF Core the easiest and most productive way to work with [Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/#overview).

We made [significant improvements to the EF Core Azure Cosmos DB database provider for the 6.0 release](/ef/core/what-is-new/ef-core-6.0/whatsnew#cosmos-provider-enhancements). These improvements created a first-class experience for working with Azure Cosmos DB from EF Core, which is reflected by significant growth in adoption. We plan to continue this momentum in EF7 with the following further Azure Cosmos DB provider enhancements:

- [Issue #16146: Support aggregate operators](https://github.com/dotnet/efcore/issues/16146)
- [Issue #17306: Allow to use a custom JSON serializer](https://github.com/dotnet/efcore/issues/17306)
- [Issue #17670: Translate non-persisted property in query when possible](https://github.com/dotnet/efcore/issues/17670)
- [Issue #19944: Support trigger execution](https://github.com/dotnet/efcore/issues/19944)
- [Issue #20350: Detect partition key filters in more queries](https://github.com/dotnet/efcore/issues/20350)
- [Issue #20910: Add translation for string.Compare for Azure Cosmos DB](https://github.com/dotnet/efcore/issues/20910)
- [Issue #23538: Improve query tree during translation](https://github.com/dotnet/efcore/issues/23538)
- [Issue #24571: Allow terminating operators after Skip and Take](https://github.com/dotnet/efcore/issues/24571)
- [Issue #24513: Add support for pagination (MaxItemCount)](https://github.com/dotnet/efcore/issues/24513)
- [Issue #25700: translate Length/Count on collections](https://github.com/dotnet/efcore/issues/25700)
- [Issue #25701: translate indexing into collection](https://github.com/dotnet/efcore/issues/25701)
- [Issue #26478: Implement interceptors for Azure Cosmos DB](https://github.com/dotnet/efcore/issues/26478)
- [Issue #26491: Support AAD RBAC via the ClientSecretCredential](https://github.com/dotnet/efcore/issues/26491)

Please make sure to vote (👍) for the [Azure Cosmos DB provider features](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-cosmos) that you need so that we can assess where to invest for the most benefit.

### Migrations experience

Tracked by [Issue #22946: Improvements to database migrations](https://github.com/dotnet/efcore/issues/22946)

Value proposition: It is easy to get started with migrations and later use them effectively in CI/CD pipelines.

EF Core 6.0 introduced [migrations bundles](xref:core/managing-schemas/migrations/applying#bundles), significantly improving the experience for cloud-native applications and database deployment within continuous integration and deployment systems. In EF7, we plan to make additional improvements in this area based on customer feedback. For example, we plan to support [executing all migrations in a single transaction](https://github.com/dotnet/efcore/issues/22616) to facilitate easier recovery if something goes wrong.

In addition, we plan to improve the experience for developers getting started with migrations. This includes being able to create the database automatically when learning or beginning a project, and then [easily switching to managed migrations](https://github.com/dotnet/efcore/issues/3053) as the project matures. Alternately, for projects with an existing database, we plan to make it easy to create an initial EF model and then [switch to managing the database using migrations](https://github.com/dotnet/efcore/issues/2167) going forward.

### Modern .NET

As .NET continues to evolve we want to ensure that accessing data continues to be a great experience. To facilitate this, we plan to make progress on three areas during the EF7 timeframe.

#### Trimming

Tracked by [Issue #21894: Improve trimming support for EF Core apps to reduce application size](https://github.com/dotnet/efcore/issues/21894)

Value proposition: Smaller applications that can be efficiently AOT compiled.

EF Core performs large amounts of runtime code generation. This is challenging for app models that depend on linker tree shaking, such as .NET MAUI and Blazor, and platforms that don't allow dynamic compilation, such as iOS. We plan to greatly improve trimming of unused code in EF7. This will facilitate smaller assembly sizes when using EF Core, thus helping deployment and making ahead-of-time (AOT) compilation more efficient.

#### Evolve System.Linq.Expression

Value proposition: Use modern C# language features in LINQ queries.

We are working with the Roslyn team on a plan to allow more C# features to be used in LINQ expressions. This is ongoing work which will mostly be tracked outside the EF Core repo.

#### Translate new LINQ operators

Tracked by [Issue #25570: Support new .NET LINQ features](https://github.com/dotnet/efcore/issues/25570)

Value proposition: Use new LINQ operators when translating LINQ queries to SQL.

New LINQ operators have recently been added to the BCL, and we expect more to be added going forward.  Issue [#25570](https://github.com/dotnet/efcore/issues/25570) tracks adding support for these to the EF7 LINQ provider. This issue will be updated as new LINQ operators are added. As with all existing LINQ operators, we will only add support when the operator has a reasonable and useful translation to the database.

### Open telemetry for ADO.NET providers

Tracked by [Issue #22336: Standardize on DiagnosticSource/OpenTelemetry for database tracing](https://github.com/dotnet/runtime/issues/22336)

Value proposition: Cross-platform, industry-standard telemetry that can be monitored in your tool of choice.

[Open telemetry](https://github.com/open-telemetry/opentelemetry-specification) is an initiative of the Cloud Native Foundation to foster a common telemetry mechanism for cloud-native software. With regard to databases, this includes establishing a database client telemetry standard. We plan to do work in the EF7 timeframe to help bring open telemetry to ADO.NET providers in the .NET ecosystem. This includes work with the community on the open source [MySQL](https://github.com/mysql-net/MySqlConnector) and [Npgsql](https://github.com/npgsql/Npgsql) providers, as well as [Microsoft.Data.Sqlite](https://github.com/dotnet/efcore#microsoftdatasqlite). We will also reach out to other providers, and we encourage maintainers of ADO.NET providers to get in touch if interested.

### Enhancements to System.Data

Tracked by [issues in the dotnet\runtime repo labeled with `area-System.Data` in the 7.0 milestone](https://github.com/dotnet/runtime/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-System.Data+milestone%3A7.0.0)

Value proposition: Better low-level data access to benefit all higher-level code.

As with every release, we intend to explore improvements to .NET's low-level database access API, System.Data. We will focus on performance improvements (e.g. reduce memory allocations by eliminating boxing when using the API), as well as some usability improvements.

The scope of the precise improvements will be determined later based on feasibility.

### Research data access for cloud-native

Value proposition: Future evolution of .NET data access that supports modern approaches such as microservices and cloud native.

In the EF7 timeframe, we plan to research modern approaches to data access across .NET platforms, especially in reference to microservices and cloud-native applications. This research will help drive future investments in data access technologies for .NET.

## Theme: Clear path forward from EF6

Tracked by [Docs issue #1180: Provide a more complete guide to porting from EF6](https://github.com/dotnet/EntityFramework.Docs/issues/1180)

Value proposition: Easily move your application from EF6 to EF7.

EF Core has always [supported many scenarios not covered by the legacy EF6 stack](/ef/efcore-and-ef6/), as well as being generally much higher performing. However, EF6 has likewise supported [scenarios not covered by EF Core](/ef/efcore-and-ef6/). EF7 will add support for many of these scenarios, allowing more applications to port from legacy EF6 to EF7. At the same time, we are planning a [comprehensive porting guide](https://github.com/dotnet/EntityFramework.Docs/issues/1180) for applications moving from legacy EF6 to EF Core.

Much of the work in this theme overlaps with work already outlined above. Some of the more significant work items are:

- [Issue #214: Expose model building conventions to applications](https://github.com/dotnet/efcore/issues/214)
- [Issue #245: Map inserts, updates, and deletes (CUD operations) to stored procedures](https://github.com/dotnet/efcore/issues/245)
- [Issue #620: Entity splitting support](https://github.com/dotnet/efcore/issues/620)
- [Issue #3170: TPC inheritance mapping pattern](https://github.com/dotnet/efcore/issues/3170)
- [Issue #3864: Support unidirectional many-to-many relationships through shadow navigations](https://github.com/dotnet/efcore/issues/3864)
- [Issue #4038: Code templates for scaffolding entity types and DbContext from an existing database](https://github.com/dotnet/efcore/issues/4038)
- [Issue #10753: Support raw SQL queries without defining an entity type for the result](https://github.com/dotnet/efcore/issues/10753)
- [Issue #11003: KeyAttribute support for composite primary key](https://github.com/dotnet/efcore/issues/11003)
- [Issue #11624: Support raw SQL Queries for basic types like Guid, DateTime and int](https://github.com/dotnet/efcore/issues/11624)
- [Issue #15911: Implement ObjectMaterialized event](https://github.com/dotnet/efcore/issues/15911)
- [Issue #17653: Support for GroupBy entityType](https://github.com/dotnet/efcore/issues/17653)
- [Issue #19929: Support GroupBy when it is final operator](https://github.com/dotnet/efcore/issues/19929)
- [Issue #19930: Support GroupJoin when it is final query operator](https://github.com/dotnet/efcore/issues/19930)
- [issue #26626: Use EF6 as an oracle for procedural testing of EF Core queries](https://github.com/dotnet/efcore/issues/26626)

In addition, we plan to make it clear on the [legacy EF6 GitHub repo](https://github.com/dotnet/ef6) that we are not planning any future work on EF6. The exception to this is that we plan to add support for using EF6 with [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient). This will be limited to runtime support. Use of the EF6 designer in Visual Studio will still require `System.Data.SqlClient`.

Note that EF Core has a fundamentally different architecture to EF6. In particular, it does not make use of an Entity Data Model (EDM) specification. This means certain features such as EDMX and EntitySQL will never be supported in EF Core.

## Theme: Performance

Great performance is a fundamental tenet of EF Core, lower-level data access, and indeed all of .NET. Every release includes significant work on improving performance. As always, this theme will involve a lot of iterative investigation, which will then inform where we focus resources.

### Performance of database inserts and updates

Tracked by [Issue 26797: Improve change tracking and update performance](https://github.com/dotnet/efcore/issues/26797)

Value proposition: High performance database inserts and updates from EF Core.

Over the past few releases, we have focused on improving EF Core performance on non-tracking queries. For EF7, we plan to focus on performance related to database inserts and updates. This includes performance of change-tracking queries, performance of `DetectChanges`, and performance of the insert and update commands sent to the database.

Part of this work includes implementation of [bulk (set-based) updates](https://github.com/dotnet/efcore/issues/795), which is tracked above as a highly-requested feature. However, we also plan to improve the performance of traditional updates using `SaveChanges`.

### TechEmpower composite score

Tracked by [Issue 26796: Improve performance in the TechEmpower composite score](https://github.com/dotnet/efcore/issues/26796)

Value proposition: High performing low-level data updates for all .NET applications.

We have been running the industry standard [TechEmpower benchmarks](https://www.techempower.com/benchmarks/) on .NET against a PostgreSQL database for several years. In the last couple of releases, we have significantly improved the [Fortunes benchmark](https://www.techempower.com/benchmarks/#section=data-r19&hw=ph&test=fortune) for both low-level data access and EF Core.

In the EF7 timeframe we plan to specifically target improvements to the [TechEmpower composite score](https://www.techempower.com/benchmarks/#section=data-r20&hw=ph&test=composite). This measures performance over a wider range of scenarios.

## Miscellaneous features

Tracked by [issues labeled with `type-enhancement` in the 7.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A7.0.0+label%3Atype-enhancement)

Value proposition: Continuous improvement to EF Core to meet existing and evolving application requirements.

Miscellaneous features planned for EF 7.0 include, but are not limited to:

- [Issue #14545: Design: Modernize generated C#](https://github.com/dotnet/efcore/issues/14545)
- [Issue #15762: Provide easy way to get script from latest applied on database to latest in the codebase](https://github.com/dotnet/efcore/issues/15762)
- [Issue #16406: Fully embrace provider-specific migrations](https://github.com/dotnet/efcore/issues/16406)
- [Issue #18844: SQLite: Translate TimeSpan members](https://github.com/dotnet/efcore/issues/18844)
- [Issue #18950: Enable nullability in migrations and model snapshots](https://github.com/dotnet/efcore/issues/18950)
- [Issue #18990: Use the new batching API from ADO.NET](https://github.com/dotnet/efcore/issues/18990)
- [Issue #21615: dotnet-ef database update -1 (down latest migration)](https://github.com/dotnet/efcore/issues/21615)
- [Issue #21995: Generate the script for all migrations that have not yet been applied to a given database](https://github.com/dotnet/efcore/issues/21995)
- [Issue #22138: Upgrade to SpatiaLite 5](https://github.com/dotnet/efcore/issues/22138)
- [Issue #23085: Don't check for a connection string until after ConnectionOpening has been called](https://github.com/dotnet/efcore/issues/23085)
- [Issue #24685: Allow value converters to change nullability of columns](https://github.com/dotnet/efcore/issues/24685)
- [Issue #25103: SQLite: Translate more DateOnly and TimeOnly members](https://github.com/dotnet/efcore/issues/25103)
- [Issue #25872: Provide more info about a migration bundle](https://github.com/dotnet/efcore/issues/25872)
- [Issue #26155: Sugar for getting a migration script for just the last migration](https://github.com/dotnet/efcore/issues/26155)
- [Issue #26417: Revisit the reporting of exceptions and stack traces in logging](https://github.com/dotnet/efcore/issues/26417)
- [Issue #26528: Register a scoped DbContext automatically when using AddPooledDbContextFactory](https://github.com/dotnet/efcore/issues/26528)

## Suggestions

Your feedback on planning is important. Please comment on [GitHub Issue #26994](https://github.com/dotnet/efcore/issues/26994) with any feedback or general suggestions about the plan. The best way to indicate the importance of an issue is to vote (👍) for that [issue on GitHub](https://github.com/dotnet/efcore/issues). This data will then feed into the [planning process](xref:core/what-is-new/release-planning) for the next release.
