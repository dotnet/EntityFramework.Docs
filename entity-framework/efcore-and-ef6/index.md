---
title: Compare EF6 and EF Core
description: Guidance on how to choose between EF6 and EF Core.
author: ajcvickers
ms.date: 01/23/2019
uid: efcore-and-ef6/index
---

# Compare EF Core & EF6

## EF Core

Entity Framework Core ([EF Core](xref:core/index)) is a modern object-database mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations.

EF Core works with SQL Server/SQL Azure, SQLite, Azure Cosmos DB, MySQL, PostgreSQL, and many more databases through a [database provider plugin model](xref:core/providers/index).

## EF6

Entity Framework 6 ([EF6](xref:ef6/index)) is an object-relational mapper designed for .NET Framework but with support for .NET Core. EF6 is a stable, supported product, but is no longer being actively developed.

## Feature comparison

EF Core offers new features that won't be implemented in EF6. However, not all EF6 features are currently implemented in EF Core.

The following tables compare the features available in EF Core and EF6. This is a high-level comparison and doesn't list every feature or explain differences between the same feature in different EF versions.

The EF Core column indicates the product version in which the feature first appeared.

### Creating a model

| **Feature**                                           | **EF6.4**| **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Basic class mapping                                   | Yes      | 1.0                                   |
| Constructors with parameters                          |          | 2.1                                   |
| Property value conversions                            |          | 2.1                                   |
| Mapped types with no keys                             |          | 2.1                                   |
| Conventions                                           | Yes      | 1.0                                   |
| Custom conventions                                    | Yes      | 1.0 (partial; [#214](https://github.com/dotnet/efcore/issues/214)) |
| Data annotations                                      | Yes      | 1.0                                   |
| Fluent API                                            | Yes      | 1.0                                   |
| Inheritance: Table per hierarchy (TPH)                | Yes      | 1.0                                   |
| Inheritance: Table per type (TPT)                     | Yes      | Planned for 5.0 ([#2266](https://github.com/dotnet/efcore/issues/2266)) |
| Inheritance: Table per concrete class (TPC)           | Yes      | Stretch for 5.0 ([#3170](https://github.com/dotnet/efcore/issues/3170)) <sup>(1)</sup> |
| Shadow state properties                               |          | 1.0                                   |
| Alternate keys                                        |          | 1.0                                   |
| Many-to-many navigations                              | Yes      | Planned for 5.0 ([#19003](https://github.com/dotnet/efcore/issues/19003)) |
| Many-to-many without join entity                      | Yes      | On the backlog ([#1368](https://github.com/dotnet/efcore/issues/1368)) |
| Key generation: Database                              | Yes      | 1.0                                   |
| Key generation: Client                                |          | 1.0                                   |
| Complex/owned types                                   | Yes      | 2.0                                   |
| Spatial data                                          | Yes      | 2.2                                   |
| Model format: Code                                    | Yes      | 1.0                                   |
| Create model from database: Command line              | Yes      | 1.0                                   |
| Update model from database                            | Partial  | On the backlog ([#831](https://github.com/dotnet/efcore/issues/831)) |
| Global query filters                                  |          | 2.0                                   |
| Table splitting                                       | Yes      | 2.0                                   |
| Entity splitting                                      | Yes      | Stretch for 5.0 ([#620](https://github.com/dotnet/efcore/issues/620)) <sup>(1)</sup> |
| Database scalar function mapping                      | Poor     | 2.0                                   |
| Field mapping                                         |          | 1.1                                   |
| Nullable reference types (C# 8.0)                     |          | 3.0                                   |
| Graphical visualization of model                      | Yes      | No support planned <sup>(2)</sup>     |
| Graphical model editor                                | Yes      | No support planned <sup>(2)</sup>     |
| Model format: EDMX (XML)                              | Yes      | No support planned <sup>(2)</sup>     |
| Create model from database: VS wizard                 | Yes      | No support planned <sup>(2)</sup>     |

### Querying data

| **Feature**                                           | **EF6.4**| **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| LINQ queries                                          | Yes      | 1.0                                   |
| Readable generated SQL                                | Poor     | 1.0                                   |
| GroupBy translation                                   | Yes      | 2.1                                   |
| Loading related data: Eager                           | Yes      | 1.0                                   |
| Loading related data: Eager loading for derived types |          | 2.1                                   |
| Loading related data: Lazy                            | Yes      | 2.1                                   |
| Loading related data: Explicit                        | Yes      | 1.1                                   |
| Raw SQL queries: Entity types                         | Yes      | 1.0                                   |
| Raw SQL queries: Keyless entity types                 | Yes      | 2.1                                   |
| Raw SQL queries: Composing with LINQ                  |          | 1.0                                   |
| Explicitly compiled queries                           | Poor     | 2.0                                   |
| await foreach (C# 8.0)                                |          | 3.0                                   |
| Text-based query language (Entity SQL)                | Yes      | No support planned <sup>(2)</sup>     |

### Saving data

| **Feature**                                           | **EF6.4**| **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Change tracking: Snapshot                             | Yes      | 1.0                                   |
| Change tracking: Notification                         | Yes      | 1.0                                   |
| Change tracking: Proxies                              | Yes      | Merged for 5.0 ([#10949](https://github.com/dotnet/efcore/issues/10949)) |
| Accessing tracked state                               | Yes      | 1.0                                   |
| Optimistic concurrency                                | Yes      | 1.0                                   |
| Transactions                                          | Yes      | 1.0                                   |
| Batching of statements                                |          | 1.0                                   |
| Stored procedure mapping                              | Yes      | On the backlog ([#245](https://github.com/dotnet/efcore/issues/245)) |
| Disconnected graph low-level APIs                     | Poor     | 1.0                                   |
| Disconnected graph End-to-end                         |          | 1.0 (partial; [#5536](https://github.com/dotnet/efcore/issues/5536)) |

### Other features

| **Feature**                                           | **EF6.4**| **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Migrations                                            | Yes      | 1.0                                   |
| Database creation/deletion APIs                       | Yes      | 1.0                                   |
| Seed data                                             | Yes      | 2.1                                   |
| Connection resiliency                                 | Yes      | 1.1                                   |
| Interceptors                                          | Yes      | 3.0                                   |
| Events                                                | Yes      | 3.0 (partial; [#626](https://github.com/dotnet/efcore/issues/626)) |
| Simple Logging (Database.Log)                         | Yes      | Merged for 5.0 ([#1199](https://github.com/dotnet/efcore/issues/1199)) |
| DbContext pooling                                     |          | 2.0                                   |

### Database providers <sup>(3)</sup>

| **Feature**                                           | **EF6.4**| **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| SQL Server                                            | Yes      | 1.0                                   |
| MySQL                                                 | Yes      | 1.0                                   |
| PostgreSQL                                            | Yes      | 1.0                                   |
| Oracle                                                | Yes      | 1.0                                   |
| SQLite                                                | Yes      | 1.0                                   |
| SQL Server Compact                                    | Yes      | 1.0 <sup>(4)</sup>                    |
| DB2                                                   | Yes      | 1.0                                   |
| Firebird                                              | Yes      | 2.0                                   |
| Jet (Microsoft Access)                                |          | 2.0 <sup>(4)</sup>                    |
| Azure Cosmos DB                                       |          | 3.0                                   |
| In-memory (for testing)                               |          | 1.0                                   |

<sup>1</sup> Stretch goals are not likely to be achieved for a given release. However, if things go well, then we will try to pull them in.

<sup>2</sup> Some EF6 features will not be implemented in EF Core. These features either depend on EF6's underlying Entity Data Model (EDM) and/or are complex features with relatively low return on investment. We always welcome feedback, but while EF Core enables many things not possible in EF6, it is conversely not feasible for EF Core to support all the features of EF6.

<sup>3</sup> EF Core database providers implemented by third-parties may be delayed in updating to new major versions of EF Core. See [Database Providers](xref:core/providers/index) for more information.

<sup>4</sup> The SQL Server Compact and Jet providers only work on .NET Framework (not on .NET Core).

### Supported platforms

EF Core 3.1 runs on .NET Core and .NET Framework, through the use of .NET Standard 2.0. However, EF Core 5.0 will not run on .NET Framework. See [Platforms](xref:core/miscellaneous/platforms) for more details.

EF6.4 runs on .NET Core and .NET Framework, through multi-targeting.

## Guidance for new applications

Use EF Core on .NET Core for all new applications unless the app needs something that is [only supported on .NET Framework](/dotnet/standard/choosing-core-framework-server).

## Guidance for existing EF6 applications

EF Core is not a drop-in replacement for EF6. Moving from EF6 to EF Core will likely require changes to your application.

When moving an EF6 app to .NET Core:

* Keep using EF6 if the data access code is stable and not likely to evolve or need new features.
* Port to EF Core if the data access code is evolving or if the app needs new features only available in EF Core.
* Porting to EF Core is also often done for performance. However, not all scenarios are faster, so do some profiling first.

See [Porting from EF6 to EF Core](xref:efcore-and-ef6/porting/index) for more information.
