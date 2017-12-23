---
title: Loading Related Data - EF Core
author: rowanmiller
ms.author: divega
ms.date: 10/27/2016
ms.assetid: f9fb64e2-6699-4d70-a773-592918c04c19
ms.technology: entity-framework-core
uid: core/querying/related-data
---

# Loading Related Data

Entity Framework Core allows you to use the navigation properties in your model to load related entities. There are three common O/RM patterns used to load related data.
* **Eager loading** means that the related data is loaded from the database as part of the initial query.
* **Explicit loading** means that the related data is explicitly loaded from the database at a later time.
* **Lazy loading** means that the related data is transparently loaded from the database when the navigation property is accessed. Lazy loading is not yet possible with EF Core.

> [!TIP]  
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Querying) on GitHub.

## Eager loading

You can use the `Include` method to specify related data to be included in query results. In the following example, the blogs that are returned in the results will have their `Posts` property populated with the related posts.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#SingleInclude)]

> [!TIP]  
> Entity Framework Core will automatically fix-up navigation properties to any other entities that were previously loaded into the context instance. So even if you don't explicitly include the data for a navigation property, the property may still be populated if some or all of the related entities were previously loaded.


You can include related data from multiple relationships in a single query.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#MultipleIncludes)]

### Including multiple levels

You can drill down thru relationships to include multiple levels of related data using the `ThenInclude` method. The following example loads all blogs, their related posts, and the author of each post.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#SingleThenInclude)]

> [!NOTE]  
> Current versions of Visual Studio offer incorrect code completion options and can cause correct expressions to be flagged with syntax errors when passed to `ThenInclude` after collection navigation properties. This is a symptom of the IntelliSense bug tracked at https://github.com/dotnet/roslyn/issues/8237 and you can ignore the spurious errors. 

You can chain multiple calls to `ThenInclude` to continue including further levels of related data.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#MultipleThenIncludes)]

You can combine all of this to include related data from multiple levels and multiple roots in the same query.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#IncludeTree)]

You may want to include multiple related entities for one of the entities that is being included. For example, when querying `Blog`s, you include `Posts` and then want to include both the `Author` and `Tags` of the `Posts`. To do this, you need to specify each include path starting at the root. For example, `Blog -> Posts -> Author` and `Blog -> Posts -> Tags`. This does not mean you will get redundant joins, in most cases EF will consolidate the joins when generating SQL.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#MultipleLeafIncludes)]

### Ignored includes

If you change the query so that it no longer returns instances of the entity type that the query began with, then the include operators are ignored.

In the following example, the include operators are based on the `Blog`, but then the `Select` operator is used to change the query to return an anonymous type. In this case, the include operators have no effect.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#IgnoredInclude)]

By default, EF Core will log a warning when include operators are ignored. See [Logging](../miscellaneous/logging.md) for more information on viewing logging output. You can change the behavior when an include operator is ignored to either throw or do nothing. This is done when setting up the options for your context - typically in `DbContext.OnConfiguring`, or in `Startup.cs` if you are using ASP.NET Core.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/ThrowOnIgnoredInclude/BloggingContext.cs#OnConfiguring)]

## Explicit loading

> [!NOTE]  
> This feature was introduced in EF Core 1.1.

You can explicitly load a navigation property via the `DbContext.Entry(...)` API.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#Eager)]

You can also explicitly load a navigation property by executing a seperate query that returns the related entities. If change tracking is enabled, then when loading an entity, EF Core will automatically set the navigation properties of the newly-loaded entitiy to refer to any entities already loaded, and set the navigation properties of the already-loaded entities to refer to the newly-loaded entity.

### Querying related entities

You can also get a LINQ query that represents the contents of a navigation property.

This allows you to do things such as running an aggregate operator over the related entities without loading them into memory.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#NavQueryAggregate)]

You can also filter which related entities are loaded into memory.

[!code-csharp[Main](../../../samples/core/Querying/Querying/RelatedData/Sample.cs#NavQueryFiltered)]

## Lazy loading

Lazy loading is not yet supported by EF Core. You can view the [lazy loading item on our backlog](https://github.com/aspnet/EntityFramework/issues/3797) to track this feature.

## Related data and serialization

Because EF Core will automatically fix-up navigation properties, you can end up with cycles in your object graph. For example, Loading a blog and it's related posts will result in a blog object that references a collection of posts. Each of those posts will have a reference back to the blog.

Some serialization frameworks do not allow such cycles. For example, Json.NET will throw the following exception if a cycle is encoutered.

> Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property 'Blog' with type 'MyApplication.Models.Blog'.

If you are using ASP.NET Core, you can configure Json.NET to ignore cycles that it finds in the object graph. This is done in the `ConfigureServices(...)` method in `Startup.cs`.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddMvc()
        .AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        );

    ...
}
```
