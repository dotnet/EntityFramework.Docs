---
title: "Entity Framework No-Tracking Queries - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: f80ac260-c2dc-484d-94a3-3424fd862f8b
caps.latest.revision: 3
---
# Entity Framework No-Tracking Queries
Sometimes you may want to get entities back from a query but not have those entities be tracked by the context. This may result in better performance when querying for large numbers of entities in read-only scenarios. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

A new extension method AsNoTracking allows any query to be run in this way. For example:  

```  
using (var context = new BloggingContext())
{
    // Query for all blogs without tracking them
    var blogs1 = context.Blogs.AsNoTracking();

    // Query for some blogs without tracking them
    var blogs2 = context.Blogs
                        .Where(b => b.Name.Contains(".NET"))
                        .AsNoTracking()
                        .ToList();
}
```  
