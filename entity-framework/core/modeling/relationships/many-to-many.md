---
title: Many-to-many relationships - EF Core
description: How to configure many-to-many relationships between entity types when using Entity Framework Core
author: SamMonoRT
ms.date: 03/30/2023
uid: core/modeling/relationships/many-to-many
---
# Many-to-many relationships

Many-to-many relationships are used when any number entities of one entity type is associated with any number of entities of the same or another entity type. For example, a `Post` can have many associated `Tags`, and each `Tag` can in turn be associated with any number of `Posts`.

## Understanding many-to-many relationships

Many-to-many relationships are different from [one-to-many](xref:core/modeling/relationships/one-to-many) and [one-to-one](xref:core/modeling/relationships/one-to-one) relationships in that they cannot be represented in a simple way using just a foreign key. Instead, an additional entity type is needed to "join" the two sides of the relationship. This is known as the "join entity type" and maps to a "join table" in a relational database. The entities of this join entity type contain pairs of foreign key values, where one of each pair points to an entity on one side of the relationship, and the other points to an entity on the other side of the relationship. Each join entity, and therefore each row in the join table, therefore represents one association between the entity types in the relationship.

EF Core can hide the join entity type and manage it behind the scenes. This allows the navigations of a many-to-many relationship to be used in a natural manner, adding or removing entities from each side as needed. However, it is useful to understand what is happening behind the scenes so that their overall behavior, and in particular the mapping to a relational database, makes sense. Let's start with a relational database schema setup to represent a many-to-many relationship between posts and tags:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

In this schema, `PostTag` is the join table. It contains two columns: `PostsId`, which is a foreign key to the primary key of the `Posts` table, and `TagsId`, which is a foreign key to primary key of the `Tags` table. Each row in this table therefore represents an association between one `Post` and one `Tag`.

A simplistic mapping for this schema in EF Core consists of three entity types--one for each table. If each of these entity types are represented by a .NET class, then those classes might look the following:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostsId { get; set; }
            public int TagsId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
-->
[!code-csharp[DirectJoinTableMapping](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=DirectJoinTableMapping)]

Notice that in this mapping there is no many-to-many relationship, but rather two one-to-many relationships, one for each of the foreign keys defined in the join table. This is not an unreasonable way to map these tables, but doesn't reflect the intent of the join table, which is to represent a single many-to-many relationship, rather than two one-to-many relationships.

EF allows for a more natural mapping through the introduction of two collection navigations, one on `Post` containing its related `Tags`, and an inverse on `Tag` containing its related `Posts`. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = new();
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<PostTag> PostTags { get; } = new();
            public List<Post> Posts { get; } = new();
        }

        public class PostTag
        {
            public int PostsId { get; set; }
            public int TagsId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
-->
[!code-csharp[FullMappingWithJoinEntity](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=FullMappingWithJoinEntity)]

> [!TIP]
> These new navigations are known as "skip navigations", because they skip over the join entity to provide direct access to the other side of the many-to-many relationship.

As is shown in the examples below, a many-to-many relationship can be mapped in this way--that is, with a .NET class for the join entity, and with both navigations for the two one-to-many relationships _and_ skip navigations exposed on the entity types. However, EF can manage the join entity transparently, without a .NET class defined for it, and without navigations for the two one-to-many relationships. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }
-->
[!code-csharp[BasicManyToMany](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=BasicManyToMany)]

Indeed, EF [model building conventions](xref:core/modeling/relationships/conventions) will, by default, map the `Post` and `Tag` types shown here to the three tables in the database schema at the top of this section. This mapping, without explicit use of the join type, is what is typically meant by the term "many-to-many".

## Examples

The following sections contain examples of many-to-many relationships, including the configuration needed to achieve each mapping.

> [!TIP]
> The code for all the examples below can be found in [ManyToMany.cs](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Modeling/Relationships/ManyToMany.cs).

## Basic many-to-many

In the most basic case for a many-to-many, the entity types on each end of the relationship both have a collection navigation. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }
-->
[!code-csharp[BasicManyToMany](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=BasicManyToMany)]

