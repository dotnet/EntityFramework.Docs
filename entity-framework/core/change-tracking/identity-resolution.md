---
title: Identity Resolution - EF Core
description: Resolving multiple entity instances into a single instance using primary key values
author: SamMonoRT
ms.date: 12/30/2020
uid: core/change-tracking/identity-resolution
---

# Identity Resolution in EF Core

A <xref:Microsoft.EntityFrameworkCore.DbContext> can only track one entity instance with any given primary key value. This means multiple instances of an entity with the same key value must be resolved to a single instance. This is called "identity resolution". Identity resolution ensures Entity Framework Core (EF Core) is tracking a consistent graph with no ambiguities about the relationships or property values of the entities.

> [!TIP]
> This document assumes that entity states and the basics of EF Core change tracking are understood. See [Change Tracking in EF Core](xref:core/change-tracking/index) for more information on these topics.

> [!TIP]
> You can run and debug into all the code in this document by [downloading the sample code from GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/ChangeTracking/IdentityResolutionInEFCore).

## Introduction

The following code queries for an entity and then attempts to attach a different instance with the same primary key value:

<!--
            using var context = new BlogsContext();

            var blogA = context.Blogs.Single(e => e.Id == 1);
            var blogB = new Blog { Id = 1, Name = ".NET Blog (All new!)" };

            try
            {
                context.Update(blogB); // This will throw
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
-->
[!code-csharp[Identity_Resolution_in_EF_Core_1](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Identity_Resolution_in_EF_Core_1)]

Running this code results in the following exception:

> System.InvalidOperationException: The instance of entity type 'Blog' cannot be tracked because another instance with the key value '{Id: 1}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached.

EF Core requires a single instance because:

- Property values may be different between multiple instances. When updating the database, EF Core needs to know which property values to use.
- Relationships with other entities may be different between multiple instances. For example, "blogA" may be related to a different collection of posts than "blogB".

The exception above is commonly encountered in these situations:

- When attempting to update an entity
- When attempting to track a serialized graph of entities
- When failing to set a key value that is not automatically generated
- When reusing a DbContext instance for multiple units-of-work

Each of these situations is discussed in the following sections.

## Updating an entity

There are several different approaches to update an entity with new values, as covered in [Change Tracking in EF Core](xref:core/change-tracking/index) and [Explicitly Tracking Entities](xref:core/change-tracking/explicit-tracking). These approaches are outlined below in the context of identity resolution. An important point to notice is that each of the approaches use either a query or a call to one of `Update` or `Attach`, but **_never both_**.

### Call Update

Often the entity to update does not come from a query on the DbContext that we are going to use for SaveChanges. For example, in a web application, an entity instance may be created from the information in a POST request. The simplest way to handle this is to use <xref:Microsoft.EntityFrameworkCore.DbContext.Update*?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.DbSet`1.Update*?displayProperty=nameWithType>. For example:

<!--
    public static void UpdateFromHttpPost1(Blog blog)
    {
        using var context = new BlogsContext();

        context.Update(blog);

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_1](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_1)]

In this case:

- Only a single instance of the entity is created.
- The entity instance is **not** queried from the database as part of making the update.
- All property values will be updated in the database, regardless of whether they have actually changed or not.
- One database round-trip is made.

### Query then apply changes

Usually it is not known which property values have actually been changed when an entity is created from information in a POST request or similar. Often it is fine to just update all values in the database, as we did in the previous example. However, if the application is handling many entities and only a small number of those have actual changes, then it may be useful to limit the updates sent. This can be achieved by executing a query to track the entities as they currently exist in the database, and then applying changes to these tracked entities. For example:

<!--
    public static void UpdateFromHttpPost2(Blog blog)
    {
        using var context = new BlogsContext();

        var trackedBlog = context.Blogs.Find(blog.Id);

        trackedBlog.Name = blog.Name;
        trackedBlog.Summary = blog.Summary;

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_2](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_2)]

In this case:

- Only a single instance of the entity is tracked; the one that is returned from the database by the `Find` query.
- `Update`, `Attach`, etc. are **not** used.
- Only property values that have actually changed are updated in the database.
- Two database round-trips are made.

EF Core has some helpers for transferring property values like this. For example, <xref:Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues.SetValues*?displayProperty=nameWithType> will copy all the values from the given object and set them on the tracked object:

<!--
    public static void UpdateFromHttpPost3(Blog blog)
    {
        using var context = new BlogsContext();

        var trackedBlog = context.Blogs.Find(blog.Id);

        context.Entry(trackedBlog).CurrentValues.SetValues(blog);

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_3](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_3)]

`SetValues` accepts various object types, including data transfer objects (DTOs) with property names that match the properties of the entity type. For example:

<!--
    public static void UpdateFromHttpPost4(BlogDto dto)
    {
        using var context = new BlogsContext();

        var trackedBlog = context.Blogs.Find(dto.Id);

        context.Entry(trackedBlog).CurrentValues.SetValues(dto);

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_4](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_4)]

Or a dictionary with name/value entries for the property values:

<!--
    public static void UpdateFromHttpPost5(Dictionary<string, object> propertyValues)
    {
        using var context = new BlogsContext();

        var trackedBlog = context.Blogs.Find(propertyValues["Id"]);

        context.Entry(trackedBlog).CurrentValues.SetValues(propertyValues);

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_5](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_5)]

See [Accessing tracked entities](xref:core/change-tracking/entity-entries) for more information on working with property values like this.

### Use original values

So far each approach has either executed a query before making the update, or updated all property values regardless of whether or not they have changed. To update only values that have changed without querying as part of the update requires specific information about which property values have changed. A common way to get this information is to send back both the current and original values in the HTTP Post or similar. For example:

<!--
    public static void UpdateFromHttpPost6(Blog blog, Dictionary<string, object> originalValues)
    {
        using var context = new BlogsContext();

        context.Attach(blog);
        context.Entry(blog).OriginalValues.SetValues(originalValues);

        context.SaveChanges();
    }
-->
[!code-csharp[Updating_an_entity_6](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Updating_an_entity_6)]

In this code the entity with modified values is first attached. This causes EF Core to track the entity in the `Unchanged` state; that is, with no property values marked as modified. The dictionary of original values is then applied to this tracked entity. This will mark as modified properties with different current and original values. Properties that have the same current and original values will not be marked as modified.

In this case:

- Only a single instance of the entity is tracked, using Attach.
- The entity instance is **not** queried from the database as part of making the update.
- Applying the original values ensures that only property values that have actually changed are updated in the database.
- One database round-trip is made.

As with the examples in the previous section, the original values do not have to be passed as a dictionary; an entity instance or DTO will also work.

> [!TIP]
> While this approach has appealing characteristics, it does require sending the entity's original values to and from the web client. Carefully consider whether this extra complexity is worth the benefits; for many applications one of the simpler approaches is more pragmatic.

## Attaching a serialized graph

EF Core works with graphs of entities connected via foreign keys and navigation properties, as described in [Changing Foreign Keys and Navigations](xref:core/change-tracking/relationship-changes). If these graphs are created outside of EF Core using, for example, from a JSON file, then they can have multiple instances of the same entity. These duplicates need to be resolved into single instances before the graph can be tracked.

### Graphs with no duplicates

Before going any further it is important to recognize that:

- Serializers often have options for handling loops and duplicate instances in the graph.
- The choice of object used as the graph root can often help reduce or remove duplicates.

If possible, use serialization options and choose roots that do not result in duplicates. For example, the following code uses [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/) to serialize a list of blogs each with its associated posts:

<!--
            using var context = new BlogsContext();

            var blogs = context.Blogs.Include(e => e.Posts).ToList();

            var serialized = JsonConvert.SerializeObject(
                blogs,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                });

            Console.WriteLine(serialized);
-->
[!code-csharp[Attaching_a_serialized_graph_1a](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_1a)]

The JSON generated from this code is:

```json
[
  {
    "Id": 1,
    "Name": ".NET Blog",
    "Summary": "Posts about .NET",
    "Posts": [
      {
        "Id": 1,
        "Title": "Announcing the Release of EF Core 5.0",
        "Content": "Announcing the release of EF Core 5.0, a full featured cross-platform...",
        "BlogId": 1
      },
      {
        "Id": 2,
        "Title": "Announcing F# 5",
        "Content": "F# 5 is the latest version of F#, the functional programming language...",
        "BlogId": 1
      }
    ]
  },
  {
    "Id": 2,
    "Name": "Visual Studio Blog",
    "Summary": "Posts about Visual Studio",
    "Posts": [
      {
        "Id": 3,
        "Title": "Disassembly improvements for optimized managed debugging",
        "Content": "If you are focused on squeezing out the last bits of performance for your .NET service or...",
        "BlogId": 2
      },
      {
        "Id": 4,
        "Title": "Database Profiling with Visual Studio",
        "Content": "Examine when database queries were executed and measure how long the take using...",
        "BlogId": 2
      }
    ]
  }
]
```

Notice that there are no duplicate blogs or posts in the JSON. This means that simple calls to `Update` will work to update these entities in the database:

<!--
        public static void UpdateBlogsFromJson(string json)
        {
            using var context = new BlogsContext();

            var blogs = JsonConvert.DeserializeObject<List<Blog>>(json);

            foreach (var blog in blogs)
            {
                context.Update(blog);
            }

            context.SaveChanges();
        }
-->
[!code-csharp[Attaching_a_serialized_graph_1b](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_1b)]

### Handling duplicates

The code in the previous example serialized each blog with its associated posts. If this is changed to serialize each post with its associated blog, then duplicates are introduced into the serialized JSON. For example:

<!--
            using var context = new BlogsContext();

            var posts = context.Posts.Include(e => e.Blog).ToList();

            var serialized = JsonConvert.SerializeObject(
                posts,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                });

            Console.WriteLine(serialized);
-->
[!code-csharp[Attaching_a_serialized_graph_2](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_2)]

The serialized JSON now looks like this:

```json
[
  {
    "Id": 1,
    "Title": "Announcing the Release of EF Core 5.0",
    "Content": "Announcing the release of EF Core 5.0, a full featured cross-platform...",
    "BlogId": 1,
    "Blog": {
      "Id": 1,
      "Name": ".NET Blog",
      "Summary": "Posts about .NET",
      "Posts": [
        {
          "Id": 2,
          "Title": "Announcing F# 5",
          "Content": "F# 5 is the latest version of F#, the functional programming language...",
          "BlogId": 1
        }
      ]
    }
  },
  {
    "Id": 2,
    "Title": "Announcing F# 5",
    "Content": "F# 5 is the latest version of F#, the functional programming language...",
    "BlogId": 1,
    "Blog": {
      "Id": 1,
      "Name": ".NET Blog",
      "Summary": "Posts about .NET",
      "Posts": [
        {
          "Id": 1,
          "Title": "Announcing the Release of EF Core 5.0",
          "Content": "Announcing the release of EF Core 5.0, a full featured cross-platform...",
          "BlogId": 1
        }
      ]
    }
  },
  {
    "Id": 3,
    "Title": "Disassembly improvements for optimized managed debugging",
    "Content": "If you are focused on squeezing out the last bits of performance for your .NET service or...",
    "BlogId": 2,
    "Blog": {
      "Id": 2,
      "Name": "Visual Studio Blog",
      "Summary": "Posts about Visual Studio",
      "Posts": [
        {
          "Id": 4,
          "Title": "Database Profiling with Visual Studio",
          "Content": "Examine when database queries were executed and measure how long the take using...",
          "BlogId": 2
        }
      ]
    }
  },
  {
    "Id": 4,
    "Title": "Database Profiling with Visual Studio",
    "Content": "Examine when database queries were executed and measure how long the take using...",
    "BlogId": 2,
    "Blog": {
      "Id": 2,
      "Name": "Visual Studio Blog",
      "Summary": "Posts about Visual Studio",
      "Posts": [
        {
          "Id": 3,
          "Title": "Disassembly improvements for optimized managed debugging",
          "Content": "If you are focused on squeezing out the last bits of performance for your .NET service or...",
          "BlogId": 2
        }
      ]
    }
  }
]
```

Notice that the graph now includes multiple Blog instances with the same key value, as well as multiple Post instances with the same key value. Attempting to track this graph like we did in the previous example will throw:

> System.InvalidOperationException: The instance of entity type 'Post' cannot be tracked because another instance with the key value '{Id: 2}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached.

We can fix this in two ways:

- Use JSON serialization options that preserve references
- Perform identity resolution while the graph is being tracked

#### Preserve references

Json.NET provides the `PreserveReferencesHandling` option to handle this. For example:

<!--
            var serialized = JsonConvert.SerializeObject(
                posts,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    Formatting = Formatting.Indented
                });
-->
[!code-csharp[Attaching_a_serialized_graph_3](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_3)]

The resulting JSON now looks like this:

```json
{
  "$id": "1",
  "$values": [
    {
      "$id": "2",
      "Id": 1,
      "Title": "Announcing the Release of EF Core 5.0",
      "Content": "Announcing the release of EF Core 5.0, a full featured cross-platform...",
      "BlogId": 1,
      "Blog": {
        "$id": "3",
        "Id": 1,
        "Name": ".NET Blog",
        "Summary": "Posts about .NET",
        "Posts": [
          {
            "$ref": "2"
          },
          {
            "$id": "4",
            "Id": 2,
            "Title": "Announcing F# 5",
            "Content": "F# 5 is the latest version of F#, the functional programming language...",
            "BlogId": 1,
            "Blog": {
              "$ref": "3"
            }
          }
        ]
      }
    },
    {
      "$ref": "4"
    },
    {
      "$id": "5",
      "Id": 3,
      "Title": "Disassembly improvements for optimized managed debugging",
      "Content": "If you are focused on squeezing out the last bits of performance for your .NET service or...",
      "BlogId": 2,
      "Blog": {
        "$id": "6",
        "Id": 2,
        "Name": "Visual Studio Blog",
        "Summary": "Posts about Visual Studio",
        "Posts": [
          {
            "$ref": "5"
          },
          {
            "$id": "7",
            "Id": 4,
            "Title": "Database Profiling with Visual Studio",
            "Content": "Examine when database queries were executed and measure how long the take using...",
            "BlogId": 2,
            "Blog": {
              "$ref": "6"
            }
          }
        ]
      }
    },
    {
      "$ref": "7"
    }
  ]
}
```

Notice that this JSON has replaced duplicates with references like `"$ref": "5"` that refer to the already existing instance in the graph. This graph can again be tracked using the simple calls to `Update`, as shown above.

The <xref:System.Text.Json> support in the .NET base class libraries (BCL) has a similar option which produces the same result. For example:

<!--
            var serialized = System.Text.Json.JsonSerializer.Serialize(posts, new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            });
-->
[!code-csharp[Attaching_a_serialized_graph_4](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_4)]

#### Resolve duplicates

If it is not possible to eliminate duplicates in the serialization process, then <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.TrackGraph*?displayProperty=nameWithType> provides a way to handle this. TrackGraph works like `Add`, `Attach` and `Update` except that it generates a callback for every entity instance before tracking it. This callback can be used to either track the entity or ignore it. For example:

<!--
        public static void UpdatePostsFromJsonWithIdentityResolution(string json)
        {
            using var context = new BlogsContext();

            var posts = JsonConvert.DeserializeObject<List<Post>>(json);

            foreach (var post in posts)
            {
                context.ChangeTracker.TrackGraph(
                    post, node =>
                        {
                            var keyValue = node.Entry.Property("Id").CurrentValue;
                            var entityType = node.Entry.Metadata;

                            var existingEntity = node.Entry.Context.ChangeTracker.Entries()
                                .FirstOrDefault(
                                    e => Equals(e.Metadata, entityType)
                                         && Equals(e.Property("Id").CurrentValue, keyValue));

                            if (existingEntity == null)
                            {
                                Console.WriteLine($"Tracking {entityType} entity with key value {keyValue}");

                                node.Entry.State = EntityState.Modified;
                            }
                            else
                            {
                                Console.WriteLine($"Discarding duplicate {entityType} entity with key value {keyValue}");
                            }
                        });
            }

            context.SaveChanges();
        }
-->
[!code-csharp[Attaching_a_serialized_graph_5](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/SerializedGraphExamples.cs?name=Attaching_a_serialized_graph_5)]

For each entity in the graph, this code will:

- Find the entity type and key value of the entity
- Lookup the entity with this key in the change tracker
  - If the entity is found, then no further action is taken as the entity is a duplicate
  - If the entity is not found, then it is tracked by setting the state to `Modified`

The output from running this code is:

```output
Tracking EntityType: Post entity with key value 1
Tracking EntityType: Blog entity with key value 1
Tracking EntityType: Post entity with key value 2
Discarding duplicate EntityType: Post entity with key value 2
Tracking EntityType: Post entity with key value 3
Tracking EntityType: Blog entity with key value 2
Tracking EntityType: Post entity with key value 4
Discarding duplicate EntityType: Post entity with key value 4
```

> [!IMPORTANT]
> This code **assumes that all duplicates are identical**. This makes it safe to arbitrarily choose one of the duplicates to track while discarding the others. If the duplicates can differ, then the code will need to decide how to determine which one to use, and how to combine property and navigation values together.

> [!NOTE]
> For simplicity, this code assumes each entity has a primary key property called `Id`. This could be codified into an abstract base class or interface. Alternately, the primary key property or properties could be obtained from the <xref:Microsoft.EntityFrameworkCore.Metadata.IEntityType> metadata such that this code would work with any type of entity.

## Failing to set key values

Entity types are often configured to use [automatically generated key values](xref:core/modeling/generated-properties). This is the default for integer and GUID properties of non-composite keys. However, if the entity type is not configured to use automatically generated key values, then an explicit key value must be set before tracking the entity. For example, using the following entity type:

<!--
    public class Pet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
-->
[!code-csharp[Pet](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Pet)]

Consider code that attempts to track two new entity instances without setting key values:

<!--
            using var context = new BlogsContext();

            context.Add(new Pet { Name = "Smokey" });

            try
            {
                context.Add(new Pet { Name = "Clippy" }); // This will throw
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
-->
[!code-csharp[Failing_to_set_key_values_1](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=Failing_to_set_key_values_1)]

This code will throw:

> System.InvalidOperationException: The instance of entity type 'Pet' cannot be tracked because another instance with the key value '{Id: 0}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached.

The fix for this is to either to set key values explicitly or configure the key property to use generated key values. See [Generated Values](xref:core/modeling/generated-properties) for more information.

## Overusing a single DbContext instance

<xref:Microsoft.EntityFrameworkCore.DbContext> is designed to represent a short-lived unit-of-work, as described in [DbContext Initialization and Configuration](xref:core/dbcontext-configuration/index), and elaborated on in [Change Tracking in EF Core](xref:core/change-tracking/index). Not following this guidance makes it easy to run into situations where an attempt is made to track multiple instances of the same entity. Common examples are:

- Using the same DbContext instance to both set up test state and then execute the test. This often results in the DbContext still tracking one entity instance from test setup, while then attempting to attach a new instance in the test proper. Instead, use a different DbContext instance for setting up test state and the test code proper.
- Using a shared DbContext instance in a repository or similar code. Instead, make sure your repository uses a single DbContext instance for each unit-of-work.

## Identity resolution and queries

Identity resolution happens automatically when entities are tracked from a query. This means that if an entity instance with a given key value is already tracked, then this existing tracked instance is used instead of creating a new instance. This has an important consequence: if the data has changed in the database, then this will not be reflected in the results of the query. This is a good reason to use a new DbContext instance for each unit-of-work, as described in [DbContext Initialization and Configuration](xref:core/dbcontext-configuration/index), and elaborated on in [Change Tracking in EF Core](xref:core/change-tracking/index).

> [!IMPORTANT]
> It is important to understand that EF Core always executes a LINQ query on a DbSet against the database and only returns results based on what is in the database. However, for a tracking query, if the entities returned are already tracked, then the tracked instances are used instead of creating instances from the data in the database.

<xref:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.Reload> or <xref:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.GetDatabaseValues> can be used when tracked entities need to be refreshed with the latest data from the database. See [Accessing Tracked Entities](xref:core/change-tracking/entity-entries) for more information.

In contrast to tracking queries, no-tracking queries do not perform identity resolution. This means that no-tracking queries can return duplicates just like in the JSON serialization case described earlier. This is usually not an issue if the query results are going to be serialized and sent to the client.

> [!TIP]
> Do not routinely perform a no-tracking query and then attach the returned entities to the same context. This will be both slower and harder to get right than using a tracking query.

No-tracking queries do not perform identity resolution because doing so impacts the performance of streaming a large number of entities from a query. This is because identity resolution requires keeping track of each instance returned so that it can be used instead of later creating a duplicate.

No-tracking queries can be forced to perform identity resolution by using <xref:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution%60`1(System.Linq.IQueryable{%60%600})>. The query will then keep track of returned instances (without tracking them in the normal way) and ensure no duplicates are created in the query results.

## Overriding object equality

EF Core uses [reference equality](/dotnet/csharp/programming-guide/statements-expressions-operators/equality-comparisons) when comparing entity instances. This is the case even if the entity types override <xref:System.Object.Equals(System.Object)?displayProperty=nameWithType> or otherwise change object equality. However, there is one place where overriding equality can impact EF Core behavior: when collection navigations use the overridden equality instead of reference equality, and hence report multiple instances as the same.

Because of this it is recommended that overriding entity equality should be avoided. If it is used, then make sure to create collection navigations that force reference equality. For example, create an equality comparer that uses reference equality:

<!--
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        private ReferenceEqualityComparer()
        {
        }

        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        bool IEqualityComparer<object>.Equals(object x, object y) => x == y;

        int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
-->
[!code-csharp[ReferenceEqualityComparer](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/ReferenceEqualityComparer.cs?name=ReferenceEqualityComparer)]

(Starting with .NET 5, this is included in the BCL as <xref:System.Collections.Generic.ReferenceEqualityComparer>.)

This comparer can then be used when creating collection navigations. For example:

<!--
        public ICollection<Order> Orders { get; set; }
            = new HashSet<Order>(ReferenceEqualityComparer.Instance);
-->
[!code-csharp[OrdersCollection](../../../samples/core/ChangeTracking/IdentityResolutionInEFCore/IdentityResolutionSamples.cs?name=OrdersCollection)]

### Comparing key properties

In addition to equality comparisons, key values also need to be ordered. This is important for avoiding deadlocks when updating multiple entities in a single call to SaveChanges. All types used for primary, alternate, or foreign key properties, as well as those used for unique indexes, must implement <xref:System.IComparable`1> and <xref:System.IEquatable`1>. Types normally used as keys (int, Guid, string, etc.) already support these interfaces. Custom key types may add these interfaces.
