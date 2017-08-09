---
title: "Entity Framework Windows SQL Azure | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: ce0bc4e4-f036-452e-80b3-c483f742631e
caps.latest.revision: 4
---
# Entity Framework Windows SQL Azure
This topic provides guidance for deploying to Windows Azure and accessing a Windows Azure SQL Database using Entity Framework. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.


## Deploying to Windows Azure

### Spatial Data Types

In order to use spatial data types on Windows Azure you will need to deploy the Microsoft.SqlServer.Types assembly with your application. The easiest way to do this is to install the [Microsoft.SqlServer.Types NuGet package](http://www.nuget.org/packages/Microsoft.SqlServer.Types/) in your application.

```
Install-Package Microsoft.SqlServer.Types
```

 

## Connecting to SQL Azure

Entity Framework can connect to SQL Azure without any additional configuration simply by treating it as any other SQL Server database. This is true for models created using Code First and the EF Designer - whether you are targeting an existing SQL Azure database or having EF create a new database for you.

However, there are a number of characteristics of a SQL Azure database that need to be taken into account when using EF, these are discussed below.

### Connection Fault Handling

Experiencing connectivity drops, classified as general network issues, is common when accessing SQL Azure and must be taken into account.

#### Connection Resiliency (EF6 onwards)

Starting with EF6, Entity Framework has built-in retry logic. For more information, see [Connection Resiliency (Retry Logic)](../ef6/entity-framework-connection-resiliency-and-retry-logic-ef6-onwards.md).

#### Handling of Transaction Commit Failures (EF6.1 onwards)

Starting with EF6.1, Entity Framework has built-in logic to help recover from failures that occur while committing a transaction. For more information, see [Handling of Transaction Commit Failures (EF6.1 Onwards)](../ef6/entity-framework-handling-of-transaction-commit-failures-ef6-1-onwards.md).

#### Prior to EF6

Prior to EF6, Entity Framework doesn't have native support for retrying an operation when the connection drops. The [SQL Azure and Entity Framework Connection Fault Handling](http://blogs.msdn.com/b/appfabriccat/archive/2010/12/11/sql-azure-and-entity-framework-connection-fault-handling.aspx) post provides guidance on handling connection faults with EF5 and earlier.

### SQL Azure Federations

The current release of Entity Framework can be used to work with SQL Azure Federations, however a federated database cannot be created by the Entity Framework. Our Customer Advisory Team has started a series of blog posts with the goal of providing guidance around common scenarios and issues that arise when using the Entity Framework with SQL Azure Federations.

The first blog post in this series, SQL Azure Federations with Entity Framework Code-First, is a great getting started guide. It explains the correct procedure to submit the USE FEDERATION statement before sending queries to the database via the Entity Framework (query execution or update operations).

The next post, Understanding SQL Azure Federations No-MARS Support and Entity Framework, explains the impact of the lack of support for MARS on Entity Framework applications.

Here are some general guidelines/considerations for using SQL Azure Federations with Entity Framework:

-   The Entity Framework based application needs to be aware and manage the access to the different federation members. What this means is that the application would have to explicitly open the store connection with which the context is associated and issue the “USE FEDERATION” statement to connect to the correct federation member before interacting with the database via the Entity Framework.
-   Any needed database transaction would have to be started after the “USE FEDERATION” statement is issued. This is because federated databases do not support the “USE FEDERATION” statement in a transaction.
-   Any connection retries would also need to be handled by the application.
-   Instances of the context class should not span across federation members. In general, this also means that all changes managed by the context should be associated with a single federation member. This is because at the time SaveChanges is invoked, it would issue the corresponding database data modification operations only to the federation member to which the associated store connection currently points.
