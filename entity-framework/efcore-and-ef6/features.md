---
title: Feature Comparison
author: rowanmiller
ms.author: rowmil
manager: rowanmiller
ms.date: 10/27/2016
ms.topic: article
ms.assetid: f22f29ef-efc0-475d-b0b2-12a054f80f95
ms.prod: entity-framework
uid: efcore-and-ef6/features
---

# Feature Comparison

The following information will help you choose between Entity Framework Core and Entity Framework 6.x.

## Features not in EF Core

This is a list of features not currently implemented in EF Core that are likely to impact your ability to use it in a given application. This is by no means an exhaustive list of possible O/RM features, but the features that we feel have the highest impact on developers.

* Creating a Model

     * **Complex/value types** are types that do not have a primary key and are used to represent a set of properties on an entity type.

     * **Visualizing a model** to see a graphical representation of the code-based model.

     * **Simple type conversions** such as string => xml.

     * **Spatial data types** such as SQL Server's *geography* & *geometry*.

     * **Many-to-many relationships** without join entity. You can already model a many-to-many relationship with a join entity, see [Relationships](../core/modeling/relationships.md) for details.

     * **Alternate inheritance mapping patterns** for relational databases, such as table per type (TPT) and table per concrete type (TPC). Table per hierarchy (TPH) is already supported.

* Querying Data

     * **Improved translation** to enable more queries to successfully execute, with more logic being evaluated in the database (rather than in-memory).

     * **GroupBy translation** in particular will move translation of the LINQ GroupBy operator to the database, rather than in-memory.

     * **Lazy loading** enables navigation properties to be automatically populated from the database when they are accessed.

     * **Explicit Loading** allows you to trigger population of a navigation property on an entity that was previously loaded from the database.

     * **Raw SQL queries for non-model types** allows a raw SQL query to be used to populate types that are not part of the model (typically for denormalized view-model data).

* Saving Data

     * **Simple command interception** provides an easy way to read/write commands before/after they are sent to the database.

     * **Missing EntityEntry APIs from EF6.x** such as `Reload`, `GetModifiedProperties`, `GetDatabaseValues` etc.

     * **Stored procedure mapping** allows EF to use stored procedures to persist changes to the database (`FromSql` already provides good support for using a stored procedure to query, see [Raw SQL Queries](../core/querying/raw-sql.md) for details).

     * **Connection resiliency** automatically retries failed database commands. This is especially useful when connection to SQL Azure, where transient failures are common.

* Database Schema Management

     * **Visual Studio wizard for reverse engineer** that allows you to visually configure connection, select tables, etc. when creating a model from an existing database.

     * **Update model from database** allows a model that was previously reverse engineered from the database to be refreshed with changes made to the schema.

     * **Seed data** allows a set of data to be easily upserted to the database.

## Side-by-side comparison

The following table compares the features available in EF Core and EF6.x. It is intended to give a high level comparison and does not list every feature, or attempt to give details on possible differences between how the same feature works.

