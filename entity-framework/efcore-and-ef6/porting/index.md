---
title: Port from EF6 to EF Core - EF
description: A detailed guide to port your EF6 apps to EF Core
author: jeremylikness
ms.alias: jeliknes
ms.date: 12/09/2021
uid: efcore-and-ef6/porting/index
---
# Port from EF6 to EF Core

Entity Framework Core, or EF Core for short, is a total rewrite of Entity Framework for modern application architectures. Due to fundamental changes, there is not a direct upgrade path. The purpose of this documentation is to provide an end-to-end guide for porting your EF6 applications to EF Core.

> [!IMPORTANT]
> Before you start the porting process it is important to validate that EF Core meets the data access requirements for your application. You can find everything you need in the [EF Core documentation](xref:core/index).

> [!IMPORTANT]
> There is a known issue ([microsoft/dotnet-apiport #993](https://github.com/microsoft/dotnet-apiport/issues/993)) with the [portability analyzer](/dotnet/standard/analyzers/portability-analyzer) that erroneously reports EF Core as incompatible with .NET 5 and .NET 6. These warnings can be safely ignored as EF Core is 100% compatible with .NET 5 and .NET 6 target frameworks.

## Reasons to upgrade

All new Entity Framework development is happening in EF Core. There are no plans to backport any new features to EF6. EF Core runs on the latest .NET runtimes and takes full advantage of runtime, platform-specific (such as ASP.NET Core or WPF) and language-specific features. Here are a few of the benefits you gain from upgrading:

- Take advantage of the ongoing **performance improvements** in EF Core. For example, one customer who migrated from EF6 to EF Core 6 saw a 40x reduction in use of a heavy query due to the [query splitting feature](xref:core/querying/single-split-queries). Many customers report enormous performance gains simply by moving to the latest EF Core.
- Use **new features** in EF Core. There will be no new features added to EF6. All of the new functionality, for example the [Azure Cosmos DB provider](xref:core/providers/cosmos/index) and [`DbContextFactory`](xref:core/what-is-new/ef-core-5.0/whatsnew#dbcontextfactory), will only be added to EF Core. For a full comparison of EF6 to EF Core, including several features exclusive to EF Core, see: [Compare EF Core & EF6](xref:efcore-and-ef6/index).
- **Modernize your application stack** by using dependency injection and seamlessly integrate your data access with technologies like gRPC and GraphQL.

## A note on migrations

This documentation uses the terms _port_ and _upgrade_ to avoid confusion with the term [_migrations_](xref:core/managing-schemas/migrations/index) as a feature of EF Core. Migrations in EF Core are not compatible with [EF6 Code First migrations](xref:ef6/modeling/code-first/migrations/index) due to significant improvements to how migrations are handled. There is not a recommended approach to port your migrations history, so plan to start "fresh" in EF Core. You can maintain the codebase and data from your EF6 migrations. Apply your final migration in EF6, then create an initial migration in EF Core. You will be able to track history in EF Core moving forward.

## Upgrade steps

The upgrade path has been split into several documents that are organized by the phase of your upgrade and the type of application.

### Determine your EF Core "flavor"

There are several approaches to how EF Core works with your domain model and database implementation. In general, most apps will follow one of these patterns and how you approach your port will depend on the application "flavor".

**Code as the source of truth** is an approach in which everything is modeled through code and classes, whether through data attributes, fluent configuration, or a combination of both. The database is initially generated based on the model defined in EF Core and further updates are typically handled through migrations. This is often referred to as "code first," but the name isn't entirely accurate because one approach is to start with an existing database, generate your entities, and then maintain with code moving forward.

The **Database as source of truth** approach involves reverse-engineering or scaffolding your code from the database. When schema changes are made, the code is either regenerated or updated to reflect the changes. This is often called "database first."

Finally, a more advanced **Hybrid mapping** approach follows the philosophy that the code and database are managed separately, and EF Core is used to map between the two. This approach typically eschews migrations.

The following table summarizes some high level differences:

|**Approach**|**Developer role**|**DBA role**|**Migrations**|**Scaffolding**|**Repo**|
|---|---|---|---|---|---|
|**Code first**|Design entities and verify/customize generated migrations|Verify schema definitions and changes|Per commit|N/A|Track entities, DbContext, and migrations|
|**Database first**|Reverse engineer after changes and verify generated entities|Inform developers when the database changes to re-scaffold|N/A|Per schema change|Track extensions/partial classes that extend the generated entities|
|**Hybrid**|Update fluent configuration to map whenever entities or database change|Inform developers when the database has changed so they can update entities and model configuration|N/A|N/A|Track entities and DbContext|

The hybrid approach is a more advanced approach with additional overhead compared to the traditional code and database approaches.

### Understand the impact of moving away from EDMX

EF6 supported a special model definition format named _Entity Data Model XML (EDMX)_. EDMX files contain multiple definitions, including conceptual schema definitions (CSDL), mapping specifications (MSL), and store schema definitions (SSDL). EF Core tracks the domain, mapping, and database schemas through internal model graphs and does not support the EDMX format. Many blog posts and articles mistakenly state this means EF Core only supports "code first." EF Core supports all three application models described in the previous section. You can rebuild the model in EF Core by [reverse-engineering the database](xref:core/managing-schemas/scaffolding). If you use EDMX for a visual representation of your entity model, consider using the open source [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools) that provide similar capabilities for EF Core.

For more information on the impact of lack of support for EDMX files, read the [porting EDMX](xref:efcore-and-ef6/porting/port-edmx#other-considerations) guide.

### Perform the upgrade steps

It is not a requirement to port the entire application. EF6 and EF Core can run in the same application (see: [using EF Core and EF6 in the same application](xref:efcore-and-ef6/side-by-side)). To minimize risk, you might consider:

1. Move to EF6 on .NET Core if you haven't already.
1. Migrate a small portion of your app to EF Core and run it side-by-side with EF6.
1. Eventually bring the rest of the codebase to EF Core and retire the EF6 code.

As for the port itself, at a high level, you will:

1. [Review behavior changes between EF6 and EF Core](xref:efcore-and-ef6/porting/port-behavior).
1. Perform your final migrations, if any, in EF6.
1. Create your EF Core project.
1. Either copy code to the new project, run reverse-engineering, or a combination of both.
1. Rename references and entities and update behaviors:
    - `System.Data.Entity` to `Microsoft.EntityFrameworkCore`
    - Change `DbContext` constructor to consume options and/or override `OnConfiguring`
    - `DbModelBuilder` to `ModelBuilder`
    - Rename `DbEntityEntry<T>` to `EntityEntry<T>`
    - Move from `Database.Log` to `Microsoft.Extensions.Logging` (advanced) or `DbContextOptionsBuilder.LogTo` (simple) APIs
    - Apply changes for `WithRequired` and `WithOptional` (see [here](xref:efcore-and-ef6/porting/port-detailed-cases#required-and-optional))
    - Update validation code. There is no data validation built into EF Core, but you can [do it yourself](/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations#use-validation-attributes-in-the-model-based-on-data-annotations).
    - Follow any necessary steps to [port from EDMX](xref:efcore-and-ef6/porting/port-edmx).
1. Perform specific steps based on your EF Core approach:
    - [Code as source of truth](xref:efcore-and-ef6/porting/port-code)
    - [Database as source of truth](xref:efcore-and-ef6/porting/port-database)
    - [Hybrid model](xref:efcore-and-ef6/porting/port-hybrid)

There are many considerations that relate to all of the approaches, so you will also want to review ways to address and work around the [detailed differences between EF6 and EF Core](xref:efcore-and-ef6/porting/port-detailed-cases).
