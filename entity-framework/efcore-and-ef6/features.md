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

| **Creating a Model** |**EF 6** |**EF Core** |
|-|-|-|
| Basic class mapping                         | Yes | 1.0 |
| Conventions                                 | Yes | 1.0 |
| Custom conventions                          | Yes | 1.0 (partial) |
| Data annotations                            | Yes | 1.0 |
| Fluent API                                  | Yes | 1.0 |
| Inheritance: Table per hierarchy (TPH)      | Yes | 1.0 |
| Inheritance: Table per type (TPT)           | Yes |     |
| Inheritance: Table per concrete class (TPC) | Yes |     |
| Shadow state properties                     |     | 1.0 |
| Alternate keys                              |     | 1.0 |
| Many-to-many without join entity            | Yes |     |
| Key generation: Database                    | Yes | 1.0 |
| Key generation: Client                      |     | 1.0 |
| Complex/owned types                         | Yes | 2.0 |
| Spatial data                                | Yes |     |
| Graphical visualization of model            | Yes |     |
| Graphical model editor                      | Yes |     |
| Model format: Code                          | Yes | 1.0 |
| Model format: EDMX (XML)                    | Yes |     |
| Create model from database: Command line    | Yes | 1.0 |
| Create model from database: VS wizard       | Yes |     |
| Update model from database                  | Partial | |
| Global query filters                        |     | 2.0 |
| Table splitting                             | Yes | 2.0 |
| Entity splitting                            | Yes |     |
| Database scalar function mapping            | Poor | 2.0 |
| Field mapping                               |     | 1.1 |
| | | |
| **Querying Data** |**EF6** |**EF Core** |
| LINQ queries                                | Yes | 1.0 (in-progress for complex queries) |
| Readable generated SQL                      | Poor | 1.0 |
| Mixed client/server evaluation              |     | 1.0 |
| Loading related data: Eager                 | Yes | 1.0 |
| Loading related data: Lazy                  | Yes |     |
| Loading related data: Explicit              | Yes | 1.1 |
| Raw SQL queries: Model types                | Yes | 1.0 |
| Raw SQL queries: Non-model types            | Yes |     |
| Raw SQL queries: Composing with LINQ        |     | 1.0 |
| Explicitly compiled queries                 | Poor | 2.0 |
| | | |
| **Saving Data** |**EF6** |**EF Core** |
| Change tracking: Snapshot                   | Yes | 1.0 |
| Change tracking: Notification               | Yes | 1.0 |
| Accessing tracked state                     | Yes | 1.0 |
| Optimistic concurrency                      | Yes | 1.0 |
| Transactions                                | Yes | 1.0 |
| Batching of statements                      |     | 1.0 |
| Stored procedure                            | Yes |     |
| Disconnected graph low-level APIs           | Poor | 1.0 |
| Disconnected graph End-to-end               |     | 1.0 (partial) |
| | | |
| **Other Features** |**EF6** |**EF Core** |
| Migrations                                  | Yes | 1.0 |
| Database creation/deletion APIs             | Yes | 1.0 |
| Seed data                                   | Yes |     |
| Connection resiliency                       | Yes | 1.1 |
| Lifecycle hooks (events, interception)      | Yes |     |
| DbContext pooling                           |     | 2.0 |
| | | |
| **Database Providers** |**EF6**|**EF Core** |
| SQL Server                                  | Yes | 1.0 |
| MySQL                                       | Yes | 1.0 |
| PostgreSQL                                  | Yes | 1.0 |
| Oracle                                      | Yes | 1.0 (paid only<sup>(1)</sup>) |
| SQLite                                      | Yes | 1.0 |
| SQL Compact                                 | Yes | 1.0 <sup>(2)</sup> |
| DB2                                         | Yes |     |
| In-memory (for testing)                      |     | 1.0 |
| | | |
| **Platforms** |**EF6** |**EF Core** |
| .NET Framework (Console, WinForms, WPF, ASP.NET) | Yes | 1.0 |
| .NET Core (Console, ASP.NET Core)           |     | 1.0 |
| Mono & Xamarin                              |     | 1.0 (in-progress) |
| UWP                                         |     | 1.0 (in-progress) |

<sup>1</sup> A free official provider for Oracle is being worked on.
<sup>2</sup> The SQL Server Compact provider only works on .NET Framework (not .NET Core).
