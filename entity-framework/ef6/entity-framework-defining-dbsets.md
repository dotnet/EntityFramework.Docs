---
title: "Entity Framework Defining DbSets - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 4528a509-ace7-4dfb-8065-1b833f5e03a0
caps.latest.revision: 3
---
# Entity Framework Defining DbSets
When developing with the Code First workflow you define a derived DbContext that represents you session with the database and exposes a DbSet for each type in your model. This topic covers the various ways you can define the DbSet properties.  
  
## DbContext with DbSet properties  
  
The common case shown in Code First examples is to have a DbContext with public automatic DbSet properties for the entity types of your model. For example:  
  
```  
public class BloggingContext : DbContext 
{ 
    public DbSet<Blog> Blogs { get; set; } 
    public DbSet<Post> Posts { get; set; } 
}
```  
  
When used in Code First mode, this will configure Unicorn, Princess, LadyInWaiting, and Castle as entity types, as well as configuring other types reachable from these. In addition DbContext will automatically call the setter for each of these properties to set an instance of the appropriate DbSet.  
  
## DbContext with IDbSet properties  
  
There a situations, such as when creating mocks or fakes, where it is more useful to declare your set properties using an interface. In such cases the IDbSet interface can be used in place of DbSet. For example:  
  
```  
public class BloggingContext : DbContext 
{ 
    public IDbSet<Blog> Blogs { get; set; } 
    public IDbSet<Post> Posts { get; set; } 
}
```  
  
This context works in exactly the same way as the context that uses the DbSet class for its set properties.  
  
## DbContext with read-only set properties  
  
If you do not wish to expose public setters for your DbSet or IDbSet properties you can instead create read-only properties and create the set instances yourself. For example:  
  
```  
public class BloggingContext : DbContext 
{ 
    public DbSet<Blog> Blogs 
    { 
        get { return Set<Blog>(); } 
    } 
 
    public DbSet<Post> Posts 
    { 
        get { return Set<Post>(); } 
    } 
}
```  
  
Note that DbContext caches the instance of DbSet returned from the Set method so that each of these properties will return the same instance every time it is called.  
  
Discovery of entity types for Code First works in the same way here as it does for properties with public getters and setters.  
  