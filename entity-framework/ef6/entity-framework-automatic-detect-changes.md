---
title: "Entity Framework Automatic Detect Changes | Microsoft Docs"
ms.custom: ""
ms.date: "2016-10-23"
ms.prod: "visual-studio-2013"
ms.reviewer: ""
ms.suite: ""
ms.technology: 
  - "visual-studio-sdk"
ms.tgt_pltfrm: ""
ms.topic: "article"
ms.assetid: a8d1488d-9a54-4623-a76b-e81329ff2756
caps.latest.revision: 3
---
# Entity Framework Automatic Detect Changes
When using most POCO entities the determination of how an entity has changed (and therefore which updates need to be sent to the database) is handled by the Detect Changes algorithm. Detect Changes works by detecting the differences between the current property values of the entity and the original property values that are stored in a snapshot when the entity was queried or attached. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  
  
By default, the Entity Framework performs Detect Changes automatically when the following methods are called:  
  
- DbSet.Find  
- DbSet.Local  
- DbSet.Remove  
- DbSet.Add  
- DbSet.Attach  
- DbContext.SaveChanges  
- DbContext.GetValidationErrors  
- DbContext.Entry  
- DbChangeTracker.Entries  
  
## Disabling automatic detection of changes  
  
If you are tracking a lot of entities in your context and you call one of these methods many times in a loop, then you may get significant performance improvements by turning off detection of changes for the duration of the loop. For example:  
  
```  
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
  