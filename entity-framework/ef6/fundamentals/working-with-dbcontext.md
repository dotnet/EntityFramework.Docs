---
title: Working with DbContext - EF6
description: Working with DbContext in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/working-with-dbcontext
---
# Working with DbContext

In order to use Entity Framework to query, insert, update, and delete data using .NET objects, you first need to [Create a Model](xref:ef6/modeling/index) which maps the entities and relationships that are defined in your model to tables in a database.

Once you have a model, the primary class your application interacts with is `System.Data.Entity.DbContext` (often referred to as the context class). You can use a DbContext associated to a model to:
- Write and execute queries   
- Materialize query results as entity objects
- Track changes that are made to those objects
- Persist object changes back on the database
- Bind objects in memory to UI controls

This page gives some guidance on how to manage the context class.  

## Defining a DbContext derived class  

The recommended way to work with context is to define a class that derives from DbContext and exposes DbSet properties that represent collections of the specified entities in the context. If you are working with the EF Designer, the context will be generated for you. If you are working with Code First, you will typically write the context yourself.  

``` csharp
public class ProductContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
}
```  

Once you have a context, you would query for, add (using `Add` or `Attach` methods ) or remove (using `Remove`) entities in the context through these properties. Accessing a `DbSet` property on a context object represent a starting query that returns all entities of the specified type. Note that just accessing a property will not execute the query. A query is executed when:  

- It is enumerated by a `foreach` (C#) or `For Each` (Visual Basic) statement.  
- It is enumerated by a collection operation such as `ToArray`, `ToDictionary`, or `ToList`.  
- LINQ operators such as `First` or `Any` are specified in the outermost part of the query.  
- One of the following methods are called: the `Load` extension method, `DbEntityEntry.Reload`,  `Database.ExecuteSqlCommand`, and `DbSet<T>.Find`, if an entity with the specified key is not found already loaded in the context.  

## Lifetime  

The lifetime of the context begins when the instance is created and ends when the instance is either disposed or garbage-collected. Use **using** if you want all the resources that the context controls to be disposed at the end of the block. When you use **using**, the compiler automatically creates a try/finally block and calls dispose in the **finally** block.  

``` csharp
public void UseProducts()
{
    using (var context = new ProductContext())
    {     
        // Perform data access using the context
    }
}
```  

Here are some general guidelines when deciding on the lifetime of the context:  

- When working with Web applications, use a context instance per request.  
- When working with Windows Presentation Foundation (WPF) or Windows Forms, use a context instance per form. This lets you use change-tracking functionality that context provides.  
- If the context instance is created by a dependency injection container, it is usually the responsibility of the container to dispose the context.
- If the context is created in application code, remember to dispose of the context when it is no longer required.  
- When working with long-running context consider the following:  
    - As you load more objects and their references into memory, the memory consumption of the context may increase rapidly. This may cause performance issues.  
    - The context is not thread-safe, therefore it should not be shared across multiple threads doing work on it concurrently.
    - If an exception causes the context to be in an unrecoverable state, the whole application may terminate.  
    - The chances of running into concurrency-related issues increase as the gap between the time when the data is queried and updated grows.  

## Connections  

By default, the context manages connections to the database. The context opens and closes connections as needed. For example, the context opens a connection to execute a query, and then closes the connection when all the result sets have been processed.  

There are cases when you want to have more control over when the connection opens and closes. For example, when working with SQL Server Compact, it is often recommended to maintain a separate open connection to the database for the lifetime of the application to improve performance. You can manage this process manually by using the `Connection` property.  
