---
title: The Load Method - EF6
description: The Load Method in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/querying/load-method
---
# The Load Method
There are several scenarios where you may want to load entities from the database into the context without immediately doing anything with those entities. A good example of this is loading entities for data binding as described in [Local Data](xref:ef6/querying/local-data). One common way to do this is to write a LINQ query and then call ToList on it, only to immediately discard the created list. The Load extension method works just like ToList except that it avoids the creation of the list altogether.  

The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

Here are two examples of using Load. The first is taken from a Windows Forms data binding application where Load is used to query for entities before binding to the local collection, as described in [Local Data](xref:ef6/querying/local-data):  

``` csharp
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);

    _context = new ProductContext();

    _context.Categories.Load();
    categoryBindingSource.DataSource = _context.Categories.Local.ToBindingList();
}
```  

The second example shows using Load to load a filtered collection of related entities, as described in [Loading Related Entities](xref:ef6/querying/related-data):  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);

    // Load the posts with the 'entity-framework' tag related to a given blog
    context.Entry(blog)
        .Collection(b => b.Posts)
        .Query()
        .Where(p => p.Tags.Contains("entity-framework"))
        .Load();
}
```  
