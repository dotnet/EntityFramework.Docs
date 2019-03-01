---
title: New features in EF Core 3.0 - EF Core
author: divega
ms.date: 02/19/2019
ms.assetid: 2EBE2CCC-E52D-483F-834C-8877F5EB0C0C
uid: core/what-is-new/ef-core-3.0/features
---

# New features included in EF Core 3.0 (currently in preview)

The following is a list of the main improvements planned for EF Core 3.0. 

> [!IMPORTANT]
> Many of these features are not included in the current preview, but will become available as we make progress towards RTM.
Please note that the feature sets and schedules of future releases are always subject to change, and although we will try to keep this page up to date, it may not reflect our latest plans at all times.

## LINQ improvements 
Tracking issue [#12795](https://github.com/aspnet/EntityFrameworkCore/issues/12795)

LINQ enables you to write database queries without leaving your language of choice, taking advantage of rich type information to get IntelliSense and compile-time type checking.
But LINQ also enables you to write an unlimited number of complicated queries, and that has always been a huge challenge for LINQ providers.
In the first few versions of EF Core, we solved that in part by figuring out what portions of a query could be translated to SQL, and then by allowing the rest of the query to execute in memory on the client.
This client-side execution can be desirable in some situations, but in many other cases it can result in inefficient queries that may not be identified until an application is deployed to production.
In EF Core 3.0, we are planning to make profound changes to how our LINQ implementation works, and how we test it.
The goals are to make it more robust (for example, to avoid breaking queries in patch releases), to be able to translate more expressions correctly into SQL, to generate efficient queries in more cases, and to prevent inefficient queries from going undetected.

## Cosmos DB support 
Tracking issue [#8443](https://github.com/aspnet/EntityFrameworkCore/issues/8443)

We're working on a Cosmos DB provider for EF Core, to enable developers familiar with the EF programing model to easily target Azure Cosmos DB as an application database.
The goal is to make some of the advantages of Cosmos DB, like global distribution, “always on” availability, elastic scalability, and low latency, even more accessible to .NET developers.
The provider will enable most EF Core features, like automatic change tracking, LINQ, and value conversions, against the SQL API in Cosmos DB. We started this effort before EF Core 2.2, and [we have made some preview versions of the provider available](https://blogs.msdn.microsoft.com/dotnet/2018/10/17/announcing-entity-framework-core-2-2-preview-3/).
The new plan is to continue developing the provider alongside EF Core 3.0.   

## C# 8.0 support 
Tracking issues [#12047](https://github.com/aspnet/EntityFrameworkCore/issues/12047) and [#10347] (https://github.com/aspnet/EntityFrameworkCore/issues/10347)

We want our customers to take advantage of some of the [new features coming in C# 8.0](https://blogs.msdn.microsoft.com/dotnet/2018/11/12/building-c-8-0/) like async streams (including await for each) and nullable reference types while using EF Core.

## Reverse engineering database views into query types 
Tracking issue [#1679](https://github.com/aspnet/EntityFrameworkCore/issues/1679)

In EF Core 2.1, we added support for query types, which can represent data that can be read from the database, but cannot be updated.
Query types are a great fit for mapping database views, so in EF Core 3.0, we would like to automate the creation of query types for database views.

## Property bag entities 
Tracking issue [#13610](https://github.com/aspnet/EntityFrameworkCore/issues/13610) and [#9914](https://github.com/aspnet/EntityFrameworkCore/issues/9914)

This feature is about enabling entities that store data in indexed properties instead of regular properties, and also about being able to use instances of the same .NET class (potentially something as simple as a `Dictionary<string, object>`) to represent different entity types in the same EF Core model.
This feature is a stepping stone to support many-to-many relationships without a join entity, which is one of the most requested improvements for EF Core.

## EF 6.3 on .NET Core 
Tracking issue [EF6#271](https://github.com/aspnet/EntityFramework6/issues/271)

We understand that many existing applications use previous versions of EF, and that porting them to EF Core only to take advantage of .NET Core can sometimes require a significant effort.
For that reason, we will be adapting the next version of EF 6 to run on .NET Core 3.0.
We are doing this to facilitate porting existing applications with minimal changes.
There are going to be some limitations (for example, it will require new providers, spatial support with SQL Server won't be enabled), and there are no new features planned for EF 6.