This relationship is [mapped by convention](xref:core/modeling/relationships/conventions). Even though it is not needed, an equivalent explicit configuration for this relationship is shown below as a learning tool:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts);
            }
-->
[!code-csharp[BasicManyToManyConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=BasicManyToManyConfig)]

Even with this explicit configuration, many aspects of the relationship are still configured by convention. A more complete explicit configuration, again for learning purposes, is:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        "PostTag",
                        r => r.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                        l => l.HasOne(typeof(Post)).WithMany().HasForeignKey("PostsId").HasPrincipalKey(nameof(Post.Id)),
                        j => j.HasKey("PostsId", "TagsId"));
            }
-->
[!code-csharp[BasicManyToManyFullConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=BasicManyToManyFullConfig)]

> [!IMPORTANT]
> Please don't attempt to fully configure everything even when it is not needed. As can be seen above, the code gets complicated quickly and its easy to make a mistake. And even in the example above there are many things in the model that are still configured by convention. It's not realistic to think that everything in an EF model can always be fully configured explicitly.

Regardless of whether the relationship is built by convention or using either of the shown explicit configurations, the resulting mapped schema (using SQLite) is:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

> [!TIP]
> When using a Database First flow to [scaffold a DbContext from an existing database](xref:core/managing-schemas/scaffolding), EF Core 6 and later looks for this pattern in the database schema and scaffolds a many-to-many relationship as described in this document. This behavior can be changed through use of a [custom T4 template](xref:core/managing-schemas/scaffolding/templates). For other options, see [_Many-to-many relationships without mapped join entities are now scaffolded_](xref:core/what-is-new/ef-core-6.0/breaking-changes#many-to-many).

> [!IMPORTANT]
> Currently, EF Core uses `Dictionary<string, object>` to represent join entity instances for which no .NET class has been configured. However, to improve performance, a different type may be used in a future EF Core release. Do not depend on the join type being `Dictionary<string, object>` unless this has been explicitly configured.

## Many-to-many with named join table

In the previous example, the join table was named `PostTag` by convention. It can be given an explicit name with `UsingEntity`. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity("PostsToTagsJoinTable");
            }
-->
[!code-csharp[ManyToManyNamedJoinTableConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyNamedJoinTableConfig)]

Everything else about the mapping remains the same, with only the name of the join table changing:

```sql
CREATE TABLE "PostsToTagsJoinTable" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostsToTagsJoinTable" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostsToTagsJoinTable_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostsToTagsJoinTable_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many with join table foreign key names

Following on from the previous example, the names of the foreign key columns in the join table can also be changed. There are two ways to do this. The first is to explicitly specify the foreign key property names on the join entity. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        r => r.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagForeignKey"),
                        l => l.HasOne(typeof(Post)).WithMany().HasForeignKey("PostForeignKey"));
            }
-->
[!code-csharp[ManyToManyNamedForeignKeyColumnsConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyNamedForeignKeyColumnsConfig)]

The second way is to leave the properties with their by-convention names, but then map these properties to different column names. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        j =>
                        {
                            j.Property("PostsId").HasColumnName("PostForeignKey");
                            j.Property("TagsId").HasColumnName("TagForeignKey");
                        });
            }
-->
[!code-csharp[ManyToManyNamedForeignKeyColumnsAlternateConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyNamedForeignKeyColumnsAlternateConfig)]

In either case, the mapping remains the same, with only the foreign key column names changed:

```sql
CREATE TABLE "PostTag" (
    "PostForeignKey" INTEGER NOT NULL,
    "TagForeignKey" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostForeignKey", "TagForeignKey"),
    CONSTRAINT "FK_PostTag_Posts_PostForeignKey" FOREIGN KEY ("PostForeignKey") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagForeignKey" FOREIGN KEY ("TagForeignKey") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

> [!TIP]
> Although not shown here, the previous two examples can be combined to map change the join table name and its foreign key column names.

## Many-to-many with class for join entity

So far in the examples, the join table has been automatically mapped to a [shared-type entity type](xref:core/modeling/entity-types#shared-type-entity-types). This removes the need for a dedicated class to be created for the entity type. However, it can be useful to have such a class so that it can be referenced easily, especially when navigations or a payload are added to the class, as is shown in later examples below. To do this, first create a type `PostTag` for the join entity in addition to the existing types for `Post` and `Tag`:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
-->
[!code-csharp[ManyToManyWithJoinClass](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClass)]

> [!TIP]
> The class can have any name, but it is common to combine the names of the types at either end of the relationship.

Now the `UsingEntity` method can be used to configure this as the join entity type for the relationship. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
-->
[!code-csharp[ManyToManyWithJoinClassConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClassConfig)]

The `PostId` and `TagId` are automatically picked up as the foreign keys and are configured as the composite primary key for the join entity type. The properties to use for the foreign keys can be explicitly configured for cases where they don't match the EF convention. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        r => r.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId),
                        l => l.HasOne<Post>().WithMany().HasForeignKey(e => e.PostId));
            }
