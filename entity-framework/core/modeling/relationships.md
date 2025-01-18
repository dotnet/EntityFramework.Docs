---
title: Introduction to relationships - EF Core
description: How to configure relationships between entity types when using Entity Framework Core
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships
---
# Introduction to relationships

This document provides a simple introduction to the representation of relationships in object models and relational databases, including how EF Core maps between the two.

## Relationships in object models

A relationship defines how two entities relate to each other. For example, when modeling posts in a blog, each post is related to the blog it is published on, and the blog is related to all the posts published on that blog.

In an object-oriented language like C#, the blog and post are typically represented by two classes: `Blog` and `Post`. For example:

```csharp
public class Blog
{
    public string Name { get; set; }
    public virtual Uri SiteUri { get; set; }
}
```

```csharp
public class Post
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }
    public bool Archived { get; set; }
}
```

In the classes above, there is nothing to indicate that `Blog` and `Post` are related. This can be added to the object model by adding a reference from `Post` to the `Blog` on which it is published:

```csharp
public class Post
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateOnly PublishedOn { get; set; }
    public bool Archived { get; set; }

    public Blog Blog { get; set; }
}
```

Likewise, the opposite direction of the same relationship can be represented as a collection of `Post` objects on each `Blog`:

```csharp
public class Blog
{
    public string Name { get; set; }
    public virtual Uri SiteUri { get; set; }

    public ICollection<Post> Posts { get; }
}
```

This connection from `Blog` to `Post` and, inversely, from `Post` back to `Blog` is known as a "relationship" in EF Core.

> [!IMPORTANT]
> A **single** relationship can typically be traversed in either direction. In this example, that is from `Blog` to `Post` via the `Blog.Posts` property, and from `Post` back to `Blog` via the `Post.Blog` property. This is **one** relationship, not two.

> [!TIP]
> In EF Core, the `Blog.Posts` and `Post.Blog` properties are called "navigations".

## Relationships in relational databases

Relational databases represent relationships using foreign keys. For example, using SQL Server or Azure SQL, the following tables can be used to represent our `Post` and `Blog` classes:

```sql
CREATE TABLE [Posts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [Content] nvarchar(max) NULL,
    [PublishedOn] datetime2 NOT NULL,
    [Archived] bit NOT NULL,
    [BlogId] int NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Posts_Blogs_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([Id]) ON DELETE CASCADE);

CREATE TABLE [Blogs] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [SiteUri] nvarchar(max) NULL,
    CONSTRAINT [PK_Blogs] PRIMARY KEY ([Id]));
```

In this relational model, the `Posts` and `Blogs` tables are each given a "primary key" column. The value of the primary key uniquely identifies each post or blog. In addition, the `Posts` table is given a "foreign key" column. The `Blogs` primary key column `Id` is referenced by the `BlogId` foreign key column of the `Posts` table. This column is "constrained" such that any value in the `BlogId` column of `Posts` **must** match a value in the `Id` column of `Blogs`. This match determines which blog every post is related to. For example, if the `BlogId` value in one row of the `Posts` table is 7, then the post represented by that row is published in the blog with the primary key 7.

## Mapping relationships in EF Core

EF Core relationship mapping is all about mapping the primary key/foreign key representation used in a relational database to the references between objects used in an object model.

In the most basic sense, this involves:

- Adding a primary key property to each entity type.
- Adding a foreign key property to one entity type.
- Associating the references between entity types with the primary and foreign keys to form a single relationship configuration.

Once this mapping is made, EF changes the foreign key values as needed when the references between objects change, and changes the references between objects as needed when the foreign key values change.

> [!NOTE]
> Primary keys are used for more than mapping relationships. See [_Keys_](xref:core/modeling/keys) for more information.

For example, the entity types shown above can be updated with primary and foreign key properties:

```csharp
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual Uri SiteUri { get; set; }

    public ICollection<Post> Posts { get; }
}
```

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }
    public bool Archived { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}
