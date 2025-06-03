---
title: Conventions for relationship discovery - EF Core
description: How navigations, foreign keys, and other aspects of relationships are discovered by EF Core model building conventions
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships/conventions
---
# Conventions for relationship discovery

EF Core uses a set of [conventions](xref:core/modeling/bulk-configuration#conventions) when discovering and building a [model](xref:core/modeling/index) based on entity type classes. This document summarizes the conventions used for discovering and configuring [relationships between entity types](xref:core/modeling/relationships).

> [!IMPORTANT]
> The conventions described here can be overridden by explicit configuration of the relationship using either [mapping attributes](xref:core/modeling/relationships/mapping-attributes) or the model building API.

> [!TIP]
> The code below can be found in [RelationshipConventions.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/Relationships/RelationshipConventions.cs).

## Discovering navigations

Relationship discovery begins by discovering [navigations](xref:core/modeling/relationships/navigations) between entity types.

### Reference navigations

A property of an entity type is discovered as a [reference navigation](xref:core/modeling/relationships/navigations) when:

- The property is public.
- The property has a getter and a setter.
  - The setter does not need to be public; it can be private or have any other [accessibility](/dotnet/csharp/language-reference/keywords/access-modifiers?source=recommendations).
  - The setter can be [Init-only](/dotnet/csharp/properties#init-only).
- The property type is, or could be, an entity type. This means that the type
  - Must be a [reference type](/dotnet/csharp/language-reference/keywords/reference-types).
  - Must not have been configured explicitly as a [primitive property type](xref:core/modeling/entity-properties).
  - Must not be mapped as a primitive property type by the database provider being used.
  - Must not be [automatically convertable](xref:core/modeling/value-conversions) to a primitive property type mapped by the database provider being used.
- The property is not static.
- The property is not an [indexer property](/dotnet/csharp/programming-guide/indexers/).

For example, consider the following entity types:

<!--
        public class Blog
        {
            // Not discovered as reference navigations:
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public Uri? Uri { get; set; }
            public ConsoleKeyInfo ConsoleKeyInfo { get; set; }
            public Author DefaultAuthor => new() { Name = $"Author of the blog {Title}" };

            // Discovered as a reference navigation:
            public Author? Author { get; private set; }
        }

        public class Author
        {
            // Not discovered as reference navigations:
            public Guid Id { get; set; }
            public string Name { get; set; } = null!;
            public int BlogId { get; set; }

            // Discovered as a reference navigation:
            public Blog Blog { get; init; } = null!;
        }
-->
[!code-csharp[ReferenceNavigations](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=ReferenceNavigations)]

For these types, `Blog.Author` and `Author.Blog` are discovered as reference navigations. On the other hand, the following properties are _not_ discovered as reference navigations:

- `Blog.Id`, because `int` is a mapped primitive type
- `Blog.Title`, because 'string` is a mapped primitive type
- `Blog.Uri`, because `Uri` is automatically converted to a mapped primitive type
- `Blog.ConsoleKeyInfo`, because `ConsoleKeyInfo` is a C# value type
- `Blog.DefaultAuthor`, because the property does not have a setter
- `Author.Id`, because `Guid` is a mapped primitive type
- `Author.Name`, because 'string` is a mapped primitive type
- `Author.BlogId`, because `int` is a mapped primitive type

### Collection navigations

A property of an entity type is discovered as a [collection navigation](xref:core/modeling/relationships/navigations) when:

- The property is public.
- The property has a getter. Collection navigations can have setters, but this is not required.
- The property type is or implements `IEnumerable<TEntity>`, where `TEntity` is, or could be, an entity type. This means that the type of `TEntity`:
  - Must be a [reference type](/dotnet/csharp/language-reference/keywords/reference-types).
  - Must not have been configured explicitly as a [primitive property type](xref:core/modeling/entity-properties).
  - Must not be mapped as a primitive property type by the database provider being used.
  - Must not be [automatically convertable](xref:core/modeling/value-conversions) to a primitive property type mapped by the database provider being used.
- The property is not static.
- The property is not an [indexer property](/dotnet/csharp/programming-guide/indexers/).

For example, in the following code, both `Blog.Tags` and `Tag.Blogs` are discovered as collection navigations:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; set; } = null!;
        }

        public class Tag
        {
            public Guid Id { get; set; }
            public IEnumerable<Blog> Blogs { get; } = new List<Blog>();
        }