-->
[!code-csharp[ManyToManyWithJoinClassFkConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClassFkConfig)]

The mapped database schema for the join table in this example is structurally equivalent to the previous examples, but with some different column names:

```sql
CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many with navigations to join entity

Following on from the previous example, now that there is a class representing the join entity, it becomes easy to add navigations that reference this class. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
-->
[!code-csharp[ManyToManyWithNavsToJoinClass](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsToJoinClass)]

> [!IMPORTANT]
> As shown in this example, navigations to the join entity type can be used _in addition to_ the skip navigations between the two ends of the many-to-many relationship. This means that the skip navigations can be used to interact with the many-to-many relationship in a natural manner, while the navigations to the join entity type can be used when greater control over the join entities themselves is needed. In a sense, this mapping provides the best of both worlds between a simple many-to-many mapping, and a mapping that more explicitly matches the database schema.

Nothing needs to be changed in the `UsingEntity` call, since the navigations to the join entity are picked up by convention. Therefore, the configuration for this example is the same as for the last example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
-->
[!code-csharp[ManyToManyWithJoinClassConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClassConfig)]

The navigations can be configured explicitly for cases where they cannot be determined by convention. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        r => r.HasOne<Tag>().WithMany(e => e.PostTags),
                        l => l.HasOne<Post>().WithMany(e => e.PostTags));
            }
-->
[!code-csharp[ManyToManyWithNavsToJoinClassWithNavConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsToJoinClassWithNavConfig)]

The mapped database schema is not affected by including navigations in the model:

```sql
CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many with navigations to and from join entity

The previous example added navigations to the join entity type from the entity types at either end of the many-to-many relationship. Navigations can also be added in the other direction, or in both directions. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
-->
[!code-csharp[ManyToManyWithNavsToAndFromJoinClass](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsToAndFromJoinClass)]

Nothing needs to be changed in the `UsingEntity` call, since the navigations to the join entity are picked up by convention. Therefore, the configuration for this example is the same as for the last example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
-->
[!code-csharp[ManyToManyWithNavsToAndFromJoinClassConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsToAndFromJoinClassConfig)]

The navigations can be configured explicitly for cases where they cannot be determined by convention. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        r => r.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags),
                        l => l.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags));
            }
-->
[!code-csharp[ManyToManyWithNavsToAndFromJoinClassWithNavConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsToAndFromJoinClassWithNavConfig)]

The mapped database schema is not affected by including navigations in the model:

