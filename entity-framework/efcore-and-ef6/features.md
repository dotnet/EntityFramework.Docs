---
title: EF Core & EF6 Feature by Feature Comparison
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016
ms.assetid: f22f29ef-efc0-475d-b0b2-12a054f80f95
uid: efcore-and-ef6/features
---

# EF Core and EF6 Feature by Feature Comparison

The following table compares the features available in EF Core and EF6. It is intended to give a high level comparison and does not list every feature, or attempt to give details on possible differences between how the same feature works.

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
| Raw SQL queries: Non-entity types (e.g. query types)  | Yes      | 2.1                                   |
| Raw SQL queries: Composing with LINQ                  |          | 1.0                                   |
| Explicitly compiled queries                           | Poor     | 2.0                                   |
| Text-based query language (e.g. Entity SQL)           | 1.0      |                                       |
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
| Simple Logging (e.g. Database.Log)                    | Yes      |                                       |
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
