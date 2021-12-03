---
title: Port from EF6 to EF Core - EF
description: A detailed guide to port your 
author: jeremylikness
ms.alias: jeliknes
ms.date: 10/25/2021
uid: efcore-and-ef6/porting/index
---
# Port from EF6 to EF Core

Entity Framework Core, or EF Core for short, is a total rewrite of Entity Framework for modern application architectures. Due to fundamental changes, there is not a direct upgrade path. The purpose of this documentation is to provide an end-to-end guide for porting your EF6 applications to EF Core.

> [!IMPORTANT]
> Before you start the porting process it is important to validate that EF Core meets the data access requirements for your application.

## When to port

It is not a requirement that you port your code to EF Core. There are many reasons why you may choose to remain on EF6. The latest version of EF6 supports .NET Core. If your application is running fine and you don't need any of the latest features, you may wish to consider remaining on EF6. Some reasons you might consider porting:

- Take advantage of the ongoing performance improvements in EF Core. For example, one customer who migrated from EF6 to EF Core 6 saw a 40x reduction in use of a heavy query due to the [query splitting feature](/ef/core/querying/single-split-queries/). Many customers report enormous performance gains simply by moving to the latest EF Core.

- Use new features in EF Core. There will be no new features added to EF6. All of the new functionality, for example the [Azure Cosmos DB provider](/ef/core/providers/cosmos/) and [`DbContextFactory`](/ef/core/what-is-new/ef-core-5.0/whatsnew#dbcontextfactory), will only be added EF Core.

- Modernize your application stack by seamlessly integrating your data access with technologies like gRPC and GraphQL.

Regardless of your reasons for considering an upgrade, make sure that EF Core has all the features you need to use in your application. See [Feature Comparison](xref:efcore-and-ef6/index) for a detailed comparison of how the feature set in EF Core compares to EF6. If any required features are missing, ensure that you can compensate for the lack of these features before porting to EF Core.

## Migrations

This documentation uses the terms _port_ and _upgrade_ to avoid confusion with the term [_migrations_](/ef/core/managing-schemas/migrations/) as a feature of EF Core. Migrations in EF Core are fundamentally different than [EF6 Code First migrations](/ef/ef6/modeling/code-first/migrations/). There is not a recommend approach to port your migrations history, so plan to start "fresh" in EF Core. You can maintain the codebase and data from your EF6 migrations. Apply your final migration in EF6, then create an initial migration in EF Core. You will be able to track history in EF Core moving forward.

## Upgrade steps

The upgrade path has been split into several documents that are organized by the phase of your upgrade and the type of application.

### Determine your EF Core "flavor"

There are several approaches to how EF Core works with your domain model and database implementation. In general, most apps will follow one of these patterns and how you approach your port will depend on the application "flavor.

**Code as the source of truth** is an approach in which everything is modeled through code and classes, whether through data attributes, fluent configuration, or a combination of both. The database is initially generated based on the model defined in EF Core and further updates are typically handled through migrations.

The **Database as source of truth** approach involves reverse-engineering or scaffolding your code from the database. When schema changes are made, the code is either regenerated or updated to reflect the changes.

Finally, a more common **Hybrid mapping** approach follows the philosophy that the code and database are managed separately, and EF Core is used to map between the two. This approach may or may not use migrations.

### Understand the impact of moving away from EDMX

EF6 supported a special model definition format named **Entity Data Model XML (EDMX)**. EDMX files contain multiple definitions, including conceptual schema definitions (CSDL), mapping specifications (MSL), and store schema definitions (SSDL). EF Core tracks the domain, mapping, and database schemas through internal model graphs and does not support the EDMX format. Many blog posts and articles mistakenly claim this means EF Core only supports code first. EF Core supports all three application models described in the previous section. You can rebuild the model in EF Core by [reverse-engineering the database](/ef/core/managing-schemas/scaffolding). If you use EDMX for a visual representation of your entity model, consider using the open source [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools).

For more information on the impact of lack of support for EDMX files, read the [porting EDMX](/efcore-and-ef6/porting/port-edmx#other-considerations) guide.

### Perform the upgrade steps

The following documents detail specific steps for the port. At a high level, you will:

1. [Review behavior changes between EF6 and EF Core](/efcore-and-ef6/porting/port-behavior).
1. Perform your final migrations, if any, in EF6.
1. Create your EF Core project.
1. Either copy code to the new project, run reverse-engineering, or a combination of both.
1. Rename references and entities and update behaviors:
    - `System.Data.Entity` to `Microsoft.EntityFrameworkCore`
    - Change `DbContext` constructor to consume options and/or override `OnConfiguring`
    - `DbModelBuilder` to `ModelBuilder`
    - Rename `DbEntityEntry<T>` to `EntityEntry<T>`
    - Move from `Database.Log` to `Microsoft.Extensions.Logging APIs`
    - Apply changes for `WithRequired` and `WithOptional` (see [here](/efcore-and-ef6/porting/port-detailed-cases#required-and-optional))
    - Update validation code. There is no data validation built into EF Core, but you can [do it yourself](/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations#use-validation-attributes-in-the-model-based-on-data-annotations).
1. Perform specific steps based on your EF Core approach:
    - [Code as source of truth](/efcore-and-ef6/porting/port-code.md)
    - [Database as source of truth](/efcore-and-ef6/porting/port-database.md)
    - [Hybrid model](/efcore-and-ef6/porting/port-hybrid.md)

Finally, review some ways to address and work around the [detailed differences](/efcore-and-ef6/porting/port-detailed-cases.md).
