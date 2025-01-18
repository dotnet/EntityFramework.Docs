---
title: Handling transaction commit failures - EF6
description: Handling transaction commit failures in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/connection-resiliency/commit-failures
---
# Handling transaction commit failures

> [!NOTE]
> **EF6.1 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6.1. If you are using an earlier version, some or all of the information does not apply.  

As part of 6.1 we are introducing a new connection resiliency feature for EF: the ability to detect and recover automatically when transient connection failures affect the acknowledgment of transaction commits. The full details of the scenario are best described in the blog post [SQL Database Connectivity and the Idempotency Issue](/archive/blogs/adonet/sql-database-connectivity-and-the-idempotency-issue).  In summary, the scenario is that when an exception is raised during a transaction commit there are two possible causes:  

1. The transaction commit failed on the server
2. The transaction commit succeeded on the server but a connectivity issue prevented the success notification from reaching the client  

When the first situation happens the application or the user can retry the operation, but when the second situation occurs retries should be avoided and the application could recover automatically. The challenge is that without the ability to detect what was the actual reason an exception was reported during commit, the application cannot choose the right course of action. The new feature in EF 6.1 allows EF to double-check with the database if the transaction succeeded and take the right course of action transparently.  

## Using the feature  

In order to enable the feature you need include a call to [SetTransactionHandler](https://msdn.microsoft.com/library/system.data.entity.dbconfiguration.setdefaulttransactionhandler.aspx) in the constructor of your **DbConfiguration**. If you are unfamiliar with **DbConfiguration**, see [Code Based Configuration](xref:ef6/fundamentals/configuring/code-based). This feature can be used in combination with the automatic retries we introduced in EF6, which help in the situation in which the transaction actually failed to commit on the server due to a transient failure:  

``` csharp
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

## How to handle commit failures with previous versions

Before EF 6.1 there was no mechanism to handle commit failures in the EF product. There are several ways to dealing with this situation that can be applied to previous versions of EF6:  

* Option 1 - Do nothing  

  The likelihood of a connection failure during transaction commit is low so it may be acceptable for your application to just fail if this condition actually occurs.  

* Option 2 - Use the database to reset state  

  1. Discard the current DbContext  
  2. Create a new DbContext and restore the state of your application from the database  
  3. Inform the user that the last operation might not have been completed successfully  

* Option 3 - Manually track the transaction  

  1. Add a non-tracked table to the database used to track the status of the transactions.  
  2. Insert a row into the table at the beginning of each transaction.  
  3. If the connection fails during the commit, check for the presence of the corresponding row in the database.  
     * If the row is present, continue normally, as the transaction was committed successfully  
     * If the row is absent, use an execution strategy to retry the current operation.  
  4. If the commit is successful, delete the corresponding row to avoid the growth of the table.  

[This blog post](/archive/blogs/adonet/sql-database-connectivity-and-the-idempotency-issue) contains sample code for accomplishing this on SQL Azure.  
