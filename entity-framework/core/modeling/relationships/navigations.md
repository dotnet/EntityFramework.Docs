---
title: Relationship navigations - EF Core
description: Reference and collection navigations in Entity Framework Core
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships/navigations
---
# Relationship navigations

EF Core relationships are defined by [foreign keys](xref:core/modeling/relationships/foreign-and-principal-keys). Navigations are layered over foreign keys to provide a natural, object-oriented view for reading and manipulating relationships. By using navigations, applications can work with graphs of entities without being concerned with what is happening to the foreign key values.

> [!IMPORTANT]
> Multiple relationships cannot share navigations. Any foreign key can be associated with at most one navigation from principal to dependent, and at most one navigation from dependent to principal.

> [!TIP]
> There is no need to make navigations virtual unless they are being used by [lazy-loading](xref:core/querying/related-data/lazy) or [change-tracking](xref:core/change-tracking/change-detection#change-tracking-proxies) proxies.

## Reference navigations

Navigations come in two forms--reference and collection. Reference navigations are simple object references to another entity. They represent the "one" side(s) of [one-to-many](xref:core/modeling/relationships/one-to-many) and [one-to-one](xref:core/modeling/relationships/one-to-one) relationships. For example:

```csharp
public Blog TheBlog { get; set; }
```

Reference navigations must have a setter, although it does not need to be public. Reference navigations should not be automatically initialized to a non-null default value; doing so is equivalent to asserting that an entity exists when it does not.

When using [C# nullable reference types](/dotnet/csharp/tutorials/nullable-reference-types), reference navigations must be nullable for optional relationships:

```csharp
public Blog? TheBlog { get; set; }
```

Reference navigations for required relationships can be nullable or non-nullable.

## Collection navigations

Collection navigations are instances of a .NET collection type; that is, any type implementing <xref:System.Collections.Generic.ICollection`1>. The collection contains instances of the related entity type, of which there can be any number. They represent the "many" side(s) of [one-to-many](xref:core/modeling/relationships/one-to-many) and [many-to-many](xref:core/modeling/relationships/many-to-many) relationships. For example:

```csharp
public ICollection<Post> ThePosts { get; set; }
```

Collection navigations do not need to have a setter. It is common to initialize the collection inline, thereby removing the need to ever check if the property is `null`. For example:

```csharp
public ICollection<Post> ThePosts { get; } = new List<Post>();
```

> [!TIP]
> Don't accidentally create an expression bodied property, such as `public ICollection<Post> ThePosts => new List<Post>();`. This will create a new, empty collection instance each time the property is accessed, and will therefore be useless as a navigation.

### Collection types

The underlying collection instance must implement <xref:System.Collections.Generic.ICollection`1>, and must have a working `Add` method. It is common to use <xref:System.Collections.Generic.List`1> or <xref:System.Collections.Generic.HashSet`1>. `List<T>` is efficient for small numbers of related entities and maintains a stable ordering. `HashSet<T>` has more efficient lookups for large numbers of entities, but does not have stable ordering. You can also use your own custom collection implementation.

> [!IMPORTANT]
> The collection must use reference equality. When creating a `HashSet<T>` for a collection navigation, make sure to use <xref:System.Collections.Generic.ReferenceEqualityComparer>.

Arrays cannot be used for collection navigations because, even though they implement `ICollection<T>`, the `Add` method throws an exception when called.

Even though the collection instance must be an `ICollection<T>`, the collection does not need to be exposed as such. For example, it is common to expose the navigation as an <xref:System.Collections.Generic.IEnumerable`1>, which provides a read-only view that cannot be randomly modified by application code. For example:

```csharp
public class Blog
{
    public int Id { get; set; }
    public IEnumerable<Post> ThePosts { get; } = new List<Post>();
}
```

A variation on this pattern includes methods for manipulation of the collection as needed. For example:

```csharp
public class Blog
{
    private readonly List<Post> _posts = new();

    public int Id { get; set; }

    public IEnumerable<Post> Posts => _posts;

    public void AddPost(Post post) => _posts.Add(post);
}
```

Application code could still cast the exposed collection to an `ICollection<T>` and then manipulate it. If this is a concern, then the entity could return a defensive copy of the collection. For example:

```csharp
public class Blog
{
    private readonly List<Post> _posts = new();

    public int Id { get; set; }

    public IEnumerable<Post> Posts => _posts.ToList();

    public void AddPost(Post post) => _posts.Add(post);
}
```

Carefully consider whether the value gained from this is high enough that it outweighs the overhead of creating a copy of the collection every time the navigation is accessed.

> [!TIP]
> This final pattern works because, by-default, EF accesses the collection through its backing field. This means that EF itself adds and removes entities from the actual collection, while applications only interact with a defensive copy of the collection.

### Initialization of collection navigations

Collection navigations can be initialized by the entity type, either eagerly:

```csharp
public class Blog
{
    public ICollection<Post> Posts { get; } = new List<Post>();
}
```

Or lazily:

```csharp
public class Blog
{
    private ICollection<Post>? _posts;

    public ICollection<Post> Posts => _posts ??= new List<Post>();
}
```

If EF needs to add an entity to a collection navigation, for example, while executing a query, then it will initialize the collection if it is currently `null`. The instance created depends on the exposed type of the navigation.

- If the navigation is exposed as a `HashSet<T>`, then an instance of `HashSet<T>` using <xref:System.Collections.Generic.ReferenceEqualityComparer> is created.
- Otherwise, if the navigation is exposed as a concrete type with a parameterless constructor, then an instance of that concrete type is created. This applies to `List<T>`, but also to other collection types, including custom collection types.
- Otherwise, if the navigation is exposed as an `IEnumerable<T>`, an `ICollection<T>`, or an `ISet<T>`, then an instance of `HashSet<T>` using `ReferenceEqualityComparer` is created.
- Otherwise, if the navigation is exposed as an `IList<T>`, then an instance of `List<T>` is created.
- Otherwise, an exception is thrown.

> [!NOTE]
> If [notification entities](xref:core/change-tracking/change-detection#notification-entities), including [change-tracking proxies](xref:core/change-tracking/change-detection#change-tracking-proxies), are being used, then <xref:System.Collections.ObjectModel.ObservableCollection`1> and <xref:Microsoft.EntityFrameworkCore.ChangeTracking.ObservableHashSet`1> are used in place of `List<T>` and `HashSet<T>`.

> [!IMPORTANT]
> As described in the [change tracking documentation](xref:core/change-tracking/identity-resolution), EF only tracks a single instance of any entity with a given key value. This means that collections used as navigations must use [reference equality semantics](/dotnet/csharp/language-reference/operators/equality-operators). Entity types that don't override object equality will get this by default. Make sure to use <xref:System.Collections.Generic.ReferenceEqualityComparer> when creating a `HashSet<T>` for use as a navigation to ensure it works for all entity types.

## Configuring navigations

Navigations are included in the model as part of configuring a relationship. That is, by [convention](xref:core/modeling/relationships/conventions), or using `HasOne`, `HasMany`, etc. in the model building API. Most configuration associated with navigations is done by configuring the relationship itself.

However, there are some types of configuration which are specific to the navigation properties themselves, rather than being part of the overall relationship configuration. This type of configuration is done with the `Navigation` method. For example, to force EF to access the navigation through its property, rather than using the backing field:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Navigation(e => e.Posts)
        .UsePropertyAccessMode(PropertyAccessMode.Property);

    modelBuilder.Entity<Post>()
        .Navigation(e => e.Blog)
        .UsePropertyAccessMode(PropertyAccessMode.Property);
}
```

> [!NOTE]
> The `Navigation` call cannot be used to create a navigation property. It is only used to configure a navigation property which has been previously created by defining a relationship or from a convention.

### Required navigations

A navigation from dependent to principal is required if the relationship is required, which in turn means that the foreign key property is non-nullable. Conversely, the navigation is optional if the foreign key is nullable, and the relationship is therefore optional.

Reference navigations from principal to dependent are different. In most cases, a principal entity can _always_ exist without any dependent entities. That is, a required relationship does _not_ indicate that there will always be at least one dependent entity. There is no way in the EF model, and also no standard way in a relational database, to ensure that a principal is associated with a certain number of dependents. If this is needed, then it must be implemented in application (business) logic.

There is one exception to this rule--when the principal and dependent types are sharing the same table in a relational database, or contained in a document. This can happen with [owned types](xref:core/modeling/owned-entities), or non-owned types [sharing the same table](xref:core/modeling/table-splitting). In this case, the navigation property from the principal to the dependent can be marked as required, indicating that the dependent must exist.

Configuration of the navigation property as required is done using the `Navigation` method. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .Navigation(e => e.BlogHeader)
        .IsRequired();
}
```