```

> [!TIP]
> Primary and foreign key properties don't need to be publicly visible properties of the entity type. However, even when the properties are hidden, it is important to recognize that they still exist in the EF model.

The primary key property of `Blog`, `Blog.Id`, and the foreign key property of `Post`, `Post.BlogId`, can then be associated with the references ("navigations") between the entity types (`Blog.Posts` and `Post.Blog`). This is done automatically by EF when building a simple relationship like this, but can also be specified explicitly when overriding the `OnModelCreating` method of your `DbContext`. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasMany(e => e.Posts)
        .WithOne(e => e.Blog)
        .HasForeignKey(e => e.BlogId)
        .HasPrincipalKey(e => e.Id);
}
```

Now all these properties will behave coherently together as a representation of a single relationship between `Blog` and `Post`.

## Find out more

EF supports many different types of relationships, with many different ways these relationships can be represented and configured. To jump into examples for different kinds of relationships, see:

- [_One-to-many relationships_](xref:core/modeling/relationships/one-to-many), in which a single entity is associated with any number of other entities.
- [_One-to-one relationships_](xref:core/modeling/relationships/one-to-one), in which a single entity is associated with another single entity.
- [_Many-to-many relationships_](xref:core/modeling/relationships/many-to-many), in which any number of entities are associated with any number of other entities.

If you are new to EF, then trying the examples linked in in the bullet points above is a good way to get a feel for how relationships work.

To dig deeper into the properties of entity types involved in relationship mapping, see:

- [_Foreign and principal keys in relationships_](xref:core/modeling/relationships/foreign-and-principal-keys), which covers how foreign keys map to the database.
- [_Relationship navigations_](xref:core/modeling/relationships/navigations), which describes how navigations are layered over a foreign key to provide an object-oriented view of the relationship.

EF models are built using a combination of three mechanisms: conventions, mapping attributes, and the model builder API. Most of the examples show the model building API. To find out more about other options, see:

- [_Relationship conventions_](xref:core/modeling/relationships/conventions), which discover entity types, their properties, and the relationships between the types.
- [_Relationship mapping attributes_](xref:core/modeling/relationships/mapping-attributes), which can be used an alternative to the model building API for some aspects of relationship configuration.

> [!IMPORTANT]
> The model-building API is the final source of truth for the EF model--it always takes precedence over configuration discovered by convention or specified by mapping attributes. It is also the only mechanism with full fidelity to configure every aspect of the EF model.

Other topics related to relationships include:

- [_Cascade deletes_](xref:core/saving/cascade-delete), which describe how related entities can be automatically deleted when `SaveChanges` or `SaveChangesAsync` is called.
- [_Owned entity types_](xref:core/modeling/owned-entities) use a special type of "owning" relationship that implies a stronger connection between the two types than the "normal" relationships discussed here. Many of the concepts described here for normal relationships are carried over to owned relationships. However, owned relationships also have their own special behaviors.

> [!TIP]
> Refer to the [glossary of relationship terms](xref:core/modeling/relationships/glossary) as needed when reading the documentation to help understand the terminology used.

## Using relationships

Relationships defined in the model can be used in various ways. For example:

- Relationships can be used to [query related data](xref:core/querying/related-data) in any of three ways:
  - [Eagerly](xref:core/querying/related-data/eager) as part of a LINQ query, using `Include`.
  - [Lazily](xref:core/querying/related-data/lazy) using lazy-loading proxies, or lazy-loading without proxies.
  - [Explicitly](xref:core/querying/related-data/explicit) using the `Load` or `LoadAsync` methods.
- Relationships can be used in [data seeding](xref:core/modeling/data-seeding) through matching of PK values to FK values.
- Relationships can be used to [track graphs of entities](xref:core/change-tracking/index). Relationships are then used by the change tracker to:
  - [Detect changes in relationships and perform fixup](xref:core/change-tracking/relationship-changes)
  - [Send foreign key updates to the database](xref:core/saving/related-data) with `SaveChanges` or `SaveChangesAsync`
