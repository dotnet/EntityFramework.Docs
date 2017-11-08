---
title: "Entity Framework Connection Resiliency and Retry Logic - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 47d68ac1-927e-4842-ab8c-ed8c8698dff2
caps.latest.revision: 3
---
# Entity Framework Connection Resiliency and Retry Logic (EF6 onwards)
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

Applications connecting to a database server have always been vulnerable to connection breaks due to back-end failures and network instability. However, in a LAN based environment working against dedicated database servers these errors are rare enough that extra logic to handle those failures is not often required. With the rise of cloud based database servers such as Windows Azure SQL Database and connections over less reliable networks it is now more common for connection breaks to occur. This could be due to defensive techniques that cloud databases use to ensure fairness of service, such as connection throttling, or to instability in the network causing intermittent timeouts and other transient errors.  

Connection Resiliency refers to the ability for EF to automatically retry any commands that fail due to these connection breaks.  

## Execution Strategies  

Connection retry is taken care of by an implementation of the IDbExecutionStrategy interface. Implementations of the IDbExecutionStrategy will be responsible for accepting an operation and, if an exception occurs, determining if a retry is appropriate and retrying if it is. There are four execution strategies that ship with EF:  

1. **DefaultExecutionStrategy**: this execution strategy does not retry any operations, it is the default for databases other than sql server.  
2. **DefaultSqlExecutionStrategy**: this is an internal execution strategy that is used by default. This strategy does not retry at all, however, it will wrap any exceptions that could be transient to inform users that they might want to enable connection resiliency.  
3. **DbExecutionStrategy**: this class is suitable as a base class for other execution strategies, including your own custom ones. It implements an exponential retry policy, where the initial retry happens with zero delay and the delay increases exponentially until the maximum retry count is hit. This class has an abstract ShouldRetryOn method that can be implemented in derived execution strategies to control which exceptions should be retried.  
4. **SqlAzureExecutionStrategy**: this execution strategy inherits from DbExecutionStrategy and will retry on exceptions that are known to be possibly transient when working with SqlAzure.  

> **Note**: Execution strategies 2 and 4 are included in the Sql Server provider that ships with EF, which is in the EntityFramework.SqlServer assembly and are designed to work with SQL Server.  

## Enabling an Execution Strategy  

The easiest way to tell EF to use an execution strategy is with the SetExecutionStrategy method of the [DbConfiguration](../ef6/entity-framework-code-based-configuration-ef6-onwards.md) class:  

```  
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

```  
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

## Limitations  

There are some known limitations when using ExecutionStrategies. If you want to use user initiated transactions or streaming instead of buffered queries then you should read more [here](../ef6/entity-framework-limitations-with-retrying-execution-strategies-ef6-onwards.md).  
