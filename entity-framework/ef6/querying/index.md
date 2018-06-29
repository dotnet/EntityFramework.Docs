---
title: "Querying and Finding Entities - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 65bb3db2-2226-44af-8864-caa575cf1b46
caps.latest.revision: 3
---
# Querying and Finding Entities
This topic covers the various ways you can query for data using Entity Framework, including LINQ and the Find method. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

## Finding entities using a query  

DbSet and IDbSet implement IQueryable and so can be used as the starting point for writing a LINQ query against the database. This is not the appropriate place for an in-depth discussion of LINQ, but here are a couple of simple examples:  

``` csharp
using (var context = new BloggingContext())
{
    // Query for all blogs with names starting with B
    var blogs = from b in context.Blogs
                   where b.Name.StartsWith("B")
                   select b;

    // Query for the Blog named ADO.NET Blog
    var blog = context.Blogs
                    .Where(b => b.Name == "ADO.NET Blog")
                    .FirstOrDefault();
}
```  

Note that DbSet and IDbSet always create queries against the database and will always involve a round trip to the database even if the entities returned already exist in the context. A query is executed against the database when:  

- It is enumerated by a **foreach** (C#) or **For Each** (Visual Basic) statement.  
- It is enumerated by a collection operation such as [ToArray](https://msdn.microsoft.com/library/bb298736), [ToDictionary](https://msdn.microsoft.com/library/system.linq.enumerable.todictionary), or ToList[enter link description here](https://msdn.microsoft.com/library/bb342261).  
- LINQ operators such as [First](https://msdn.microsoft.com/library/bb291976) or [Any](https://msdn.microsoft.com/library/bb337697) are specified in the outermost part of the query.  
- The following methods are called: the [Load](https://msdn.microsoft.com/library/system.data.entity.dbextensions.load) extension method on a DbSet, [DbEntityEntry.Reload](https://msdn.microsoft.com/library/system.data.entity.infrastructure.dbentityentry.reload.aspx), and Database.ExecuteSqlCommand.  

When results are returned from the database, objects that do not exist in the context are attached to the context. If an object is already in the context, the existing object is returned (the current and original values of the object's properties in the entry are **not** overwritten with database values).  

When you perform a query, entities that have been added to the context but have not yet been saved to the database are not returned as part of the result set. To get the data that is in the context, see [Local Data](~/ef6/querying/local-data.md).  

If a query returns no rows from the database, the result will be an empty collection, rather than **null**.  

## Finding entities using primary keys  

The Find method on DbSet uses the primary key value to attempt to find an entity tracked by the context. If the entity is not found in the context then a query will be sent to the database to find the entity there. Null is returned if the entity is not found in the context or in the database.  

Find is different from using a query in two significant ways:  

- A round-trip to the database will only be made if the entity with the given key is not found in the context.  
- Find will return entities that are in the Added state. That is, Find will return entities that have been added to the context but have not yet been saved to the database.  
### Finding an entity by primary key  

The following code shows some uses of Find:  

``` csharp
using (var context = new BloggingContext())
{
    // Will hit the database
    var blog = context.Blogs.Find(3);

    // Will return the same instance without hitting the database
    var blogAgain = context.Blogs.Find(3);

    context.Blogs.Add(new Blog { Id = -1 });

    // Will find the new blog even though it does not exist in the database
    var newBlog = context.Blogs.Find(-1);

    // Will find a User which has a string primary key
    var user = context.Users.Find("johndoe1987");
}
```  

### Finding an entity by composite primary key  

Entity Framework allows your entities to have composite keys - that's a key that is made up of more than one property. For example, you could have a BlogSettings entity that represents a users settings for a particular blog. Because a user would only ever have one BlogSettings for each blog you could chose to make the primary key of BlogSettings a combination of BlogId and Username. The following code attempts to find the BlogSettings with BlogId = 3 and Username = "johndoe1987":  

``` csharp  
using (var context = new BloggingContext())
{
    var settings = context.BlogSettings.Find(3, "johndoe1987");
}
```  

Note that when you have composite keys you need to use ColumnAttribute or the fluent API to specify an ordering for the properties of the composite key. The call to Find must use this order when specifying the values that form the key.  
