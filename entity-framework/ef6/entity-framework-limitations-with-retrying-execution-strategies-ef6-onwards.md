---
title: "Entity Framework Limitations with Retrying Execution Strategies (EF6 onwards) - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 2751cc3a-b099-4016-9faf-453dbf4af617
caps.latest.revision: 3
---
# Entity Framework Limitations with Retrying Execution Strategies (EF6 onwards)
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

There are a couple of limitations when using a retrying execution strategy:  

## Streaming Queries Not Supported  

By default, EF6 and later version will buffer query results rather than streaming them. If you want to have results streamed you can use the AsStreaming method to change a LINQ to Entities query to streaming.  

```  
using (var db = new BloggingContext())
{
    var query = (from b in db.Blogs
                orderby b.Url
                select b).AsStreaming();
    }
}
```  

Streaming is not supported when a retrying execution strategy is registered. This limitation exists because the connection could drop part way through the results being returned. When this occurs, EF needs to re-run the entire query but has no reliable way of knowing which results have already been returned (data may have changed since the initial query was sent, results may come back in a different order, results may not have a unique identifier, etc.).  

## User initiated transactions not supported  

When you have configured an execution strategy that results in retries, there are some limitations around the use of transactions.  

### What’s Supported: EF’s Default Transaction Behavior  

By default, EF will perform any database updates within a transaction. You don’t need to do anything to enable this, EF always does this automatically.  

For example, in the following code SaveChanges is automatically performed within a transaction. If SaveChanges were to fail after inserting one of the new Site’s then the transaction would be rolled back and no changes applied to the database. The context is also left in a state that allows SaveChanges to be called again to retry applying the changes.  

```  
using (var db = new BloggingContext())
{
    db.Blogs.Add(new Site { Url = "http://msdn.com/data/ef" });
    db.Blogs.Add(new Site { Url = "http://blogs.msdn.com/adonet" });
    db.SaveChanges();
}
```  

### What’s Not Supported: User Transactions  

When not using a retrying execution strategy you can wrap multiple operations in a single transaction. For example, the following code wraps two SaveChanges calls in a single transaction. If any part of either operation fails then none of the changes are applied.  

```  
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

### Possbile workarounds  

#### Suspend Execution Strategy  

One possible workaround is to suspend the retrying execution strategy for the piece of code that needs to use a user initiated transaction. The easiest way to do this is to add a SuspendExecutionStrategy flag to your code based configuration class and change the execution strategy lambda to return the default (non-retying) execution strategy when the flag is set.  

```  
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
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy")  false;
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

```  
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

#### Manually Call Execution Strategy  

Another option is to manually use the execution strategy and give it the entire set of logic to be run, so that it can retry everything if one of the operations fails. We still need to suspend the execution strategy - using the technique shown above - so that any contexts used inside the retryable code block do not attempt to retry.  

Note that any contexts should be constructed within the code block to be retried. This ensures that we are starting with a clean state for each retry.  

```  
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
