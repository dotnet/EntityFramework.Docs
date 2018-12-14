---
title: Tools & Extensions - EF Core
author: ErikEJ
ms.date: 07/03/2018
ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
uid: core/extensions/index
---

# EF Core Tools & Extensions

Tools and extensions provide additional functionality for Entity Framework Core.

> [!IMPORTANT]  
> Extensions are built by a variety of sources and not maintained as part of the Entity Framework Core project. When considering a third party extension, be sure to evaluate quality, licensing, compatibility, support, etc. to ensure they meet your requirements.

## Tools

### LLBLGen Pro

LLBLGen Pro is an entity modeling solution with support for Entity Framework and Entity Framework Core. It lets you easily define your entity model and map it to your database, using database first or model first, so you can get started writing queries right away.

[website](https://www.llblgen.com/)

### Devart Entity Developer

Entity Developer is a powerful ORM designer for ADO.NET Entity Framework, NHibernate, LinqConnect, Telerik Data Access, and LINQ to SQL. You can use  Model-First and Database-First approaches to design your ORM model and generate C# or Visual Basic .NET code for it. It introduces new approaches for designing ORM models, boosts productivity, and facilitates the development of database applications.

[website](https://www.devart.com/entitydeveloper/)

### EF Core Power Tools

Visual Studio 2017+ extension. You can reverse engineer of DbContext and POCO classes from an existing database or SQL Server Database project, and visualize and inspect your DbContext in various ways.

[GitHub wiki](https://github.com/ErikEJ/SqlCeToolbox/wiki/EF-Core-Power-Tools)

### Entity Framework Visual Editor

A Visual Studio 2017 extension that adds an ORM designer for visual design of Entity Framework 6, Core 2.0 and Core 2.1 classes. Code is generated using T4 templates so can be completely customized to suit any needs. Inheritance, unidirectional and bidirectional associations are all supported, as are enumerations and the ability to color-code your classes and add text blocks to explain potentially arcane parts of your design.

[Marketplace](https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner)

### CatFactory

CatFactory is a scaffolding engine for .NET Core and Entity Framework Core. The concept behind CatFactory it's export an existing database from SQL Server instance, then with the representation in models for database; scaffold entities, configurations, repositories and more.<!-- sentence needs review -->

[GitHub repository](https://github.com/hherzl/CatFactory.EntityFrameworkCore)

### LoreSoft's Entity Framework Core Generator

Entity Framework Core Generator (efg) is a .NET Core CLI tool that can generate EF Core models from an existing database, much like `dotnet ef dbcontext scaffold`. However it's different in that it also supports safe code [regeneration](https://efg.loresoft.com/en/latest/regeneration/). Regeneration is accomplished either via region replacement or by parsing mapping files. The tool also supports generating view models, validation and object mapper code. For more information, see the tutorial and the product documentation links.

[Tutorial](http://www.loresoft.com/Generate-ASP-NET-Web-API)
[Documentation](https://efg.loresoft.com/en/latest/)

## Extensions

### Microsoft.EntityFrameworkCore.AutoHistory

A plugin for Microsoft.EntityFrameworkCore to support automatically recording data changes history.

[GitHub repository](https://github.com/Arch/AutoHistory/)

### Microsoft.EntityFrameworkCore.DynamicLinq

Dynamic Linq extensions for Microsoft.EntityFrameworkCore which adds Async support

 [GitHub repository](https://github.com/StefH/System.Linq.Dynamic.Core/)

### EFCore.Practices

Attempt to capture some good or best practices in an API that supports testing – including a small framework to scan for N+1 queries.

[GitHub repository](https://github.com/riezebosch/efcore-practices/tree/master/src/EFCore.Practices/)

### EFSecondLevelCache.Core

Second Level Caching Library. Second level caching is a query cache. The results of EF commands will be stored in the cache, so that the same EF commands will retrieve their data from the cache rather than executing them against the database again.

[GitHub repository](https://github.com/VahidN/EFSecondLevelCache.Core/)

### Detached.EntityFramework

Loads and saves entire detached entity graphs (the entity with their child entities and lists). Inspired by [GraphDiff](https://github.com/refactorthis/GraphDiff/). The idea is also add some plugins to simplificate some repetitive tasks, like auditing and pagination.

[GitHub repository](https://github.com/leonardoporro/Detached/)

### EntityFrameworkCore.PrimaryKey

Retrieve the primary key (including composite keys) from any entity as a dictionary.

[GitHub repository](https://github.com/NickStrupat/EntityFramework.PrimaryKey/)

### EntityFrameworkCore.Rx

Reactive extension wrappers for hot observables of Entity Framework entities.

[GitHub repository](https://github.com/NickStrupat/EntityFramework.Rx/)

### EntityFrameworkCore.Triggers

Add triggers to your entities with insert, update, and delete events. There are three events for each: before, after, and upon failure.

[GitHub repository](https://github.com/NickStrupat/EntityFramework.Triggers/)

### EntityFrameworkCore.TypedOriginalValues

Get typed access to the OriginalValue of your entity properties. Simple and complex properties are supported, navigation/collections are not.

[GitHub repository](https://github.com/NickStrupat/EntityFramework.TypedOriginalValues/)

### Geco

Geco provides a Reverse Model generator with support for Pluralization/Singularization and editable templates based on C# 6.0 interpolated strings and running on .Net Core. It also provides an Seed script generator with SQL Merge scripts and an script runner.

[Github repository](https://github.com/iQuarc/Geco)

### LinqKit.Microsoft.EntityFrameworkCore

LinqKit.Microsoft.EntityFrameworkCore is a free set of extensions for LINQ to SQL and EntityFrameworkCore power users. With Include(...) and IDbAsync support.

[GitHub repository](https://github.com/scottksmith95/LINQKit/)

### NeinLinq.EntityFrameworkCore

NeinLinq.EntityFrameworkCore provides helpful extensions for using LINQ providers such as Entity Framework that support only a minor subset of .NET functions, reusing functions, rewriting queries, even making them null-safe, and building dynamic queries using translatable predicates and selectors.

[GitHub repository](https://github.com/axelheer/nein-linq/)

### Microsoft.EntityFrameworkCore.UnitOfWork

A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple database with distributed transaction supported.

[GitHub repository](https://github.com/Arch/UnitOfWork/)

### EntityFramework.LazyLoading

Lazy Loading for EF Core 1.1

[GitHub repository](https://github.com/darxis/EntityFramework.LazyLoading)

### EFCore.BulkExtensions

EntityFrameworkCore extensions for Bulk operations (Insert, Update, Delete).

[GitHub repository](https://github.com/borisdj/EFCore.BulkExtensions)

### Bricelam.EntityFrameworkCore.Pluralizer

Adds design-time pluralization to EF Core.

[GitHub repository](https://github.com/bricelam/EFCore.Pluralizer)