```sql
CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many with navigations and changed foreign keys

The previous example showed a many-to-many with navigations to and from the join entity type. This example is the same, except that the foreign key properties used are also changed. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostForeignKey { get; set; }
            public int TagForeignKey { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
-->
[!code-csharp[ManyToManyWithNamedFksAndNavsToAndFromJoinClass](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNamedFksAndNavsToAndFromJoinClass)]

Again, the `UsingEntity` method is used to configure this:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        r => r.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasForeignKey(e => e.TagForeignKey),
                        l => l.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasForeignKey(e => e.PostForeignKey));
            }
-->
[!code-csharp[ManyToManyWithNamedFksAndNavsToAndFromJoinClassConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNamedFksAndNavsToAndFromJoinClassConfig)]

The mapped database schema is now:

```sql
CREATE TABLE "PostTag" (
    "PostForeignKey" INTEGER NOT NULL,
    "TagForeignKey" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostForeignKey", "TagForeignKey"),
    CONSTRAINT "FK_PostTag_Posts_PostForeignKey" FOREIGN KEY ("PostForeignKey") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagForeignKey" FOREIGN KEY ("TagForeignKey") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Unidirectional many-to-many

> [!NOTE]
> Unidirectional many-to-many relationships were introduced in EF Core 7. In earlier releases, a private navigation could be used as a workaround.

It is not necessary to include a navigation on both sides of the many-to-many relationship. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
        }
-->
[!code-csharp[UnidirectionalManyToMany](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=UnidirectionalManyToMany)]

EF needs some configuration to know that this should be a many-to-many relationship, rather than a one-to-many. This is done using `HasMany` and `WithMany`, but with no argument passed on the side without a navigation. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany();
            }
-->
[!code-csharp[UnidirectionalManyToManyConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=UnidirectionalManyToManyConfig)]

Removing the navigation does not affect the database schema:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT);

CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many and join table with payload

In the examples so far, the join table has been used only to store the foreign key pairs representing each association. However, it can also be used to store information about the association--for example, the time it was created. In such cases it is best to define a type for the join entity and add the "association payload" properties to this type. It is also common to create navigations to the join entity in addition to the "skip navigations" used for the many-to-many relationship. These additional navigations allow the join entity to be easily referenced from code, thereby facilitating reading and/or changing the payload data. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public DateTime CreatedOn { get; set; }
        }
-->
[!code-csharp[ManyToManyWithPayloadAndNavsToJoinClass](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithPayloadAndNavsToJoinClass)]

It is also common to use generated values for payload properties--for example, a database timestamp that is automatically set when the association row is inserted. This requires some minimal configuration. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
            }
-->
[!code-csharp[ManyToManyWithPayloadAndNavsToJoinClassConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithPayloadAndNavsToJoinClassConfig)]

The result maps to a entity type schema with a timestamp set automatically when a row is inserted:

