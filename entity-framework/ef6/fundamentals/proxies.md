---
title: Working with proxies - EF6
description: Working with proxies in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/proxies
---
# Working with proxies
When creating instances of POCO entity types, Entity Framework often creates instances of a dynamically generated derived type that acts as a proxy for the entity. This proxy overrides some virtual properties of the entity to insert hooks for performing actions automatically when the property is accessed. For example, this mechanism is used to support lazy loading of relationships. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

## Disabling proxy creation  

Sometimes it is useful to prevent Entity Framework from creating proxy instances. For example, serializing non-proxy instances is considerably easier than serializing proxy instances. Proxy creation can be turned off by clearing the `ProxyCreationEnabled` flag. One place you could do this is in the constructor of your context. For example:  

``` csharp
public class BloggingContext : DbContext
{
    public BloggingContext()
    {
        this.Configuration.ProxyCreationEnabled = false;
    }  

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}
```  

Note that the EF will not create proxies for types where there is nothing for the proxy to do. This means that you can also avoid proxies by having types that are sealed and/or have no virtual properties.  

## Explicitly creating an instance of a proxy  

A proxy instance will not be created if you create an instance of an entity using the new operator. This may not be a problem, but if you need to create a proxy instance (for example, so that lazy loading or proxy change tracking will work) then you can do so using the `Create` method of `DbSet`. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Create();
}
```  

The generic version of `Create` can be used if you want to create an instance of a derived entity type. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var admin = context.Users.Create<Administrator>();
}
```  

Note that the `Create` method does not add or attach the created entity to the context.  

Note that the `Create` method will just create an instance of the entity type itself if creating a proxy type for the entity would have no value because it would not do anything. For example, if the entity type is sealed and/or has no virtual properties then `Create` will just create an instance of the entity type.  

## Getting the actual entity type from a proxy type  

Proxy types have names that look something like this:  

```
System.Data.Entity.DynamicProxies.Blog_5E43C6C196972BF0754973E48C9C941092D86818CD94005E9A759B70BF6E48E6
```

You can find the entity type for this proxy type using the `GetObjectType` method from `ObjectContext`. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    var entityType = ObjectContext.GetObjectType(blog.GetType());
}
```  

Note that if the type passed to `GetObjectType` is an instance of an entity type that is not a proxy type then the type of entity is still returned. This means you can always use this method to get the actual entity type without any other checking to see if the type is a proxy type or not.  
