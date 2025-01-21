---
title: Working with Transactions - EF6
description: Working with Transactions in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/saving/transactions
---
# Working with Transactions
> [!NOTE]
> **EF6 Onwards Only** - The features, APIs, etc. discussed in this page were introduced in Entity Framework 6. If you are using an earlier version, some or all of the information does not apply.  

This document will describe using transactions in EF6 including the enhancements we have added since EF5 to make working with transactions easy.  

## What EF does by default  

In all versions of Entity Framework, whenever you execute **SaveChanges()** to insert, update or delete on the database the framework will wrap that operation in a transaction. This transaction lasts only long enough to execute the operation and then completes. When you execute another such operation a new transaction is started.  

Starting with EF6 **Database.ExecuteSqlCommand()** by default will wrap the command in a transaction if one was not already present. There are overloads of this method that allow you to override this behavior if you wish. Also in EF6 execution of stored procedures included in the model through APIs such as **ObjectContext.ExecuteFunction()** does the same (except that the default behavior cannot at the moment be overridden).  

In either case, the isolation level of the transaction is whatever isolation level the database provider considers its default setting. By default, for instance, on SQL Server this is READ COMMITTED.  

Entity Framework does not wrap queries in a transaction.  

This default functionality is suitable for a lot of users and if so there is no need to do anything different in EF6; just write the code as you always did.  

However some users require greater control over their transactions – this is covered in the following sections.  

## How the APIs work  

Prior to EF6 Entity Framework insisted on opening the database connection itself (it threw an exception if it was passed a connection that was already open). Since a transaction can only be started on an open connection, this meant that the only way a user could wrap several operations into one transaction was either to use a [TransactionScope](https://msdn.microsoft.com/library/system.transactions.transactionscope.aspx) or use the **ObjectContext.Connection** property and start calling **Open()** and **BeginTransaction()** directly on the returned **EntityConnection** object. In addition, API calls which contacted the database would fail if you had started a transaction on the underlying database connection on your own.  

> [!NOTE]
> The limitation of only accepting closed connections was removed in Entity Framework 6. For details, see [Connection Management](xref:ef6/fundamentals/connection-management).  

Starting with EF6 the framework now provides:  

1. **Database.BeginTransaction()** : An easier method for a user to start and complete transactions themselves within an existing DbContext – allowing several operations to be combined within the same transaction and hence either all committed or all rolled back as one. It also allows the user to more easily specify the isolation level for the transaction.  
2. **Database.UseTransaction()** : which allows the DbContext to use a transaction which was started outside of the Entity Framework.  

### Combining several operations into one transaction within the same context  

**Database.BeginTransaction()** has two overrides – one which takes an explicit [IsolationLevel](https://msdn.microsoft.com/library/system.data.isolationlevel.aspx) and one which takes no arguments and uses the default IsolationLevel from the underlying database provider. Both overrides return a **DbContextTransaction** object which provides **Commit()** and **Rollback()** methods which perform commit and rollback on the underlying store transaction.  

The **DbContextTransaction** is meant to be disposed once it has been committed or rolled back. One easy way to accomplish this is the **using(…) {…}** syntax which will automatically call **Dispose()** when the using block completes:  

``` csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace TransactionsExamples
{
    class TransactionsExample
    {
        static void StartOwnTransactionWithinContext()
        {
            using (var context = new BloggingContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlCommand(
                        @"UPDATE Blogs SET Rating = 5" +
                            " WHERE Name LIKE '%Entity Framework%'"
                        );

                    var query = context.Posts.Where(p => p.Blog.Rating >= 5);
                    foreach (var post in query)
                    {
                        post.Title += "[Cool Blog]";
                    }

                    context.SaveChanges();

                    dbContextTransaction.Commit();
                }
            }
        }
    }
}
```  

> [!NOTE]
> Beginning a transaction requires that the underlying store connection is open. So calling Database.BeginTransaction() will open the connection  if it is not already opened. If DbContextTransaction opened the connection then it will close it when Dispose() is called.  

### Passing an existing transaction to the context  

Sometimes you would like a transaction which is even broader in scope and which includes operations on the same database but outside of EF completely. To accomplish this you must open the connection and start the transaction yourself and then tell EF a) to use the already-opened database connection, and b) to use the existing transaction on that connection.  

To do this you must define and use a constructor on your context class which inherits from one of the DbContext constructors which take i) an existing connection parameter and ii) the contextOwnsConnection boolean.  

> [!NOTE]
> The contextOwnsConnection flag must be set to false when called in this scenario. This is important as it informs Entity Framework that it should not close the connection when it is done with it (for example, see line 4 below):  

``` csharp
using (var conn = new SqlConnection("..."))
{
    conn.Open();
    using (var context = new BloggingContext(conn, contextOwnsConnection: false))
    {
    }
}
```  

Furthermore, you must start the transaction yourself (including the IsolationLevel if you want to avoid the default setting) and let Entity Framework know that there is an existing transaction already started on the connection (see line 33 below).  

Then you are free to execute database operations either directly on the SqlConnection itself, or on the DbContext. All such operations are executed within one transaction. You take responsibility for committing or rolling back the transaction and for calling Dispose() on it, as well as for closing and disposing the database connection. For example:  

``` csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace TransactionsExamples
{
     class TransactionsExample
     {
        static void UsingExternalTransaction()
        {
            using (var conn = new SqlConnection("..."))
            {
               conn.Open();

               using (var sqlTxn = conn.BeginTransaction(System.Data.IsolationLevel.Snapshot))
               {
                   var sqlCommand = new SqlCommand();
                   sqlCommand.Connection = conn;
                   sqlCommand.Transaction = sqlTxn;
                   sqlCommand.CommandText =
                       @"UPDATE Blogs SET Rating = 5" +
                        " WHERE Name LIKE '%Entity Framework%'";
                   sqlCommand.ExecuteNonQuery();

                   using (var context =  
                     new BloggingContext(conn, contextOwnsConnection: false))
                    {
                        context.Database.UseTransaction(sqlTxn);

                        var query =  context.Posts.Where(p => p.Blog.Rating >= 5);
                        foreach (var post in query)
                        {
                            post.Title += "[Cool Blog]";
                        }
                       context.SaveChanges();
                    }

                    sqlTxn.Commit();
                }
            }
        }
    }
}
```  

### Clearing up the transaction

You can pass null to Database.UseTransaction() to clear Entity Framework’s knowledge of the current transaction. Entity Framework will neither commit nor rollback the existing transaction when you do this, so use with care and only if you’re sure this is what you want to do.  

### Errors in UseTransaction

You will see an exception from Database.UseTransaction() if you pass a transaction when:  
- Entity Framework already has an existing transaction  
- Entity Framework is already operating within a TransactionScope  
- The connection object in the transaction passed is null. That is, the transaction is not associated with a connection – usually this is a sign that that transaction has already completed  
- The connection object in the transaction passed does not match the Entity Framework’s connection.  

## Using transactions with other features  

This section details how the above transactions interact with:  

- Connection resiliency  
- Asynchronous methods  
- TransactionScope transactions  

### Connection Resiliency  

The new Connection Resiliency feature does not work with user-initiated transactions. For details, see [Retrying Execution Strategies](xref:ef6/fundamentals/connection-resiliency/retry-logic#user-initiated-transactions-are-not-supported).  

### Asynchronous Programming  

The approach outlined in the previous sections needs no further options or settings to work with the [asynchronous query and save methods](xref:ef6/fundamentals/async
). But be aware that, depending on what you do within the asynchronous methods, this may result in long-running transactions – which can in turn cause deadlocks or blocking which is bad for the performance of the overall application.  

### TransactionScope Transactions  

Prior to EF6 the recommended way of providing larger scope transactions was to use a TransactionScope object:  

``` csharp
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace TransactionsExamples
{
    class TransactionsExample
    {
        static void UsingTransactionScope()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (var conn = new SqlConnection("..."))
                {
                    conn.Open();

                    var sqlCommand = new SqlCommand();
                    sqlCommand.Connection = conn;
                    sqlCommand.CommandText =
                        @"UPDATE Blogs SET Rating = 5" +
                            " WHERE Name LIKE '%Entity Framework%'";
                    sqlCommand.ExecuteNonQuery();

                    using (var context =
                        new BloggingContext(conn, contextOwnsConnection: false))
                    {
                        var query = context.Posts.Where(p => p.Blog.Rating > 5);
                        foreach (var post in query)
                        {
                            post.Title += "[Cool Blog]";
                        }
                        context.SaveChanges();
                    }
                }

                scope.Complete();
            }
        }
    }
}
```  

The SqlConnection and Entity Framework would both use the ambient TransactionScope transaction and hence be committed together.  

Starting with .NET 4.5.1 TransactionScope has been updated to also work with asynchronous methods via the use of the [TransactionScopeAsyncFlowOption](https://msdn.microsoft.com/library/system.transactions.transactionscopeasyncflowoption.aspx) enumeration:  

``` csharp
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace TransactionsExamples
{
    class TransactionsExample
    {
        public static void AsyncTransactionScope()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var conn = new SqlConnection("..."))
                {
                    await conn.OpenAsync();

                    var sqlCommand = new SqlCommand();
                    sqlCommand.Connection = conn;
                    sqlCommand.CommandText =
                        @"UPDATE Blogs SET Rating = 5" +
                            " WHERE Name LIKE '%Entity Framework%'";
                    await sqlCommand.ExecuteNonQueryAsync();

                    using (var context = new BloggingContext(conn, contextOwnsConnection: false))
                    {
                        var query = context.Posts.Where(p => p.Blog.Rating > 5);
                        foreach (var post in query)
                        {
                            post.Title += "[Cool Blog]";
                        }

                        await context.SaveChangesAsync();
                    }
                }
                
                scope.Complete();
            }
        }
    }
}
```  

There are still some limitations to the TransactionScope approach:  

- Requires .NET 4.5.1 or greater to work with asynchronous methods.  
- It cannot be used in cloud scenarios unless you are sure you have one and only one connection (cloud scenarios do not support distributed transactions).  
- It cannot be combined with the Database.UseTransaction() approach of the previous sections.  
- It will throw exceptions if you issue any DDL and have not enabled distributed transactions through the MSDTC Service.  

Advantages of the TransactionScope approach:  

- It will automatically upgrade a local transaction to a distributed transaction if you make more than one connection to a given database or combine a connection to one database with a connection to a different database within the same transaction (note: you must have the MSDTC service configured to allow distributed transactions for this to work).  
- Ease of coding. If you prefer the transaction to be ambient and dealt with implicitly in the background rather than explicitly under you control then the TransactionScope approach may suit you better.  

In summary, with the new Database.BeginTransaction() and Database.UseTransaction() APIs above, the TransactionScope approach is no longer necessary for most users. If you do continue to use TransactionScope then be aware of the above limitations. We recommend using the approach outlined in the previous sections instead where possible.  
