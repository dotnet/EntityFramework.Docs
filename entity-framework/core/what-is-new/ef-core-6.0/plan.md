---
title: Plan for Entity Framework Core 6.0
description: The themes and features planned for EF Core 6.0
author: SamMonoRT
ms.date: 11/09/2021
uid: core/what-is-new/ef-core-6.0/plan
---

# Plan for Entity Framework Core 6.0

> [!IMPORTANT]
> EF Core 6.0 [has now been released](xref:core/what-is-new/index). This page remains as a historical record of the plan.

As described in the [planning process](xref:core/what-is-new/release-planning), we have gathered input from stakeholders into a plan for the Entity Framework Core (EF Core) 6.0 release. This plan is periodically updated to reflect schedule and scope adjustments.

Unlike previous releases, this plan does not attempt to cover all work for the 6.0 release. Instead, it indicates where and how we intend to invest in this release, but with flexibility to adjust scope or pull in new work as we gather feedback and learn while working on the release.

> [!IMPORTANT]
> This plan is not a commitment. It is a starting point that will evolve as we learn more. Some things not currently planned for 6.0 may get pulled in. Some things currently planned for 6.0 may get punted out.

## General information

### Version number and release date

EF Core 6.0 is the next release after EF Core 5.0 and is currently scheduled for release in November 2021 at the same time as .NET 6.

### Supported platforms

EF Core 6.0 requires .NET 6. EF Core 6.0 does not target any .NET Standard version; for more information see [the future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/).

EF Core 6.0 will not run on .NET Framework.

