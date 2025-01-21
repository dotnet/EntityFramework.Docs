---
title: Raw SQL Queries - EF6
description: Raw SQL Queries in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/querying/raw-sql
---
# Raw SQL Queries (EF6)

Entity Framework allows you to query using LINQ with your entity classes. However, there may be times that you want to run queries using raw SQL directly against the database. This includes calling stored procedures, which can be helpful for Code First models that currently do not support mapping to stored procedures. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

## Writing SQL queries for entities  

The SqlQuery method on DbSet allows a raw SQL query to be written that will return entity instances. The returned objects will be tracked by the context just as they would be if they were returned by a LINQ query. For example:  

``` csharp  
using (var context = new BloggingContext())
{
    var blogs = context.Blogs.SqlQuery("SELECT * FROM dbo.Blogs").ToList();
}
```  

Note that, just as for LINQ queries, the query is not executed until the results are enumerated—in the example above this is done with the call to ToList.  

Care should be taken whenever raw SQL queries are written for two reasons. First, the query should be written to ensure that it only returns entities that are really of the requested type. For example, when using features such as inheritance it is easy to write a query that will create entities that are of the wrong CLR type.  

Second, some types of raw SQL query expose potential security risks, especially around SQL injection attacks. Make sure that you use parameters in your query in the correct way to guard against such attacks.  

### Loading entities from stored procedures  

You can use DbSet.SqlQuery to load entities from the results of a stored procedure. For example, the following code calls the dbo.GetBlogs procedure in the database:  

``` csharp
using (var context = new BloggingContext())
{
    var blogs = context.Blogs.SqlQuery("dbo.GetBlogs").ToList();
}
```  

You can also pass parameters to a stored procedure using the following syntax:  

``` csharp
using (var context = new BloggingContext())
{
    var blogId = 1;

    var blogs = context.Blogs.SqlQuery("dbo.GetBlogById @p0", blogId).Single();
}
```  

## Writing SQL queries for non-entity types  

A SQL query returning instances of any type, including primitive types, can be created using the SqlQuery method on the Database class. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blogNames = context.Database.SqlQuery<string>(
                       "SELECT Name FROM dbo.Blogs").ToList();
}
```  

The results returned from SqlQuery on Database will never be tracked by the context even if the objects are instances of an entity type.  

## Sending raw commands to the database  

Non-query commands can be sent to the database using the ExecuteSqlCommand method on Database. For example:  

``` csharp
using (var context = new BloggingContext())
{
    context.Database.ExecuteSqlCommand(
        "UPDATE dbo.Blogs SET Name = 'Another Name' WHERE BlogId = 1");
}
```  

Note that any changes made to data in the database using ExecuteSqlCommand are opaque to the context until entities are loaded or reloaded from the database.  

### Output Parameters  

If output parameters are used, their values will not be available until the results have been read completely. This is due to the underlying behavior of DbDataReader, see [Retrieving Data Using a DataReader](https://go.microsoft.com/fwlink/?LinkID=398589) for more details.  