-->
[!code-csharp[CollectionNavigations](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=CollectionNavigations)]

### Pairing navigations

Once a navigation going from, for example, entity type A to entity type B is discovered, it must next be determined if this navigation has an inverse going in the opposite direction--that is, from entity type B to entity type A. If such an inverse is found, then the two navigations are paired together to form a single, bidirectional relationship.

The type of relationship is determined by whether the navigation and its inverse are reference or collection navigations. Specifically:

- If one navigation is a collection navigation and the other is a reference navigation, then the relationship is [one-to-many](xref:core/modeling/relationships/one-to-many).
- If both navigations are reference navigations, then the relationship is [one-to-one](xref:core/modeling/relationships/one-to-one).
- If both navigations are collection navigations, then the relationship is [many-to-many](xref:core/modeling/relationships/many-to-many).

Discovery of each of these types of relationship is shown in the examples below:

A single, one-to-many relationship is discovered between `Blog` and `Post` is discovered by pairing the `Blog.Posts` and `Post.Blog` navigations:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; set; }
        }
-->
[!code-csharp[OneToManySingleRelationship](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=OneToManySingleRelationship)]

A single, one-to-one relationship is discovered between `Blog` and `Author` is discovered by pairing the `Blog.Author` and `Author.Blog` navigations:

<!--
        public class Blog
        {
            public int Id { get; set; }
            public Author? Author { get; set; }
        }

        public class Author
        {
            public int Id { get; set; }
            public int? BlogId { get; set; }
            public Blog? Blog { get; set; }
        }
-->
[!code-csharp[OneToOneSingleRelationship](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=OneToOneSingleRelationship)]

A single, many-to-many relationship is discovered between `Post` and `Tag` is discovered by pairing the `Post.Tags` and `Tag.Posts` navigations:

<!--
        public class Post
        {
            public int Id { get; set; }
            public ICollection<Tag> Tags { get; } = new List<Tag>();
        }

        public class Tag
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }
-->
[!code-csharp[ManyToManySingleRelationship](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=ManyToManySingleRelationship)]

> [!NOTE]
> This pairing of navigations may be incorrect if the two navigations represent two, different, unidirectional relationships. In this case, the two relationships must be configured explicitly.

Pairing of relationships only works when there is a single relationship between two types. Multiple relationships between two types must be configured explicitly.

> [!NOTE]
> The descriptions here are in terms of relationships between two different types. However, it is possible for the same type to be on both ends of a relationship, and therefore for a single type to have two navigations both paired with each other. This is called a self-referencing relationship.

## Discovering foreign key properties

Once the navigations for a relationship have either been discovered or configured explicitly, then these navigations are used to discover appropriate foreign key properties for the relationship. A property is discovered as a foreign key when:

- The property type is compatible with the primary or alternate key on the principal entity type.
  - Types are compatible if they are the same, or if the foreign key property type is a nullable version of the primary or alternate key property type.
- The property name matches one of the naming conventions for a foreign key property. The naming conventions are:
  - `<navigation property name><principal key property name>`
  - `<navigation property name>Id`
  - `<principal entity type name><principal key property name>`
  - `<principal entity type name>Id`
- In addition, if the dependent end has been explicitly configured using the model building API, and the dependent primary key is compatible, then the dependent primary key will also be used as the foreign key.

> [!TIP]
> The "Id" suffix can have any casing.

The following entity types show examples for each of these naming conventions.

`Post.TheBlogKey` is discovered as the foreign key because it matches the pattern `<navigation property name><principal key property name>`:

<!--
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? TheBlogKey { get; set; }
            public Blog? TheBlog { get; set; }
        }
-->
[!code-csharp[NavigationPrincipalKeyFKName](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=NavigationPrincipalKeyFKName)]

`Post.TheBlogID` is discovered as the foreign key because it matches the pattern `<navigation property name>Id`:

<!--
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? TheBlogID { get; set; }
            public Blog? TheBlog { get; set; }
        }
-->
[!code-csharp[NavigationIdFKName](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=NavigationIdFKName)]

`Post.BlogKey` is discovered as the foreign key because it matches the pattern `<principal entity type name><principal key property name>`:

<!--
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? BlogKey { get; set; }
            public Blog? TheBlog { get; set; }
        }