```sql
CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

> [!TIP]
> The SQL shown here is for SQLite. On SQL Server/Azure SQL, use `.HasDefaultValueSql("GETUTCDATE()")` and for `TEXT` read `datetime`.

## Custom shared-type entity type as a join entity

The previous example used the type `PostTag` as the join entity type. This type is specific to the posts-tags relationship. However, if you have multiple join tables with the same shape, then the same CLR type can be used for all of them. For example, imagine that all our join tables have a `CreatedOn` column. We can map these using `JoinType` class mapped as a [shared-type entity type](xref:core/modeling/entity-types#shared-type-entity-types):

```csharp
public class JoinType
{
    public int Id1 { get; set; }
    public int Id2 { get; set; }
    public DateTime CreatedOn { get; set; }
}
```

This type can then be referenced as the join entity type by multiple different many-to-many relationships. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<JoinType> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
            public List<JoinType> PostTags { get; } = new();
        }

        public class Blog
        {
            public int Id { get; set; }
            public List<Author> Authors { get; } = new();
            public List<JoinType> BlogAuthors { get; } = new();
        }

        public class Author
        {
            public int Id { get; set; }
            public List<Blog> Blogs { get; } = new();
            public List<JoinType> BlogAuthors { get; } = new();
        }
-->
[!code-csharp[ManyToManyWithCustomSharedTypeEntityType](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithCustomSharedTypeEntityType)]

And these relationships can then be configured appropriately to map the join type to a different table for each relationship:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<JoinType>(
                        "PostTag",
                        r => r.HasOne<Tag>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id1),
                        l => l.HasOne<Post>().WithMany(e => e.PostTags).HasForeignKey(e => e.Id2),
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));

                modelBuilder.Entity<Blog>()
                    .HasMany(e => e.Authors)
                    .WithMany(e => e.Blogs)
                    .UsingEntity<JoinType>(
                        "BlogAuthor",
                        r => r.HasOne<Author>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id1),
                        l => l.HasOne<Blog>().WithMany(e => e.BlogAuthors).HasForeignKey(e => e.Id2),
                        j => j.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
            }
-->
[!code-csharp[ManyToManyWithCustomSharedTypeEntityTypeConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithCustomSharedTypeEntityTypeConfig)]

This results in the following tables in the database schema:

```sql
CREATE TABLE "BlogAuthor" (
    "Id1" INTEGER NOT NULL,
    "Id2" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_BlogAuthor" PRIMARY KEY ("Id1", "Id2"),
    CONSTRAINT "FK_BlogAuthor_Authors_Id1" FOREIGN KEY ("Id1") REFERENCES "Authors" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BlogAuthor_Blogs_Id2" FOREIGN KEY ("Id2") REFERENCES "Blogs" ("Id") ON DELETE CASCADE);


CREATE TABLE "PostTag" (
    "Id1" INTEGER NOT NULL,
    "Id2" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("Id1", "Id2"),
    CONSTRAINT "FK_PostTag_Posts_Id2" FOREIGN KEY ("Id2") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_Id1" FOREIGN KEY ("Id1") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many with alternate keys

So far, all the examples have shown the foreign keys in the join entity type being constrained to the primary keys of the entity types on either side of the relationship. Each foreign key, or both, can instead be constrained to an alternate key. For example, consider this model where`Tag` and `Post` have alternate key properties:

<!--
        public class Post
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Post> Posts { get; } = new();
        }
-->
[!code-csharp[ManyToManyAlternateKeys](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyAlternateKeys)]

The configuration for this model is:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        r => r.HasOne(typeof(Tag)).WithMany().HasPrincipalKey(nameof(Tag.AlternateKey)),
                        l => l.HasOne(typeof(Post)).WithMany().HasPrincipalKey(nameof(Post.AlternateKey)));
            }
-->
[!code-csharp[ManyToManyAlternateKeysConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyAlternateKeysConfig)]

And the resulting database schema, for clarity, including also the tables with the alternate keys:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT,
    "AlternateKey" INTEGER NOT NULL,
    CONSTRAINT "AK_Posts_AlternateKey" UNIQUE ("AlternateKey"));

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT,
    "AlternateKey" INTEGER NOT NULL,
    CONSTRAINT "AK_Tags_AlternateKey" UNIQUE ("AlternateKey"));

CREATE TABLE "PostTag" (
    "PostsAlternateKey" INTEGER NOT NULL,
    "TagsAlternateKey" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsAlternateKey", "TagsAlternateKey"),
    CONSTRAINT "FK_PostTag_Posts_PostsAlternateKey" FOREIGN KEY ("PostsAlternateKey") REFERENCES "Posts" ("AlternateKey") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsAlternateKey" FOREIGN KEY ("TagsAlternateKey") REFERENCES "Tags" ("AlternateKey") ON DELETE CASCADE);
```

The configuration for using alternate keys is slightly different if the join entity type is represented by a .NET type. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Tag> Tags { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public int AlternateKey { get; set; }
            public List<Post> Posts { get; } = new();
            public List<PostTag> PostTags { get; } = new();
        }

        public class PostTag
        {
            public int PostId { get; set; }
            public int TagId { get; set; }
            public Post Post { get; set; } = null!;
            public Tag Tag { get; set; } = null!;
        }
-->
[!code-csharp[ManyToManyWithNavsAndAlternateKeys](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsAndAlternateKeys)]

The configuration can now use the generic `UsingEntity<>` method:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>(
                        r => r.HasOne<Tag>(e => e.Tag).WithMany(e => e.PostTags).HasPrincipalKey(e => e.AlternateKey),
                        l => l.HasOne<Post>(e => e.Post).WithMany(e => e.PostTags).HasPrincipalKey(e => e.AlternateKey));
            }
-->
[!code-csharp[ManyToManyWithNavsAndAlternateKeysConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNavsAndAlternateKeysConfig)]

And the resulting schema is:

```sql
CREATE TABLE "Posts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY AUTOINCREMENT,
    "AlternateKey" INTEGER NOT NULL,
    CONSTRAINT "AK_Posts_AlternateKey" UNIQUE ("AlternateKey"));

