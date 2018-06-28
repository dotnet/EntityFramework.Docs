---
title: "Entity Framework IQueryable doesn&#39;t implement IDbAsyncEnumerable (EF6 onwards) - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: bcbe5277-7699-46c2-a780-a9e37ca88690
caps.latest.revision: 3
---
# Entity Framework IQueryable doesn&#39;t implement IDbAsyncEnumerable (EF6 onwards)
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  
  
Entity Framework 6 introduced a set of extension methods that can be used to asynchronously execute a query. Examples of these methods include ToListAsync, FirstAsync, ForEachAsync, etc.  
  
Because Entity Framework queries make use of LINQ, the extension methods are defined on IQueryable and IEnumerable. However, because they are only designed to be used with Entity Framework you may receive the following error if you try to use them on a LINQ query that isn’t an Entity Framework query.  
  
> **Note**: The source IQueryable doesn't implement IDbAsyncEnumerable{0}. Only sources that implement IDbAsyncEnumerable can be used for Entity Framework asynchronous operations. For more details see [http://go.microsoft.com/fwlink/?LinkId=287068](http://go.microsoft.com/fwlink/?LinkId=287068).  
  
## Async Methods when Unit Testing  
  
Whilst the async methods are only supported when running against an EF query, you may want to use them in your unit test when running against an in-memory test double of a DbSet.  
  
For more details on how to achieve this scenario see the 'Testing with async queries' section of [Testing with a Mocking Framework](../ef6/entity-framework-testing-with-a-mocking-framework-ef6-onwards.md) or [Testing with Your Own Test Doubles](../ef6/entity-framework-testing-with-your-own-test-doubles-ef6-onwards.md).  
  