-->
[!code-csharp[PrincipalTypePrincipalKeyFKName](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=PrincipalTypePrincipalKeyFKName)]

`Post.Blogid` is discovered as the foreign key because it matches the pattern `<principal entity type name>Id`:

<!--
        public class Blog
        {
            public int Key { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int? Blogid { get; set; }
            public Blog? TheBlog { get; set; }
        }
-->
[!code-csharp[PrincipalTypeIdFKName](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=PrincipalTypeIdFKName)]

> [!NOTE]
> In the case of one-to-many navigations, the foreign key properties must be on the type with the reference navigation, since this will be the dependent entity. In the case of one-to-one relationships, discovery of a foreign key property is used to determine which type represents the dependent end of the relationship. If no foreign key property is discovered, then the dependent end must be configured using `HasForeignKey`. See [_One-to-one relationships_](xref:core/modeling/relationships/one-to-one) for examples of this.

The rules above also apply to [composite foreign keys](xref:core/modeling/relationships/foreign-and-principal-keys), where each property of the composite must have a compatible type with the corresponding property of the primary or alternate key, and each property name must match one of the naming conventions described above.

## Determining cardinality

EF uses the discovered navigations and foreign key properties to determine the cardinality of the relationship together with its principal and dependent ends:

- If there is one, unpaired reference navigation, then the relationship is configured as a unidirectional [one-to-many](xref:core/modeling/relationships/one-to-many), with the reference navigation on the dependent end.
- If there is one, unpaired collection navigation, then the relationship is configured as a unidirectional [one-to-many](xref:core/modeling/relationships/one-to-many), with the collection navigation on the principal end.
- If there are paired reference and collection navigations, then the relationship is configured as a bidirectional [one-to-many](xref:core/modeling/relationships/one-to-many), with the collection navigation on the principal end.
- If a reference navigation is paired with another reference navigation, then:
  - If a foreign key property was discovered on one side but not the other, then the relationship is configured as a bidirectional [one-to-one](xref:core/modeling/relationships/one-to-one), with the foreign key property on the dependent end.
  - Otherwise, the dependent side cannot be determined and EF throws an exception indicating that the dependent must be explicitly configured.
- If a collection navigation is paired with another collection navigation, then the relationship is configured as a bidirectional [many-to-many](xref:core/modeling/relationships/many-to-many).

## Shadow foreign key properties

If EF has determined the dependent end of the relationship but no foreign key property was discovered, then EF will create a [shadow property](xref:core/modeling/shadow-properties) to represent the foreign key. The shadow property:

- Has the type of the primary or alternate key property at the principal end of the relationship.
  - The type is made nullable by default, making the relationship optional by default.
- If there is a navigation on the dependent end, then the shadow foreign key property is named using this navigation name concatenated with the primary or alternate key property name.
- If there is no navigation on the dependent end, then the shadow foreign key property is named using principal entity type name concatenated with the primary or alternate key property name.

## Cascade delete

By convention, required relationships are configured to [cascade delete](xref:core/saving/cascade-delete). Optional relationships are configured to not cascade delete.

## Many-to-many

[Many-to-many relationships](xref:core/modeling/relationships/many-to-many) do not have principal and dependent ends, and neither end contains a foreign key property. Instead, many-to-many relationships use a join entity type which contains pairs of foreign keys pointing to either end of the many-to-many. Consider the following entity types, for which a many-to-many relationship is discovered by convention:

<!--
        public class Post
        {
            public int Id { get; set; }
            public ICollection<Tag> Tags { get; } = new List<Tag>();
        }

        public class Tag
        {
            public int Id { get; set; }
            public ICollection<Post> Posts { get; } = new List<Post>();
        }
-->
[!code-csharp[ManyToManySingleRelationship](../../../../samples/core/Modeling/Relationships/RelationshipConventions.cs?name=ManyToManySingleRelationship)]

The conventions used in this discovery are:

- The join entity type is named `<left entity type name><right entity type name>`. So, `PostTag` in this example.
  - The join table has the same name as the join entity type.
- The join entity type is given a foreign key property for each direction of the relationship. These are named `<navigation name><principal key name>`. So, in this example, the foreign key properties are `PostsId` and `TagsId`.
  - For a unidirectional many-to-many, the foreign key property without an associated navigation is named `<principal entity type name><principal key name>`.
- The foreign key properties are non-nullable, making both relationships to the join entity required.
  - The cascade delete conventions mean that these relationships will be configured for cascade delete.
- The join entity type is configured with a composite primary key consisting of the two foreign key properties. So, in this example, the primary key is made up of `PostsId` and `TagsId`.

This results in the following EF model:

```output
Model:
  EntityType: Post
    Properties:
      Id (int) Required PK AfterSave:Throw ValueGenerated.OnAdd
    Skip navigations:
      Tags (ICollection<Tag>) CollectionTag Inverse: Posts
    Keys:
      Id PK
  EntityType: Tag
    Properties:
      Id (int) Required PK AfterSave:Throw ValueGenerated.OnAdd
    Skip navigations:
      Posts (ICollection<Post>) CollectionPost Inverse: Tags
    Keys:
      Id PK
  EntityType: PostTag (Dictionary<string, object>) CLR Type: Dictionary<string, object>
    Properties:
      PostsId (no field, int) Indexer Required PK FK AfterSave:Throw
      TagsId (no field, int) Indexer Required PK FK Index AfterSave:Throw
    Keys:
      PostsId, TagsId PK
    Foreign keys:
      PostTag (Dictionary<string, object>) {'PostsId'} -> Post {'Id'} Cascade
      PostTag (Dictionary<string, object>) {'TagsId'} -> Tag {'Id'} Cascade
    Indexes:
      TagsId
```

And translates to the following database schema when using SQLite:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tag" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tag" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tag_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tag" ("Id") ON DELETE CASCADE);

CREATE INDEX "IX_PostTag_TagsId" ON "PostTag" ("TagsId");
```

## Indexes

By convention, EF creates a [database index](xref:core/modeling/indexes) for the property or properties of a foreign key. The type of index created is determined by:

- The cardinality of the relationship
- Whether the relationship is optional or required
- The number of properties that make up the foreign key

For a [one-to-many relationship](xref:core/modeling/relationships/one-to-many), a straightforward index is created by convention. The same index is created for optional and required relationships. For example, on SQLite:

```sql
CREATE INDEX "IX_Post_BlogId" ON "Post" ("BlogId");
```

Or on SQL Server:

```sql
CREATE INDEX [IX_Post_BlogId] ON [Post] ([BlogId]);
```

For an required [one-to-one relationship](xref:core/modeling/relationships/one-to-one), a unique index is created. For example, on SQLite:

```sql
CREATE UNIQUE INDEX "IX_Author_BlogId" ON "Author" ("BlogId");
```

Or on SQL Sever:

```sql
CREATE UNIQUE INDEX [IX_Author_BlogId] ON [Author] ([BlogId]);
```

For optional one-to-one relationships, the index created on SQLite is the same:

```sql
CREATE UNIQUE INDEX "IX_Author_BlogId" ON "Author" ("BlogId");
```

However, on SQL Server, an `IS NOT NULL` filter is added to better handle null foreign key values. For example:

```sql
CREATE UNIQUE INDEX [IX_Author_BlogId] ON [Author] ([BlogId]) WHERE [BlogId] IS NOT NULL;
```

For composite foreign keys, an index is created covering all the foreign key columns. For example:

```sql
CREATE INDEX "IX_Post_ContainingBlogId1_ContainingBlogId2" ON "Post" ("ContainingBlogId1", "ContainingBlogId2");
```

> [!NOTE]
> EF does not create indexes for properties that are already covered by an existing index or primary key constraint.

### How to stop EF creating indexes for foreign keys

Indexes have overhead, and, [as asked here](https://github.com/dotnet/efcore/issues/10855), it may not always be appropriate to create them for all FK columns. To achieve this, the `ForeignKeyIndexConvention` can be removed when building the model:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
}
```

When desired, indexes can still be [explicitly created](xref:core/modeling/indexes) for those foreign key columns that do need them.

## Foreign key constraint names

By convention foreign key constraints are named `FK_<dependent type name>_<principal type name>_<foreign key property name>`. For composite foreign keys, `<foreign key property name>` becomes an underscore separated list of foreign key property names.

## Additional resources

- [.NET Data Community Standup video on custom model conventions](https://www.youtube.com/live/6apfe1L1FhY?feature=share).
