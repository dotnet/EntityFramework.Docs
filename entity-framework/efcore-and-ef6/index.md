---
title: Compare Entity Framework 6 and Entity Framework Core
description: Provides guidance on how to choose between Entity Framework 6 and Entity Framework Core. 
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: a6b9cd22-6803-4c6c-a4d4-21147c0a81cb
uid: efcore-and-ef6/index
---

# Compare EF Core & EF6

Entity Framework is an object-relational mapper (O/RM) for .NET. This article compares the two versions: Entity Framework 6 and Entity Framework Core.

## Entity Framework 6

Entity Framework 6 (EF6) is a tried and tested data access technology. It was first released in 2008, as part of .NET Framework 3.5 SP1 and Visual Studio 2008 SP1. Starting with the 4.1 release it has shipped as the [EntityFramework](https://www.nuget.org/packages/EntityFramework/) NuGet package. EF6 runs on the .NET Framework 4.x, which means it runs only on Windows. 

EF6 continues to be a supported product, and will continue to see bug fixes and minor improvements.

## Entity Framework Core

Entity Framework Core (EF Core) is a complete rewrite of EF6 that was first released in 2016. It ships in Nuget packages, the main one being [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/). EF Core is a cross-platform product that can run on .NET Core or .NET Framework.

EF Core was designed to provide a developer experience similar to EF6. Most of the top-level APIs remain the same, so EF Core will feel familiar to developers who have used EF6.

## Feature comparison

EF Core offers new features that won't be implemented in EF6 (such as [alternate keys](xref:core/modeling/alternate-keys), [batch updates](xref:core/what-is-new/ef-core-1.0#relational-batching-of-statements), and [mixed client/database evaluation in LINQ queries](xref:core/querying/client-eval). But because it's a new code base, it also lacks some features that EF6 has.

The following tables compare the features available in EF Core and EF6. It's a high-level comparison and doesn't list every feature or explain differences between the same feature in different EF versions.

The EF Core column indicates the product version in which the feature first appeared.

### Creating a model

| **Feature**                                           | **EF 6** | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Basic class mapping                                   | Yes      | 1.0                                   |
| Constructors with parameters                          |          | 2.1                                   |
| Property value conversions                            |          | 2.1                                   |
| Mapped types with no keys (query types)               |          | 2.1                                   |
| Conventions                                           | Yes      | 1.0                                   |
| Custom conventions                                    | Yes      | 1.0 (partial)                         |
| Data annotations                                      | Yes      | 1.0                                   |
| Fluent API                                            | Yes      | 1.0                                   |
| Inheritance: Table per hierarchy (TPH)                | Yes      | 1.0                                   |
| Inheritance: Table per type (TPT)                     | Yes      |                                       |
| Inheritance: Table per concrete class (TPC)           | Yes      |                                       |
| Shadow state properties                               |          | 1.0                                   |
| Alternate keys                                        |          | 1.0                                   |
| Many-to-many without join entity                      | Yes      |                                       |
| Key generation: Database                              | Yes      | 1.0                                   |
| Key generation: Client                                |          | 1.0                                   |
| Complex/owned types                                   | Yes      | 2.0                                   |
| Spatial data                                          | Yes      | 2.2                                   |
| Graphical visualization of model                      | Yes      |                                       |
| Graphical model editor                                | Yes      |                                       |
| Model format: Code                                    | Yes      | 1.0                                   |
| Model format: EDMX (XML)                              | Yes      |                                       |
| Create model from database: Command line              | Yes      | 1.0                                   |
| Create model from database: VS wizard                 | Yes      |                                       |
| Update model from database                            | Partial  |                                       |
| Global query filters                                  |          | 2.0                                   |
| Table splitting                                       | Yes      | 2.0                                   |
| Entity splitting                                      | Yes      |                                       |
| Database scalar function mapping                      | Poor     | 2.0                                   |
| Field mapping                                         |          | 1.1                                   |

### Querying data

| **Feature**                                           | **EF6**  | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| LINQ queries                                          | Yes      | 1.0 (in-progress for complex queries) |
| Readable generated SQL                                | Poor     | 1.0                                   |
| Mixed client/server evaluation                        |          | 1.0                                   |
| GroupBy translation                                   | Yes      | 2.1                                   |
| Loading related data: Eager                           | Yes      | 1.0                                   |
| Loading related data: Eager loading for derived types |          | 2.1                                   |
| Loading related data: Lazy                            | Yes      | 2.1                                   |
| Loading related data: Explicit                        | Yes      | 1.1                                   |
| Raw SQL queries: Entity types                         | Yes      | 1.0                                   |
| Raw SQL queries: Non-entity types (query types)       | Yes      | 2.1                                   |
| Raw SQL queries: Composing with LINQ                  |          | 1.0                                   |
| Explicitly compiled queries                           | Poor     | 2.0                                   |
| Text-based query language (Entity SQL)                | Yes      |                                       |

### Saving data

| **Feature**                                           | **EF6**  | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Change tracking: Snapshot                             | Yes      | 1.0                                   |
| Change tracking: Notification                         | Yes      | 1.0                                   |
| Change tracking: Proxies                              | Yes      |                                       |
| Accessing tracked state                               | Yes      | 1.0                                   |
| Optimistic concurrency                                | Yes      | 1.0                                   |
| Transactions                                          | Yes      | 1.0                                   |
| Batching of statements                                |          | 1.0                                   |
| Stored procedure mapping                              | Yes      |                                       |
| Disconnected graph low-level APIs                     | Poor     | 1.0                                   |
| Disconnected graph End-to-end                         |          | 1.0 (partial)                         |

### Other features

| **Feature**                                           | **EF6**  | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| Migrations                                            | Yes      | 1.0                                   |
| Database creation/deletion APIs                       | Yes      | 1.0                                   |
| Seed data                                             | Yes      | 2.1                                   |
| Connection resiliency                                 | Yes      | 1.1                                   |
| Lifecycle hooks (events, interception)                | Yes      |                                       |
| Simple Logging (Database.Log)                         | Yes      |                                       |
| DbContext pooling                                     |          | 2.0                                   |

### Database providers

| **Feature**                                           | **EF6**  | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| SQL Server                                            | Yes      | 1.0                                   |
| MySQL                                                 | Yes      | 1.0                                   |
| PostgreSQL                                            | Yes      | 1.0                                   |
| Oracle                                                | Yes      | 1.0 <sup>(1)</sup>                    |
| SQLite                                                | Yes      | 1.0                                   |
| SQL Server Compact                                    | Yes      | 1.0 <sup>(2)</sup>                    |
| DB2                                                   | Yes      | 1.0                                   |
| Firebird                                              | Yes      | 2.0                                   |
| Jet (Microsoft Access)                                |          | 2.0 <sup>(2)</sup>                    |
| In-memory (for testing)                               |          | 1.0                                   |

<sup>1</sup> There is currently a paid provider available for Oracle. A free official provider for Oracle is being worked on.

<sup>2</sup> The SQL Server Compact and Jet providers only work on .NET Framework (not on .NET Core).

### .NET implementations

| **Feature**                                           | **EF6**  | **EF Core**                           |
|:------------------------------------------------------|:---------|:--------------------------------------|
| .NET Framework (Console, WinForms, WPF, ASP.NET)      | Yes      | 1.0                                   |
| .NET Core (Console, ASP.NET Core)                     |          | 1.0                                   |
| Mono & Xamarin                                        |          | 1.0 (in-progress)                     |
| UWP                                                   |          | 1.0 (in-progress)                     |

## Guidance for new applications

Consider using EF Core for a new application if both of the following conditions are true:
* The app needs the capabilities of .NET Core. For more information, see [Choosing between .NET Core and .NET Framework for server apps](https://docs.microsoft.com/dotnet/standard/choosing-core-framework-server).
* EF Core supports all of the features that the app requires. If a desired feature is missing, check the [EF Core Roadmap](xref:core/what-is-new/roadmap) to find out if there are plans to support it in the future. 

Consider using EF6 if both of the following conditions are true:
* The app will run on Windows and the .NET Framework 4.0 or later.
* EF6 supports all of the features that the app requires.

## Guidance for existing EF6 applications

Because of the fundamental changes in EF Core, we do not recommend moving an EF6 application to EF Core unless there is a compelling reason to make the change. If you want to move to EF Core to use new features, make sure you're aware of its limitations. For more information, see [Porting from EF6 to EF Core](porting/index.md). **The move from EF6 to EF Core is more a port than an upgrade.** 

## Next steps

For more information, see the documentation:
* <xref:core/index>
* <xref:ef6/index>
