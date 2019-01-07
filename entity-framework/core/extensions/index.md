---
title: Tools & Extensions - EF Core
author: ErikEJ
ms.date: 01/07/2019
ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
uid: core/extensions/index
---

# EF Core Tools & Extensions

These tools and extensions provide additional functionality for Entity Framework Core 2.0 and later.

> [!IMPORTANT]  
> Extensions are built by a variety of sources and aren't maintained as part of the Entity Framework Core project. When considering a third party extension, be sure to evaluate its quality, licensing, compatibility, support, etc. to ensure it meets your requirements.

## Tools

### LLBLGen Pro

LLBLGen Pro is an entity modeling solution with support for Entity Framework and Entity Framework Core. It lets you easily define your entity model and map it to your database, using database first or model first, so you can get started writing queries right away.

[Website](https://www.llblgen.com/)

### Devart Entity Developer

Entity Developer is a powerful ORM designer for ADO.NET Entity Framework, NHibernate, LinqConnect, Telerik Data Access, and LINQ to SQL. It supports designing EF Core models visually, using model first or database first approaches, and C# or Visual Basic code generation. 

[Website](https://www.devart.com/entitydeveloper/)

### EF Core Power Tools

EF Core Power Tools is a Visual Studio 2017 extension that exposes various EF Core design-time tasks in a simple user interface. It includes reverse engineering of DbContext and entity classes from existing databases and [SQL Server DACPACs](https://docs.microsoft.com/sql/relational-databases/data-tier-applications/data-tier-applications), management of database migrations, and model visualizations.

[GitHub wiki](https://github.com/ErikEJ/EFCorePowerTools/wiki)

### Entity Framework Visual Editor

Entity Framework Visual Editor is a Visual Studio 2017 extension that adds an ORM designer for visual design of EF 6, and EF Core classes. Code is generated using T4 templates so can be customized to suit any needs. It supports inheritance, unidirectional and bidirectional associations, enumerations, and the ability to color-code your classes and add text blocks to explain potentially arcane parts of your design.

[Marketplace](https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner)

### CatFactory

CatFactory is a scaffolding engine for .NET Core that can automate the generation of DbContext classes, entities, mapping configurations, and repository classes from a SQL Server database.

[GitHub repository](https://github.com/hherzl/CatFactory.EntityFrameworkCore)

### LoreSoft's Entity Framework Core Generator

Entity Framework Core Generator (efg) is a .NET Core CLI tool that can generate EF Core models from an existing database, much like `dotnet ef dbcontext scaffold`, but it also supports safe code [regeneration](https://efg.loresoft.com/en/latest/regeneration/) via region replacement or by parsing mapping files. This tool supports generating view models, validation, and object mapper code. 

[Tutorial](http://www.loresoft.com/Generate-ASP-NET-Web-API)
[Documentation](https://efg.loresoft.com/en/latest/)

## Extensions

### Microsoft.EntityFrameworkCore.AutoHistory

A plugin library that enables automatically recording the data changes performed by EF Core into a history table.

[GitHub repository](https://github.com/Arch/AutoHistory/)

### Microsoft.EntityFrameworkCore.DynamicLinq

A .NET Core / .NET Standard port of System.Linq.Dynamic that includes async support with EF Core.
System.Linq.Dynamic originated as a Microsoft sample that shows how to construct LINQ queries dynamically from string expressions rather than code.

[GitHub repository](https://github.com/StefH/System.Linq.Dynamic.Core/)

### EFSecondLevelCache.Core

An extension that enables storing the results of EF Core queries into a second-level cache, so that subsequent executions of the same queries can avoid accessing the database and retrieve the data directly from the cache.

[GitHub repository](https://github.com/VahidN/EFSecondLevelCache.Core/)

### EntityFrameworkCore.PrimaryKey

This library allows retrieving the values of primary key (including composite keys) from any entity as a dictionary.

[GitHub repository](https://github.com/NickStrupat/EntityFramework.PrimaryKey/)

### EntityFrameworkCore.TypedOriginalValues

This library enables strongly typed access to the original values of entity properties. 

[GitHub repository](https://github.com/NickStrupat/EntityFramework.TypedOriginalValues/)

### Geco

Geco (Generator Console) is a simple code generator based on a console project, that runs on .NET Core and uses C# interpolated strings for code generation. Geco includes a reverse model generator for EF Core with support for pluralization, singularization, and editable templates. It also provides a seed data script generator, a script runner, and a database cleaner.

[GitHub repository](https://github.com/iQuarc/Geco)

### LinqKit.Microsoft.EntityFrameworkCore

LinqKit.Microsoft.EntityFrameworkCore is an EF Core-compatible version of the LINQKit library. LINQKit is a free set of extensions for LINQ to SQL and Entity Framework power users. It enables advanced functionality like dynamic building of predicate expressions, and using expression variables in subqueries.  

[GitHub repository](https://github.com/scottksmith95/LINQKit/)

### NeinLinq.EntityFrameworkCore

NeinLinq extends LINQ providers such as Entity Framework to enable reusing functions, rewriting queries, and building dynamic queries using translatable predicates and selectors.

[GitHub repository](https://github.com/axelheer/nein-linq/)

### Microsoft.EntityFrameworkCore.UnitOfWork

A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple databases with distributed transaction supported.

[GitHub repository](https://github.com/Arch/UnitOfWork/)

### EFCore.BulkExtensions

EF Core extensions for Bulk operations (Insert, Update, Delete).

[GitHub repository](https://github.com/borisdj/EFCore.BulkExtensions)

### Bricelam.EntityFrameworkCore.Pluralizer

Adds design-time pluralization to EF Core.

[GitHub repository](https://github.com/bricelam/EFCore.Pluralizer)

### PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql

A simple extension method that obtains the SQL statement EF Core would generate for a given LINQ query in simple scenarios. The ToSql method is limited to simple scenarios because EF Core can generate more than one SQL statement for a single LINQ query, and different SQL statements depending on parameter values.

[GitHub repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql)

### Toolbelt.EntityFrameworkCore.IndexAttribute

Revival of [Index] attribute for EF Core (with extension for model building).

[GitHub repository](https://github.com/jsakamoto/EntityFrameworkCore.IndexAttribute)

### EfCore.InMemoryHelpers

Provides a wrapper around the EF Core In-Memory Database Provider. Makes it act more like a relational provider.

[GitHub repository](https://github.com/SimonCropp/EfCore.InMemoryHelpers)

### EFCore.TemporalSupport

An implementation of temporal support for EF Core.

[GitHub repository](https://github.com/cpoDesign/EFCore.TemporalSupport)

### EntityFrameworkCore.Cacheable

A high-performance second-level query cache for EF Core.

[GitHub repository](https://github.com/SteffenMangold/EntityFrameworkCore.Cacheable)
