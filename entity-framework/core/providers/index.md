---
title: Database Providers - EF Core
author: rowanmiller
ms.author: divega

ms.date: 10/27/2016

ms.assetid: 14fffb6c-a687-4881-a094-af4a1359a296
ms.technology: entity-framework-core

uid: core/providers/index
---
# Database Providers

Entity Framework Core uses a provider model to allow EF to be used to access many different databases. Some concepts are common to most databases, and are included in the primary EF Core components. Such concepts include expressing queries in LINQ, transactions, and tracking changes to objects once they are loaded from the database. Some concepts are specific to a particular provider. For example, the SQL Server provider allows you to configure memory-optimized tables (a feature specific to SQL Server). Other concepts are specific to a class of providers. For example, EF Core providers for relational databases build on the common `Microsoft.EntityFrameworkCore.Relational` library, which provides APIs for configuring table and column mappings, foreign key constraints, etc.

EF Core providers are built by a variety of sources. Not all providers are maintained as part of the Entity Framework Core project. When considering a third party provider, be sure to evaluate quality, licensing, support, etc. to ensure they meet your requirements.