EF Core 6.0 will align with .NET 6 as a [long-term support (LTS) release](https://dotnet.microsoft.com/platform/support/policy/dotnet-core).

### Breaking changes

EF Core 6.0 will contain a small number of [breaking changes](xref:core/what-is-new/ef-core-6.0/breaking-changes) as we continue to evolve both EF Core and the .NET platform. Our goal is to allow the vast majority of applications to update without breaking.

## Themes

The following areas will form the basis for the large investments in EF Core 6.0.

## Highly requested features

As always, a major input into the [planning process](xref:core/what-is-new/release-planning) comes from the [voting (👍) for features on GitHub](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc). For EF Core 6.0 we plan to work on the following highly requested features:

### SQL Server temporal tables

Tracked by [#4693](https://github.com/dotnet/efcore/issues/4693)

Status: Complete

T-shirt size: Large

Temporal tables support queries for data stored in the table at _any point in time_, as opposed to only the most recent data stored as is the case for normal tables. EF Core 6.0 will allow temporal tables to be created via Migrations, as well as allowing access to the data through LINQ queries.

This work is initially scoped as [described on the issue](https://github.com/dotnet/efcore/issues/4693#issuecomment-625048974). We may pull in additional support based on feedback during the release.

### JSON columns

Tracked by [#4021](https://github.com/dotnet/efcore/issues/4021)

Status: Cut

T-shirt size: Medium

This feature will introduce a common mechanism and patterns for JSON support that can be implemented by any database provider. We will work with the community to align existing implementations for [Npgsql](https://github.com/npgsql/efcore.pg) and [Pomelo MySQL](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql), and also add support for SQL Server and SQLite.

### ColumnAttribute.Order

Tracked by [#10059](https://github.com/dotnet/efcore/issues/10059)

Status: In-progress

T-shirt size: Small

This feature will allow arbitrary ordering of columns when **creating a table** with Migrations or `EnsureCreated`. Note that changing the order of columns in an existing tables requires that the table be rebuilt, and this is not something that we plan to support in any EF Core release.

## Performance

While EF Core is generally faster than EF6, there are still areas where significant improvements in performance are possible. We plan to tackle several of these areas in EF Core 6.0, while also improving our perf infrastructure and testing.

This theme will involve a lot of iterative investigation, which will inform where we focus resources. We plan to begin with:

### Performance infrastructure and new tests

Status: Scoped/Complete

T-shirt size: Medium

The EF Core codebase already contains a set of performance benchmarks that are executed every day. For 6.0, we plan to improve the infrastructure for these tests as well as adding new tests. We will also profile mainline perf scenarios and fix any low-hanging fruit found.

Update: We have improved test infrastructure and added new tests to support the work done for EF Core 6. Additional improvements in this area have been scoped out of the EF Core 6.0 release.

### Compiled models

Tracked by [#1906](https://github.com/dotnet/efcore/issues/1906)

Status: Complete

T-shirt size: X-Large

Compiled models will allow the generation of a compiled form of the EF model. This will provide both better startup performance, as well as generally better performance when accessing the model.

### TechEmpower Fortunes

Tracked by [#23611](https://github.com/dotnet/efcore/issues/23611)

Status: Complete

T-shirt size: X-Large

We have been running the industry standard [TechEmpower benchmarks](https://www.techempower.com/benchmarks/) on .NET against a PostgreSQL database for several years. The [Fortunes benchmark](https://www.techempower.com/benchmarks/#section=data-r19&hw=ph&test=fortune) is particularly relevant to EF scenarios. We have multiple variations of this benchmark, including:

- An implementation that uses ADO.NET directly. This is the fastest implementation of the three listed here.
- An implementation that uses [Dapper](https://www.nuget.org/packages/Dapper/). This is slower than using ADO.NET directly, but still fast.
- An implementation that uses EF Core. This is currently the slowest implementation of the three.

The goal for EF Core 6.0 is to get the EF Core performance to match that of Dapper on the TechEmpower Fortunes benchmark. (This is a significant challenge but we will do our best to get as close as we can.)

### Linker/AOT

Tracked by [#10963](https://github.com/dotnet/efcore/issues/10963)

Status: Scoped/Complete

T-shirt size: Medium

EF Core performs large amounts of runtime code generation. This is challenging for app models that depend on linker tree shaking, such as .NET MAUI and Blazor, and platforms that don't allow dynamic compilation, such as iOS. We will continue investigating in this space as part of EF Core 6.0 and make targeted improvements as we can. However, we do not expect to fully close the gap in the 6.0 time frame.

## Migrations and deployment

Following on from the [investigations done for EF Core 5.0](xref:core/what-is-new/ef-core-5.0/plan#migrations-and-deployment-experience), we plan to introduce improved support for managing migrations and deploying databases. This includes two major areas:

### Migrations bundles

Tracked by [#19693](https://github.com/dotnet/efcore/issues/19693)

Status: Complete

T-shirt size: Medium

A migrations bundle is a self-contained executable that applies migrations to a production database. The behavior will match `dotnet ef database update`, but should make SSH/Docker/PowerShell deployment much easier, since everything needed is contained in the single executable.

### Managing migrations

Tracked by [#22945](https://github.com/dotnet/efcore/issues/22945)

Status: Cut

T-shirt size: Large

The number of migrations created for an application can grow to become a burden. In addition, these migrations are frequently deployed with the application even when this is not needed. In EF Core 6.0, we plan to improve this through better tooling and project/assembly management. Two specific issues we plan to address are [squash many migrations into one](https://github.com/dotnet/efcore/issues/2174) and [regenerate a clean model snapshot](https://github.com/dotnet/efcore/issues/18557).

Update: most of the work in this area has been cut for 6.0 due to resource constraints.

## Improve existing features and fix bugs

Any [issue or bug assigned to the 6.0.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A6.0.0) is currently planned for this release. This includes many small enhancements and bug fixes.

### EF6 query parity

Tracked by [issues labeled with 'ef6-parity' and in the 6.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aef6-parity+milestone%3A6.0.0)

Status: Scoped/Complete

T-shirt size: Large

EF Core 5.0 supports most query patterns supported by EF6, in addition to patterns not supported in EF6. For EF Core 6.0, we plan to close the gap and make supported EF Core queries a true superset of supported EF6 queries. This will be driven by investigation of the gaps, but already includes GroupBy issues such as [translate GroupBy followed by FirstOrDefault](https://github.com/dotnet/efcore/issues/12088), and raw SQL queries for [primitive](https://github.com/dotnet/efcore/issues/11624) and [unmapped](https://github.com/dotnet/efcore/issues/10753) types.

Update: Raw SQL queries for primitive and unmapped types has been cut from 6.0 due to resourcing constraints and priority adjustments.

### Value objects

Tracked by [#9906](https://github.com/dotnet/efcore/issues/9906)

Status: Cut

T-shirt size: Medium

It was previously the team view that owned entities, intended for [aggregate support](https://www.martinfowler.com/bliki/DDD_Aggregate.html), would also be a reasonable approximation to [value objects](https://www.martinfowler.com/bliki/ValueObject.html). Experience has shown this not to be the case. Therefore, in EF Core 6.0 we plan to introduce a better experience focused on the needs of value objects in domain-driven design. This approach will be based on value converters rather than owned entities.

This work is initially scoped to allow [value converters which map to multiple columns](https://github.com/dotnet/efcore/issues/13947). We may pull in additional support based on feedback during the release.

### Azure Cosmos DB database provider

Tracked by [issues labeled with 'area-cosmos' and in the 6.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A6.0.0+label%3Aarea-cosmos)

Status: Expanded/Complete

T-shirt size: Large

We are actively gathering feedback on which improvements to make to the Azure Cosmos DB provider in EF Core 6.0. We will update this document as we learn more. For now, please make sure to vote (👍) for the Azure Cosmos DB features that you need.

Update: We have been doing extensive customer development around the Azure Cosmos DB provider. This has resulted in the following enhancements being pulled into EF Core 6.0:

- [Azure Cosmos DB provider should default to implicit ownership](https://github.com/dotnet/efcore/issues/24803)
- [Set partition key on join entity type by convention](https://github.com/dotnet/efcore/issues/23491)
- [FromSql support](https://github.com/dotnet/efcore/issues/17311)
- [Configure TTL per entity/entity type/collection](https://github.com/dotnet/efcore/issues/17307)
- [API to configure container facets (throughput, size, partition key, etc.)](https://github.com/dotnet/efcore/issues/17301)
- [Diagnostic events including statistics (query cost, activity id)](https://github.com/dotnet/efcore/issues/17298)
- [Distinct operator in queries](https://github.com/dotnet/efcore/issues/16144)
- [Add translators for member/methods which map to built-in functions](https://github.com/dotnet/efcore/issues/16143)
- [Add basic support for collections and dictionaries of primitive types](https://github.com/dotnet/efcore/issues/14762)

Update: The following issues were cut from the 6.0 release:

- [Find/FindAsync performs SQL API query when entity has embedded entities](https://github.com/dotnet/efcore/issues/24202)
- [Optimize more queries that could use ReadItem](https://github.com/dotnet/efcore/issues/20693)
- [Detect partition key filters in more queries](https://github.com/dotnet/efcore/issues/20350)
- [Translate subquery in filter condition](https://github.com/dotnet/efcore/issues/17957)
- [Allow to specify consistency level for CUD operations](https://github.com/dotnet/efcore/issues/17309)
- [Support aggregate operators](https://github.com/dotnet/efcore/issues/16146)

### Expose model building conventions to applications

Tracked by [#214](https://github.com/dotnet/efcore/issues/214)

Status: Cut

T-shirt size: Medium

EF Core uses a set of conventions for building a model from .NET types. These conventions are currently controlled by the database provider. In EF Core 6.0, we intend to allow applications to hook into and change these conventions.

### Zero bug balance (ZBB)

Tracked by [issues labeled with `type-bug` in the 6.0 milestone](https://github.com/dotnet/efcore/issues?utf8=%E2%9C%93&q=is%3Aissue+milestone%3A6.0.0+label%3Atype-bug+)

Status: In-progress/Scoped

T-shirt size: Large

We plan to fix all outstanding bugs during the EF Core 6.0 time frame. Some things to keep in mind:

- This specifically applies to issues labeled [type-bug](https://github.com/dotnet/efcore/issues?q=is%3Aissue+label%3Atype-bug).
- There will be exceptions, such as when the bug requires a design change or new feature to fix properly. These issues will be marked with the `blocked` label.
- We will punt bugs based on risk when needed as is normal as we get close to a GA/RTM release.

### Miscellaneous features

Tracked by [issues labeled with `type-enhancement` in the 6.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A6.0.0+label%3Atype-enhancement)

Status: Complete

T-shirt size: Large

Miscellaneous features planned for EF 6.0 include, but are not limited to:

- [Split query for non-navigation collections](https://github.com/dotnet/efcore/issues/21234)
- [Detect simple join tables in reverse engineering and create many-to-many relationships](https://github.com/dotnet/efcore/issues/22475)
- [Mechanism/API to specify a default conversion for any property of a given type in the model](https://github.com/dotnet/efcore/issues/10784)

Update: The following issues were cut from the 6.0 release:

- [Complete full/free-text search on SQLite and SQL Server](https://github.com/dotnet/efcore/issues/4823)
- [SQL Server spatial indexes](https://github.com/dotnet/efcore/issues/12538)
- [Use the new batching API from ADO.NET](https://github.com/dotnet/efcore/issues/18990)

## .NET integration

The EF Core team also works on several related but independent technologies. In particular, we plan to work on:

### Enhancements to System.Data

Tracked by [issues in the dotnet\runtime repo labeled with `area-System.Data` in the 6.0 milestone](https://github.com/dotnet/runtime/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-System.Data+milestone%3A6.0.0)

Status: Scoped/Complete

T-shirt size: Large

This work includes:

- Implementation of the new [batching API](https://github.com/dotnet/runtime/issues/28633).
- Continued work with other .NET teams and the community to understand and evolve ADO.NET.

Update: The following issues were cut from the 6.0 release:

- [Standardize on DiagnosticSource for tracing in System.Data.* components](https://github.com/dotnet/runtime/issues/22336).

### Enhancements to Microsoft.Data.Sqlite

Tracked by [issues labeled with `type-enhancement` and `area-adonet-sqlite` in the 6.0 milestone](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+milestone%3A6.0.0+label%3Atype-enhancement+label%3Aarea-adonet-sqlite)

Status: Scoped/Complete

T-shirt size: Medium

Several small improvements are planned for the Microsoft.Data.Sqlite, including [connection pooling](https://github.com/dotnet/efcore/issues/13837) and [prepared statements](https://github.com/dotnet/efcore/issues/14044) for performance.

Update: Prepared statements has been cut from the 6.0 release.

### Nullable reference types

Tracked by [#14150](https://github.com/dotnet/efcore/issues/14150)

Status: Complete

T-shirt size: Large

We will annotate the EF Core code to use [nullable reference types](/dotnet/csharp/nullable-references).

## Experiments and investigations

The EF team is planning to invest time during the EF Core 6.0 timeframe experimenting and investigating in two areas. This is a learning process and as such no concrete deliverables are planned for the 6.0 release.

### SqlServer.Core

Tracked in the [.NET Data Lab repo](https://github.com/dotnet/datalab/)

Status: In-progress

T-shirt size: Ongoing

[Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) is a fully-featured ADO.NET database provider for SQL Server. It supports a broad range of SQL Server features on both .NET Core and .NET Framework. However, it is also a large and old codebase with many complex interactions between its behaviors. This makes it difficult to investigate the potential gains the could be made using newer .NET Core features. Therefore, we are starting an experiment in collaboration with the community to determine what potential there is for a highly performing SQL Server driver for .NET.

> [!IMPORTANT]
> Investment in Microsoft.Data.SqlClient is not changing. It will continue to be the recommended way to connect to SQL Server and SQL Azure, both with and without EF Core. It will continue to support new SQL Server features as they are introduced.

### GraphQL

Status: In-progress

T-shirt size: Ongoing

[GraphQL](https://graphql.org/) has been gaining traction over the last few years across a variety of platforms. We plan to investigate the space and find ways to improve the experience with .NET. This will involve working with the community on understanding and supporting the existing ecosystem. It may also involve specific investment from Microsoft, either in the form of contributions to existing work or in developing complimentary pieces in the Microsoft stack.

### DataVerse (formerly Common Data Services)

Status: In-progress

T-shirt size: Ongoing

[DataVerse](/powerapps/maker/data-platform/data-platform-intro) is a column-based data store designed for rapid development of business applications. It automatically handles complex data types like binary objects (BLOBs) and has built-in entities and relationships like organizations and contacts. An SDK exists but developers may benefit from having an EF Core provider to use advanced LINQ queries, take advantage of unit of work and have a consistent data access API. The team will consider different ways to improve the .NET developer experience connecting to DataVerse.

## Below-the-cut-line

Tracked by [issues labeled with `consider-for-current-release`](https://github.com/aspnet/EntityFrameworkCore/issues?q=is%3Aopen+is%3Aissue+label%3Aconsider-for-current-release)

These are bug fixes and enhancements that are **not** currently scheduled for the 6.0 release, but we will look at based on feedback throughout the release together with progress made on the work above. These issues may be pulled in to EF Core 6.0, and automatically become candidates for the next release.

In addition, we always consider the [most voted issues](https://github.com/dotnet/efcore/issues?q=is%3Aissue+is%3Aopen+sort%3Areactions-%2B1-desc) when planning. Cutting any of these issues from a release is always painful, but we do need a realistic plan for the resources we have. Make sure you have voted (👍) for the features you need.

## Suggestions

Your feedback on planning is important. The best way to indicate the importance of an issue is to vote (👍) for that issue on GitHub. This data will then feed into the [planning process](xref:core/what-is-new/release-planning) for the next release.
