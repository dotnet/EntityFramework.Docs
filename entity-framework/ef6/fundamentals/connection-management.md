---
title: "Connection management - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: ecaa5a27-b19e-4bf9-8142-a3fb00642270
caps.latest.revision: 3
---
# Connection management
This page describes the behavior of Entity Framework with regard to passing connections to the context and the functionality of the **Database.Connection.Open()** API.  

## Passing Connections to the Context  

### Behavior for EF5 and earlier versions  

There are two constructors which accept connections:  

``` csharp
public DbContext(DbConnection existingConnection, bool contextOwnsConnection)
public DbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
```  

It is possible to use these but you have to work around a couple of limitations:  

1. If you pass an open connection to either of these then the first time the framework attempts to use it an InvalidOperationException is thrown saying it cannot re-open an already open connection.  
2. The contextOwnsConnection flag is interpreted to mean whether or not the underlying store connection should be disposed when the context is disposed. But, regardless of that setting, the store connection is always closed when the context is disposed. So if you have more than one DbContext with the same connection whichever context is disposed first will close the connection (similarly if you have mixed an existing ADO.NET connection with a DbContext, DbContext will always close the connection when it is disposed).  

It is possible to work around the first limitation above by passing a closed connection and only executing code that would open it once all contexts have been created:  

``` csharp
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Linq;

namespace ConnectionManagementExamples
{
    class ConnectionManagementExampleEF5
    {         
        public static void TwoDbContextsOneConnection()
        {
            using (var context1 = new BloggingContext())
            {
                var conn =
                    ((EntityConnection)  
                        ((IObjectContextAdapter)context1).ObjectContext.Connection)  
                            .StoreConnection;

                using (var context2 = new BloggingContext(conn, contextOwnsConnection: false))
                {
                    context2.Database.ExecuteSqlCommand(
                        @"UPDATE Blogs SET Rating = 5" +
                        " WHERE Name LIKE '%Entity Framework%'");

                    var query = context1.Posts.Where(p => p.Blog.Rating > 5);
                    foreach (var post in query)
                    {
                        post.Title += "[Cool Blog]";
                    }
                    context1.SaveChanges();
                }
            }
        }
    }
}
```  

The second limitation just means you need to refrain from disposing any of your DbContext objects until you are ready for the connection to be closed.  

### Behavior in EF6 and future versions  

In EF6 and future versions the DbContext has the same two constructors but no longer requires that the connection passed to the constructor be closed when it is received. So this is now possible:  

``` csharp
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace ConnectionManagementExamples
{
    class ConnectionManagementExample
    {
        public static void PassingAnOpenConnection()
        {
            using (var conn = new SqlConnection("{connectionString}"))
            {
                conn.Open();

                var sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandText =
                    @"UPDATE Blogs SET Rating = 5" +
                     " WHERE Name LIKE '%Entity Framework%'";
                sqlCommand.ExecuteNonQuery();

                using (var context = new BloggingContext(conn, contextOwnsConnection: false))
                {
                    var query = context.Posts.Where(p => p.Blog.Rating > 5);
                    foreach (var post in query)
                    {
                        post.Title += "[Cool Blog]";
                    }
                    context.SaveChanges();
                }

                var sqlCommand2 = new SqlCommand();
                sqlCommand2.Connection = conn;
                sqlCommand2.CommandText =
                    @"UPDATE Blogs SET Rating = 7" +
                     " WHERE Name LIKE '%Entity Framework Rocks%'";
                sqlCommand2.ExecuteNonQuery();
            }
        }
    }
}
```  

Also the contextOwnsConnection flag now controls whether or not the connection is both closed and disposed when the DbContext is disposed. So in the above example the connection is not closed when the context is disposed (line 32) as it would have been in previous versions of EF, but rather when the connection itself is disposed (line 40).  

Of course it is still possible for the DbContext to take control of the connection (just set contextOwnsConnection to true or use one of the other constructors) if you so wish.  

> [!NOTE]
> There are some additional considerations when using transactions with this new model. For details see [Working with Transactions (EF6 Onwards)](~/ef6/saving/transactions.md).  

## Database.Connection.Open()  

### Behavior for EF5 and earlier versions  

In EF5 and earlier versions there is a bug such that the **ObjectContext.Connection.State** was not updated to reflect the true state of the underlying store connection. For example, if you executed the following code you can be returned the status **Closed** even though in fact the underlying store connection is **Open**.  

``` csharp
((IObjectContextAdapter)context).ObjectContext.Connection.State
```  

Separately, if you open the database connection by calling Database.Connection.Open() it will be open until the next time you execute a query or call anything which requires a database connection (e.g. SaveChanges()) but after that the underlying store connection will be closed. The context will then re-open and re-close the connection any time another database operation is required:  

``` csharp
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;

namespace ConnectionManagementExamples
{
    public class DatabaseOpenConnectionBehaviorEF5
    {
        public static void DatabaseOpenConnectionBehavior()
        {
            using (var context = new BloggingContext())
            {
                // At this point the underlying store connection is closed

                context.Database.Connection.Open();

                // Now the underlying store connection is open  
                // (though ObjectContext.Connection.State will report closed)

                var blog = new Blog { /* Blog’s properties */ };
                context.Blogs.Add(blog);

                // The underlying store connection is still open  

                context.SaveChanges();

                // After SaveChanges() the underlying store connection is closed  
                // Each SaveChanges() / query etc now opens and immediately closes
                // the underlying store connection

                blog = new Blog { /* Blog’s properties */ };
                context.Blogs.Add(blog);
                context.SaveChanges();
            }
        }
    }
}
```  

### Behavior in EF6 and future versions  

For EF6 and future versions we have taken the approach that if the calling code chooses to open the connection by calling context.Database.Connection.Open() then it has a good reason for doing so and the framework will assume that it wants control over opening and closing of the connection and will no longer close the connection automatically.  

> [!NOTE]
> This can potentially lead to connections which are open for a long time so use with care.  

We also updated the code so that ObjectContext.Connection.State now keeps track of the state of the underlying connection correctly.  

``` csharp
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;

namespace ConnectionManagementExamples
{
    internal class DatabaseOpenConnectionBehaviorEF6
    {
        public static void DatabaseOpenConnectionBehavior()
        {
            using (var context = new BloggingContext())
            {
                // At this point the underlying store connection is closed

                context.Database.Connection.Open();

                // Now the underlying store connection is open and the
                // ObjectContext.Connection.State correctly reports open too

                var blog = new Blog { /* Blog’s properties */ };
                context.Blogs.Add(blog);
                context.SaveChanges();

                // The underlying store connection remains open for the next operation  

                blog = new Blog { /* Blog’s properties */ };
                context.Blogs.Add(blog);
                context.SaveChanges();

                // The underlying store connection is still open

           } // The context is disposed – so now the underlying store connection is closed
        }
    }
}
```  
