---
title: "Entity Framework Working with DbContext | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: b0e6bddc-8a87-4d51-b1cb-7756df938c23
caps.latest.revision: 3
---
# Entity Framework Working with DbContext
The Entity Framework enables you to query, insert, update, and delete data, using common language runtime (CLR) objects (known as entities). The Entity Framework maps the entities and relationships that are defined in your model to a database. The Entity Framework provides facilities to do the following: materialize data returned from the database as entity objects; track changes that were made to the objects; handle concurrency; propagate object changes back to the database; and bind objects to controls.  
  
The primary class that is responsible for interacting with data as objects is [System.Data.Entity.DbContext](https://msdn.microsoft.com/library/system.data.entity.dbcontext) (often referred to as context). The context class manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database.  
  
This page gives some guidance on how to manage the context class.  
  
## Defining a DbContext derived class  
  
The recommended way to work with context is to define a class that derives from DbContext and exposes DbSet properties that represent collections of the specified entities in the context. If you are working with the EF Designer, the context will be generated for you. If you are working with Code First, you will typically write the context yourself.  
  
```  
public class ProductContext : DbContext 
{ 
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Product> Products { get; set; } 
}
```  
  
Once you have a context, you would query for, add (using [Add](https://msdn.microsoft.com/library/gg679587) or [Attach](https://msdn.microsoft.com/library/gg696261) methods ) or remove (using [Remove](https://msdn.microsoft.com/library/gg679171)) entities in the context through these properties. Accessing a [DbSet](https://msdn.microsoft.com/library/gg696460) property on a context object represent a starting query that returns all entities of the specified type. Note that just accessing a property will not execute the query. A query is executed when:  
  
- It is enumerated by a **foreach** (C#) or **For Each** (Visual Basic) statement.  
- It is enumerated by a collection operation such as [ToArray](https://msdn.microsoft.com/library/bb298736), [ToDictionary](https://msdn.microsoft.com/library/system.linq.enumerable.todictionary), or [ToList](https://msdn.microsoft.com/library/bb342261).  
- LINQ operators such as [First](https://msdn.microsoft.com/library/bb291976) or [Any](https://msdn.microsoft.com/library/bb337697) are specified in the outermost part of the query.  
- The following methods are called: the [Load](https://msdn.microsoft.com/library/system.data.entity.dbextensions.load) extension method on a DbSet, [DbEntityEntry.Reload](https://msdn.microsoft.com/library/system.data.entity.infrastructure.dbentityentry.reload.aspx), and [Database.ExecuteSqlCommand](https://msdn.microsoft.com/library/gg679456.aspx).  
  
## Lifetime  
  
The lifetime of the context begins when the instance is created and ends when the instance is either disposed or garbage-collected. Use **using** if you want all the resources that the context controls to be disposed at the end of the block. When you use **using**, the compiler automatically creates a try/finally block and calls dispose in the **finally** block.  
  
```  
using (var context = new ProductContext()) 
{     
    // Perform data access using the context 
}
```  
  
Here are some general guidelines when deciding on the lifetime of the context:  
  
- When working with long-running context consider the following:  
    - As you load more objects and their references into memory, the memory consumption of the context may increase rapidly. This may cause performance issues.  
    - Remember to dispose of the context when it is no longer required.  
    - If an exception causes the context to be in an unrecoverable state, the whole application may terminate.  
    - The chances of running into concurrency-related issues increase as the gap between the time when the data is queried and updated grows.  
- When working with Web applications, use a context instance per request.  
- When working with Windows Presentation Foundation (WPF) or Windows Forms, use a context instance per form. This lets you use change-tracking functionality that context provides.  
  
## Connections  
  
By default, the context manages connections to the database. The context opens and closes connections as needed. For example, the context opens a connection to execute a query, and then closes the connection when all the result sets have been processed.  
  
There are cases when you want to have more control over when the connection opens and closes. For example, when working with SQL Server Compact, opening and closing the same connection is expensive. You can manage this process manually by using the [Connection](https://msdn.microsoft.com/library/system.data.objects.objectcontext.connection) property.  
  
## Multithreading  
  
The context is not thread safe. You can still create a multithreaded application as long as an instance of the same entity class is not tracked by multiple contexts at the same time.  
  