CREATE TABLE "Tags" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tags" PRIMARY KEY AUTOINCREMENT,
    "AlternateKey" INTEGER NOT NULL,
    CONSTRAINT "AK_Tags_AlternateKey" UNIQUE ("AlternateKey"));

CREATE TABLE "PostTag" (
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("AlternateKey") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("AlternateKey") ON DELETE CASCADE);
```

## Many-to-many and join table with separate primary key

So far, the join entity type in all the examples has a primary key composed of the two foreign key properties. This is because each combination of values for these properties can occur at most once. These properties therefore form a natural primary key.

> [!NOTE]
> EF Core does not support duplicate entities in any collection navigation.

If you control the database schema, then there is no reason for the join table to have an additional primary key column, However, it is possible that an existing join table may have a primary key column defined. EF can still map to this with some configuration.

It is perhaps easiest to this by creating a class to represent the join entity. For example:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }

        public class PostTag
        {
            public int Id { get; set; }
            public int PostId { get; set; }
            public int TagId { get; set; }
        }
        #endregion
-->
[!code-csharp[ManyToManyWithJoinClassHavingPrimaryKey](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClassHavingPrimaryKey)]

This `PostTag.Id` property is now picked up as the primary key by convention, so the only configuration needed is a call to `UsingEntity` for the `PostTag` type:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity<PostTag>();
            }
-->
[!code-csharp[ManyToManyWithJoinClassHavingPrimaryKeyConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithJoinClassHavingPrimaryKeyConfig)]

And the resulting schema for the join table is:

```sql
CREATE TABLE "PostTag" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PostTag" PRIMARY KEY AUTOINCREMENT,
    "PostId" INTEGER NOT NULL,
    "TagId" INTEGER NOT NULL,
    CONSTRAINT "FK_PostTag_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

A primary key can also be added to the join entity without defining a class for it. For example, with just `Post` and `Tag` types:

<!--
        public class Post
        {
            public int Id { get; set; }
            public List<Tag> Tags { get; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }
            public List<Post> Posts { get; } = new();
        }
-->
[!code-csharp[ManyToManyWithPrimaryKeyInJoinEntity](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithPrimaryKeyInJoinEntity)]

The key can be added with this configuration:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        j =>
                        {
                            j.IndexerProperty<int>("Id");
                            j.HasKey("Id");
                        });
            }
-->
[!code-csharp[ManyToManyWithPrimaryKeyInJoinEntityConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithPrimaryKeyInJoinEntityConfig)]

Which results in a join table with a separate primary key column:

```sql
CREATE TABLE "PostTag" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PostTag" PRIMARY KEY AUTOINCREMENT,
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE CASCADE);
```

## Many-to-many without cascading delete

In all the examples shown above, the foreign keys created between the join table and the two sides of the many-to-many relationship are created with [cascading delete](xref:core/saving/cascade-delete) behavior. This is very useful because it means that if an entity on either side of the relationship is deleted, then the rows in the join table for that entity are automatically deleted. Or, in other words, when an entity no longer exists, then its relationships to other entities also no longer exist.

It's hard to imagine when it is useful to change this behavior, but it can be done if desired. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Post>()
                    .HasMany(e => e.Tags)
                    .WithMany(e => e.Posts)
                    .UsingEntity(
                        r => r.HasOne(typeof(Tag)).WithMany().OnDelete(DeleteBehavior.Restrict),
                        l => l.HasOne(typeof(Post)).WithMany().OnDelete(DeleteBehavior.Restrict));
            }
