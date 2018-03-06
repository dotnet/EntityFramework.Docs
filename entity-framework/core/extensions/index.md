---
title: Tools & Extensions - EF Core
author: ErikEJ
ms.author: divega
ms.date: 7/3/2018
ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
ms.technology: entity-framework-core
uid: core/extensions/index
---

# EF Core Tools & Extensions

Tools and extensions provide additional functionality for Entity Framework Core.

> [!IMPORTANT]  
> Extensions are built by a variety of sources. Not all extensions are maintained as part of the Entity Framework Core project. When considering a third party extension, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements.

## Tools

[LLBLGen Pro website](https://www.llblgen.com/)

LLBLGen Pro is an entity modeling solution with support for Entity Framework and Entity Framework Core. It lets you easily define your entity model and map it to your database, using database first or model first, so you can get started writing queries right away.

[Devart Entity Developer website](https://www.devart.com/entitydeveloper/)

Entity Developer is a powerful ORM designer for ADO.NET Entity Framework, NHibernate, LinqConnect, Telerik Data Access, and LINQ to SQL. You can use  Model-First and Database-First approaches to design your ORM model and generate C# or Visual Basic .NET code for it. It introduces new approaches for designing ORM models, boosts productivity, and facilitates the development of database applications.

[EF Core Power Tools GitHub wiki](https://github.com/ErikEJ/SqlCeToolbox/wiki/EF-Core-Power-Tools)

Visual Studio 2017+ extension. You can reverse engineer of DbContext and POCO classes from an existing database og SQL Server Databas project, and visualize and inspect your DbContext in various ways.

## Extensions

[Microsoft.EntityFrameworkCore.AutoHistory GitHub repository](https://github.com/Arch/AutoHistory/)

A plugin for Microsoft.EntityFrameworkCore to support automatically recording data changes history.

[Microsoft.EntityFrameworkCore.DynamicLinq GitHub repository](https://github.com/StefH/System.Linq.Dynamic.Core/)

Dynamic Linq extensions for Microsoft.EntityFrameworkCore which adds Async support

[EFCore.Practices GitHub repository](https://github.com/riezebosch/efcore-practices/tree/master/src/EFCore.Practices/)

Attempt to capture some good or best practices in an API that supports testing – including a small framework to scan for N+1 queries.

[EFSecondLevelCache.Core GitHub repository](https://github.com/VahidN/EFSecondLevelCache.Core/)

Second Level Caching Library. Second level caching is a query cache. The results of EF commands will be stored in the cache, so that the same EF commands will retrieve their data from the cache rather than executing them against the database again.

[Detached.EntityFramework GitHub repository](https://github.com/leonardoporro/Detached/)

Loads and saves entire detached entity graphs (the entity with their child entities and lists). Inspired by [GraphDiff](https://github.com/refactorthis/GraphDiff/). The idea is also add some plugins to simplificate some repetitive tasks, like auditing and pagination.

[EntityFrameworkCore.PrimaryKey GitHub repository](https://github.com/NickStrupat/EntityFramework.PrimaryKey/)

Retrieve the primary key (including composite keys) from any entity as a dictionary.

[EntityFrameworkCore.Rx GitHub repository](https://github.com/NickStrupat/EntityFramework.Rx/)

Reactive extension wrappers for hot observables of Entity Framework entities.

[EntityFrameworkCore.Triggers GitHub repository](https://github.com/NickStrupat/EntityFramework.Triggers/)

Add triggers to your entities with insert, update, and delete events. There are three events for each: before, after, and upon failure.

[EntityFrameworkCore.TypedOriginalValues GitHub repository](https://github.com/NickStrupat/EntityFramework.TypedOriginalValues/)

Get typed access to the OriginalValue of your entity properties. Simple and complex properties are supported, navigation/collections are not.

[Geco Github repository](https://github.com/iQuarc/Geco)

Geco provides a Reverse Model generator with support for Pluralization/Singularization and editable templates based on C# 6.0 interpolated strings and running on .Net Core. It also provides an Seed script generator with SQL Merge scripts and an script runner.

[LinqKit.Microsoft.EntityFrameworkCore GitHub repository](https://github.com/scottksmith95/LINQKit/)

LinqKit.Microsoft.EntityFrameworkCore is a free set of extensions for LINQ to SQL and EntityFrameworkCore power users. With Include(...) and IDbAsync support.

[NeinLinq.EntityFrameworkCore GitHub repository](https://github.com/axelheer/nein-linq/)

NeinLinq.EntityFrameworkCore provides helpful extensions for using LINQ providers such as Entity Framework that support only a minor subset of .NET functions, reusing functions, rewriting queries, even making them null-safe, and building dynamic queries using translatable predicates and selectors.

[Microsoft.EntityFrameworkCore.UnitOfWork GitHub repository](https://github.com/Arch/UnitOfWork/)

A plugin for Microsoft.EntityFrameworkCore to support repository, unit of work patterns, and multiple database with distributed transaction supported.

[EntityFramework.LazyLoading GitHub repository](https://github.com/darxis/EntityFramework.LazyLoading)

Lazy Loading for EF Core 1.1

[EFCore.BulkExtensions GitHub repository](https://github.com/borisdj/EFCore.BulkExtensions)

EntityFrameworkCore extensions for Bulk operations (Insert, Update, Delete).
