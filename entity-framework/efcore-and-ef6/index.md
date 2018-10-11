---
title: Compare EF Core & EF6
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: a6b9cd22-6803-4c6c-a4d4-21147c0a81cb
uid: efcore-and-ef6/index
---

# Compare EF Core & EF6

There are two versions of Entity Framework: Entity Framework 6 (EF6) and Entity Framework Core (EF Core). This article compares them and provides guidance on how to choose between them.

## Entity Framework 6

Entity Framework 6 (EF6) is a tried and tested data access technology. After many years of development, it is feature-rich and stable. It was first released in 2008, as part of .NET Framework 3.5 SP1 and Visual Studio 2008 SP1. Starting with the 4.1 release it has shipped as the [EntityFramework NuGet package](https://www.nuget.org/packages/EntityFramework/) &mdash; currently one of the most popular packages on NuGet.org.

EF6 continues to be a supported product and will continue to see bug fixes and minor improvements for some time to come.

## Entity Framework Core

EF Core is a rewrite of EF6, built on top of a completely new set of core components. It is a lightweight, extensible, and cross-platform version of Entity Framework. Its relative newness means:

* It is improved in many ways and offers some new features that won't be implemented in EF6 (such as alternate keys, batch updates, and mixed client/database evaluation in LINQ queries).
* It doesn't automatically inherit all the features of EF6. While some of these missing features will show up in future releases, other less commonly used features will not be implemented in EF Core.
* It is not as stable as EF6.

Although the code ase is all new, EF Core was designed offer a developer experience similar to EF6. Most of the top-level APIs remain the same, so EF Core will feel very familiar to folks who have used EF6.

## Feature comparison

The following table compares the features available in EF Core and EF6. It is intended to give a high level comparison and doesn't list every feature or detail the differences between the same feature in different EF versions.

The EF Core column contains the number of the product version in which the feature first appeared.

| **Creating a Model**                                  | **EF 6** | **EF Core**                           |
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
| Spatial data                                          | Yes      |                                       |
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
|                                                       |          |                                       |
| **Querying Data**                                     | **EF6**  | **EF Core**                           |
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
|                                                       |          |                                       |
| **Saving Data**                                       | **EF6**  | **EF Core**                           |
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
|                                                       |          |                                       |
| **Other Features**                                    | **EF6**  | **EF Core**                           |
| Migrations                                            | Yes      | 1.0                                   |
| Database creation/deletion APIs                       | Yes      | 1.0                                   |
| Seed data                                             | Yes      | 2.1                                   |
| Connection resiliency                                 | Yes      | 1.1                                   |
| Lifecycle hooks (events, interception)                | Yes      |                                       |
| Simple Logging (Database.Log)                         | Yes      |                                       |
| DbContext pooling                                     |          | 2.0                                   |
|                                                       |          |                                       |
| **Database Providers**                                | **EF6**  | **EF Core**                           |
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
|                                                       |          |                                       |
| **Platforms**                                         | **EF6**  | **EF Core**                           |
| .NET Framework (Console, WinForms, WPF, ASP.NET)      | Yes      | 1.0                                   |
| .NET Core (Console, ASP.NET Core)                     |          | 1.0                                   |
| Mono & Xamarin                                        |          | 1.0 (in-progress)                     |
| UWP                                                   |          | 1.0 (in-progress)                     |

<sup>1</sup> There is currently a paid provider available. A free official provider for Oracle is being worked on.
<sup>2</sup> This provider only works on .NET Framework (not on .NET Core).

## Guidance for new applications

Consider using EF Core for new applications if both of the following conditions are true:
* You want to take advantage of the capabilities of .NET Core and EF Core.
* The EF features that the app requires are supported by EF Core.

Consider using EF6 if both of the following conditions are true:
* The app will run on Windows and the .NET Framework 4.0 or later.
* The features that the app requires are supported by EF6.

## Guidance for existing EF6 applications

Because of the fundamental changes in EF Core we do not recommend moving an EF6 application to EF Core unless you have a compelling reason to make the change. If you want to move to EF Core to make use of new features, then make sure you are aware of its limitations before you start. For more information, see [Porting from EF6 to EF Core](porting/index.md). **The move from EF6 to EF Core is more a port than an upgrade.** 

## Next steps

For more information, see the documentation:
* <xref:core/index>
* <xref:ef6/index>