-->
[!code-csharp[ManyToManyWithNoCascadeDeleteConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=ManyToManyWithNoCascadeDeleteConfig)]

The database schema for the join table uses restricted delete behavior on the foreign key constraint:

```sql
CREATE TABLE "PostTag" (
    "PostsId" INTEGER NOT NULL,
    "TagsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PostTag" PRIMARY KEY ("PostsId", "TagsId"),
    CONSTRAINT "FK_PostTag_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES "Posts" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PostTag_Tags_TagsId" FOREIGN KEY ("TagsId") REFERENCES "Tags" ("Id") ON DELETE RESTRICT);
```

## Self-referencing many-to-many

The same entity type can be used at both ends of a many-to-many relationship; this is known as a "self-referencing" relationship. For example:

<!--
        public class Person
        {
            public int Id { get; set; }
            public List<Person> Parents { get; } = new();
            public List<Person> Children { get; } = new();
        }
-->
[!code-csharp[SelfReferencingManyToMany](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=SelfReferencingManyToMany)]

This maps to a join table called `PersonPerson`, with both foreign keys pointing back to the `People` table:

```sql
CREATE TABLE "PersonPerson" (
    "ChildrenId" INTEGER NOT NULL,
    "ParentsId" INTEGER NOT NULL,
    CONSTRAINT "PK_PersonPerson" PRIMARY KEY ("ChildrenId", "ParentsId"),
    CONSTRAINT "FK_PersonPerson_People_ChildrenId" FOREIGN KEY ("ChildrenId") REFERENCES "People" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PersonPerson_People_ParentsId" FOREIGN KEY ("ParentsId") REFERENCES "People" ("Id") ON DELETE CASCADE);
```

## Symmetrical self-referencing many-to-many

Sometimes a many-to-many relationship is naturally symmetrical. That is, if entity A is related to entity B, then entity B is also related to entity A. This is naturally modeled using a single navigation. For example, imagine the case where is person A is friends with person B, then person B is friends with person A:

<!--
        public class Person
        {
            public int Id { get; set; }
            public List<Person> Friends { get; } = new();
        }
-->
[!code-csharp[SelfReferencingUnidirectionalManyToMany](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=SelfReferencingUnidirectionalManyToMany)]

Unfortunately, this is not easy to map. The same navigation cannot be used for both ends of the relationship. The best that can be done is to map it as a unidirectional many-to-many relationship. For example:

<!--
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Person>()
                    .HasMany(e => e.Friends)
                    .WithMany();
            }
-->
[!code-csharp[SelfReferencingUnidirectionalManyToManyConfig](../../../../samples/core/Modeling/Relationships/ManyToMany.cs?name=SelfReferencingUnidirectionalManyToManyConfig)]

However, to make sure two people are both related to each other, each person will need to be manually added to the other person's `Friends` collection. For example:

```csharp
ginny.Friends.Add(hermione);
hermione.Friends.Add(ginny);
```

## Direct use of join table

All of the examples above make use of the EF Core many-to-many mapping patterns. However, it is also possible to map a join table to a normal entity type and just use the two one-to-many relationships for all operations.

For example, these entity types represent the mapping of two normal tables and join table without using any many-to-many relationships:

```csharp
public class Post
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = new();
}

public class Tag
{
    public int Id { get; set; }
    public List<PostTag> PostTags { get; } = new();
}

public class PostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }
    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
```

This requires no special mapping, since these are normal entity types with normal [one-to-many](xref:core/modeling/relationships/one-to-many) relationships.

## Additional resources

* [.NET Data Community Standup session](https://www.youtube.com/watch?v=W1sxepfIMRM&list=PLdo4fOcmZ0oX-DBuRG4u58ZTAJgBAeQ-t&index=32), with a deep dive into many-to-many and the infrastructure underpinning it.
