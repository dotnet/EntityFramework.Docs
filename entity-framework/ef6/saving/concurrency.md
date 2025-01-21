---
title: Handling Concurrency Conflicts - EF6
description: Handling Concurrency Conflicts in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/saving/concurrency
---
# Handling Concurrency Conflicts (EF6)

Optimistic concurrency involves optimistically attempting to save your entity to the database in the hope that the data there has not changed since the entity was loaded. If it turns out that the data has changed then an exception is thrown and you must resolve the conflict before attempting to save again. This topic covers how to handle such exceptions in Entity Framework. The techniques shown in this topic apply equally to models created with Code First and the EF Designer.  

This post is not the appropriate place for a full discussion of optimistic concurrency. The sections below assume some knowledge of concurrency resolution and show patterns for common tasks.  

Many of these patterns make use of the topics discussed in [Working with Property Values](xref:ef6/saving/change-tracking/property-values).  

Resolving concurrency issues when you are using independent associations (where the foreign key is not mapped to a property in your entity) is much more difficult than when you are using foreign key associations. Therefore if you are going to do concurrency resolution in your application it is advised that you always map foreign keys into your entities. All the examples below assume that you are using foreign key associations.  

A DbUpdateConcurrencyException is thrown by SaveChanges when an optimistic concurrency exception is detected while attempting to save an entity that uses foreign key associations.  

## Resolving optimistic concurrency exceptions with Reload (database wins)  

The Reload method can be used to overwrite the current values of the entity with the values now in the database. The entity is then typically given back to the user in some form and they must try to make their changes again and re-save. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    blog.Name = "The New ADO.NET Blog";

    bool saveFailed;
    do
    {
        saveFailed = false;

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Update the values of the entity that failed to save from the store
            ex.Entries.Single().Reload();
        }

    } while (saveFailed);
}
```  

A good way to simulate a concurrency exception is to set a breakpoint on the SaveChanges call and then modify an entity that is being saved in the database using another tool such as SQL Server Management Studio. You could also insert a line before SaveChanges to update the database directly using SqlCommand. For example:  

``` csharp
context.Database.SqlCommand(
    "UPDATE dbo.Blogs SET Name = 'Another Name' WHERE BlogId = 1");
```  

The Entries method on DbUpdateConcurrencyException returns the DbEntityEntry instances for the entities that failed to update. (This property currently always returns a single value for concurrency issues. It may return multiple values for general update exceptions.) An alternative for some situations might be to get entries for all entities that may need to be reloaded from the database and call reload for each of these.  

## Resolving optimistic concurrency exceptions as client wins  

The example above that uses Reload is sometimes called database wins or store wins because the values in the entity are overwritten by values from the database. Sometimes you may wish to do the opposite and overwrite the values in the database with the values currently in the entity. This is sometimes called client wins and can be done by getting the current database values and setting them as the original values for the entity. (See [Working with Property Values](xref:ef6/saving/change-tracking/property-values) for information on current and original values.) For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    blog.Name = "The New ADO.NET Blog";

    bool saveFailed;
    do
    {
        saveFailed = false;
        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Update original values from the database
            var entry = ex.Entries.Single();
            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
        }

    } while (saveFailed);
}
```  

## Custom resolution of optimistic concurrency exceptions  

Sometimes you may want to combine the values currently in the database with the values currently in the entity. This usually requires some custom logic or user interaction. For example, you might present a form to the user containing the current values, the values in the database, and a default set of resolved values. The user would then edit the resolved values as necessary and it would be these resolved values that get saved to the database. This can be done using the DbPropertyValues objects returned from CurrentValues and GetDatabaseValues on the entity’s entry. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    blog.Name = "The New ADO.NET Blog";

    bool saveFailed;
    do
    {
        saveFailed = false;
        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Get the current entity values and the values in the database
            var entry = ex.Entries.Single();
            var currentValues = entry.CurrentValues;
            var databaseValues = entry.GetDatabaseValues();

            // Choose an initial set of resolved values. In this case we
            // make the default be the values currently in the database.
            var resolvedValues = databaseValues.Clone();

            // Have the user choose what the resolved values should be
            HaveUserResolveConcurrency(currentValues, databaseValues, resolvedValues);

            // Update the original values with the database values and
            // the current values with whatever the user choose.
            entry.OriginalValues.SetValues(databaseValues);
            entry.CurrentValues.SetValues(resolvedValues);
        }
    } while (saveFailed);
}

public void HaveUserResolveConcurrency(DbPropertyValues currentValues,
                                       DbPropertyValues databaseValues,
                                       DbPropertyValues resolvedValues)
{
    // Show the current, database, and resolved values to the user and have
    // them edit the resolved values to get the correct resolution.
}
```  

## Custom resolution of optimistic concurrency exceptions using objects  

The code above uses DbPropertyValues instances for passing around current, database, and resolved values. Sometimes it may be easier to use instances of your entity type for this. This can be done using the ToObject and SetValues methods of DbPropertyValues. For example:  

``` csharp
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    blog.Name = "The New ADO.NET Blog";

    bool saveFailed;
    do
    {
        saveFailed = false;
        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Get the current entity values and the values in the database
            // as instances of the entity type
            var entry = ex.Entries.Single();
            var databaseValues = entry.GetDatabaseValues();
            var databaseValuesAsBlog = (Blog)databaseValues.ToObject();

            // Choose an initial set of resolved values. In this case we
            // make the default be the values currently in the database.
            var resolvedValuesAsBlog = (Blog)databaseValues.ToObject();

            // Have the user choose what the resolved values should be
            HaveUserResolveConcurrency((Blog)entry.Entity,
                                       databaseValuesAsBlog,
                                       resolvedValuesAsBlog);

            // Update the original values with the database values and
            // the current values with whatever the user choose.
            entry.OriginalValues.SetValues(databaseValues);
            entry.CurrentValues.SetValues(resolvedValuesAsBlog);
        }

    } while (saveFailed);
}

public void HaveUserResolveConcurrency(Blog entity,
                                       Blog databaseValues,
                                       Blog resolvedValues)
{
    // Show the current, database, and resolved values to the user and have
    // them update the resolved values to get the correct resolution.
}
```  
