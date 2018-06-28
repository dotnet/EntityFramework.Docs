---
title: "Entity Framework Connection Failures During Transaction Commit - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 3df4f2f1-b2df-4543-98e1-bfa938315b05
caps.latest.revision: 3
---
# Entity Framework Connection Failures During Transaction Commit
In general when there is a connection failure the current transaction is rolled back. However if the connection is dropped while the transaction is being committed the resulting state of the transaction is unknown. See [this blog post](http://blogs.msdn.com/b/adonet/archive/2013/03/11/sql-database-connectivity-and-the-idempotency-issue.aspx) for more details.  
  
Currently EF doesn’t provide any special tools to handle this scenario. The [SqlAzureExecutionStrategy](../ef6/entity-framework-connection-resiliency-and-retry-logic-ef6-onwards.md) will not retry an operation if it failed in this way.  
  
There are several ways to dealing with this:  
  
## Option 1 - Do nothing  
  
The likelihood of a connection failure during transaction commit is low so it may be acceptable for your application to just fail if this condition actually occurs.  
  
## Option 2 - Use the database to reset state  
  
1. Discard the current DbContext  
2. Create a new DbContext and restore the state of your application from the database.  
3. Inform the user that the last operation might not have been completed succesfully  
  
## Option 3 - Manually track the transaction  
  
1. Add a non-tracked table to the database used to track the status of the transactions.  
2. Insert a row into the table at the beginning of each transaction.  
3. If the connection fails during the commit, check for the presence of the corresponding row in the database.  
    - If the row is present, continue normally, as the transaction was committed successfully  
    - If the row is absent, use an execution strategy to retry the current operation.  
4. If the commit is successful, delete the corresponding row to avoid the growth of the table.  
  
[This blog post](http://blogs.msdn.com/b/adonet/archive/2013/03/11/sql-database-connectivity-and-the-idempotency-issue.aspx) contains sample code for accomplishing this on SQL Azure.  
  