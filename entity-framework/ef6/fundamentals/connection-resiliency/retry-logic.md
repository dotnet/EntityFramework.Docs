---
title: "Connection resiliency and retry logic - EF6"
author: divega
ms.date: "10/23/2016"
ms.assetid: 47d68ac1-927e-4842-ab8c-ed8c8698dff2
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

The easiest way to tell EF to use an execution strategy is with the SetExecutionStrategy method of the [DbConfiguration](~/ef6/fundamentals/configuring/code-based.md) class:  

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

To set the maximum number of retries to 1 and the maximum delay to 30 seconds you would execue the following:  

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

The execution strategies will only retry a limited number of exceptions that are usually tansient, you will still need to handle other errors as well as catching the RetryLimitExceeded exception for the case where an error is not transient or takes too long to resolve itself.  

There are some known of limitations when using a retrying execution strategy:  

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

### Workaround: Suspend Execution Strategy  

One possible workaround is to suspend the retrying execution strategy for the piece of code that needs to use a user initiated transaction. The easiest way to do this is to add a SuspendExecutionStrategy flag to your code based configuration class and change the execution strategy lambda to return the default (non-retying) execution strategy when the flag is set.  

``` csharp
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;

namespace Demo
{
    public class MyConfiguration : DbConfiguration
    {
        public MyConfiguration()
        {
            this.SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy
              ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
              : new SqlAzureExecutionStrategy());
        }

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }
    }
}
```  

Note that we are using CallContext to store the flag value. This provides similar functionality to thread local storage but is safe to use with asynchronous code - including async query and save with Entity Framework.  

We can now suspend the execution strategy for the section of code that uses a user initiated transaction.  

``` csharp
using (var db = new BloggingContext())
{
    MyConfiguration.SuspendExecutionStrategy = true;

    using (var trn = db.Database.BeginTransaction())
    {
        db.Blogs.Add(new Blog { Url = "http://msdn.com/data/ef" });
        db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();

        db.Blogs.Add(new Blog { Url = "http://twitter.com/efmagicunicorns" });
        db.SaveChanges();

        trn.Commit();
    }

    MyConfiguration.SuspendExecutionStrategy = false;
}
```  

### Workaround: Manually Call Execution Strategy  

Another option is to manually use the execution strategy and give it the entire set of logic to be run, so that it can retry everything if one of the operations fails. We still need to suspend the execution strategy - using the technique shown above - so that any contexts used inside the retryable code block do not attempt to retry.  

Note that any contexts should be constructed within the code block to be retried. This ensures that we are starting with a clean state for each retry.  

``` csharp
var executionStrategy = new SqlAzureExecutionStrategy();

MyConfiguration.SuspendExecutionStrategy = true;

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

MyConfiguration.SuspendExecutionStrategy = false;
```  
