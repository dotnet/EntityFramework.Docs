---
title: Connection resiliency and retry logic - EF6
description: Connection resiliency and retry logic in Entity Framework 6
author: AndriySvyryd
ms.date: 11/20/2019
uid: ef6/fundamentals/connection-resiliency/retry-logic
---
# Connection resiliency and retry logic
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

Applications connecting to a database server have always been vulnerable to connection breaks due to back-end failures and network instability. However, in a LAN based environment working against dedicated database servers these errors are rare enough that extra logic to handle those failures is not often required. With the rise of cloud based database servers such as Windows Azure SQL Database and connections over less reliable networks it is now more common for connection breaks to occur. This could be due to defensive techniques that cloud databases use to ensure fairness of service, such as connection throttling, or to instability in the network causing intermittent timeouts and other transient errors.  

Connection Resiliency refers to the ability for EF to automatically retry any commands that fail due to these connection breaks.  

## Execution Strategies  

Connection retry is taken care of by an implementation of the IDbExecutionStrategy interface. Implementations of the IDbExecutionStrategy will be responsible for accepting an operation and, if an exception occurs, determining if a retry is appropriate and retrying if it is. There are four execution strategies that ship with EF:  

1. **DefaultExecutionStrategy**: this execution strategy does not retry any operations, it is the default for databases other than sql server.  
2. **DefaultSqlExecutionStrategy**: this is an internal execution strategy that is used by default. This strategy does not retry at all, however, it will wrap any exceptions that could be transient to inform users that they might want to enable connection resiliency.  
3. **DbExecutionStrategy**: this class is suitable as a base class for other execution strategies, including your own custom ones. It implements an exponential retry policy, where the initial retry happens with zero delay and the delay increases exponentially until the maximum retry count is hit. This class has an abstract ShouldRetryOn method that can be implemented in derived execution strategies to control which exceptions should be retried.  
4. **SqlAzureExecutionStrategy**: this execution strategy inherits from DbExecutionStrategy and will retry on exceptions that are known to be possibly transient when working with Azure SQL Database.

> [!NOTE]
> Execution strategies 2 and 4 are included in the Sql Server provider that ships with EF, which is in the EntityFramework.SqlServer assembly and are designed to work with SQL Server.  

## Enabling an Execution Strategy  

The easiest way to tell EF to use an execution strategy is with the SetExecutionStrategy method of the [DbConfiguration](xref:ef6/fundamentals/configuring/code-based) class:  

``` csharp
public class MyConfiguration : DbConfiguration
{
    public MyConfiguration()
    {
        SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
    }
}
```  

This code tells EF to use the SqlAzureExecutionStrategy when connecting to SQL Server.  

## Configuring the Execution Strategy  

The constructor of SqlAzureExecutionStrategy can accept two parameters, MaxRetryCount and MaxDelay. MaxRetry count is the maximum number of times that the strategy will retry. The MaxDelay is a TimeSpan representing the maximum delay between retries that the execution strategy will use.  

To set the maximum number of retries to 1 and the maximum delay to 30 seconds you would execute the following:  

``` csharp
public class MyConfiguration : DbConfiguration
{
    public MyConfiguration()
    {
        SetExecutionStrategy(
            "System.Data.SqlClient",
            () => new SqlAzureExecutionStrategy(1, TimeSpan.FromSeconds(30)));
    }
}
```  

The SqlAzureExecutionStrategy will retry instantly the first time a transient failure occurs, but will delay longer between each retry until either the max retry limit is exceeded or the total time hits the max delay.  

The execution strategies will only retry a limited number of exceptions that are usually transient, you will still need to handle other errors as well as catching the RetryLimitExceeded exception for the case where an error is not transient or takes too long to resolve itself.  

There are some known limitations when using a retrying execution strategy:  

## Streaming queries are not supported  

By default, EF6 and later version will buffer query results rather than streaming them. If you want to have results streamed you can use the AsStreaming method to change a LINQ to Entities query to streaming.  

``` csharp
using (var db = new BloggingContext())
{
    var query = (from b in db.Blogs
                orderby b.Url
                select b).AsStreaming();
    }
}
```  

Streaming is not supported when a retrying execution strategy is registered. This limitation exists because the connection could drop part way through the results being returned. When this occurs, EF needs to re-run the entire query but has no reliable way of knowing which results have already been returned (data may have changed since the initial query was sent, results may come back in a different order, results may not have a unique identifier, etc.).  

## User initiated transactions are not supported  

When you have configured an execution strategy that results in retries, there are some limitations around the use of transactions.  

By default, EF will perform any database updates within a transaction. You don’t need to do anything to enable this, EF always does this automatically.  

For example, in the following code SaveChanges is automatically performed within a transaction. If SaveChanges were to fail after inserting one of the new Site’s then the transaction would be rolled back and no changes applied to the database. The context is also left in a state that allows SaveChanges to be called again to retry applying the changes.  

``` csharp
using (var db = new BloggingContext())
{
    db.Blogs.Add(new Site { Url = "http://msdn.com/data/ef" });
    db.Blogs.Add(new Site { Url = "http://blogs.msdn.com/adonet" });
    db.SaveChanges();
}
```  

When not using a retrying execution strategy you can wrap multiple operations in a single transaction. For example, the following code wraps two SaveChanges calls in a single transaction. If any part of either operation fails then none of the changes are applied.  

``` csharp
using (var db = new BloggingContext())
{
    using (var trn = db.Database.BeginTransaction())
    {
        db.Blogs.Add(new Site { Url = "http://msdn.com/data/ef" });
        db.Blogs.Add(new Site { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();

        db.Blogs.Add(new Site { Url = "http://twitter.com/efmagicunicorns" });
        db.SaveChanges();

        trn.Commit();
    }
}
```  

This is not supported when using a retrying execution strategy because EF isn’t aware of any previous operations and how to retry them. For example, if the second SaveChanges failed then EF no longer has the required information to retry the first SaveChanges call.  

### Solution: Manually Call Execution Strategy  

The solution is to manually use the execution strategy and give it the entire set of logic to be run, so that it can retry everything if one of the operations fails. When an execution strategy derived from DbExecutionStrategy is running it will suspend the implicit execution strategy used in SaveChanges.  

Note that any contexts should be constructed within the code block to be retried. This ensures that we are starting with a clean state for each retry.  

``` csharp
var executionStrategy = new SqlAzureExecutionStrategy();

executionStrategy.Execute(
    () =>
    {
        using (var db = new BloggingContext())
        {
            using (var trn = db.Database.BeginTransaction())
            {
                db.Blogs.Add(new Blog { Url = "http://msdn.com/data/ef" });
                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                db.SaveChanges();

                db.Blogs.Add(new Blog { Url = "http://twitter.com/efmagicunicorns" });
                db.SaveChanges();

                trn.Commit();
            }
        }
    });
```  
