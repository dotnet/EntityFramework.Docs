---
title: "Entity Framework Handling of Transaction Commit Failures (EF6.1 Onwards) - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 5b1f7a7d-1b24-4645-95ec-5608a31ef577
caps.latest.revision: 3
---
# Entity Framework Handling of Transaction Commit Failures (EF6.1 Onwards)
> **EF6.1 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6.1. If you are using an earlier version, some or all of the information does not apply.  

As part of 6.1 we are introducing a new connection resiliency feature for EF: the ability to detect and recover automatically when transient connection failures affect the acknowledgement of transaction commits. The full details of the scenario are best described in the blog post [SQL Database Connectivity and the Idempotency Issue](http://blogs.msdn.com/b/adonet/archive/2013/03/11/sql-database-connectivity-and-the-idempotency-issue.aspx).  In summary, the scenario is that when an exception is raised during a transaction commit there are two possible causes:  

1. The transaction commit failed on the server
2. The transaction commit succeeded on the server but a connectivity issue prevented the success notification from reaching the client  

When the first situation happens the application or the user can retry the operation, but when the second situation occurs retries should be avoided and the application could recover automatically. The challenge is that without the ability to detect what was the actual reason an exception was reported during commit, the application cannot choose the right course of action. The new feature allows EF to double-check with the database if the transaction succeeded and take the right course of action transparently.  

## Using the feature  

In order to enable the feature you need include a call to [SetTransactionHandler](https://msdn.microsoft.com/library/system.data.entity.dbconfiguration.setdefaulttransactionhandler.aspx) in the constructor of your **DbConfiguration**. If you are unfamiliar with **DbConfiguration**, see [Code Based Configuration](../ef6/entity-framework-code-based-configuration-ef6-onwards.md). This feature can be used in combination with the automatic retries we introduced in EF6, which help in the situation in which the transaction actually failed to commit on the server due to a transient failure:  

```  
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

public class MyConfiguration : DbConfiguration  
{
  public MyConfiguration()  
  {  
    SetTransactionHandler(SqlProviderServices.ProviderInvariantName, () => new CommitFailureHandler());  
    SetExecutionStrategy(SqlProviderServices.ProviderInvariantName, () => new SqlAzureExecutionStrategy());  
  }  
}
```  

## How transactions are tracked  

When the feature is enabled, EF will automatically add a new table to the database called **__Transactions**. A new row is inserted in this table every time a transaction is created by EF and that row is checked for existence if a transaction failure occurs during commit.  

Although EF will do a best effort to prune rows from the table when they aren’t needed anymore, the table can grow if the application exits prematurely and for that reason you may need to purge the table manually in some cases.  
