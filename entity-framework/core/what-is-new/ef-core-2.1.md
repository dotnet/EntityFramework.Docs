---
title: What is new in EF Core 2.1 - EF Core
author: divega
ms.author: divega

ms.date: 2/20/2018

ms.assetid: 585F90A3-4D5A-4DD1-92D8-5243B14E0FEC
ms.technology: entity-framework-core

uid: core/what-is-new/ef-core-2.1
---

# New features in EF Core 2.1

## Lazy loading
EF Core now contains the necessary building blocks for anyone to write entity classes that can load their navigation properties on demand. We have also created a new package, Microsoft.EntityFrameworkCore.Proxies, that leverages those building blocks to produce lazy loading proxy classes based on minimally modified entity classes. In order to use these lazy loading proxies, you only need navigation properties in your entities to be virtual.

## Parameters in entity constructors
As one of the required building blocks for lazy loading, we enabled the creation of entities that take parameters in their constructor. You can use parameters to inject property values, lazy loading delegates, and services.

## Value conversions
Until now, EF Core could only map properties of types natively supported by the underlying database provider. Values were copied back and forth between columns and properties without any transformation. Starting with EF Core 2.1, value conversions can be applied to transform the values obtained from columns before they are applied to properties, and vice versa. We have a number of conversions that can be applied by convention as necessary, as well as an explicit configuration API that allows registering delegates for the conversions between columns and properties. Some of the application of this feature are:

- Storing enums as strings
- Mapping unsigned integers with SQL Server
- Automatic encryption and decryption of property values

## LINQ GroupBy translation
Before EF Core 2.1, the GroupBy LINQ operator would always be evaluated in memory. We now support translating it to the SQL GROUP BY clause in most common cases.

## Data Seeding
With the new release it will be possible to provide initial data to populate a database. Unlike in EF6, in EF Core, seeding data is associated to an entity type as part of the model configuration. Then EF Core migrations can automatically compute what insert, update or delete operations need to be applied when upgrading the database to a new version of the model.

## Query types
An EF Core model can now include query types. Unlike entity types, query types do not have keys defined on them and cannot be inserted, deleted or updated (i.e. they are read-only), but they can be returned directly by queries. Some of the usage scenarios for query types are:

- Mapping to views without primary keys
- Mapping to tables without primary keys
- Mapping to queries defined in the model
- Serving as the return type for FromSql() queries

## Include for derived types
It will be now possible to specify navigation properties only defined in derived types when writing expressions for the Include() methods. We support both an explicit cast and the `as` operator:

 ``` csharp
 var queryUsingExplicitCast = context.People.Include(p => ((Student)p).School);
 var queryUsingAsOperator = context.People.Include(p => (p as Student).School);
```

## System.Transactions support
We have added the ability to work with System.Transactions features such as TransactionScope. This will work on both .NET Framework and .NET Core when using database providers that support it.

## Better column ordering in initial migration
Based on customer feedback, we have updated migrations to initially generate columns for tables in the same order as properties are declared in classes. Note that we cannot change order when new members are added after the initial table creation.

## Optimization of correlated subqueries
We have improved our query translation to avoid executing N + 1 SQL queries in many common scenarios in which a root query is joined with a correlated subquery. The optimization requires buffering results and you have to explicitly opt-in is a change to the current behavior to  streaming, so it is not enabled by default. In order to opt-into the optimization


## Other initiatives
Besides the major new features included in 2.1, we have made numerous smaller improvements and we have fixed more than a hundred product bugs. We also made progress on the following areas:

-	__Cosmos DB provider preview:__ We have been developing an EF Core provider for the DocumentDB API in Cosmos DB. This will be the first complete document-oriented database provider we have produced, and the learnings from this exercise are going to inform improvements in the design of the subsequent release after 2.1. The current plan is to publish an early preview of the Cosmos DB provider in the 2.1 timeframe.

-	__Sample Oracle provider for EF Core:__ We have produced a sample EF Core provider for Oracle databases. The purpose of the project is not to produce an EF Core provider owned by Microsoft, but to:

    1. Help us identify gaps in EF Core’s relational and base functionality which we need to address in order to better support Oracle.
    2. Help jumpstart the development of other Oracle providers for EF Core either by Oracle or third parties.    
