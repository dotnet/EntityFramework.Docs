---
title: Tools & Extensions - EF Core
author: ErikEJ
ms.date: 12/17/2019
ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
uid: core/extensions/index
---

# EF Core Tools & Extensions

These tools and extensions provide additional functionality for Entity Framework Core 2.1 and later.

> [!IMPORTANT]  
> Extensions are built by a variety of sources and aren't maintained as part of the Entity Framework Core project. When considering a third party extension, be sure to evaluate its quality, licensing, compatibility, support, etc. to ensure it meets your requirements. In particular, an extension built for an older version of EF Core may need updating before it will work with the latest versions.

## Tools

### LLBLGen Pro

LLBLGen Pro is an entity modeling solution with support for Entity Framework and Entity Framework Core. It lets you easily define your entity model and map it to your database, using database first or model first, so you can get started writing queries right away. For EF Core: 2.

[Website](https://www.llblgen.com/)

### Devart Entity Developer

Entity Developer is a powerful ORM designer for ADO.NET Entity Framework, NHibernate, LinqConnect, Telerik Data Access, and LINQ to SQL. It supports designing EF Core models visually, using model first or database first approaches, and C# or Visual Basic code generation. For EF Core: 2.

[Website](https://www.devart.com/entitydeveloper/)

### EF Core Power Tools

EF Core Power Tools is a Visual Studio extension that exposes various EF Core design-time tasks in a simple user interface. It includes reverse engineering of DbContext and entity classes from existing databases and [SQL Server DACPACs](https://docs.microsoft.com/sql/relational-databases/data-tier-applications/data-tier-applications), management of database migrations, and model visualizations. For EF Core: 2, 3.

[GitHub wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki)

### Entity Framework Visual Editor

Entity Framework Visual Editor is a Visual Studio extension that adds an ORM designer for visual design of EF 6, and EF Core classes. Code is generated using T4 templates so can be customized to suit any needs. It supports inheritance, unidirectional and bidirectional associations, enumerations, and the ability to color-code your classes and add text blocks to explain potentially arcane parts of your design. For EF Core: 2.

[Marketplace](https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner)

### CatFactory

CatFactory is a scaffolding engine for .NET Core that can automate the generation of DbContext classes, entities, mapping configurations, and repository classes from a SQL Server database. For EF Core: 2.

[GitHub repository](https://github.com/hherzl/CatFactory.EntityFrameworkCore)

### LoreSoft's Entity Framework Core Generator

Entity Framework Core Generator (efg) is a .NET Core CLI tool that can generate EF Core models from an existing database, much like `dotnet ef dbcontext scaffold`, but it also supports safe code [regeneration](https://efg.loresoft.com/en/latest/regeneration/) via region replacement or by parsing mapping files. This tool supports generating view models, validation, and object mapper code. For EF Core: 2.

[Tutorial](https://www.loresoft.com/Generate-ASP-NET-Web-API)
[Documentation](https://efg.loresoft.com/en/latest/)

## Extensions

### Microsoft.EntityFrameworkCore.AutoHistory

A plugin library that enables automatically recording the data changes performed by EF Core into a history table. For EF Core: 2.

[GitHub repository](https://github.com/Arch/AutoHistory/)

### EFSecondLevelCache.Core

An extension that enables storing the results of EF Core queries into a second-level cache, so that subsequent executions of the same queries can avoid accessing the database and retrieve the data directly from the cache. For EF Core: 2.

[GitHub repository](https://github.com/VahidN/EFSecondLevelCache.Core/)

### Geco

Geco (Generator Console) is a simple code generator based on a console project, that runs on .NET Core and uses C# interpolated strings for code generation. Geco includes a reverse model generator for EF Core with support for pluralization, singularization, and editable templates. It also provides a seed data script generator, a script runner, and a database cleaner. For EF Core: 2.

[GitHub repository](https://github.com/iQuarc/Geco)

### EntityFrameworkCore.Scaffolding.Handlebars 

Allows customization of classes reverse engineered from an existing database using the Entity Framework Core toolchain with Handlebars templates. For EF Core: 2, 3.

[GitHub repository](https://github.com/TrackableEntities/EntityFrameworkCore.Scaffolding.Handlebars)

### NeinLinq.EntityFrameworkCore 

NeinLinq extends LINQ providers such as Entity Framework to enable reusing functions, rewriting queries, and building dynamic queries using translatable predicates and selectors. For EF Core: 2, 3.

[GitHub repository](https://github.com/axelheer/nein-linq/)

### Microsoft.EntityFrameworkCore.UnitOfWork

A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple databases with distributed transaction supported. For EF Core: 2.

[GitHub repository](https://github.com/Arch/UnitOfWork/)

### EFCore.BulkExtensions

EF Core extensions for Bulk operations (Insert, Update, Delete). For EF Core: 2, 3.

[GitHub repository](https://github.com/borisdj/EFCore.BulkExtensions)

### Bricelam.EntityFrameworkCore.Pluralizer

Adds design-time pluralization. For EF Core: 2.

[GitHub repository](https://github.com/bricelam/EFCore.Pluralizer)

### Toolbelt.EntityFrameworkCore.IndexAttribute

Revival of [Index] attribute (with extension for model building). For EF Core: 2, 3.

[GitHub repository](https://github.com/jsakamoto/EntityFrameworkCore.IndexAttribute)

### EfCore.InMemoryHelpers

Provides a wrapper around the EF Core In-Memory Database Provider. Makes it act more like a relational provider. For EF Core: 2.

[GitHub repository](https://github.com/SimonCropp/EfCore.InMemoryHelpers)

### EFCore.TemporalSupport

An implementation of temporal support. For EF Core: 2.

[GitHub repository](https://github.com/cpoDesign/EFCore.TemporalSupport)

### EfCoreTemporalTable

Easily perform temporal queries on your favourite database. For EF Core: 3.

[GitHub repository](https://github.com/glautrou/EfCoreTemporalTable)

### EntityFrameworkCore.TemporalTables

Extension library for Entity Framework Core which allows developers who use SQL Server to easily use temporal tables. For EF Core: 2.

[GitHub repository](https://github.com/findulov/EntityFrameworkCore.TemporalTables)


### EntityFrameworkCore.Cacheable

A high-performance second-level query cache. For EF Core: 2.

[GitHub repository](https://github.com/SteffenMangold/EntityFrameworkCore.Cacheable)

### Entity Framework Plus

Extends your DbContext with features such as: Include Filter, Auditing, Caching, Query Future, Batch Delete, Batch Update, and more. For EF Core: 2, 3.

[Website](https://entityframework-plus.net/)
[GitHub repository](https://github.com/zzzprojects/EntityFramework-Plus)

### Entity Framework Extensions

Extends your DbContext with high-performance bulk operations: BulkSaveChanges, BulkInsert, BulkUpdate, BulkDelete, BulkMerge, and more. For EF Core: 2, 3.

[Website](https://entityframework-extensions.net/)
