---
title: Automatic detect changes - EF6
description: Automatic detect changes in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/saving/change-tracking/auto-detect-changes
---
# Automatic detect changes
When using most POCO entities the determination of how an entity has changed (and therefore which updates need to be sent to the database) is handled by the Detect Changes algorithm. Detect Changes works by detecting the differences between the current property values of the entity and the original property values that are stored in a snapshot when the entity was queried or attached. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

By default, Entity Framework performs Detect Changes automatically when the following methods are called:  

- DbSet.Find  
- DbSet.Local  
- DbSet.Add  
- DbSet.AddRange
- DbSet.Remove  
- DbSet.RemoveRange
- DbSet.Attach  
- DbContext.SaveChanges  
- DbContext.GetValidationErrors  
- DbContext.Entry  
- DbChangeTracker.Entries  

## Disabling automatic detection of changes  

If you are tracking a lot of entities in your context and you call one of these methods many times in a loop, then you may get significant performance improvements by turning off detection of changes for the duration of the loop. For example:  

``` csharp
using (var context = new BloggingContext())
{
    try
    {
        context.Configuration.AutoDetectChangesEnabled = false;

        // Make many calls in a loop
        foreach (var blog in aLotOfBlogs)
        {
            context.Blogs.Add(blog);
        }
    }
    finally
    {
        context.Configuration.AutoDetectChangesEnabled = true;
    }
}
```  

Don’t forget to re-enable detection of changes after the loop — We've used a try/finally to ensure it is always re-enabled even if code in the loop throws an exception.  

An alternative to disabling and re-enabling is to leave automatic detection of changes turned off at all times and either call context.ChangeTracker.DetectChanges explicitly or use change tracking proxies diligently. Both of these options are advanced and can easily introduce subtle bugs into your application so use them with care.  

If you need to add or remove many objects from a context, consider using DbSet.AddRange and DbSet.RemoveRange. This methods automatically detect changes only once after the add or remove operations are completed. 