| Creating a Model                                    | EF6.x  | EF Core 1.0.0                   |
| --------------------------------------------------- | ------ | ------------------------------- |
| Basic modelling (classes, properties, etc.)         | Yes    | Yes                             |
| Conventions                                         | Yes    | Yes                             |
| Custom conventions                                  | Yes    | Partial                         |
| Data annotations                                    | Yes    | Yes                             |
| Fluent API                                          | Yes    | Yes                             |
| Inheritance: Table per hierarchy (TPH)              | Yes    | Yes                             |
| Inheritance: Table per type (TPT)                   | Yes    |                                 |
| Inheritance: Table per concrete class (TPC)         | Yes    |                                 |
| Shadow state properties                             |        | Yes                             |
| Alternate keys                                      |        | Yes                             |
| Many-to-many: With join entity                      | Yes    | Yes                             |
| Many-to-many: Without join entity                   | Yes    |                                 |
| Key generation: Database                            | Yes    | Yes                             |
| Key generation: Client                              |        | Yes                             |
| Complex/value types                                 | Yes    |                                 |
| Spatial data                                        | Yes    |                                 |
| Graphical visualization of model                    | Yes    |                                 |
| Graphical drag/drop editor                          | Yes    |                                 |
| Model format: Code                                  | Yes    | Yes                             |
| Model format: EDMX (XML)                            | Yes    |                                 |
| Reverse engineer model from database: Command line  |        | Yes                             |
| Reverse engineer model from database: VS wizard     | Yes    |                                 |
| Incremental update model from database              | Yes    |                                 |
|                                                     |        |                                 |
| **Querying Data**                                     |**EF6.x**| **EF Core 1.0.0**                |
| LINQ: Simple queries                                | Stable | Stable                          |
| LINQ: Moderate queries                              | Stable | Stabilizing                     |
| LINQ: Complex queries                               | Stable | In-Progress                     |
| LINQ: Queries using navigation properties           | Stable | In-Progress                     |
| “Pretty” SQL generation                             | Poor   | Yes                             |
| Mixed client/server evaluation                      |        | Yes                             |
| Loading related data: Eager                         | Yes    | Yes                             |
| Loading related data: Lazy                          | Yes    |                                 |
| Loading related data: Explicit                      | Yes    |                                 |
| Raw SQL queries: Model types                        | Yes    | Yes                             |
| Raw SQL queries: Un-mapped types                    | Yes    |                                 |
| Raw SQL queries: Composing with LINQ                |        | Yes                             |
|                                                     |        |                                 |
| **Saving Data**                                       | **EF6.x** | **EF Core 1.0.0**                 |
| SaveChanges                                         | Yes    | Yes                             |
| Change tracking: Snapshot                           | Yes    | Yes                             |
| Change tracking: Notification                       | Yes    | Yes                             |
| Accessing tracked state                             | Yes    | Partial                         |
| Optimistic concurrency                              | Yes    | Yes                             |
| Transactions                                        | Yes    | Yes                             |
| Batching of statements                              |        | Yes                             |
| Stored procedure                                    | Yes    |                                 |
| Detached graph support (N-Tier): Low level APIs     | Poor   | Yes                             |
| Detached graph support (N-Tier): End-to-end         |        | Poor                            |
|                                                     |        |                                 |
| **Other Features**                                     | **EF6.x**  | **EF Core 1.0.0**                  |
| Migrations                                          | Yes    | Yes                             |
| Database creation/deletion APIs                     | Yes    | Yes                             |
| Seed data                                           | Yes    |                                 |
| Connection resiliency                               | Yes    |                                 |
| Lifecycle hooks (events, command interception, ...) | Yes    |                                 |
|                                                     |        |                                 |
| **Database Providers**                                 | **EF6.x** | **EF Core 1.0.0**                 |
| SQL Server                                          | Yes    | Yes                             |
| MySQL                                               | Yes    | Paid only, unpaid coming soon 1 |
| PostgreSQL                                          | Yes    | Yes                             |
| Oracle                                              | Yes    | Paid only, unpaid coming soon 1 |
| SQLite                                              | Yes    | Yes                             |
| SQL Compact                                         | Yes    | Yes                             |
| DB2                                                 | Yes    | Yes                             |
| InMemory (for testing)                              |        | Yes                             |
| Azure Table Storage                                 |        | Prototype                       |
| Redis                                               |        | Prototype                       |
|                                                     |        |                                 |
| **Application Models**                                  | **EF6.x**  | **EF Core 1.0.0**                   |
| WinForms                                            | Yes    | Yes                             |
| WPF                                                 | Yes    | Yes                             |
| Console                                             | Yes    | Yes                             |
| ASP.NET                                             | Yes    | Yes                             |
| ASP.NET Core                                        |        | Yes                             |
| Xamarin                                             |        | Coming soon 2                   |
| UWP                                                 |        | Yes                             |

Footnotes:
* <sup>1</sup> Paid providers are available, unpaid providers are being worked on. The teams working on the unpaid providers have not shared public details of timeline etc.

* <sup>2</sup> EF Core is built to work on Xamarin when support for .NET Standard is enabled in Xamarin.
